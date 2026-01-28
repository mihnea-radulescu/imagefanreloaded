using Avalonia.Input;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Mouse.Implementation;
using ImageFanReloaded.Core.RuntimeEnvironment;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Mouse;

public class MouseCursorFactory : IMouseCursorFactory
{
	public IMouseCursor StandardCursor { get; }
	public IMouseCursor ZoomCursor { get; }
	public IMouseCursor SelectCursor { get; }
	public IMouseCursor DragCursor { get; }

	public MouseCursorFactory(IGlobalParameters globalParameters)
	{
		StandardCursor = new MouseCursor(new Cursor(StandardCursorType.Arrow));
		ZoomCursor = new MouseCursor(new Cursor(StandardCursorType.Hand));
		SelectCursor = new MouseCursor(new Cursor(StandardCursorType.Cross));

		if (globalParameters.RuntimeEnvironmentType is
			RuntimeEnvironmentType.LinuxFlatpak)
		{
			DragCursor = new MouseCursor(
				new Cursor(StandardCursorType.SizeAll));
		}
		else
		{
			DragCursor = new MouseCursor(
				new Cursor(StandardCursorType.DragMove));
		}
	}
}
