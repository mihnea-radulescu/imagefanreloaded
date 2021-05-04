using System.Runtime.InteropServices;
using System.Security;

namespace ImageFanReloaded.CommonTypes.Disc.Unmanaged
{
	[SuppressUnmanagedCodeSecurity]
	public static class UnmanagedStringComparer
	{
		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
		public static extern int StrCmpLogicalW(string psz1, string psz2);
	}
}
