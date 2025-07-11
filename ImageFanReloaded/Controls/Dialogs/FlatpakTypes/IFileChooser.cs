using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus;

namespace ImageFanReloaded.Controls.Dialogs.FlatpakTypes;

[DBusInterface("org.freedesktop.portal.FileChooser")]
public interface IFileChooser : IDBusObject
{
	Task<ObjectPath> SaveFileAsync(
		string parent_window,
		string title,
		IDictionary<string, object> options
	);
}
