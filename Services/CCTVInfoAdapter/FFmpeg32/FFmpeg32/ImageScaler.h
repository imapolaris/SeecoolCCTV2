#pragma once

class ImageScaler
{
public:
	ImageScaler(int srcWidth, int srcHeight, enum AVPixelFormat srcFormat, int dstWidth, int dstHeight, enum AVPixelFormat dstFormat, int flags);
	~ImageScaler();

	static ImageScaler* CreateInstance(int srcWidth, int srcHeight, enum AVPixelFormat srcFormat, int dstWidth, int dstHeight, enum AVPixelFormat dstFormat, int flags);

	BOOL IsOpen() { return m_context != NULL; }

	int Scale(const uint8_t *const srcSlice[], const int srcStride[], uint8_t *const dstSlice[], const int dstStride[]);

protected:
	SwsContext*	m_context;
	int m_nSrcWidth;
	int m_nSrcHeight;
	int m_nDstWidth;
	int m_nDstHeight;
};

