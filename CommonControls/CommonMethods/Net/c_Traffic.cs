using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CommonControls.CommonMethods.Net
{
    public class c_Traffic
    {
        public static void GetNetworkTraffic(ref float DownloadSpeed, ref float UploadSpeed)
        {
            PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
            string instance = performanceCounterCategory.GetInstanceNames()[0]; // 1st NIC !
            PerformanceCounter performanceCounterSent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
            PerformanceCounter performanceCounterReceived = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);

            DownloadSpeed = performanceCounterReceived.NextValue(); // /1024
            UploadSpeed = performanceCounterSent.NextValue(); // /1024
        }
    }
}
