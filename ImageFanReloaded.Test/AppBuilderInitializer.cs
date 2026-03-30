using System.Threading;
using Avalonia;

namespace ImageFanReloaded.Test;

public class AppBuilderInitializer
{
	public static AppBuilderInitializer Instance
	{
		get
		{
			lock (InstanceCreationLock)
			{
				field ??= new AppBuilderInitializer();
			}

			return field;
		}
	}

	private static readonly Lock InstanceCreationLock = new();

	private AppBuilderInitializer()
	{
		AppBuilder
			.Configure<TestApp>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.SetupWithoutStarting();
	}
}
