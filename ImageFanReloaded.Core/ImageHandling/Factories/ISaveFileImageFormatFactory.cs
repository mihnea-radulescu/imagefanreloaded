namespace ImageFanReloaded.Core.ImageHandling.Factories;

public interface ISaveFileImageFormatFactory
{
	ISaveFileImageFormat JpegSaveFileImageFormat { get; }
	ISaveFileImageFormat GifSaveFileImageFormat { get; }
	ISaveFileImageFormat PngSaveFileImageFormat { get; }
	ISaveFileImageFormat WebpSaveFileImageFormat { get; }
	ISaveFileImageFormat TiffSaveFileImageFormat { get; }
	ISaveFileImageFormat BmpSaveFileImageFormat { get; }
}
