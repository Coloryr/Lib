#include "font_config.h"
#include "font_write.h"

#if USE_TF == 0

#include "main.h"
#include "fatfs.h"
#include "Show/show.h"
#include "lvgl.h"
#include "qspi/w25q64.h"

ramfast uint8_t data_temp[TEMP_L];
ramfast FIL font_file;

uint32_t write_font(uint32_t pos, uint32_t addr, const char *path) {
    FRESULT res;
    uint16_t len;
    uint32_t now;
    uint8_t write_temp[5];
    font_len_cov cov;
    res = f_open(&font_file, path, FA_READ);
    if (res != FR_OK) {
        lv_label_set_text_fmt(info, "no file %s", path);
        while (1);
    }
    now = 0;
    for (;;) {
        res = f_read(&font_file, data_temp, TEMP_L, (UINT *) &len);
        if (res != FR_OK) {
            lv_label_set_text_fmt(info, "file %s error res:%d", path, res);
            while (1);
        }
        W25QXX_Write(data_temp, pos + now, len);
        if (len == 0)
            break;
        now += len;
        if (now == font_file.obj.objsize)
            break;

        lv_label_set_text_fmt(info, "init font %s %d/%d", path, font_file.obj.objsize, now);
    }
    if (now != font_file.obj.objsize) {
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