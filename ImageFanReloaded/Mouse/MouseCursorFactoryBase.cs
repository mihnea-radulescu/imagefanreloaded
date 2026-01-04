using Avalonia.Input;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Mouse.Implementation;

namespace ImageFanReloaded.Mouse;

public abstract class MouseCursorFactoryBase : IMouseCursorFactory
{
	public IMouseCursor StandardCursor { get; }
	public IMouseCursor ZoomCursor { get; }
	public IMouseCursor SelectCursor { get; }

	public abstract IMouseCursor DragCursor { get; }

	protected MouseCursorFactoryBase()
	{
		StandardCursor = new MouseCursor(new Cursor(StandardCursorType.Arrow));
		ZoomCursor = new MouseCursor(new Cursor(StandardCursorType.Hand));
		SelectCursor = new MouseCursor(new Cursor(StandardCursorType.Cross));
	}
}
