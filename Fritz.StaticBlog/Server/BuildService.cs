using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Fritz.StaticBlog.Server
{

		internal class BuildService : IHostedService
		{
			private string _SourceFolder;
			private string _OutputPath;
			private FileSystemWatcher _Watcher;

			public BuildService(string sourceFolder, string outputPath)
			{
				this._SourceFolder = sourceFolder;
				this._OutputPath = outputPath;
			}

			public event EventHandler<FilesChangedArgs> BuildComplete;

			public Task StartAsync(CancellationToken cancellationToken)
			{

				_Watcher = new FileSystemWatcher(_SourceFolder) {
					EnableRaisingEvents = true,
					IncludeSubdirectories = true
				};

				_Watcher.Changed += (sender, e) => {

					// TODO: Need backoff / throttle logic

					var build = new ActionBuild {
						ThisDirectory = _SourceFolder,
						OutputPath = _OutputPath,
						_LastBuild = new LastBuild { Timestamp= DateTime.MinValue }
					};
					build.Execute();

					BuildComplete?.Invoke(this, new FilesChangedArgs { Files = new [] { e.FullPath } });

				};

				return Task.CompletedTask;

			}

			public Task StopAsync(CancellationToken cancellationToken)
			{
				_Watcher.Dispose();
				return Task.CompletedTask;
			}
		}

}

