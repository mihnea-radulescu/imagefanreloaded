using System;

namespace ImageFanReloaded.Core.ImageHandling;

public record ImageFileData
{
	public ImageFileData(
		string imageFileName,
		string imageFilePath,
		string imageFileExtension,
		string imageFileNameWithoutExtension,
		string imageFolderPath,
		decimal sizeOnDiscInKilobytes,
		DateTime lastModificationTime)
	{
		ImageFileName = imageFileName;
		ImageFilePath = imageFilePath;
		ImageFileExtension = imageFileExtension;
		ImageFileNameWithoutExtension = imageFileNameWithoutExtension;
		ImageFolderPath = imageFolderPath;

		SizeOnDiscInKilobytes = sizeOnDiscInKilobytes;
		LastModificationTime = lastModificationTime;
	}

	public string ImageFileName { get; }
	public string ImageFilePath { get; }
	public string ImageFileExtension { get; }
	public string ImageFileNameWithoutExtension { get; }
	public string ImageFolderPath { get; }

	public decimal SizeOnDiscInKilobytes { get; }
	public DateTime LastModificationTime { get; }
}
