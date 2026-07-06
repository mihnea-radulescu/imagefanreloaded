namespace ImageFanReloaded.Core.ImageHandling.Factories;

public interface ISaveFileImageFormatFactory
{
	ISaveFileImageFormat JpegSaveFileImageFormat { get; }
	ISaveFileImageFormat WebpSaveFileImageFormat { get; }
	ISaveFileImageFormat AvifSaveFileImageFormat { get; }
	ISaveFileImageFormat JxlSaveFileImageFormat { get; }
	ISaveFileImageFormat GifSaveFileImageFormat { get; }
	ISaveFileImageFormat PngSaveFileImageFormat { get; }
	ISaveFileImageFormat TiffSaveFileImageFormat { get; }
	ISaveFileImageFormat BmpSaveFileImageFormat { get; }
}
