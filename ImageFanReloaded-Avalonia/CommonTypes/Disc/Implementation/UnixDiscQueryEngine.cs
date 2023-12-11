using System.Collections.Generic;
using System.Linq;
using ImageFanReloaded.Factories;

namespace ImageFanReloaded.CommonTypes.Disc.Implementation;

public class UnixDiscQueryEngine
	: DiscQueryEngineBase
{
	static UnixDiscQueryEngine()
	{
		UnixDrivePrefixes = new List<string>
		{
			"/home/",
			"/media/",
			"/mnt/"
		};
	}
	
	public UnixDiscQueryEngine(IImageFileFactory imageFileFactory)
		: base(imageFileFactory)
	{
	}

	protected override bool IsDrive(string driveName)
	{
		if (driveName == UnixDriveRootPath)
		{
			return true;
		}

		var isDrive = UnixDrivePrefixes.Any(
			aUnixDrivePrefix => driveName.StartsWith(aUnixDrivePrefix));

		return isDrive;
	}

	#region Private

	private const string UnixDriveRootPath = "/";

	private static readonly IReadOnlyCollection<string> UnixDrivePrefixes;

	#endregion
}
