using Velopack;

namespace ProcurementPlanner.WinUI;

// Replaces the XAML generated Main (DISABLE_XAML_GENERATED_MAIN in the csproj) so Velopack runs
// before anything else: it handles install/update hooks and applies downloaded updates on startup
public static class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		VelopackApp.Build().Run();

		WinRT.ComWrappersSupport.InitializeComWrappers();
		Microsoft.UI.Xaml.Application.Start(_ =>
		{
			var context = new Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
			SynchronizationContext.SetSynchronizationContext(context);
			new App();
		});
	}
}
