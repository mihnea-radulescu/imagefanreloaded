using System;
using System.IO;
using Microsoft.Data.Sqlite;
using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Caching;

public class SqliteDatabaseLogic : IDatabaseLogic
{
	public SqliteDatabaseLogic(
		IGlobalParameters globalParameters,
		IThumbnailCacheConfig thumbnailCacheConfig)
	{
		_thumbnailCacheFolderPath =
			thumbnailCacheConfig.GetThumbnailCacheFolderPath();
		_thumbnailCacheDbFileName =
			$"{globalParameters.ApplicationName}.db";
		_thumbnailCacheDbFilePath = Path.Combine(
			_thumbnailCacheFolderPath, _thumbnailCacheDbFileName);

		_connectionString = string.Format(
			ConnectionStringPattern, _thumbnailCacheDbFilePath);
	}

	public void CreateDatabaseIfNotExisting()
	{
		try
		{
			if (!Directory.Exists(_thumbnailCacheFolderPath))
			{
				Directory.CreateDirectory(_thumbnailCacheFolderPath);
			}

			if (!File.Exists(_thumbnailCacheDbFileName))
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

	public void DeleteDatabase()
	{
		try
		{
			if (Directory.Exists(_thumbnailCacheFolderPath))
			{
				var directoryInfo = new DirectoryInfo(
					_thumbnailCacheFolderPath);

				var filesWithPattern = directoryInfo.GetFiles(
					$"{_thumbnailCacheDbFileName}*");

				foreach (var aFileWithPattern in filesWithPattern)
				{
					aFileWithPattern.Delete();
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
		"PRAGMA auto_vacuum = INCREMENTAL; PRAGMA journal_mode = WAL;";

	private const string CreateTableScript = """
		CREATE TABLE IF NOT EXISTS ThumbnailCacheEntries (
			ImageFilePath TEXT NOT NULL,
			ThumbnailSize INTEGER NOT NULL,
			ApplyImageOrientation BOOLEAN NOT NULL,
			LastModificationTime DATETIME NOT NULL,
			ThumbnailData BLOB NOT NULL,
			PRIMARY KEY (ImageFilePath, ThumbnailSize, ApplyImageOrientation));
	""";

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
	private readonly string _thumbnailCacheDbFileName;
	private readonly string _thumbnailCacheDbFilePath;

	private readonly string _connectionString;
}
