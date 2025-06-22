using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
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

		BuildGeneralInfo(imageFileData, imageInfoBuilder);

		if (imageSize is not null)
		{
			imageInfoBuilder.AppendLine($"\tImage size:\t{imageSize} pixels");
		}

		var imageInfo = imageInfoBuilder.ToString();
		return imageInfo;
	}

	private static async Task<string> BuildExtendedImageInfo(
		ImageFileData imageFileData, ImageSize imageSize)
		=> await Task.Run(() => BuildExtendedImageInfoInternal(imageFileData, imageSize));

	private static string BuildExtendedImageInfoInternal(
		ImageFileData imageFileData, ImageSize imageSize)
	{
		var imageInfoBuilder = new StringBuilder();

		BuildGeneralInfo(imageFileData, imageInfoBuilder);

		var image = new MagickImage(imageFileData.ImageFilePath);

		imageInfoBuilder.AppendLine($"\tImage size:\t{imageSize} pixels");
		imageInfoBuilder.AppendLine($"\tImage format:\t{image.Format}");

		BuildExifInfo(image, imageInfoBuilder);
		BuildIptcInfo(image, imageInfoBuilder);

		var imageInfo = imageInfoBuilder.ToString();
		return imageInfo;
	}

	private static void BuildGeneralInfo(
		ImageFileData imageFileData,
		StringBuilder imageInfoBuilder)
	{
		imageInfoBuilder.AppendLine("General info");
		imageInfoBuilder.AppendLine();
		imageInfoBuilder.AppendLine($"\tFile name:\t{imageFileData.ImageFileName}");
		imageInfoBuilder.AppendLine($"\tFile path:\t{imageFileData.ImageFilePath}");
		imageInfoBuilder.AppendLine($"\tFile size:\t{imageFileData.SizeOnDiscInKilobytes} KB");
	}

	private static void BuildExifInfo(
		MagickImage image, StringBuilder imageInfoBuilder)
	{
		var exifProfile = image.GetExifProfile();

		if (exifProfile is not null)
		{
			var metadataValuePairs = exifProfile.Values
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
		MagickImage image, StringBuilder imageInfoBuilder)
	{
		var iptcProfile = image.GetIptcProfile();

		if (iptcProfile is not null)
		{
			var metadataValuePairs = iptcProfile.Values
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
