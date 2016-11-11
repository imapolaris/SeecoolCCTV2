#pragma once

#include "FFmpegFile.h"
#include "VideoDecoder.h"
#include "StreamReader.h"
#include "ImageScaler.h"

void WINAPI FFmpeg_Init();
void WINAPI FFmpeg_FreeMemory(void* data);

FFmpegFile* WINAPI FileBuilder_Create(const char* sFileName, int nWidth, int nHeight, enum AVCodecID videoCodecID, int nBitRate);
void WINAPI FileBuilder_Close(FFmpegFile* pFile);
void WINAPI FileBuilder_WriteVideoFrame(FFmpegFile* pFile, BOOL bKey, BYTE* pFrame, int nSize, ULONGLONG pts);

VideoDecoder* WINAPI VideoDecoder_Create(enum AVCodecID videoCodecID);
void WINAPI VideoDecoder_Close(VideoDecoder* pDecoder);
BOOL WINAPI VideoDecoder_Decode(VideoDecoder* pDecoder, void* pData, int nDataSize, void*& pFrameData, int& nFrameSize, int& nWidth, int& nHeight);
BOOL WINAPI VideoDecoder_DecodePacket(VideoDecoder* pDecoder, const AVPacket& packet, void*& pFrameData, int& nFrameSize, int& nWidth, int& nHeight);

StreamReader* WINAPI StreamReader_Create(const char* url, int timeout);
void WINAPI StreamReader_Close(StreamReader* pReader);
VideoDecoder* WINAPI StreamReader_GetVideoDecoder(StreamReader* pReader);
int WINAPI StreamReader_GetVideoStreamIndex(StreamReader* pReader);
int64_t WINAPI StreamReader_GetStreamStartTime(StreamReader* pReader, int nStreamIndex);
void WINAPI StreamReader_GetStreamTimeBase(StreamReader* pReader, int nStreamIndex, int& numerator, int& denominator);
BOOL WINAPI StreamReader_ReadPacketData(StreamReader* pReader, void*& pPacketData, int& nPacketSize, int& nStreamIndex, int64_t& llTimeStamp);
BOOL WINAPI StreamReader_ReadPacket(StreamReader* pReader, AVPacket*& pPacket, int& nStreamIndex, int64_t& llTimeStamp);
void WINAPI StreamReader_FreePacket(AVPacket* pPacket);

ImageScaler* WINAPI ImageScaler_Create(int srcWidth, int srcHeight, enum AVPixelFormat srcFormat, int dstWidth, int dstHeight, enum AVPixelFormat dstFormat, int flags);
void WINAPI ImageScaler_Close(ImageScaler* pScaler);
int WINAPI ImageScaler_Scale(ImageScaler* pScaler, const uint8_t *const srcSlice[], const int srcStride[], uint8_t *const dstSlice[], const int dstStride[]);
