#include "stdafx.h"
#include "StreamReader.h"

StreamReader* StreamReader::CreateInstance(const char* url, int timeout)
{
	StreamReader* pReader = new StreamReader(url, timeout);
	if (pReader->IsOpen())
		return pReader;
	else
	{
		delete pReader;
		return NULL;
	}
}

StreamReader::StreamReader(const char* url, int timeout)
{
	_isOpen = FALSE;
	_vs = -1;
	_as = -1;
	_vidDecoder = NULL;

	_start_time = system_clock::now();
	_timeout = milliseconds(timeout);

	_pfc = avformat_alloc_context();
	const AVIOInterruptCB int_cb = { io_interrupt_callback, this };
	_pfc->interrupt_callback = int_cb;

	if (avformat_open_input(&_pfc, url, NULL, NULL) >= 0)
	{
		AVCodec* pVideoCodec = NULL;
		_vs = av_find_best_stream(_pfc, AVMEDIA_TYPE_VIDEO, -1, -1, &pVideoCodec, 0);

		if (_vs >= 0 && NULL != pVideoCodec)
			_vidDecoder = new VideoDecoder(_pfc->streams[_vs]->codec, pVideoCodec);

		AVCodec* pAudioCodec = NULL;
		_as = av_find_best_stream(_pfc, AVMEDIA_TYPE_AUDIO, -1, -1, &pAudioCodec, 0);
	}
	else
		avformat_close_input(&_pfc);
}

StreamReader::~StreamReader()
{
	if (NULL != _vidDecoder)
		delete _vidDecoder;

	avformat_close_input(&_pfc);
}

int StreamReader::io_interrupt()
{
	system_clock::time_point now = system_clock::now();
	system_clock::duration duration = now - _start_time;
	BOOL b = duration > _timeout;
	return b;
}

BOOL StreamReader::ReadPacketData(void*& pPacketData, int& nPacketSize, int& nStreamIndex, int64_t& llTimeStamp)
{
	AVPacket* pPacket = NULL;
	if (ReadPacket(pPacket, nStreamIndex, llTimeStamp))
	{
		nPacketSize = pPacket->size;
		pPacketData = malloc(nPacketSize);
		memcpy(pPacketData, pPacket->data, nPacketSize);
		FreePacket(pPacket);

		return TRUE;
	}
	else
		return FALSE;
}

BOOL StreamReader::ReadPacket(AVPacket*& pPacket, int& nStreamIndex, int64_t& llTimeStamp)
{
	pPacket = new AVPacket();
	av_init_packet(pPacket);
	_start_time = system_clock::now();
	if (av_read_frame(_pfc, pPacket) >= 0)
	{
		nStreamIndex = pPacket->stream_index;
		llTimeStamp = pPacket->pts;
		return TRUE;
	}
	else
	{
		FreePacket(pPacket);
		pPacket = NULL;
		return FALSE;
	}
}

void StreamReader::FreePacket(AVPacket* pPacket)
{
	if (pPacket)
	{
		av_free_packet(pPacket);
		delete pPacket;
	}
}
