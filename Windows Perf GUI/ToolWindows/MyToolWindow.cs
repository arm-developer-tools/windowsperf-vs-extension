using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Windows_Perf_GUI
{
    public class MyToolWindow : BaseToolWindow<MyToolWindow>
    {
        public override string GetTitle(int toolWindowId) => "My Tool Window";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new MyToolWindowControl());
        }

        [Guid("e92265f4-3d92-4a09-a90b-75fe312ffed2")]
        internal class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}