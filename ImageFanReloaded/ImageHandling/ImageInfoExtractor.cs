using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling;

public class ImageInfoExtractor : IImageInfoExtractor
{
	public async Task<string> BuildImageInfo(IImageFile imageFile)
	{
		var imageInfoBuilder = new StringBuilder();

		imageInfoBuilder.AppendLine("General info");
		imageInfoBuilder.AppendLine();
		imageInfoBuilder.AppendLine($"\tFile name:\t{imageFile.ImageFileName}");
		imageInfoBuilder.AppendLine($"\tFile path:\t{imageFile.ImageFilePath}");
		imageInfoBuilder.AppendLine($"\tFile size:\t{imageFile.SizeOnDiscInKilobytes} KB");

		(var bitsPerPixel, var imageMetadata) = await GetImageData(imageFile);

		if (bitsPerPixel is not null)
		{
			imageInfoBuilder.AppendLine($"\tImage size:\t{imageFile.ImageSize} pixels");
			imageInfoBuilder.AppendLine($"\tBits per pixel:\t{bitsPerPixel}");
		}

		if (imageMetadata is not null)
		{
			BuildExifInfo(imageMetadata, imageInfoBuilder);
			BuildIptcInfo(imageMetadata, imageInfoBuilder);
		}

		var imageInfo = imageInfoBuilder.ToString();
		return imageInfo;
	}

	#region Private

	private static async Task<(int? bitsPerPixel, ImageMetadata? imageMetadata)> GetImageData(
		IImageFile imageFile)
	{
		int? bitsPerPixel = null;
		ImageMetadata? imageMetadata = null;

		try
		{
			using (var image = await Image.LoadAsync(imageFile.ImageFilePath))
			{
				bitsPerPixel = image.PixelType.BitsPerPixel;
				imageMetadata = image.Metadata;
			}
		}
		catch
		{
		}

		return (bitsPerPixel, imageMetadata);
	}
	
	private static void BuildExifInfo(
		ImageMetadata imageMetadata, StringBuilder imageInfoBuilder)
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
					aMetadataValuePair.ValueContent is not null &&
					aMetadataValuePair.ValueContent is not Array)
				.ToList();

			if (metadataValuePairs.Any())
			{
				imageInfoBuilder.AppendLine();
				imageInfoBuilder.AppendLine("EXIF info");
				imageInfoBuilder.AppendLine();

				foreach (var aMetadataValuePair in metadataValuePairs)
				{
					imageInfoBuilder.AppendLine($"\t{aMetadataValuePair.ValueTag}:\t{aMetadataValuePair.ValueContent}");
				}
			}
		}
	}

	private static void BuildIptcInfo(
		ImageMetadata imageMetadata, StringBuilder imageInfoBuilder)
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

	#endregion
}
