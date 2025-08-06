using CommunityToolkit.Maui;
using DeliveryManager.Db;
using DeliveryManager.Interfaces;
using DeliveryManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace DeliveryManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			})
			.UseMauiCommunityToolkit();

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddMudServices();

		// Register Db

		var appFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		var dbPath = Path.Combine(appFolder, "delivery.db");

		//TODO remove static path
		dbPath = @"f:\_databases\DeliveryManager\delivery.db";
		var connectionString = $"Data Source={dbPath}";

		builder.Services.AddDbContext<DeliveryDbContext>(options => options.UseSqlite(connectionString));
		builder.Services.AddDbContextFactory<DeliveryDbContext>(options => options.UseSqlite(connectionString));

		// Register Services 
		builder.Services.AddScoped<IComponentService, ComponentService>();
		builder.Services.AddScoped<IItemService, ItemService>();
		builder.Services.AddScoped<DeliveryPlanningService>();
		builder.Services.AddScoped<IDeliveryDocumentService, DeliveryDocumentService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// Ensure database is created on startup
		using var scope = app.Services.CreateScope();

		var context = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
		context.Database.Migrate();

		return app;
	}
}