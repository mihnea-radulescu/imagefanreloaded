using System;
using ImageMagick;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

namespace ImageFanReloaded.ImageHandling.Extensions;

public static class SaveFileImageFormatExtensions
{
	extension(ISaveFileImageFormat saveFileImageFormat)
	{
		public MagickFormat MagickFormat
		{
			get
			{
				var saveFileImageFormatType = saveFileImageFormat.GetType();

				if (saveFileImageFormatType == typeof(JpegSaveFileImageFormat))
				{
					return MagickFormat.Jpg;
				}

				if (saveFileImageFormatType == typeof(GifSaveFileImageFormat))
				{
					return MagickFormat.Gif;
				}

				if (saveFileImageFormatType == typeof(PngSaveFileImageFormat))
				{
					return MagickFormat.Png;
				}

				if (saveFileImageFormatType == typeof(WebpSaveFileImageFormat))
				{
					return MagickFormat.WebP;
				}

				if (saveFileImageFormatType == typeof(TiffSaveFileImageFormat))
				{
					return MagickFormat.Tif;
				}

				if (saveFileImageFormatType == typeof(BmpSaveFileImageFormat))
				{
					return MagickFormat.Bmp;
				}

				throw new NotSupportedException(
					"Save image format not supported.");
			}
		}
	}
}
