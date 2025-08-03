namespace ImageFanReloaded.Core.AboutInfo;

public interface IAboutInfoProvider
{
	string VersionString { get; }
	int Year { get; }
}
