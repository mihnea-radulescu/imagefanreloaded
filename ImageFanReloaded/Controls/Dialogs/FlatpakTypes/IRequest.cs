using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmds.DBus;

namespace ImageFanReloaded.Controls.Dialogs.FlatpakTypes;

[DBusInterface("org.freedesktop.portal.Request")]
public interface IRequest : IDBusObject
{
	Task<IDisposable> WatchResponseAsync(
		Action<(uint response, IDictionary<string, object> results)> handler);
}
