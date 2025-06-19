using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling;

public class ImageOrientationHandler : IImageOrientationHandler
{
	public void ChangeImageOrientation(object imageObject, ushort imageOrientation)
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
}
