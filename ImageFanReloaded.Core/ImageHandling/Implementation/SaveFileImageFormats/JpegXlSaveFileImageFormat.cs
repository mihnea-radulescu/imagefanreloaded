namespace
	ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class JpegXlSaveFileImageFormat : ISaveFileImageFormat
{
	public string Name => "JPEG XL";
	public string Extension => ".jxl";

	public bool DoesSupportAnimation => true;
}
