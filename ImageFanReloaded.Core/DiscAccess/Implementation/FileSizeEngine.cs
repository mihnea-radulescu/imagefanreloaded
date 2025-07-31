namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class FileSizeEngine : IFileSizeEngine
{
	public decimal ConvertToKilobytes(long sizeInBytes) => ConvertToHigherUnit(sizeInBytes);

	public decimal ConvertToMegabytes(decimal sizeInKilobytes)
		=> ConvertToHigherUnit(sizeInKilobytes);

	#region Private

	private const decimal ByteMultiple = 1024;

	private static decimal ConvertToHigherUnit(decimal size) => size / ByteMultiple;

	#endregion
}
