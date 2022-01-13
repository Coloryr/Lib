#include "font_config.h"
#include "font_write.h"

#if USE_TF == 0

#include "main.h"
#include "fatfs.h"
#include "Show/show.h"
#include "lvgl.h"
#include "qspi/w25q64.h"

uint8_t data_temp[TEMP_L];
uint8_t data_temp1[TEMP_L];
ramfast FIL font_file;

typedef unsigned int uint ;
uint POLYNOMIAL = 0xEDB88320 ;
int have_table = 0 ;
uint table[256] ;

void make_table() {
    int i, j;
    have_table = 1;
    for (i = 0; i < 256; i++)
        for (j = 0, table[i] = i; j < 8; j++)
            table[i] = (table[i] >> 1) ^ ((table[i] & 1) ? POLYNOMIAL : 0);
}

uint32_t crc32(uint crc, const uint8_t *buff, uint32_t len) {
    if (!have_table) make_table();
    crc = ~crc;
    for (int i = 0; i < len; i++)
        crc = (crc >> 8) ^ table[(crc ^ buff[i]) & 0xff];
    return ~crc;
}

uint32_t write_font(uint32_t pos, uint32_t addr, const char *path) {
    FRESULT res;
    uint16_t len;
    uint16_t len1;
    uint32_t now;
    uint32_t crc_res;
    uint32_t size;
    uint8_t write_temp[5];
    font_len_cov cov;
    res = f_open(&font_file, path, FA_READ);
    if (res != FR_OK) {
        lv_label_set_text_fmt(info, "no file %s", path);
        while (1);
    }
    size = f_size(&font_file);
    now = 0;
    for (;;) {
        re:
        res = f_read(&font_file, data_temp, TEMP_L, (UINT *) &len);
        if (res != FR_OK) {
            lv_label_set_text_fmt(info, "file %s error res:%d", path, res);
            while (1);
        }
        crc_res = crc32(0, data_temp, len);
        res = f_lseek(&font_file, f_tell(&font_file) - len);
        if (res != FR_OK) {
            lv_label_set_text_fmt(info, "file %s error res:%d", path, res);
            while (1);
        }
        res = f_read(&font_file, data_temp, TEMP_L, (UINT *) &len1);
        if (res != FR_OK) {
            lv_label_set_text_fmt(info, "file %s error res:%d", path, res);
            while (1);
        }
        if (len != len1) {
            f_lseek(&font_file, f_tell(&font_file) - len1);
            goto re;
        }
        if (crc_res != crc32(0, data_temp, len)) {
            f_lseek(&font_file, f_tell(&font_file) - len);
            goto re;
        }
        re1:
        W25QXX_Write(data_temp, pos + now, len);
        W25QXX_Read(data_temp1, pos + now, len);
        if (crc_res != crc32(0, data_temp1, len))
            goto re1;
        if (len == 0)
            break;
        now += len;
        if (now == size)
            break;

        lv_label_set_text_fmt(info, "init font %s %d/%d", path, size, now);
    }
    if (now != size) {
        lv_label_set_text_fmt(info, "init font %s error:size check fail", path);
        while (1);
    }

    lv_label_set_text_fmt(info, "init font %s done.", path);

    f_close(&font_file);

    cov.u32 = now + 10;
    write_temp[0] = SAVE_BIT;
    write_temp[1] = cov.u8[0];
    write_temp[2] = cov.u8[1];
    write_temp[3] = cov.u8[2];
    write_temp[4] = cov.u8[3];
    osDelay(10);
    W25QXX_Write(write_temp, addr, 5);

    return cov.u32;
}

#endif