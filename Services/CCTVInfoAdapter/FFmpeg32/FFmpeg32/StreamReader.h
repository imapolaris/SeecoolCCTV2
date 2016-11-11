#pragma once

#include <chrono>
using namespace std::chrono;

#include "VideoDecoder.h"

class StreamReader
{
public:
	StreamReader(const char* url, int timeout = 3000);
	~StreamReader();

	static StreamReader* CreateInstance(const char* url, int timeout = 3000);

	BOOL IsOpen() { return NULL != _pfc; }

	BOOL ReadPacketData(void*& pPacketData, int& nPacketSize, int& nStreamIndex, int64_t& llTimeStamp);

	BOOL ReadPacket(AVPacket*& pPacket, int& nStreamIndex, int64_t& llTimeStamp);
	static void FreePacket(AVPacket* pPacket);

	int GetVideoStreamIndex() { return _vs; }
	int GetAudioStreamIndex() { return _as; }

	VideoDecoder* GetVideoDecoder() { return _vidDecoder; }

	AVRational GetStreamTimeBase(int nStreamIndex) { const AVRational avTimeBase = { 1, AV_TIME_BASE }; return nStreamIndex < 0 ? avTimeBase : _pfc->streams[nStreamIndex]->time_base; }
	int64_t GetStreamStartTime(int nStreamIndex) { return nStreamIndex < 0 ? _pfc->start_time : _pfc->streams[nStreamIndex]->start_time; }

protected:
	system_clock::time_point _start_time;
	system_clock::duration _timeout;

	static int io_interrupt_callback(void * ctx)
	{
		return ((StreamReader*)ctx)->io_interrupt();
	}
	int io_interrupt();

	BOOL _isOpen;
	AVFormatContext* _pfc;
	int _vs;
	int _as;
	VideoDecoder* _vidDecoder;
};

