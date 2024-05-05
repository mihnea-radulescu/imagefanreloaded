namespace ImageFanReloaded.Core.AboutInformation;

public interface IAboutInformationProvider
{
	string VersionString { get; }
	int CurrentYear { get; }
}
