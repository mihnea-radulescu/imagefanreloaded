using System;

namespace ImageFanReloaded.Core.ImageHandling;

public interface IImage : IDisposable
{
	ImageSize Size { get; }
	
	TImageImpl GetInstance<TImageImpl>() where TImageImpl : class, IDisposable;
}
