using System;

namespace ImageFanReloaded.Core.Mouse;

public interface IMouseCursor
{
	TCursor GetInstance<TCursor>() where TCursor : class, IDisposable;
}
