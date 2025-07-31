using System;

namespace ImageFanReloaded.Core.ImageHandling;

public record TransientImageFileData
{
	public TransientImageFileData(
		decimal? sizeOnDiscInKilobytes, DateTime? lastModificationTime)
	{
		SizeOnDiscInKilobytes = sizeOnDiscInKilobytes;
		LastModificationTime = lastModificationTime;
	}

	public decimal? SizeOnDiscInKilobytes { get; }
	public DateTime? LastModificationTime { get; }
}
