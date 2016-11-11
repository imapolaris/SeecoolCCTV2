// FFmpeg32.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "FFmpeg32.h"

void WINAPI FFmpeg_Init()
{
	av_register_all();
	avformat_network_init();
}

void WINAPI FFmpeg_FreeMemory(void* data)
{
	free(data);
}

FFmpegFile* WINAPI FileBuilder_Create(const char* sFileName, int nWidth, int nHeight, enum AVCodecID videoCodecID, int nBitRate)
{
	return new FFmpegFile(sFileName, nWidth, nHeight, videoCodecID, nBitRate);
}

void WINAPI FileBuilder_Close(FFmpegFile* pFile)
{
	delete pFile;
}

void WINAPI FileBuilder_WriteVideoFrame(FFmpegFile* pFile, BOOL bKey, BYTE* pFrame, int nSize, ULONGLONG pts)
{
	pFile->WriteVideoFrame(bKey, pFrame, nSize, pts);
}

VideoDecoder* WINAPI VideoDecoder_Create(enum AVCodecID videoCodecID)
{
	return new VideoDecoder(videoCodecID);
}

void WINAPI VideoDecoder_Close(VideoDecoder* pDecoder)
{
	delete pDecoder;
}

BOOL WINAPI VideoDecoder_Decode(VideoDecoder* pDecoder, void* pData, int nDataSize, void*& pFrameData, int& nFrameSize, int& nWidth, int& nHeight)
{
	return pDecoder->Decode(pData, nDataSize, pFrameData, nFrameSize, nWidth, nHeight);
}

BOOL WINAPI VideoDecoder_DecodePacket(VideoDecoder* pDecoder, const AVPacket& packet, void*& pFrameData, int& nFrameSize, int& nWidth, int& nHeight)
{
	return pDecoder->Decode(packet, pFrameData, nFrameSize, nWidth, nHeight);
}

StreamReader* WINAPI StreamReader_Create(const char* url, int timeout)
{
	return StreamReader::CreateInstance(url, timeout);
}

void WINAPI StreamReader_Close(StreamReader* pReader)
{
	delete pReader;
}

VideoDecoder* WINAPI StreamReader_GetVideoDecoder(StreamReader* pReader)
{
	return pReader->GetVideoDecoder();
}

int WINAPI StreamReader_GetVideoStreamIndex(StreamReader* pReader)
{
	return pReader->GetVideoStreamIndex();
}

int64_t WINAPI StreamReader_GetStreamStartTime(StreamReader* pReader, int nStreamIndex)
{
	return pReader->GetStreamStartTime(nStreamIndex);
}

void WINAPI StreamReader_GetStreamTimeBase(StreamReader* pReader, int nStreamIndex, int& numerator, int& denominator)
{
	AVRational rational = pReader->GetStreamTimeBase(nStreamIndex);
	numerator = rational.num;
	denominator = rational.den;
}

BOOL WINAPI StreamReader_ReadPacketData(StreamReader* pReader, void*& pPacketData, int& nPacketSize, int& nStreamIndex, int64_t& llTimeStamp)
{
	return pReader->ReadPacketData(pPacketData, nPacketSize, nStreamIndex, llTimeStamp);
}

BOOL WINAPI StreamReader_ReadPacket(StreamReader* pReader, AVPacket*& pPacket, int& nStreamIndex, int64_t& llTimeStamp)
{
	return pReader->ReadPacket(pPacket, nStreamIndex, llTimeStamp);
}

void WINAPI StreamReader_FreePacket(AVPacket* pPacket)
{
	StreamReader::FreePacket(pPacket);
}

ImageScaler* WINAPI ImageScaler_Create(int srcWidth, int srcHeight, enum AVPixelFormat srcFormat, int dstWidth, int dstHeight, enum AVPixelFormat dstFormat, int flags)
{
	return ImageScaler::CreateInstance(srcWidth, srcHeight, srcFormat, dstWidth, dstHeight, dstFormat, flags);
}

void WINAPI ImageScaler_Close(ImageScaler* pScaler)
{
	delete pScaler;
}

int WINAPI ImageScaler_Scale(ImageScaler* pScaler, const uint8_t *const srcSlice[], const int srcStride[], uint8_t *const dstSlice[], const int dstStride[])
{
	return pScaler->Scale(srcSlice, srcStride, dstSlice, dstStride);
}
