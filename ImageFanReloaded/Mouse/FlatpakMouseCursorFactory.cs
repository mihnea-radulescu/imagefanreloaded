using Avalonia.Input;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Mouse.Implementation;

namespace ImageFanReloaded.Mouse;

public class FlatpakMouseCursorFactory : MouseCursorFactoryBase
{
	public FlatpakMouseCursorFactory()
	{
		DragCursor = new MouseCursor(new Cursor(StandardCursorType.SizeAll));
	}

	public override IMouseCursor DragCursor { get; }
}
