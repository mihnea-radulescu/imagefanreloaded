using System;
using ImageFanReloaded.Core.Exceptions;

namespace ImageFanReloaded.Core.RuntimeEnvironment.Implementation;

public class RuntimeEnvironmentSettings : IRuntimeEnvironmentSettings
{
	public RuntimeEnvironmentSettings()
	{
		if (OperatingSystem.IsLinux())
		{
			RuntimeEnvironmentType = IsInsideFlatpakContainer
				? RuntimeEnvironmentType.LinuxFlatpak
				: RuntimeEnvironmentType.Linux;
		}
		else if (OperatingSystem.IsWindows())
		{
			RuntimeEnvironmentType = RuntimeEnvironmentType.Windows;
		}
		else if (OperatingSystem.IsMacOS())
		{
			RuntimeEnvironmentType = RuntimeEnvironmentType.MacOs;
		}
		else
		{
			throw new RuntimeEnvironmentNotSupportedException();
		}
	}

	public RuntimeEnvironmentType RuntimeEnvironmentType { get; }

	private const string FlatpakEnvironmentVariable = "FLATPAK_ID";
	private const string FlatpakAppId =
		"io.github.mihnea_radulescu.imagefanreloaded";

	private static bool IsInsideFlatpakContainer
		=> Environment.GetEnvironmentVariable(FlatpakEnvironmentVariable) ==
			FlatpakAppId;
}
