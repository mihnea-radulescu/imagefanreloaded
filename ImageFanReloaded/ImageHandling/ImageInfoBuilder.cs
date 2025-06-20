using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling;

public class ImageInfoBuilder : IImageInfoBuilder
{
	public async Task<string> BuildImageInfo(IImageFile imageFile)
	{
		if (imageFile.HasReadImageError)
		{
			return BuildBasicImageInfo(imageFile.ImageFileData, default);
		}

		try
		{
			return await BuildExtendedImageInfo(imageFile.ImageFileData, imageFile.ImageSize);
		}
		catch
		{
			return BuildBasicImageInfo(imageFile.ImageFileData, imageFile.ImageSize);
		}
	}

	#region Private

	private static string BuildBasicImageInfo(ImageFileData imageFileData, ImageSize? imageSize)
	{
		var imageInfoBuilder = new StringBuilder();

		BuildGeneralInfoCommonEntries(imageFileData, imageInfoBuilder);

		if (imageSize is not null)
		{
			imageInfoBuilder.AppendLine($"\tImage size:\t{imageSize} pixels");
		}

		var imageInfo = imageInfoBuilder.ToString();
		return imageInfo;
	}

	private static async Task<string> BuildExtendedImageInfo(
		ImageFileData imageFileData, ImageSize imageSize)
	{
		var image = await SixLabors.ImageSharp.Image.LoadAsync(imageFileData.ImageFilePath);
		
		var imageInfoBuilder = new StringBuilder();
		
		BuildGeneralInfoCommonEntries(imageFileData, imageInfoBuilder);

		imageInfoBuilder.AppendLine($"\tImage size:\t{imageSize} pixels");

		var bitsPerPixel = image.PixelType.BitsPerPixel;
		imageInfoBuilder.AppendLine($"\tBits per pixel:\t{bitsPerPixel}");

		var imageMetadata = image.Metadata;

		BuildImageResolutionInfo(imageMetadata, imageInfoBuilder);
		BuildImageFormatInfo(imageMetadata, imageInfoBuilder);
		BuildExifInfo(imageMetadata, imageInfoBuilder);
		BuildIptcInfo(imageMetadata, imageInfoBuilder);

		var imageInfo = imageInfoBuilder.ToString();
		return imageInfo;
	}

	private static void BuildGeneralInfoCommonEntries(
		ImageFileData imageFileData,
		StringBuilder imageInfoBuilder)
	{
		imageInfoBuilder.AppendLine("General info");
		imageInfoBuilder.AppendLine();
		imageInfoBuilder.AppendLine($"\tFile name:\t{imageFileData.ImageFileName}");
		imageInfoBuilder.AppendLine($"\tFile path:\t{imageFileData.ImageFilePath}");
		imageInfoBuilder.AppendLine($"\tFile size:\t{imageFileData.SizeOnDiscInKilobytes} KB");
	}

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
			var imageFormat = imageMetadata.DecodedImageFormat?.Name.ToLower();
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

	#endregion
}
