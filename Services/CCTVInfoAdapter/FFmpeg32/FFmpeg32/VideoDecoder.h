#pragma once
class VideoDecoder
{
public:
	VideoDecoder(enum AVCodecID videoCodecID);
	VideoDecoder(AVCodecContext* pCodecContext, const AVCodec* pCodec);
	~VideoDecoder();

	BOOL Decode(void* pData, int nDataSize, void*& pFrameData, int& nFrameSize, int& nWidth, int& nHeight);
	BOOL Decode(const AVPacket& packet, void*& pFrameData, int& nFrameSize, int& nWidth, int& nHeight);

protected:
	AVCodecContext* m_pCodecContext;
	AVFrame* m_pFrame;
	BOOL m_bAttachedCodecContext;

protected:
	void init(const AVCodec* pCodec);
};

