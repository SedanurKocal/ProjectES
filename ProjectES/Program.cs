using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectES.Data;
using ProjectES.Models;
using System.Globalization;
using System.Reflection;
using ProjectES.Data;
using ProjectES.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<UserDetails, IdentityRole>()
	.AddDefaultTokenProviders()
	.AddDefaultUI()
	.AddEntityFrameworkStores<ApplicationDbContext>();
//*******
builder.Services.AddControllersWithViews(); //**
builder.Services.AddSingleton<LanguageService>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddMvc()
	.AddViewLocalization()
	.AddDataAnnotationsLocalization(options =>
	{
		options.DataAnnotationLocalizerProvider = (type, factory) =>
		{

			var assemblyName = new AssemblyName(typeof(ShareResource).GetTypeInfo().Assembly.FullName);

			return factory.Create("ShareResource", assemblyName.Name);

		};

	});


builder.Services.Configure<RequestLocalizationOptions>(
	options =>
	{
		var supportedCultures = new List<CultureInfo>
		{
							new CultureInfo("en-US"),
							new CultureInfo("de-DE"),
							new CultureInfo("tr-TR"),
		};



		options.DefaultRequestCulture = new RequestCulture(culture: "tr-TR", uiCulture: "tr-TR");

		options.SupportedCultures = supportedCultures;
		options.SupportedUICultures = supportedCultures;
		options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());

	});
//*************

builder.Services.AddControllersWithViews();
builder.Services.Configure<IdentityOptions>(options =>
{
	// Password settings.
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 3;
	options.Password.RequiredUniqueChars = 0;

});

builder.Services.AddAuthentication(
		CertificateAuthenticationDefaults.AuthenticationScheme)
	.AddCertificate();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
var options = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options.Value);
//******
var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);
//*******
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
