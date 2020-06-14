/*
 * This file is part of OpenCollar.Extensions.Security.
 *
 * OpenCollar.Extensions.Security is free software: you can redistribute it
 * and/or modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * OpenCollar.Extensions.Security is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public
 * License for more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * OpenCollar.Extensions.Security.  If not, see <https://www.gnu.org/licenses/>.
 *
 * Copyright © 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace ApiDocs
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var options = new FileServerOptions()
            {
                EnableDefaultFiles = true
            };
            options.StaticFileOptions.DefaultContentType = @"text/html";
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            var path = System.IO.Path.GetFullPath(Path.Combine(env.ContentRootPath, @"..", @"..", @"docs"));
            options.StaticFileOptions.FileProvider = new PhysicalFileProvider(path);
            options.DefaultFilesOptions.DefaultFileNames.Add(@"index.html");

            options.StaticFileOptions.OnPrepareResponse = ctx =>
            {
                ctx.Context.Response.Headers.Append("Cache-Control", $"no-store");
            };

            app.UseFileServer(options);
        }

        // This method gets called by the runtime. Use this method to add services to the container. For more
        // information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}