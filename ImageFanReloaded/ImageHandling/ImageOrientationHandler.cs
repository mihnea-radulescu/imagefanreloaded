using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
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
		var imageOrientation = GetImageOrientation(imageObject);

		if (ShouldUpdateImage(imageOrientation))
		{
			UpdateImage(imageObject, imageOrientation);
		}
	}

	#region Private

	private const string ExifOrientationTag = "Orientation";
	private const ushort DefaultImageOrientation = 1;

	private static readonly HashSet<ushort> ImageOrientationsToProcess;

	private static ushort GetImageOrientation(object imageObject)
	{
		var image = (Image)imageObject;

		if (image.Metadata.ExifProfile is null)
		{
			return DefaultImageOrientation;
		}

		var exifValues = image.Metadata.ExifProfile.Values;

		var imageOrientation = exifValues
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

	private static void UpdateImage(object imageObject, ushort imageOrientation)
	{
		var image = (Image)imageObject;

		switch (imageOrientation)
		{
			case 2:
				image.Mutate(context => context.Flip(FlipMode.Horizontal));
				break;

			case 3:
				image.Mutate(context => context.Rotate(RotateMode.Rotate180));
				break;

			case 4:
				image.Mutate(context => context.Flip(FlipMode.Vertical));
				break;

			case 5:
				image.Mutate(context => context.RotateFlip(RotateMode.Rotate90, FlipMode.Horizontal));
				break;

			case 6:
				image.Mutate(context => context.Rotate(RotateMode.Rotate90));
				break;

			case 7:
				image.Mutate(context => context.RotateFlip(RotateMode.Rotate270, FlipMode.Horizontal));
				break;

			case 8:
				image.Mutate(context => context.Rotate(RotateMode.Rotate270));
				break;
		}
	}

	#endregion
}
