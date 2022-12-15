using MachineBoxingManagement.Repositories.Data;
using MachineBoxingManagement.Web.Services.Implements;
using MachineBoxingManagement.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var configuration = builder.Configuration;

// Add services to the container.
// ���U�A��
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
/*
 *Ref�G
 *1.�p��b ASP.NET Core 3 �ϥ� Token-based �������һP���v (JWT)
 *https://blog.miniasp.com/post/2019/12/16/How-to-use-JWT-token-based-auth-in-aspnet-core-31 
 */

    options.IncludeErrorDetails = true; // �w�]�Ȭ� true�A���ɷ|�S�O����

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true, //�O�_����Issuer
        ValidIssuer = configuration["JwtSettings:Issuer"], //�o��HIssuer

        ValidateAudience = false, //�O�_����Audience
        //ValidAudience = configuration["Jwt:Audience"], //�q�\�HAudience

        // �@��ڭ̳��|���� Token �����Ĵ���
        ValidateLifetime = true,

        // �p�G Token ���]�t key �~�ݭn���ҡA�@�볣�u��ñ���Ӥw
        ValidateIssuerSigningKey = false,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SignKey"]))
    };
});
builder.Services.AddAuthorization();
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
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IXlsxReportService, XlsxReportService>();
builder.Services.AddScoped<IJwtService, JwtService>();




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
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireAuthorization();
});
app.UseSpa(spa => { });
app.Run();
