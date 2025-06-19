using System;
using System.Linq;
using System.Text;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling;

public class ImageInfoBuilder : IImageInfoBuilder
{
	public ImageInfo BuildImageInfo(
		string imageFileName,
		string imageFilePath,
		decimal sizeOnDiscInKilobytes,
		object? imageObject)
	{
		var imageInfoBuilder = new StringBuilder();

		imageInfoBuilder.AppendLine("General info");
		imageInfoBuilder.AppendLine();
		imageInfoBuilder.AppendLine($"\tFile name:\t{imageFileName}");
		imageInfoBuilder.AppendLine($"\tFile path:\t{imageFilePath}");
		imageInfoBuilder.AppendLine($"\tFile size:\t{sizeOnDiscInKilobytes} KB");

		var image = (SixLabors.ImageSharp.Image?)imageObject;
		var imageMetadata = image?.Metadata;

		if (image is not null)
		{
			var bitsPerPixel = image.PixelType.BitsPerPixel;

			imageInfoBuilder.AppendLine($"\tImage size:\t{image.Size.Width}x{image.Size.Height} pixels");
			imageInfoBuilder.AppendLine($"\tBits per pixel:\t{bitsPerPixel}");

			if (imageMetadata is not null)
			{
				BuildImageResolutionInfo(imageMetadata, imageInfoBuilder);
				BuildImageFormatInfo(imageMetadata, imageInfoBuilder);
				BuildExifInfo(imageMetadata, imageInfoBuilder);
				BuildIptcInfo(imageMetadata, imageInfoBuilder);
			}
		}

		var imageFormat = GetImageFormat(imageMetadata);
		var imageOrientation = GetImageOrientation(imageMetadata);
		var imageInfoText = imageInfoBuilder.ToString();

		var imageInfo = new ImageInfo(imageFormat, imageOrientation, imageInfoText);
		return imageInfo;
	}

	#region Private

	private const string ImageOrientationExifTag = "Orientation";

	private static void BuildImageResolutionInfo(
		SixLabors.ImageSharp.Metadata.ImageMetadata imageMetadata, StringBuilder imageInfoBuilder)
	{
		var horizontalResolution = imageMetadata.HorizontalResolution;
		var verticalResolution = imageMetadata.VerticalResolution;
		var resolutionUnits = Enum.GetName(imageMetadata.ResolutionUnits);

		imageInfoBuilder.AppendLine($"\tResolution:\t{horizontalResolution}x{verticalResolution}");
		imageInfoBuilder.AppendLine($"\tResolution units:\t{resolutionUnits}");
	}

	private static void BuildImageFormatInfo(
		SixLabors.ImageSharp.Metadata.ImageMetadata imageMetadata, StringBuilder imageInfoBuilder)
	{
		if (imageMetadata.DecodedImageFormat is not null)
		{
			var imageFormat = GetImageFormat(imageMetadata);
			imageInfoBuilder.AppendLine($"\tImage format:\t{imageFormat}");
		}
	}

	private static void BuildExifInfo(
		SixLabors.ImageSharp.Metadata.ImageMetadata imageMetadata, StringBuilder imageInfoBuilder)
	{
		if (imageMetadata.ExifProfile is not null)
		{
			var metadataValuePairs = imageMetadata.ExifProfile.Values
				.Select(aMetadataValue => new
				{
					ValueTag = aMetadataValue.Tag,
					ValueContent = aMetadataValue.GetValue()
				})
				.Where(aMetadataValuePair =>
					aMetadataValuePair.ValueContent is not null)
				.ToList();

			if (metadataValuePairs.Any())
			{
				imageInfoBuilder.AppendLine();
				imageInfoBuilder.AppendLine("EXIF info");
				imageInfoBuilder.AppendLine();

				foreach (var aMetadataValuePair in metadataValuePairs)
				{
					if (aMetadataValuePair.ValueContent is Array valueContentArray)
					{
						var valueContentText = GetValueContentText(valueContentArray);
						imageInfoBuilder.AppendLine($"\t{aMetadataValuePair.ValueTag}:\t[ {valueContentText} ]");
					}
					else
					{
						imageInfoBuilder.AppendLine($"\t{aMetadataValuePair.ValueTag}:\t{aMetadataValuePair.ValueContent}");
					}
				}
			}
		}
	}

	private static void BuildIptcInfo(
		SixLabors.ImageSharp.Metadata.ImageMetadata imageMetadata, StringBuilder imageInfoBuilder)
	{
		if (imageMetadata.IptcProfile is not null)
		{
			var metadataValuePairs = imageMetadata.IptcProfile.Values
				.Select(aMetadataValue => new
				{
					ValueTag = aMetadataValue.Tag,
					ValueContent = aMetadataValue.Value
				})
				.Where(aMetadataValuePair =>
					!aMetadataValuePair.ValueContent.Contains('\0'))
				.ToList();

			if (metadataValuePairs.Any())
			{
				imageInfoBuilder.AppendLine();
				imageInfoBuilder.AppendLine("IPTC info");
				imageInfoBuilder.AppendLine();

				foreach (var aMetadataValuePair in metadataValuePairs)
				{
					imageInfoBuilder.AppendLine($"\t{aMetadataValuePair.ValueTag}:\t{aMetadataValuePair.ValueContent}");
				}
			}
		}
	}

	private static string GetValueContentText(Array valueContentArray)
	{
		var valueContentItems = new object[valueContentArray.Length];
		Array.Copy(valueContentArray, valueContentItems, valueContentArray.Length);

		var valueContentText = string.Join(", ", valueContentItems);
		return valueContentText;
	}

	private static ushort? GetImageOrientation(
		SixLabors.ImageSharp.Metadata.ImageMetadata? imageMetadata)
	{
		if (imageMetadata is null || imageMetadata.ExifProfile is null)
		{
			return default;
		}

		var exifValues = imageMetadata.ExifProfile.Values;

		var imageOrientation = exifValues
			.FirstOrDefault(anExifValue => anExifValue.Tag.ToString() == ImageOrientationExifTag);

		if (imageOrientation is not null)
		{
			var imageOrientationValue = imageOrientation.GetValue();

			if (imageOrientationValue is not null)
			{
				var imageOrientationNumericValue = (ushort)imageOrientationValue;

				return imageOrientationNumericValue;
			}
		}

		return default;
	}

	private static string? GetImageFormat(SixLabors.ImageSharp.Metadata.ImageMetadata? imageMetadata)
		=> imageMetadata?.DecodedImageFormat?.Name.ToLower();

	#endregion
}
