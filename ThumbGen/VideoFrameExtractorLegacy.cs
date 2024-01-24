using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;

namespace ThumbGen
{
	/*public class VideoFrameExtractionException : Exception
    {
		public VideoFrameExtractionException(string message) : base(message) { }
    }

	public unsafe class VideoFrameExtractor
	{
		protected AVFormatContext* avfCtx;
		protected AVCodecParameters* avcCtx;
		protected AVStream** streams;
		protected int vStreamIndex;
		protected int aStreamIndex;

		public string Filename { get; set; }
		public long Duration { get; set; }
		public long Bitrate { get; set; }
		public uint NumberOfStreams { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public string VideoCodec { get; set; }
		public string VideoCodecLongName { get; set; }
		public string AudioCodec { get; set; }
		public string AudioCodecLongName { get; set; }
		public int FPS { get; set; }

		public object Orientation { get; set; }

		public static VideoFrameExtractor FromFile(string filename)
        {
			VideoFrameExtractor vfe = new VideoFrameExtractor();
			vfe.InitFile(filename);
        }

        internal unsafe void InitFile(string filename)
        {
			Filename = filename;
			avfCtx = ffmpeg.avformat_alloc_context();

			//cfn:= C.CString(fn)
			//defer C.free(unsafe.Pointer(cfn))
			fixed (AVFormatContext** avfCtxPtr = &avfCtx) {
				if (ffmpeg.avformat_open_input(avfCtxPtr, filename, null, null) != 0)
				{
					throw new VideoFrameExtractionException("Can't open input stream");
				}
			}
					defer func()
					{
						if err != nil {
							C.avformat_close_input(&avfCtx)
						}
					} ()
			if (ffmpeg.avformat_find_stream_info(avfCtx, null) < 0) 
			{
				throw new VideoFrameExtractionException("Can't get stream info");
			}
			Duration = avfCtx->duration / 1000;
			Bitrate = avfCtx->bit_rate / 1000;
			NumberOfStreams = avfCtx->nb_streams;
			//hdr:= reflect.SliceHeader{
			//			Data: uintptr(unsafe.Pointer(avfCtx.streams)),
			//	Len: numberOfStreams,
			//	Cap: numberOfStreams,
			//}
			//var streams = *(*[] * C.struct_AVStream)(unsafe.Pointer(&hdr))
			streams = avfCtx->streams;
			vStreamIndex = -1;
			aStreamIndex = -1;
			for (int i = 0; i < NumberOfStreams; i++) {
				if (streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO) {
					vStreamIndex = i;
				}
				else if (streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO) {
					aStreamIndex = i;
				}
			}
			if (vStreamIndex == -1) {
				throw new VideoFrameExtractionException("no video stream");
			}
			var avcCtx = streams[vStreamIndex]->codecpar;
			var vCodec = ffmpeg.avcodec_find_decoder(avcCtx->codec_id);
			if (vCodec == null) {
				throw new VideoFrameExtractionException("can't find decoder");
			}

			//if (ffmpeg.avcodec_open2(avcCtx->, vCodec, null) != 0) {
			//	throw new VideoFrameExtractionException("can't initialize codec context");
			//}
			Width = avcCtx->width;
			Height = avcCtx->height;
			FPS = (streams[vStreamIndex]->avg_frame_rate.num / streams[vStreamIndex]->avg_frame_rate.den);
			VideoCodec = Marshal.PtrToStringAnsi((IntPtr)vCodec->name).ToUpper();
			VideoCodecLongName = Marshal.PtrToStringAnsi((IntPtr)vCodec->long_name);

			if (aStreamIndex != -1) {
				var aacCtx = streams[aStreamIndex]->codecpar;
				var aCodec = ffmpeg.avcodec_find_decoder(aacCtx->codec_id);
				if (aCodec != null) {
					AudioCodec = Marshal.PtrToStringAnsi((IntPtr)aCodec->name);
					AudioCodecLongName = Marshal.PtrToStringAnsi((IntPtr)aCodec->long_name);
				}
			}

			var displayMatrix = ffmpeg.av_stream_get_side_data(streams[vStreamIndex], AVPacketSideDataType.AV_PKT_DATA_DISPLAYMATRIX, null);
			var orientation = AVIdentity;
			if (displayMatrix != null) {
				hdr:= reflect.SliceHeader{
					Data: uintptr(unsafe.Pointer(displayMatrix)),
			Len: 9,
			Cap: 9,
		}
				matrix:= *(*[]C.int32_t)(unsafe.Pointer(&hdr))
				var matrix = new int[9];
				Marshal.Copy((IntPtr)displayMatrix, matrix, 0, 9);

				var det = matrix[0] * matrix[4] - matrix[1] * matrix[3];
				var matrixCopy = new int[9];
				matrix = matrixCopy;
				if (det < 0) {
					var orientation = AVFlipHorizontal;
					var flip = new int[] { -1, 0, 1 };
					for (int i = 0; i < 9; i++) 
					{
						matrix[i] *= flip[i % 3];
					}
				}
				if (matrix[1] == 1 << 16 && matrix[3] == -(1 << 16)) {
					orientation |= AVRotation90;
				}
				else if (matrix[0] == -(1 << 16) && matrix[4] == -(1 << 16)) 
				{
					if (det < 0) {
						orientation = AVFlipVertical;
					}
					else
					{
						orientation |= AVRotation180;
	  				}
				}
				else if (matrix[1] == -(1 << 16) && matrix[3] == 1 << 16) {
					orientation |= AVRotation270;
	  			}
				else if (matrix[0] == 1 << 16 && matrix[4] == 1 << 16) {
					orientation |= AVIdentity;
	  			}
				else
				{
					orientation |= AVRotationCustom;
	  			}
				if (orientation == AVRotation90 || orientation == AVRotation270) {
					var h = Height;
					Height = Width;
					Width = h;
				}
			}

			return new VideoFrameExtractor()
			{
		Orientation:        orientation,

			};
		}

		public Bitmap ExtractFrame()
        {

        }
    }*/
}
