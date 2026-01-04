using System;

namespace ImageFanReloaded.Core.Settings.Implementation;

public class EnvironmentSettings : IEnvironmentSettings
{
	public bool IsInsideFlatpakContainer
		=> Environment.GetEnvironmentVariable(FlatpakEnvironmentVariable) == FlatpakAppId;

	private const string FlatpakEnvironmentVariable = "FLATPAK_ID";

	private const string FlatpakAppId = "io.github.mihnea_radulescu.imagefanreloaded";
}
