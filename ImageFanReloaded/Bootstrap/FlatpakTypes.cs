using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tmds.DBus;

namespace ImageFanReloaded.Bootstrap;

[DBusInterface("org.freedesktop.portal.FileChooser")]
public interface IFileChooser : IDBusObject
{
	Task<ObjectPath> SaveFileAsync(
		string parent_window,
		string title,
		IDictionary<string, object> options
	);
}

[DBusInterface("org.freedesktop.portal.Request")]
public interface IRequest : IDBusObject
{
	Task<IDisposable> WatchResponseAsync(Action<(uint response, IDictionary<string, object> results)> handler);
}

public class Portals
{
	public static async Task Call()
	{
		using var connection = new Connection(Address.Session);
		await connection.ConnectAsync();

		var fileChooser = connection.CreateProxy<IFileChooser>(
			"org.freedesktop.portal.Desktop",
			"/org/freedesktop/portal/desktop"
		);

		string parentWindow = "";
		string title = "Save File";
		var options = new Dictionary<string, object>
		{
			["current_name"] = "untitled.txt"
		};

		var requestPath = await fileChooser.SaveFileAsync(parentWindow, title, options);

		var request = connection.CreateProxy<IRequest>(
			"org.freedesktop.portal.Desktop",
			requestPath
		);

		var tcs = new TaskCompletionSource<(uint, IDictionary<string, object>)>();

		await request.WatchResponseAsync(tuple =>
		{
			tcs.TrySetResult(tuple);
		});

		var (response, results) = await tcs.Task;

		if (response == 0 && results.ContainsKey("uris"))
		{
			var uris = results["uris"] as string[];

			if (uris is not null && uris.Any() && uris[0].StartsWith("file://"))
			{
				var filePath = uris[0].Substring("file://".Length);
				File.WriteAllText(filePath, "Hello, world!");
			}
		}
	}
}
