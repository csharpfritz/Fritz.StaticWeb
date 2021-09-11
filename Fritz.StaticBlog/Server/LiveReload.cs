using System;
using Microsoft.AspNetCore.Builder;

namespace Fritz.StaticBlog.Server
{

	// Configure to use WebSocket middleware and behave like middleware as explained
	// in https://docs.microsoft.com/aspnet/core/fundamentals/websockets#accept-websocket-requests

	public class LiveReload
	{
		internal void Use(IApplicationBuilder app)
		{
			throw new NotImplementedException();
		}
	}

	public static class LiveReloadExtensions 
	{

		public static IApplicationBuilder UseLiveReload(this IApplicationBuilder app)
		{

			app.UseWebSockets();

			var liveReload = new LiveReload();
			liveReload.Use(app);

			return app;

		}

	}

}