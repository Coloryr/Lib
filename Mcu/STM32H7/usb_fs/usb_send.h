#ifndef PROJECT12_USB_SEND_H
#define PROJECT12_USB_SEND_H

#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
    int8_t USBD_CUSTOM_HID_SendReport_FS(uint8_t *report, uint16_t len);

#ifdef __cplusplus
}
#endif

#endif //PROJECT12_USB_SEND_H
