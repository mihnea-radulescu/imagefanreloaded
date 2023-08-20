using Avalonia;

namespace ImageFanReloaded.Avalonia.Tests;

public class AppBuilderInitializer
{
	public static AppBuilderInitializer Instance
	{
		get
		{
			lock (_locker)
			{
				if (_instance == null)
				{
					_instance = new AppBuilderInitializer();
				}
			}

			return _instance;
		}
	}

	static AppBuilderInitializer()
	{
		_locker = new object();
	}

	#region Private

	private static readonly object _locker;

	private static AppBuilderInitializer _instance;

    private AppBuilderInitializer()
    {
		AppBuilder
			.Configure<TestApp>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.SetupWithoutStarting();
	}

	#endregion
}
