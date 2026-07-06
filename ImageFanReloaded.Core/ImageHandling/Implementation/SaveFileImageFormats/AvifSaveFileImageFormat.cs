namespace
	ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class AvifSaveFileImageFormat : ISaveFileImageFormat
{
	public string Name => "AVIF";
	public string Extension => ".avif";

	public bool DoesSupportAnimation => true;
}
