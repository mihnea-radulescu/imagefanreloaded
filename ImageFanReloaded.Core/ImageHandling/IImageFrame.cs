using System;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImageFrame : IDisposable
{
	ImageSize Size { get; }
	TimeSpan DelayUntilNextFrame { get; }

	TImageImpl GetInstance<TImageImpl>() where TImageImpl : class, IDisposable;
}
