using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views
{
    public static class WindowExtensions
    {
        public static ImageDimensions GetScreenDimensions(this Window window)
        {
            var windowHandle = new WindowInteropHelper(window).Handle;
            var screen = Screen.FromHandle(windowHandle);

			var width = screen.Bounds.Width * DpiStandardUnit / DpiCurrentUnit;
			var height = screen.Bounds.Height * DpiStandardUnit / DpiCurrentUnit;

            var screenDimensions = new ImageDimensions(width, height);
			return screenDimensions;
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
