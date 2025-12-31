using Avalonia.Input;
using ImageFanReloaded.Core.Mouse;

namespace ImageFanReloaded.Mouse;

public static class MouseCursorExtensions
{
	extension(IMouseCursor mouseCursor)
	{
		public Cursor Cursor => mouseCursor.GetInstance<Cursor>();
	}
}
