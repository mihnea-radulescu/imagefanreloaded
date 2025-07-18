using Avalonia.Input;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Mouse.Implementation;

namespace ImageFanReloaded.Mouse;

public class DefaultMouseCursorFactory : MouseCursorFactoryBase
{
	public DefaultMouseCursorFactory()
	{
		DragCursor = new MouseCursor(new Cursor(StandardCursorType.DragMove));
	}
	
	public override IMouseCursor DragCursor { get; }
}
