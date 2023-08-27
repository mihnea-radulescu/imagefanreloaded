using Avalonia;

namespace ImageFanReloaded.Avalonia.Tests;

public class AppBuilderInitializer
{
	public static AppBuilderInitializer Instance
	{
		get
		{
			lock (Locker)
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
		Locker = new object();
	}

	#region Private

	private static readonly object Locker;

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
