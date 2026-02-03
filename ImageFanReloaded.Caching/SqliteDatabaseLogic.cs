using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.DiscAccess;
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
			$"{globalParameters.ApplicationName}.db";
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
		string imageFilePath,
		int thumbnailSize,
		bool applyImageOrientation,
		DateTime lastModificationTime)
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
						"@imageFilePath", imageFilePath);
					dbCommand.Parameters.AddWithValue(
						"@thumbnailSize", thumbnailSize);
					dbCommand.Parameters.AddWithValue(
						"@applyImageOrientation", applyImageOrientation);
					dbCommand.Parameters.AddWithValue(
						"@lastModificationTime", lastModificationTime);

					using (var dataReader = dbCommand.ExecuteReader())
					{
						if (dataReader.Read())
						{
							var thumbnailData = dataReader
								.GetFieldValue<byte[]>(0);

							return new ThumbnailCacheEntry
							{
								ImageFilePath = imageFilePath,
								ThumbnailSize = thumbnailSize,
								ApplyImageOrientation = applyImageOrientation,
								LastModificationTime = lastModificationTime,
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
						"@imageFilePath", thumbnailCacheEntry.ImageFilePath);
					dbCommand.Parameters.AddWithValue(
						"@thumbnailSize", thumbnailCacheEntry.ThumbnailSize);
					dbCommand.Parameters.AddWithValue(
						"@applyImageOrientation",
						thumbnailCacheEntry.ApplyImageOrientation);
					dbCommand.Parameters.AddWithValue(
						"@lastModificationTime",
						thumbnailCacheEntry.LastModificationTime);
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
			ImageFilePath TEXT NOT NULL,
			ThumbnailSize INTEGER NOT NULL,
			ApplyImageOrientation BOOLEAN NOT NULL,
			LastModificationTime DATETIME NOT NULL,
			ThumbnailData BLOB NOT NULL,
			PRIMARY KEY (ImageFilePath, ThumbnailSize, ApplyImageOrientation));
	""";

	private const string ClearDataScript = "DELETE FROM ThumbnailCacheEntries;";
	private const string ClearWriteAheadLogScript =
		"PRAGMA wal_checkpoint(FULL);";
	private const string TrimDatabaseScript = "VACUUM;";

	private const string GetThumbnailCacheEntryScript = """
		SELECT ThumbnailData
		FROM ThumbnailCacheEntries
		WHERE ImageFilePath = @imageFilePath
			AND ThumbnailSize = @thumbnailSize
			AND ApplyImageOrientation = @applyImageOrientation
			AND LastModificationTime = @lastModificationTime
	""";

	private const string UpsertThumbnailCacheEntryScript = """
		INSERT INTO ThumbnailCacheEntries (
			ImageFilePath,
			ThumbnailSize,
			ApplyImageOrientation,
			LastModificationTime,
			ThumbnailData)
		VALUES (
			@imageFilePath,
			@thumbnailSize,
			@applyImageOrientation,
			@lastModificationTime,
			@thumbnailData)
		ON CONFLICT (ImageFilePath, ThumbnailSize, ApplyImageOrientation)
		DO
		UPDATE
		SET LastModificationTime = excluded.LastModificationTime,
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
