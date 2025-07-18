namespace ImageFanReloaded.Core.Mouse;

public interface IMouseCursorFactory
{
	IMouseCursor StandardCursor { get; }
	IMouseCursor ZoomCursor { get; }
	IMouseCursor DragCursor { get; }
}
