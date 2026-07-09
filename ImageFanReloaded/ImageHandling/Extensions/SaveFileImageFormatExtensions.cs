using System;
using System.Threading.Tasks;
using ImageMagick;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

namespace ImageFanReloaded.ImageHandling.Extensions;

public static class SaveFileImageFormatExtensions
{
	extension(ISaveFileImageFormat saveFileImageFormat)
	{
		public async Task SaveImage(
			MagickImageCollection image, string imageFilePath)
		{
			if (saveFileImageFormat.DoesSupportAnimation)
			{
				await image.WriteAsync(
					imageFilePath, saveFileImageFormat.MagickFormat);
			}
			else
			{
				var singleFrameImage = image[0];

				await singleFrameImage.WriteAsync(
					imageFilePath, saveFileImageFormat.MagickFormat);
			}
		}

		private MagickFormat MagickFormat
		{
			get
			{
				var saveFileImageFormatType = saveFileImageFormat.GetType();

				if (saveFileImageFormatType == typeof(JpegSaveFileImageFormat))
				{
					return MagickFormat.Jpg;
				}

				if (saveFileImageFormatType == typeof(WebpSaveFileImageFormat))
				{
					return MagickFormat.WebP;
				}

				if (saveFileImageFormatType == typeof(AvifSaveFileImageFormat))
				{
					return MagickFormat.Avif;
				}

				if (saveFileImageFormatType == typeof(JpegXlSaveFileImageFormat))
				{
					return MagickFormat.Jxl;
				}

				if (saveFileImageFormatType == typeof(GifSaveFileImageFormat))
				{
					return MagickFormat.Gif;
				}

				if (saveFileImageFormatType == typeof(PngSaveFileImageFormat))
				{
					return MagickFormat.Png;
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
