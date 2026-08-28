using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageMagick;

namespace ImageFanReloaded.ImageHandling.Extensions;

public static class ImageFileDataExtensions
{
	extension(IImageFileData imageFileData)
	{
		public MagickFormat MagickFormat
		{
			get
			{
				var normalizedFileExtension =
					imageFileData.FileExtension.ToLowerInvariant();

				return normalizedFileExtension switch
				{
					".cur" => MagickFormat.Cur,
					".dng" => MagickFormat.Dng,
					".ico" => MagickFormat.Ico,
					".nrw" => MagickFormat.Nrw,
					".pef" => MagickFormat.Pef,
					".pict" => MagickFormat.Pict,
					".tga" => MagickFormat.Tga,
					".wbmp" => MagickFormat.Wbmp,
					_ => MagickFormat.Unknown
				};
			}
		}
	}
}
