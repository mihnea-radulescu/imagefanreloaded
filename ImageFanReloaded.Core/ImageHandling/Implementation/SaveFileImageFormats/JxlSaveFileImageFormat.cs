namespace
	ImageFanReloaded.Core.ImageHandling.Implementation.SaveFileImageFormats;

public class JxlSaveFileImageFormat : ISaveFileImageFormat
{
	public string Name => "JXL";
	public string Extension => ".jxl";

	public bool DoesSupportAnimation => true;
}
