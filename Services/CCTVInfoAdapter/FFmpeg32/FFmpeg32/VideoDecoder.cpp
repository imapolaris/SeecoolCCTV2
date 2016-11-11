#include "stdafx.h"
#include "VideoDecoder.h"


VideoDecoder::VideoDecoder(enum AVCodecID videoCodecID)
{
	m_bAttachedCodecContext = FALSE;
	AVCodec* pCodec = avcodec_find_decoder(videoCodecID);
	m_pCodecContext = avcodec_alloc_context3(pCodec);
	init(pCodec);
}

VideoDecoder::VideoDecoder(AVCodecContext* pCodecContext, const AVCodec* pCodec)
{
	m_pCodecContext = pCodecContext;
	m_bAttachedCodecContext = TRUE;
	init(pCodec);
}

void VideoDecoder::init(const AVCodec* pCodec)
{
	m_pFrame = av_frame_alloc();

	avcodec_open2(m_pCodecContext, pCodec, NULL);
}

VideoDecoder::~VideoDecoder()
{
	if (m_pCodecContext && !m_bAttachedCodecContext)
	{
		avcodec_close(m_pCodecContext);
		av_freep(m_pCodecContext);
	}
	m_pCodecContext = NULL;

	av_frame_free(&m_pFrame);
}

BOOL VideoDecoder::Decode(void* pData, int nDataSize, void*& pFrameData, int& nFrameSize, int& nWidth, int& nHeight)
{
	AVPacket packet;
	av_init_packet(&packet);
	packet.data = (uint8_t*)pData;
	packet.size = nDataSize;

	return Decode(packet, pFrameData, nFrameSize, nWidth, nHeight);
}

BOOL VideoDecoder::Decode(const AVPacket& packet, void*& pFrameData, int& nFrameSize, int& nWidth, int& nHeight)
{
	int got = 0;
	avcodec_decode_video2(m_pCodecContext, m_pFrame, &got, &packet);
	if (got)
	{
		nWidth = m_pFrame->width;
		nHeight = m_pFrame->height;
		nFrameSize = nWidth * nHeight * 3 / 2;
		pFrameData = malloc(nFrameSize);
		uint8_t* pDest = (uint8_t*)pFrameData;
		uint8_t* pSrc = m_pFrame->data[0];

		int nLineSize = nWidth;
		int nTempHeight = nHeight;
		for (int i = 0; i < nTempHeight; i++)
		{
			memcpy(pDest, pSrc, nLineSize);
			pDest += nLineSize;
			pSrc += m_pFrame->linesize[0];
		}

		nLineSize /= 2;
		nTempHeight /= 2;
		pSrc = m_pFrame->data[2];
		for (int i = 0; i < nTempHeight; i++)
		{
			memcpy(pDest, pSrc, nLineSize);
			pDest += nLineSize;
			pSrc += m_pFrame->linesize[2];
		}

		pSrc = m_pFrame->data[1];
		for (int i = 0; i < nTempHeight; i++)
		{
			memcpy(pDest, pSrc, nLineSize);
			pDest += nLineSize;
			pSrc += m_pFrame->linesize[1];
		}

		return TRUE;
	}
	else
		return FALSE;
}
