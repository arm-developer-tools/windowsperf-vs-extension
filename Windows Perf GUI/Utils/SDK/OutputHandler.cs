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

        public List<string> Outputh = new List<string>();
        public Action<string> OutputhCb { get; set; }
        public void OutputhHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            // Collect the Std output of the process and store it in a list
            this.Outputh.Add(outLine.Data);
            // Execute the callback function if it is set
            OutputhCb?.Invoke(outLine.Data);
        }
    }
}
