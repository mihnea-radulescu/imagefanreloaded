using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ImageMagick;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling;

public class ImageInfoBuilder : IImageInfoBuilder
{
	public ImageInfoBuilder(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	public async Task<string> BuildImageInfo(IImageFile imageFile)
		=> await Task.Run(() => BuildImageInfoInternal(imageFile));

	#region Private

	private readonly IGlobalParameters _globalParameters;

	private string BuildImageInfoInternal(IImageFile imageFile)
	{
		if (imageFile.HasImageReadError)
		{
			return BuildBasicImageInfo(imageFile);
		}
		else
		{
			try
			{
				return BuildExtendedImageInfo(imageFile);
			}
			catch
			{
				return BuildBasicImageInfo(imageFile);
			}
		}
	}

	private string BuildBasicImageInfo(IImageFile imageFile)
	{
		var imageInfoBuilder = new StringBuilder();

		BuildFileProfile(imageFile, imageInfoBuilder);

		var imageInfo = imageInfoBuilder.ToString();
		return imageInfo;
	}

	private string BuildExtendedImageInfo(IImageFile imageFile)
	{
		var imageInfoBuilder = new StringBuilder();

		var staticImageFileData = imageFile.StaticImageFileData;
		IMagickImage image = new MagickImage(staticImageFileData.ImageFilePath);

		BuildFileProfile(imageFile, imageInfoBuilder);

		var imageSize = imageFile.ImageSize;
		var isAnimatedImage = imageFile.IsAnimatedImage;
		BuildImageProfile(image, imageSize, imageInfoBuilder, isAnimatedImage);

		BuildColorProfile(image, imageInfoBuilder);

		BuildExifProfile(image, imageInfoBuilder);
		BuildIptcProfile(image, imageInfoBuilder);
		BuildXmpProfile(image, imageInfoBuilder);

		var imageInfo = imageInfoBuilder.ToString();
		return imageInfo;
	}

	private void BuildFileProfile(IImageFile imageFile, StringBuilder imageInfoBuilder)
	{
		imageInfoBuilder.AppendLine("File profile");
		imageInfoBuilder.AppendLine();

		var staticImageFileData = imageFile.StaticImageFileData;
		imageInfoBuilder.AppendLine($"\tFile name:\t{staticImageFileData.ImageFileName}");
		imageInfoBuilder.AppendLine($"\tFile path:\t{staticImageFileData.ImageFilePath}");

		var transientImageFileData = imageFile.TransientImageFileData;

		var sizeOnDiscInKilobytes = transientImageFileData.SizeOnDiscInKilobytes.GetValueOrDefault();
		var sizeOnDiscInKilobytesForDisplay = decimal.Round(
			sizeOnDiscInKilobytes, _globalParameters.DecimalDigitCountForDisplay);
		imageInfoBuilder.AppendLine($"\tFile size:\t{sizeOnDiscInKilobytesForDisplay} KB");

		var lastModificationTime = transientImageFileData.LastModificationTime;
		var lastModificationTimeText = lastModificationTime.HasValue
			? lastModificationTime.ToString()
			: "-";
		imageInfoBuilder.AppendLine($"\tFile last modification time:\t{lastModificationTimeText}");
	}

	private static void BuildImageProfile(
		IMagickImage image,
		ImageSize imageSize,
		StringBuilder imageInfoBuilder,
		bool isAnimatedImage)
	{
		imageInfoBuilder.AppendLine();
		imageInfoBuilder.AppendLine("Image profile");
		imageInfoBuilder.AppendLine();

		imageInfoBuilder.AppendLine($"\tImage size:\t{imageSize}");
		imageInfoBuilder.AppendLine($"\tImage format:\t{image.Format}");
		imageInfoBuilder.AppendLine($"\tImage depth:\t{image.Depth} bpp");

		var isAnimatedImageInfo = isAnimatedImage ? "yes" : "no";
		imageInfoBuilder.AppendLine($"\tImage animation:\t{isAnimatedImageInfo}");
	}

	private static void BuildColorProfile(IMagickImage image, StringBuilder imageInfoBuilder)
	{
		var colorProfile = image.GetColorProfile();

		if (colorProfile is not null)
		{
			imageInfoBuilder.AppendLine();
			imageInfoBuilder.AppendLine("Color profile");
			imageInfoBuilder.AppendLine();

			imageInfoBuilder.AppendLine($"\tName:\t{colorProfile.Name}");
			imageInfoBuilder.AppendLine($"\tColor space:\t{colorProfile.ColorSpace.ToString()}");

			if (colorProfile.Manufacturer is not null)
			{
				imageInfoBuilder.AppendLine($"\tManufacturer:\t{colorProfile.Manufacturer}");
			}

			if (colorProfile.Model is not null)
			{
				imageInfoBuilder.AppendLine($"\tModel:\t{colorProfile.Model}");
			}

			if (colorProfile.Description is not null)
			{
				imageInfoBuilder.AppendLine($"\tDescription:\t{colorProfile.Description}");
			}

			if (colorProfile.Copyright is not null)
			{
				imageInfoBuilder.AppendLine($"\tCopyright:\t{colorProfile.Copyright}");
			}
		}
	}

	private static void BuildExifProfile(IMagickImage image, StringBuilder imageInfoBuilder)
	{
		var exifProfile = image.GetExifProfile();

		if (exifProfile is not null)
		{
			var valueTagValueContentPairs = exifProfile.Values
				.Select(aValueTagValueContentPair => new
				{
					ValueTag = aValueTagValueContentPair.Tag,
					ValueContent = aValueTagValueContentPair.GetValue()
				})
				.Where(aValueTagValueContentPair =>
					aValueTagValueContentPair.ValueContent is not null)
				.ToList();

			if (valueTagValueContentPairs.Any())
			{
				imageInfoBuilder.AppendLine();
				imageInfoBuilder.AppendLine("EXIF profile");
				imageInfoBuilder.AppendLine();

				foreach (var aValueTagValueContentPair in valueTagValueContentPairs)
				{
					if (aValueTagValueContentPair.ValueContent is Array valueContentArray)
					{
						var valueContentText = GetValueContentText(valueContentArray);
						imageInfoBuilder.AppendLine($"\t{aValueTagValueContentPair.ValueTag}:\t[ {valueContentText} ]");
					}
					else if (aValueTagValueContentPair.ValueContent is Number valueContentNumber)
					{
						var valueContentText = (uint)valueContentNumber;
						imageInfoBuilder.AppendLine($"\t{aValueTagValueContentPair.ValueTag}:\t{valueContentText}");
					}
					else
					{
						imageInfoBuilder.AppendLine($"\t{aValueTagValueContentPair.ValueTag}:\t{aValueTagValueContentPair.ValueContent}");
					}
				}
			}
		}
	}

	private static void BuildIptcProfile(IMagickImage image, StringBuilder imageInfoBuilder)
	{
		var iptcProfile = image.GetIptcProfile();

		if (iptcProfile is not null)
		{
			var valueTagValueContentPairs = iptcProfile.Values
				.Select(aValueTagValueContentPair => new
				{
					ValueTag = aValueTagValueContentPair.Tag,
					ValueContent = aValueTagValueContentPair.Value
				})
				.Where(aValueTagValueContentPair =>
					!aValueTagValueContentPair.ValueContent.Contains('\0'))
				.ToList();

			if (valueTagValueContentPairs.Any())
			{
				imageInfoBuilder.AppendLine();
				imageInfoBuilder.AppendLine("IPTC profile");
				imageInfoBuilder.AppendLine();

				foreach (var aValueTagValueContentPair in valueTagValueContentPairs)
				{
					imageInfoBuilder.AppendLine($"\t{aValueTagValueContentPair.ValueTag}:\t{aValueTagValueContentPair.ValueContent}");
				}
			}
		}
	}

	private static void BuildXmpProfile(IMagickImage image, StringBuilder imageInfoBuilder)
	{
		var xmpProfile = image.GetXmpProfile();

		XDocument? xmpProfileXmlContent;
		try
		{
			xmpProfileXmlContent = xmpProfile?.ToXDocument();
		}
		catch
		{
			return;
		}

		if (xmpProfileXmlContent is not null)
		{
			var xmpProfileXmlContentString = xmpProfileXmlContent.ToString();
			var xmlContentForDisplay = FormatXmlContentForDisplay(xmpProfileXmlContentString);
			var indentedXmlContentForDisplay = IndentText(xmlContentForDisplay, "\t");

			imageInfoBuilder.AppendLine();
			imageInfoBuilder.AppendLine("XMP profile");
			imageInfoBuilder.AppendLine();

			imageInfoBuilder.Append(indentedXmlContentForDisplay);
		}
	}

	private static string GetValueContentText(Array valueContentArray)
	{
		var valueContentItems = new object[valueContentArray.Length];
		Array.Copy(valueContentArray, valueContentItems, valueContentArray.Length);

		var valueContentText = string.Join(", ", valueContentItems);
		return valueContentText;
	}

	private static string FormatXmlContentForDisplay(string xmlContent)
	{
		using var xmlContentStream = new MemoryStream();
		using var xmlTextWriter = new XmlTextWriter(xmlContentStream, Encoding.UTF8);

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xmlContent);

		xmlTextWriter.Formatting = Formatting.Indented;
		xmlTextWriter.Indentation = 1;
		xmlTextWriter.IndentChar = '\t';

		xmlDocument.WriteContentTo(xmlTextWriter);

		xmlTextWriter.Flush();
		xmlContentStream.Flush();
		xmlContentStream.Reset();

		var formattedXmlContentStreamReader = new StreamReader(xmlContentStream);
		var formattedXmlContent = formattedXmlContentStreamReader.ReadToEnd();

		return formattedXmlContent;
	}

	private static string IndentText(string text, string indentation)
	{
		var indentedTextBuilder = new StringBuilder();

		var textLines = text.Split(Environment.NewLine);

		foreach (var aTextLine in textLines)
		{
			indentedTextBuilder.AppendLine($"{indentation}{aTextLine}");
		}

		var indentedText = indentedTextBuilder.ToString();
		return indentedText;
	}

	#endregion
}
