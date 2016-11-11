#pragma once
class FFmpegFile
{
public:
	FFmpegFile(const char* sFileName, int nWidth, int nHeight, enum AVCodecID videoCodecID, int nBitRate);
	~FFmpegFile();

	void WriteVideoFrame(BOOL bKey, BYTE* pFrame, int nSize, ULONGLONG pts);

protected:
	AVFormatContext* m_fc;
	int m_vi;
	ULONGLONG m_ptsFirst;
};

