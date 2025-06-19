using System.Collections.Generic;
using System.IO;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling;

public class ImageFile : ImageFileBase
{
	static ImageFile()
	{
		AvaloniaUnsupportedImageFormats = ["pbm", "qoi", "tga", "tiff"];
	}

	public ImageFile(
		IGlobalParameters globalParameters,
		IImageResizer imageResizer,
		string imageFileName,
		string imageFilePath,
		decimal sizeOnDiscInKilobytes,
		IImageInfoBuilder imageInfoBuilder)
		: base(globalParameters, imageResizer, imageFileName, imageFilePath, sizeOnDiscInKilobytes)
	{
		_imageInfoBuilder = imageInfoBuilder;
	}

	#region Protected

	public override ImageInfo? ImageInfo { get; protected set; }

	protected override IImage GetImageFromDisc(string imageFilePath)
	{
		try
		{
			var imageFileContent = File.ReadAllBytes(imageFilePath);
			using var readOnlyImageStream = new MemoryStream(
				imageFileContent, 0, imageFileContent.Length, false, false);

			var image = SixLabors.ImageSharp.Image.Load(readOnlyImageStream);

			ImageInfo = _imageInfoBuilder.BuildImageInfo(
				ImageFileName, ImageFilePath, SizeOnDiscInKilobytes, image);

			var imageFormat = ImageInfo.ImageFormat;

			if (IsAvaloniaSupportedImageFormat(imageFormat))
			{
				return BuildImageFromStream(readOnlyImageStream);
			}
			else
			{
				using var readWriteImageStream = new MemoryStream();
				SixLabors.ImageSharp.ImageExtensions.SaveAsJpeg(image, readWriteImageStream);

				return BuildImageFromStream(readWriteImageStream);
			}
		}
		catch
		{
			ImageInfo = _imageInfoBuilder.BuildImageInfo(
				ImageFileName, ImageFilePath, SizeOnDiscInKilobytes, default);

			throw;
		}
	}

	#endregion

	#region Private

	private static readonly HashSet<string> AvaloniaUnsupportedImageFormats;

	private readonly IImageInfoBuilder _imageInfoBuilder;

	private static bool IsAvaloniaSupportedImageFormat(string? imageFormat)
		=> imageFormat is null || !AvaloniaUnsupportedImageFormats.Contains(imageFormat);

	private static Image BuildImageFromStream(Stream inputStream)
	{
		inputStream.Reset();

		var bitmap = new Avalonia.Media.Imaging.Bitmap(inputStream);
		var bitmapSize = new ImageSize(bitmap.Size.Width, bitmap.Size.Height);

		var imageFromStream = new Image(bitmap, bitmapSize);
		return imageFromStream;
	}

	#endregion
}
