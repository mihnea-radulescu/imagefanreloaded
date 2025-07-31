namespace ImageFanReloaded.Core.ImageHandling;

public record StaticImageFileData
{
	public StaticImageFileData(
		string imageFileName,
		string imageFilePath,
		string imageFileExtension,
		string imageFileNameWithoutExtension,
		string imageFolderPath)
	{
		ImageFileName = imageFileName;
		ImageFilePath = imageFilePath;
		ImageFileExtension = imageFileExtension;
		ImageFileNameWithoutExtension = imageFileNameWithoutExtension;
		ImageFolderPath = imageFolderPath;
	}

	public string ImageFileName { get; }
	public string ImageFilePath { get; }
	public string ImageFileExtension { get; }
	public string ImageFileNameWithoutExtension { get; }
	public string ImageFolderPath { get; }
}
