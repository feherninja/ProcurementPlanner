using ProcurementPlanner.Services;
using Microsoft.AspNetCore.Components;

namespace ProcurementPlanner.Components.Pages;

public class SettingsBase : ComponentBase
{
	[Inject]
	protected AppUpdateService AppUpdateService { get; set; } = null!;

	[Inject]
	protected DatabaseImportService DatabaseImportService { get; set; } = null!;

	[Inject]
	protected DatabaseImportFlow DatabaseImportFlow { get; set; } = null!;
}
