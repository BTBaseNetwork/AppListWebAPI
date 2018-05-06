using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AppListWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(op =>
            {
                op.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                op.SerializerSettings.Formatting = Formatting.None;
            });

            services.AddDbContextPool<DAL.BTBaseDbContext>(ac =>
            {
                ac.UseMySQL(Environment.GetEnvironmentVariable("MYSQL_CONSTR"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            LoadAPISignCodes();
            ConnectDB();
        }

        private void ConnectDB()
        {

        }

        public static Dictionary<string, string> APISigncodesDict { get; private set; }
        private void LoadAPISignCodes()
        {
            APISigncodesDict = new Dictionary<string, string>();
            var signcodes = Environment.GetEnvironmentVariable("API_SIGNCODES");
            foreach (var item in signcodes.Split(';'))
            {
                var signkv = item.Split(':');
                APISigncodesDict[signkv[0]] = signkv[1];
            }
        }
    }
}
