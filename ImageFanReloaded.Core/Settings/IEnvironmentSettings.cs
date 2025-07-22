namespace ImageFanReloaded.Core.Settings;

public interface IEnvironmentSettings
{
	bool IsInsideFlatpakContainer { get; }
}
