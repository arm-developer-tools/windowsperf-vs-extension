using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPerfGUI.SDK
{
    internal class WperfClientFactory : WperfClient
    {
        public WperfClientFactory()
        {
            AssignPathAsync().FireAndForget();
        }
        private async Task AssignPathAsync()
        {
            var options = await WPerfPath.GetLiveInstanceAsync();
            this.Path = options.WperfPath;
        }
    }
}
