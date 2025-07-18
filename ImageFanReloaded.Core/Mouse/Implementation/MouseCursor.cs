using System;
using ImageFanReloaded.Core.BaseTypes;

namespace ImageFanReloaded.Core.Mouse.Implementation;

public class MouseCursor : DisposableBase, IMouseCursor
{
	public MouseCursor(IDisposable mouseCursorImplementationInstance)
	{
		_mouseCursorImplementationInstance = mouseCursorImplementationInstance;
	}
	
	public TCursor GetInstance<TCursor>() where TCursor : class, IDisposable
	{
		ThrowObjectDisposedExceptionIfNecessary();

		return (TCursor)_mouseCursorImplementationInstance;
	}

	#region Protected

	protected override void DisposeSpecific() => _mouseCursorImplementationInstance.Dispose();

	#endregion

	#region Private

	private readonly IDisposable _mouseCursorImplementationInstance;

	#endregion
}
