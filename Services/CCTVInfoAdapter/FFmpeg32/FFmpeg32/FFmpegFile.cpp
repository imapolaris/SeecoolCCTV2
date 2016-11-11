#include "stdafx.h"
#include "FFmpegFile.h"

FFmpegFile::FFmpegFile(const char* sFileName, int nWidth, int nHeight, enum AVCodecID videoCodecID, int nBitRate)
{
	m_fc = NULL;
	m_vi = NULL;
	m_ptsFirst = 0;

	if (avformat_alloc_output_context2(&m_fc, NULL, NULL, sFileName) >= 0)
	{
		AVOutputFormat* of = m_fc->oformat;
		AVCodec* codec = avcodec_find_encoder(videoCodecID);
		AVStream* st = avformat_new_stream(m_fc, codec);
		st->id = m_fc->nb_streams - 1;
		AVCodecContext* cc = st->codec;
		avcodec_get_context_defaults3(cc, codec);
		m_vi = st->index;
		cc->codec_type = AVMEDIA_TYPE_VIDEO;
		cc->codec_id = videoCodecID;
		cc->bit_rate = nBitRate;
		cc->width = nWidth;
		cc->height = nHeight;
		cc->time_base.num = 1;
		cc->time_base.den = 25;
		cc->gop_size = 12;
		cc->pix_fmt = AV_PIX_FMT_YUV420P;

		if (of->flags & AVFMT_GLOBALHEADER)
			cc->flags |= CODEC_FLAG_GLOBAL_HEADER;

		avcodec_open2(cc, codec, NULL);

		av_dump_format(m_fc, 0, sFileName, 1);

		m_fc->pb = NULL;
		if (!(m_fc->oformat->flags & AVFMT_NOFILE))
			avio_open(&m_fc->pb, m_fc->filename, AVIO_FLAG_WRITE);

		avformat_write_header(m_fc, NULL);
	}
	else
		m_fc = NULL;
}


FFmpegFile::~FFmpegFile()
{
	if (m_fc)
	{
		if (m_fc->pb)
		{
			av_write_trailer(m_fc);
			avio_close(m_fc->pb);
		}

		if (m_fc->streams[m_vi] && m_fc->streams[m_vi]->codec)
			avcodec_close(m_fc->streams[m_vi]->codec);

		av_free(m_fc);
		m_fc = NULL;
	}
}

void FFmpegFile::WriteVideoFrame(BOOL bKey, BYTE* pFrame, int nSize, ULONGLONG pts)
{
	if (m_fc && m_fc->pb)
	{
		if (0 == m_ptsFirst)
			m_ptsFirst = pts;

		AVStream* st = m_fc->streams[m_vi];
		AVPacket pkt = { 0 };
		av_init_packet(&pkt);
		if (bKey)
			pkt.flags |= AV_PKT_FLAG_KEY;
		pkt.stream_index = st->index;
		pkt.data = (uint8_t*)pFrame;
		pkt.size = nSize;
		AVRational time_base;
		time_base.num = 1;
		time_base.den = 1000;
		pkt.dts = pkt.pts = av_rescale_q(pts - m_ptsFirst, time_base, st->time_base);
		av_write_frame(m_fc, &pkt);
	}
}
