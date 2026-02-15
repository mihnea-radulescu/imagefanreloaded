using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Caching;

public class SqliteDatabaseLogic : IDatabaseLogic
{
	public SqliteDatabaseLogic(
		IGlobalParameters globalParameters,
		ISettingsFactory settingsFactory,
		IFileSizeEngine fileSizeEngine)
	{
		_thumbnailCacheFolderPath = settingsFactory.GetCacheFolderPath();

		var thumbnailCacheDbFileName =
			$"{globalParameters.ApplicationName}.cache.db";
		_thumbnailCacheDbFilePath = Path.Combine(
			_thumbnailCacheFolderPath, thumbnailCacheDbFileName);

		_connectionString = string.Format(
			ConnectionStringPattern, _thumbnailCacheDbFilePath);

		_fileSizeEngine = fileSizeEngine;
	}

	public void CreateDatabaseIfNotExisting()
	{
		try
		{
			Directory.CreateDirectory(_thumbnailCacheFolderPath);

			if (ShouldCreateDatabase())
			{
				using (var dbConnection = new SqliteConnection(
						_connectionString))
				{
					dbConnection.Open();

					using (var dbCommand = dbConnection.CreateCommand())
					{
						dbCommand.CommandText =
							$"{PragmasScript}{CreateTableScript}";

						dbCommand.ExecuteNonQuery();
					}
				}
			}
		}
		catch
		{
		}
	}

	public int GetThumbnailCacheSizeInMegabytes()
	{
		try
		{
			if (Directory.Exists(_thumbnailCacheFolderPath))
			{
				var thumbnailCacheDbFileInfo = new FileInfo(
					_thumbnailCacheDbFilePath);

				var thumbnailCacheSizeInBytes = thumbnailCacheDbFileInfo.Length;

				var thumbnailCacheSizeInKilobytes = _fileSizeEngine
					.ConvertToKilobytes(thumbnailCacheSizeInBytes);
				var thumbnailCacheSizeInMegabytes = _fileSizeEngine
					.ConvertToMegabytes(thumbnailCacheSizeInKilobytes);

				return (int)thumbnailCacheSizeInMegabytes;
			}
			else
			{
				return 0;
			}
		}
		catch
		{
			return 0;
		}
	}

	public async Task ClearDatabase()
	{
		try
		{
			await using (var dbConnection = new SqliteConnection(
				_connectionString))
			{
				await dbConnection.OpenAsync();

				await using (var dbCommand = dbConnection.CreateCommand())
				{
					dbCommand.CommandText = ClearDataScript;
					await dbCommand.ExecuteNonQueryAsync();
				}

				await using (var dbCommand = dbConnection.CreateCommand())
				{
					dbCommand.CommandText = ClearWriteAheadLogScript;
					await dbCommand.ExecuteNonQueryAsync();
				}

				await using (var dbCommand = dbConnection.CreateCommand())
				{
					dbCommand.CommandText = TrimDatabaseScript;
					await dbCommand.ExecuteNonQueryAsync();
				}
			}
		}
		catch
		{
		}
	}

	public ThumbnailCacheEntry? GetThumbnailCacheEntry(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation)
	{
		try
		{
			using (var dbConnection = new SqliteConnection(_connectionString))
			{
				dbConnection.Open();

				using (var dbCommand = dbConnection.CreateCommand())
				{
					dbCommand.CommandText = GetThumbnailCacheEntryScript;

					dbCommand.Parameters.AddWithValue(
						"@filePath", imageFileData.FilePath);
					dbCommand.Parameters.AddWithValue(
						"@fileSizeInBytes", imageFileData.FileSizeInBytes);
					dbCommand.Parameters.AddWithValue(
						"@fileLastModificationTime",
						imageFileData.FileLastModificationTime);

					dbCommand.Parameters.AddWithValue(
						"@thumbnailSize", thumbnailSize);
					dbCommand.Parameters.AddWithValue(
						"@applyImageOrientation", applyImageOrientation);

					using (var dataReader = dbCommand.ExecuteReader())
					{
						if (dataReader.Read())
						{
							var thumbnailData = dataReader
								.GetFieldValue<byte[]>(0);

							return new ThumbnailCacheEntry
							{
								FilePath = imageFileData.FilePath,
								FileSizeInBytes = imageFileData.FileSizeInBytes,
								FileLastModificationTime =
									imageFileData.FileLastModificationTime,

								ThumbnailSize = thumbnailSize,
								ApplyImageOrientation = applyImageOrientation,

								ThumbnailData = thumbnailData
							};
						}
					}
				}
			}

			return null;
		}
		catch
		{
			return null;
		}
	}

	public void UpsertThumbnailCacheEntry(
		ThumbnailCacheEntry thumbnailCacheEntry)
	{
		try
		{
			using (var dbConnection = new SqliteConnection(_connectionString))
			{
				dbConnection.Open();

				using (var dbCommand = dbConnection.CreateCommand())
				{
					dbCommand.CommandText = UpsertThumbnailCacheEntryScript;

					dbCommand.Parameters.AddWithValue(
						"@filePath", thumbnailCacheEntry.FilePath);
					dbCommand.Parameters.AddWithValue(
						"@fileSizeInBytes",
						thumbnailCacheEntry.FileSizeInBytes);
					dbCommand.Parameters.AddWithValue(
						"@fileLastModificationTime",
						thumbnailCacheEntry.FileLastModificationTime);

					dbCommand.Parameters.AddWithValue(
						"@thumbnailSize", thumbnailCacheEntry.ThumbnailSize);
					dbCommand.Parameters.AddWithValue(
						"@applyImageOrientation",
						thumbnailCacheEntry.ApplyImageOrientation);

					dbCommand.Parameters.AddWithValue(
						"@thumbnailData", thumbnailCacheEntry.ThumbnailData);

					dbCommand.ExecuteNonQuery();
				}
			}
		}
		catch
		{
		}
	}

	private const string ConnectionStringPattern = "Data Source={0}";

	private const string PragmasScript =
		"PRAGMA auto_vacuum = FULL; PRAGMA journal_mode = WAL;";

	private const string CreateTableScript = """
		CREATE TABLE IF NOT EXISTS ThumbnailCacheEntries (
			FilePath TEXT NOT NULL,
			FileSizeInBytes INTEGER NOT NULL,
			FileLastModificationTime DATETIME NOT NULL,
			ThumbnailSize INTEGER NOT NULL,
			ApplyImageOrientation BOOLEAN NOT NULL,
			ThumbnailData BLOB NOT NULL,
			PRIMARY KEY (FilePath, ThumbnailSize, ApplyImageOrientation));
	""";

	private const string ClearDataScript = "DELETE FROM ThumbnailCacheEntries;";
	private const string ClearWriteAheadLogScript =
		"PRAGMA wal_checkpoint(FULL);";
	private const string TrimDatabaseScript = "VACUUM;";

	private const string GetThumbnailCacheEntryScript = """
		SELECT ThumbnailData
		FROM ThumbnailCacheEntries
		WHERE FilePath = @filePath
			AND FileSizeInBytes = @fileSizeInBytes
			AND FileLastModificationTime = @fileLastModificationTime
			AND ThumbnailSize = @thumbnailSize
			AND ApplyImageOrientation = @applyImageOrientation
	""";

	private const string UpsertThumbnailCacheEntryScript = """
		INSERT INTO ThumbnailCacheEntries (
			FilePath,
			FileSizeInBytes,
			FileLastModificationTime,
			ThumbnailSize,
			ApplyImageOrientation,
			ThumbnailData)
		VALUES (
			@filePath,
			@fileSizeInBytes,
			@fileLastModificationTime,
			@thumbnailSize,
			@applyImageOrientation,
			@thumbnailData)
		ON CONFLICT (FilePath, ThumbnailSize, ApplyImageOrientation)
		DO
		UPDATE
		SET FileSizeInBytes = excluded.FileSizeInBytes,
			FileLastModificationTime = excluded.FileLastModificationTime,
			ThumbnailData = excluded.ThumbnailData;
	""";

	private readonly string _thumbnailCacheFolderPath;
	private readonly string _thumbnailCacheDbFilePath;

	private readonly string _connectionString;

	private readonly IFileSizeEngine _fileSizeEngine;

	private bool ShouldCreateDatabase()
	{
		if (!File.Exists(_thumbnailCacheDbFilePath))
		{
			return true;
		}

		var thumbnailCacheDbFileInfo = new FileInfo(_thumbnailCacheDbFilePath);
		if (thumbnailCacheDbFileInfo.Length == 0)
		{
			return true;
		}

		return false;
	}
}
