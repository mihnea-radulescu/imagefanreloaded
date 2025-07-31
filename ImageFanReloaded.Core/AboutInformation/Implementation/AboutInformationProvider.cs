using System.Reflection;

namespace ImageFanReloaded.Core.AboutInformation.Implementation;

public class AboutInformationProvider : IAboutInformationProvider
{
	public AboutInformationProvider()
	{
		var assemblyVersion = Assembly.GetEntryAssembly()!.GetName().Version!;

		var major = assemblyVersion.Major;
		var minor = assemblyVersion.Minor;
		var build = assemblyVersion.Build.ToString().PadLeft(PaddingMinimumLength, PaddingCharacter);
		var revision = assemblyVersion.Revision.ToString().PadLeft(PaddingMinimumLength, PaddingCharacter);

		VersionString = $"{major}.{minor}.{build}.{revision}";
		Year = minor;
	}

	public string VersionString { get; }
	public int Year { get; }

	#region Private

	private const int PaddingMinimumLength = 2;
	private const char PaddingCharacter = '0';

	#endregion
}
