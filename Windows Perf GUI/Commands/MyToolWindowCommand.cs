namespace Windows_Perf_GUI
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyToolWindowCommand : BaseCommand<MyToolWindowCommand>
    {
        protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            return MyToolWindow.ShowAsync();
        }
    }
}
