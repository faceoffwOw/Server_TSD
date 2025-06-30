using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server_TSD.Models;
using Server_TSD.Models.db_DaxBko;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������������ TLS
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

// ������ ������������ �� �����
string strAosList = System.IO.File.ReadAllText(@"\\bko\NETLOGON\axapta\ax2009\aoslistTSD.txt");
string[] strSplit = strAosList.Trim().Split(';');
string strServer = strSplit[0];
string strDB = strSplit[1];
string conn = "Server=" + strServer + ";Database=" + strDB + ";Trusted_Connection=True;UID=login;PWD=pass;TrustServerCertificate=true;";

// ���������� �������� (������ ConfigureServices � Startup.cs)
builder.Services.AddDbContext<LabelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("bko-shtrih1")));

builder.Services.AddDbContext<dbReplDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReplData")));

builder.Services.AddDbContext<db_DaxBkoContext>(options =>
    options.UseSqlServer(conn));

// ���������� ������������ (�������� AddMvc)
builder.Services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Latest);


var app = builder.Build();

// ������������ ��������� HTTP (������ Configure � Startup.cs)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ��������� ��� middleware ��� API �����
// IS ������� �� ������ ASP NET Core 8.0 � 2.1
// 8.0 ������������� � ������� ������. �� ������� ���������� �� ���������� � ������ �����, ������� ��������� �����
app.UseWhen(context => context.Request.Path.StartsWithSegments("//api"), appBuilder =>
{
    appBuilder.Use((Func<HttpContext, Func<Task>, Task>)(async (context, next) =>
    {
        context.Response.Redirect(context.Request.Path.Value!.Replace("//api", "/api"), true);
        return;
    }));
});

app.UseAuthorization();

app.MapControllers(); // �������� app.UseMvc()

app.Run();