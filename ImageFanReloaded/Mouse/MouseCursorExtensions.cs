using Avalonia.Input;
using ImageFanReloaded.Core.Mouse;

namespace ImageFanReloaded.Mouse;

public static class MouseCursorExtensions
{
	public static Cursor GetCursor(this IMouseCursor mouseCursor)
		=> mouseCursor.GetInstance<Cursor>();
}
