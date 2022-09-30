using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Web.Services.Implements;
using MachineBoxingManagement.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddControllers();
// In production, the Angular files will be served from this directory
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

builder.Services.AddDbContext<CAEDB01Context>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("CAEDB01Connection")));
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IBoxOutService, BoxOutService>();
builder.Services.AddScoped<IBoxInService, BoxInService>();
builder.Services.AddScoped<IXlsxReportService, XlsxReportService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("allowCors",
    builder =>
    {
        builder.WithOrigins("http://localhost:4200")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials()
          .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = System.TimeSpan.FromSeconds(3600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseCors("allowCors");
}

app.UseHttpsRedirection();
app.UseSpaStaticFiles();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseSession();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireAuthorization();
});
app.UseSpa(spa => { });
app.Run();
