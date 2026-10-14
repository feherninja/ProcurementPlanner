using CommunityToolkit.Maui;
using ProcurementPlanner.Db;
using ProcurementPlanner.Interfaces;
using ProcurementPlanner.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using MudBlazor.Services;

namespace ProcurementPlanner;

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

		builder.ConfigureLifecycleEvents(events =>
		{
#if WINDOWS
			events.AddWindows(windows =>
			{
				windows.OnWindowCreated(window =>
				{
					var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
					var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
					var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);					if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
						presenter.Maximize();
				});
			});
#endif
		});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddMudServices();

		// Register Db
		var appUpdateService = new AppUpdateService();
		builder.Services.AddSingleton(appUpdateService);

		var connectionString = $"Data Source={appUpdateService.DatabasePath}";

		builder.Services.AddDbContext<ProcurementDbContext>(options => options.UseSqlite(connectionString));
		builder.Services.AddDbContextFactory<ProcurementDbContext>(options => options.UseSqlite(connectionString));

		// Register Services 
		builder.Services.AddScoped<IComponentService, ComponentService>();
		builder.Services.AddScoped<IItemService, ItemService>();
		builder.Services.AddScoped<DeliveryPlanningService>();
		builder.Services.AddScoped<IDeliveryDocumentService, DeliveryDocumentService>();
		builder.Services.AddScoped<ICompanyService, CompanyService>();
		builder.Services.AddScoped<IPurchaseOrderDocumentService, PurchaseOrderDocumentService>();
		builder.Services.AddScoped<CompanyContext>();
		builder.Services.AddSingleton<DatabaseImportService>();
		builder.Services.AddScoped<DatabaseImportFlow>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// Ensure database is created on startup
		using var scope = app.Services.CreateScope();

		var context = scope.ServiceProvider.GetRequiredService<ProcurementDbContext>();
		context.Database.Migrate();

		return app;
	}
}