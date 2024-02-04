using System.Collections.Generic;
using ImageFanReloaded.Factories;

namespace ImageFanReloaded.CommonTypes.Disc.Implementation;

public class LinuxDiscQueryEngine : UnixDiscQueryEngineBase
{
	public LinuxDiscQueryEngine(IImageFileFactory imageFileFactory)
		: base(imageFileFactory)
	{
		_supportedDrivePrefixes = new List<string>
		{
			"/home/",
			"/media/",
			"/mnt/"
		};
	}

	protected override IReadOnlyCollection<string> SupportedDrivePrefixes
		=> _supportedDrivePrefixes;

	#region Private

	private readonly IReadOnlyCollection<string> _supportedDrivePrefixes;

	#endregion
}
