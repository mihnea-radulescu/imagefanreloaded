using System.IO;
using ImageFanReloaded.Core.RuntimeEnvironment;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class SettingsFactory : ISettingsFactory
{
	public ITabOptions GetTabOptions() => new TabOptions();

	public SettingsFactory(IGlobalParameters globalParameters)
	{
		SetupDefaultTabOptions(globalParameters);
	}

	private static void SetupDefaultTabOptions(
		IGlobalParameters globalParameters)
	{
		string configFolderPath;

		if (globalParameters.RuntimeEnvironmentType is
			RuntimeEnvironmentType.Linux)
		{
			configFolderPath = Path.Combine(
				globalParameters.UserHomePath,
				".config",
				globalParameters.ApplicationName);
		}
		else if (globalParameters.RuntimeEnvironmentType is
				 RuntimeEnvironmentType.LinuxFlatpak)
		{
			configFolderPath = globalParameters.UserConfigPath;
		}
		else
		{
			configFolderPath = Path.Combine(
				globalParameters.UserConfigPath,
				globalParameters.ApplicationName);
		}

		TabOptions.LoadDefaultTabOptions(configFolderPath);
	}
}
