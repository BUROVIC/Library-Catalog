using AutoMapper;
using LibraryCatalog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LibraryCatalog
{
	public class Startup
	{
		private readonly string _connectionString;

		public Startup(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<LibraryCatalogDbContext>(options => options.UseSqlite(_connectionString));

			services.AddAutoMapper(typeof(MappingProfile));

			services.AddControllers();

			services.AddSwaggerGen();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LibraryCatalogDbContext dataContext)
		{
			app.UseSwagger();

			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Catalog API V1");
				options.RoutePrefix = string.Empty;
			});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			dataContext.Database.EnsureCreated();

			app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
			app.UseRouting();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}