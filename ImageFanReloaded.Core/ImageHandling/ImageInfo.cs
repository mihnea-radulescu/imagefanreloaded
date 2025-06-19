namespace ImageFanReloaded.Core.ImageHandling;

public record ImageInfo
{
	public ImageInfo(string? imageFormat, ushort? imageOrientation, string imageInfoText)
	{
		ImageFormat = imageFormat;
		ImageOrientation = imageOrientation ?? DefaultImageOrientation;
		ImageInfoText = imageInfoText;
	}

	public string? ImageFormat { get; }
	public ushort ImageOrientation { get; }
	public string ImageInfoText { get; }

	public bool IsChangeImageOrientationRequired => ImageOrientation != DefaultImageOrientation;

	#region Private

	private const ushort DefaultImageOrientation = 1;

	#endregion
}
