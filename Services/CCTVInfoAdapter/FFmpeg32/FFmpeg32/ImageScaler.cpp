#include "stdafx.h"
#include "ImageScaler.h"


ImageScaler::ImageScaler(int srcWidth, int srcHeight, enum AVPixelFormat srcFormat, int dstWidth, int dstHeight, enum AVPixelFormat dstFormat, int flags)
{
	m_nSrcWidth = srcWidth;
	m_nSrcHeight = srcHeight;
	m_nDstWidth = dstWidth;
	m_nDstHeight = dstHeight;
	if (sws_isSupportedInput(srcFormat) && sws_isSupportedOutput(dstFormat))
		m_context = sws_getContext(srcWidth, srcHeight, srcFormat, dstWidth, dstHeight, dstFormat, flags, NULL, NULL, NULL);
	else
		m_context = NULL;
}

ImageScaler::~ImageScaler()
{
	sws_freeContext(m_context);
	m_context = NULL;
}

ImageScaler* ImageScaler::CreateInstance(int srcWidth, int srcHeight, enum AVPixelFormat srcFormat, int dstWidth, int dstHeight, enum AVPixelFormat dstFormat, int flags)
{
	ImageScaler* pScaler = new ImageScaler(srcWidth, srcHeight, srcFormat, dstWidth, dstHeight, dstFormat, flags);
	if (pScaler->IsOpen())
		return pScaler;
	else
	{
		delete pScaler;
		return NULL;
	}
}

int ImageScaler::Scale(const uint8_t *const srcSlice[], const int srcStride[], uint8_t *const dstSlice[], const int dstStride[])
{
	if (m_context != NULL)
		return sws_scale(m_context, srcSlice, srcStride, 0, m_nSrcHeight, dstSlice, dstStride);
	else
		return 0;
}