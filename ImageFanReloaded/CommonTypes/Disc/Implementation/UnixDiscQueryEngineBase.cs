using System.Collections.Generic;
using System.Linq;
using ImageFanReloaded.Factories;

namespace ImageFanReloaded.CommonTypes.Disc.Implementation;

public abstract class UnixDiscQueryEngineBase : DiscQueryEngineBase
{
	protected UnixDiscQueryEngineBase(IImageFileFactory imageFileFactory)
		: base(imageFileFactory)
	{
	}

	protected override bool IsSupportedDrive(string driveName)
	{
		if (driveName == RootPath)
		{
			return true;
		}

		var isSupportedDrive = SupportedDrivePrefixes.Any(driveName.StartsWith);

		return isSupportedDrive;
	}

	protected abstract IReadOnlyCollection<string> SupportedDrivePrefixes { get; }

	#region Private

	private const string RootPath = "/";

	#endregion
}
