namespace ImageFanReloaded.Core.ImageHandling.Factories;

public interface IImageFileFactory
{
	IImageFile GetImageFile(
		StaticImageFileData staticImageFileData, TransientImageFileData transientImageFileData);
}
