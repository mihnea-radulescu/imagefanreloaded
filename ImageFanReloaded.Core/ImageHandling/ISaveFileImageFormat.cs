namespace ImageFanReloaded.Core.ImageHandling;

public interface ISaveFileImageFormat
{
	string Name { get; }
	string Extension { get; }

	bool IsAnimationEnabled { get; }
}
