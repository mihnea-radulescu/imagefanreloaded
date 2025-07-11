namespace ImageFanReloaded.Core.ImageHandling;

public record ImageFileData
{
	public ImageFileData(
		string imageFileName,
	    string imageFilePath,
		string imageFileExtension,
		string imageFileNameWithoutExtension,
		string imageFolderPath,
	    decimal sizeOnDiscInKilobytes)
	{
		ImageFileName = imageFileName;
		ImageFilePath = imageFilePath;
		ImageFileExtension = imageFileExtension;
		ImageFileNameWithoutExtension = imageFileNameWithoutExtension;
		ImageFolderPath = imageFolderPath;
		SizeOnDiscInKilobytes = sizeOnDiscInKilobytes;
	}

	public string ImageFileName { get; }
	public string ImageFilePath { get; }
	public string ImageFileExtension { get; }
	public string ImageFileNameWithoutExtension { get; }
	public string ImageFolderPath { get; }
	public decimal SizeOnDiscInKilobytes { get; }
}
