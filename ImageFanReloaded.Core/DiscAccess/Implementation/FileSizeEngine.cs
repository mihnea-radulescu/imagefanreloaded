namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class FileSizeEngine : IFileSizeEngine
{
	public decimal ConvertToKilobytes(long sizeInBytes) => ConvertToHigherUnit(sizeInBytes);

	public decimal ConvertToMegabytes(decimal sizeInKilobytes) => ConvertToHigherUnit(sizeInKilobytes);
	
	#region Private
	
	private const decimal ByteMultiple = 1024;
	
	private static decimal ConvertToHigherUnit(decimal size)
	{
		var sizeInHigherUnit = size / ByteMultiple;
		
		var roundedSizeInHigherUnit = RoundValue(sizeInHigherUnit);
		return roundedSizeInHigherUnit;
	}

	private static decimal RoundValue(decimal value) => decimal.Round(value, 2);

	#endregion
}
