using Avalonia;

namespace ImageFanReloaded.Test;

public class AppBuilderInitializer
{
	public static AppBuilderInitializer Instance
	{
		get
		{
			lock (Locker)
			{
				_instance ??= new AppBuilderInitializer();
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

	private static AppBuilderInitializer? _instance;

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
