using System.Reflection;

namespace ImageFanReloaded.Core.AboutInfo.Implementation;

public class AboutInfoProvider : IAboutInfoProvider
{
	public AboutInfoProvider()
	{
		var assemblyVersion = Assembly.GetEntryAssembly()!.GetName().Version!;

		var major = assemblyVersion.Major;
		var minor = assemblyVersion.Minor;
		var build = assemblyVersion.Build.ToString().PadLeft(
			PaddingMinimumLength, PaddingCharacter);
		var revision = assemblyVersion.Revision.ToString().PadLeft(
			PaddingMinimumLength, PaddingCharacter);

		VersionString = $"{major}.{minor}.{build}.{revision}";
		Year = minor;
	}

	public string VersionString { get; }
	public int Year { get; }

	private const int PaddingMinimumLength = 2;
	private const char PaddingCharacter = '0';
}
