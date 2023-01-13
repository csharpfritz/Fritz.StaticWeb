﻿using System.IO.Abstractions;
using System.Text.Json;

namespace Fritz.StaticBlog;

public abstract class ActionBase
{
	
	protected readonly IFileSystem _FileSystem;

	protected ActionBase(IFileSystem fileSystem)
	{

		_FileSystem = fileSystem;
	
	}

	internal Config Config { get; set; }

	protected string WorkingDirectory { get; set; }

	protected bool ValidateConfig()
	{

		try
		{

			Console.WriteLine($"WorkingDirectory: {WorkingDirectory}");

			// Set default WorkingDirectory
			if (string.IsNullOrEmpty(WorkingDirectory)) WorkingDirectory = ".";

			var rdr = _FileSystem.File.OpenRead(_FileSystem.Path.Combine(WorkingDirectory, "config.json"));
			var json = new StreamReader(rdr).ReadToEnd();
			this.Config = JsonSerializer.Deserialize<Config>(json);
		}
		catch (Exception ex)
		{
			System.Console.WriteLine($"Error while reading config: {ex.Message}");
			return false;
		}

		if (!_FileSystem.Directory.Exists(_FileSystem.Path.Combine(WorkingDirectory, "themes", Config.Theme)))
		{
			System.Console.WriteLine($"Theme folder '{Config.Theme}' does not exist");
			return false;
		}

		return true;

	}

	public abstract bool Validate();

	public virtual int Execute()
	{

		if (!ValidateConfig()) return 1;

		if (!Validate()) return 1;

		return 0;

	}

}

