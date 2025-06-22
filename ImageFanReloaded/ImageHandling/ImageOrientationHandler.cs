using System.Collections.Generic;
using System.Linq;
using ImageMagick;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling;

public class ImageOrientationHandler : IImageOrientationHandler
{
	static ImageOrientationHandler()
	{
		ImageOrientationsToProcess = [2, 3, 4, 5, 6, 7, 8];
	}

	public void ApplyImageOrientation(object imageObject)
	{
		var image = (MagickImage)imageObject;

		var imageOrientation = GetImageOrientation(image);

		if (ShouldUpdateImage(imageOrientation))
		{
			UpdateImage(image, imageOrientation);
		}
	}

	#region Private

	private const string ExifOrientationTag = "Orientation";
	private const ushort DefaultImageOrientation = 1;

	private static readonly HashSet<ushort> ImageOrientationsToProcess;

	private static ushort GetImageOrientation(MagickImage image)
	{
		var exifProfile = image.GetExifProfile();

		if (exifProfile is null)
		{
			return DefaultImageOrientation;
		}

		var imageOrientation = exifProfile.Values
			.FirstOrDefault(anExifValue => anExifValue.Tag.ToString() == ExifOrientationTag);

		if (imageOrientation is not null)
		{
			var imageOrientationValue = imageOrientation.GetValue();

			if (imageOrientationValue is not null)
			{
				var imageOrientationNumericValue = (ushort)imageOrientationValue;

				return imageOrientationNumericValue;
			}
		}

		return DefaultImageOrientation;
	}

	private static bool ShouldUpdateImage(ushort imageOrientation)
		=> ImageOrientationsToProcess.Contains(imageOrientation);

	private static void UpdateImage(MagickImage image, ushort imageOrientation)
	{
		switch (imageOrientation)
		{
			case 2:
				image.Flop();
				break;

			case 3:
				image.Rotate(180);
				break;

			case 4:
				image.Flip();
				break;

			case 5:
				image.Rotate(90);
				image.Flop();
				break;

			case 6:
				image.Rotate(90);
				break;

			case 7:
				image.Rotate(270);
				image.Flop();
				break;

			case 8:
				image.Rotate(270);
				break;
		}
	}

	#endregion
}
