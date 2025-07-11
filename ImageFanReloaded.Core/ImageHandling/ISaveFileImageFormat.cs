namespace ImageFanReloaded.Core.ImageHandling;

public interface ISaveFileImageFormat
{
	string Extension { get; }

	bool IsAnimationEnabled { get; }
}
