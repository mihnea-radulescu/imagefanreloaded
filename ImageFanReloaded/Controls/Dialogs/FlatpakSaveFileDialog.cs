using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tmds.DBus;
using ImageFanReloaded.Controls.Dialogs.FlatpakTypes;
using ImageFanReloaded.Core.Controls.Dialogs.Implementation;

namespace ImageFanReloaded.Controls.Dialogs;

public class FlatpakSaveFileDialog : SaveFileDialogBase
{
	public override async Task<string?> ShowDialog(string imageFileName, string imageFolderPath)
	{
		using var connection = new Connection(Address.Session);
		await connection.ConnectAsync();

		var fileChooser = connection.CreateProxy<IFileChooser>(
			FreedesktopPortalServiceName, FreedesktopPortalPath);

		var saveFileDialogParentWindow = string.Empty;
		var saveFileDialogTitle = SaveFileDialogTitle;
		var saveFileDialogOptions = new Dictionary<string, object>
		{
			["current_name"] = imageFileName
		};

		var saveFileDialogRequestPath = await fileChooser.SaveFileAsync(
			saveFileDialogParentWindow, saveFileDialogTitle, saveFileDialogOptions);
		var saveFileDialogRequest = connection.CreateProxy<IRequest>(
			FreedesktopPortalServiceName, saveFileDialogRequestPath);

		var saveFileDialogTcs = new TaskCompletionSource<(uint, IDictionary<string, object>)>();
		await saveFileDialogRequest.WatchResponseAsync(
			responseTuple => saveFileDialogTcs.TrySetResult(responseTuple));

		var (saveFileDialogResponse, saveFileDialogResults) = await saveFileDialogTcs.Task;

		if (saveFileDialogResponse == 0 && saveFileDialogResults.ContainsKey(ResultsUrisKey))
		{
			var uris = saveFileDialogResults[ResultsUrisKey] as string[];

			if (uris is not null && uris.Any() && uris[0].StartsWith(FileUriPrefix))
			{
				var saveFileDialogFilePath = uris[0].Substring(FileUriPrefix.Length);
				return saveFileDialogFilePath;
			}
		}

		return default;
	}

	#region Private

	private const string FreedesktopPortalServiceName = "org.freedesktop.portal.Desktop";
	private const string FreedesktopPortalPath = "/org/freedesktop/portal/desktop";

	private const string ResultsUrisKey = "uris";
	private const string FileUriPrefix = "file://";

	#endregion
}
