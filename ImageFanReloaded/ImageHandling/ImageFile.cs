using System;
using System.IO;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling;

public class ImageFile : ImageFileBase
{
	public ImageFile(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer,
		ImageFileData imageFileData,
		IImageOrientationHandler imageOrientationHandler)
		: base(globalParameters, imageResizer, imageFileData)
	{
		_imageOrientationHandler = imageOrientationHandler;

		_applyImageOrientation = (image) =>
		{
			var imageOrientation = _imageOrientationHandler.GetImageOrientation(image);

			_imageOrientationHandler.ApplyImageOrientation(image, imageOrientation);
		};
	}

	#region Protected

	protected override IImage GetImageFromDisc(bool applyImageOrientation)
	{
		if (applyImageOrientation)
		{
			try
			{
				return BuildAndTransformImage(ImageFileData.ImageFilePath, _applyImageOrientation);
			}
			catch
			{
				return BuildImageFromFile(ImageFileData.ImageFilePath);
			}
		}

		if (IsAvaloniaSupportedImageFileExtension)
		{
			return BuildImageFromFile(ImageFileData.ImageFilePath);
		}
		else
		{
			return BuildAndTransformImage(ImageFileData.ImageFilePath, default);
		}
	}

	#endregion

	#region Private

	private readonly IImageOrientationHandler _imageOrientationHandler;

	private readonly Action<SixLabors.ImageSharp.Image> _applyImageOrientation;

	private bool IsAvaloniaSupportedImageFileExtension
		=> _globalParameters.DirectlySupportedImageFileExtensions.Contains(
			ImageFileData.ImageFileExtension);

	private static IImage BuildAndTransformImage(
		string inputFilePath,
		Action<SixLabors.ImageSharp.Image>? transformImage)
	{
		var image = SixLabors.ImageSharp.Image.Load(inputFilePath);

		if (transformImage is not null)
		{
			transformImage(image);
		}

		using var imageStream = new MemoryStream();
		SixLabors.ImageSharp.ImageExtensions.SaveAsJpeg(image, imageStream);

		return BuildImageFromStream(imageStream);
	}

	private static Image BuildImageFromFile(string inputFilePath)
	{
		var bitmap = new Avalonia.Media.Imaging.Bitmap(inputFilePath);

		return BuildImage(bitmap);
	}

	private static Image BuildImageFromStream(Stream inputStream)
	{
		inputStream.Reset();
		var bitmap = new Avalonia.Media.Imaging.Bitmap(inputStream);

		return BuildImage(bitmap);
	}

	private static Image BuildImage(Avalonia.Media.Imaging.Bitmap bitmap)
	{
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);

		return new Image(bitmap, bitmapSize);
	}

	#endregion
}
