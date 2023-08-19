using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ImageFanReloaded.Views
{
    public static class WindowExtensions
    {
        public static Rectangle GetScreenBounds(this Window window)
        {
            var windowHandle = new WindowInteropHelper(window).Handle;
            var screen = Screen.FromHandle(windowHandle);

			var width = screen.Bounds.Width * DpiStandardUnit / DpiCurrentUnit;
			var height = screen.Bounds.Height * DpiStandardUnit / DpiCurrentUnit;

            var bounds = new Rectangle(0, 0, width, height);

			return bounds;
		}

		#region Private

		private const int DpiStandardUnit = 96;

		private static readonly int DpiCurrentUnit;

		static WindowExtensions()
		{
			DpiCurrentUnit = (int)typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static)
														  .GetValue(null, null);
		}

		#endregion
	}
}
