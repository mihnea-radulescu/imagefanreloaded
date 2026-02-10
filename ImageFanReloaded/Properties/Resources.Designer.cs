namespace ImageFanReloaded.Properties {
	using System;

	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class Resources {

		private static System.Resources.ResourceManager resourceMan;

		private static System.Globalization.CultureInfo resourceCulture;

		[System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Resources() {
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static System.Resources.ResourceManager ResourceManager {
			get {
				if (object.Equals(null, resourceMan)) {
					System.Resources.ResourceManager temp = new System.Resources.ResourceManager("ImageFanReloaded.Properties.Resources", typeof(Resources).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}

		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static System.Globalization.CultureInfo Culture {
			get {
				return resourceCulture;
			}
			set {
				resourceCulture = value;
			}
		}

		internal static byte[] DesktopFolderIcon {
			get {
				object obj = ResourceManager.GetObject("DesktopFolderIcon", resourceCulture);
				return ((byte[])(obj));
			}
		}

		internal static byte[] DocumentsFolderIcon {
			get {
				object obj = ResourceManager.GetObject("DocumentsFolderIcon", resourceCulture);
				return ((byte[])(obj));
			}
		}

		internal static byte[] DownloadsFolderIcon {
			get {
				object obj = ResourceManager.GetObject("DownloadsFolderIcon", resourceCulture);
				return ((byte[])(obj));
			}
		}

		internal static byte[] DriveIcon {
			get {
				object obj = ResourceManager.GetObject("DriveIcon", resourceCulture);
				return ((byte[])(obj));
			}
		}

		internal static byte[] FolderIcon {
			get {
				object obj = ResourceManager.GetObject("FolderIcon", resourceCulture);
				return ((byte[])(obj));
			}
		}

		internal static byte[] HomeFolderIcon {
			get {
				object obj = ResourceManager.GetObject("HomeFolderIcon", resourceCulture);
				return ((byte[])(obj));
			}
		}

		internal static byte[] PicturesFolderIcon {
			get {
				object obj = ResourceManager.GetObject("PicturesFolderIcon", resourceCulture);
				return ((byte[])(obj));
			}
		}

		internal static byte[] InvalidImage {
			get {
				object obj = ResourceManager.GetObject("InvalidImage", resourceCulture);
				return ((byte[])(obj));
			}
		}

		internal static byte[] LoadingImage {
			get {
				object obj = ResourceManager.GetObject("LoadingImage", resourceCulture);
				return ((byte[])(obj));
			}
		}
	}
}
