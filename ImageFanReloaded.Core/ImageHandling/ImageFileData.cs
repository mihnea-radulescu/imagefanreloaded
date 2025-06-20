namespace ImageFanReloaded.Core.ImageHandling;

public record ImageFileData
{
	public ImageFileData(
		string imageFileName,
	    string imageFilePath,
		string imageFileExtension,
	    decimal sizeOnDiscInKilobytes)
	{
		ImageFileName = imageFileName;
		ImageFilePath = imageFilePath;
		ImageFileExtension = imageFileExtension;
		SizeOnDiscInKilobytes = sizeOnDiscInKilobytes;
	}

	public string ImageFileName { get; }
	public string ImageFilePath { get; }
	public string ImageFileExtension { get; }
	public decimal SizeOnDiscInKilobytes { get; }
}
