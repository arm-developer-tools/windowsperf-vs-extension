using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Perf_GUI.Utils.SDK
{
    public class OutputHandler
    {
        public OutputHandler() { }

        public List<string> Output = new List<string>();
        public Action<string> OutputCb { get; set; }
        public void OutputhHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // Collect the Std output of the process and store it in a list
            this.Output.Add(outLine.Data);
            // Execute the callback function if it is set
            OutputCb?.Invoke(outLine.Data);
        }
    }
}
