using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using ImageFanReloaded.CommonTypes.ImageHandling;

namespace ImageFanReloaded.Views.Implementation
{
	public class ScreenInformation
		: IScreenInformation
	{
		static ScreenInformation()
		{
			DpiProperty = typeof(SystemParameters)
				.GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
		}
		
		public ScreenInformation(Window mainWindow)
        {
			_mainWindow = mainWindow;
		}

        public ImageSize GetScreenSize()
		{
			var windowHandle = new WindowInteropHelper(_mainWindow).Handle;
			var screen = Screen.FromHandle(windowHandle);

			var dpiCurrentUnit = (int)DpiProperty.GetValue(null, null);

			var width = screen.Bounds.Width * DpiStandardUnit / dpiCurrentUnit;
			var height = screen.Bounds.Height * DpiStandardUnit / dpiCurrentUnit;

			var screenSize = new ImageSize(width, height);

			return screenSize;
		}

		#region Private

		private const int DpiStandardUnit = 96;

		private static readonly PropertyInfo DpiProperty;

		private readonly Window _mainWindow;

		#endregion
	}
}
