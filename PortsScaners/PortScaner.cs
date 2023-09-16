using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WinOpenPortsScaner
{
    internal static class PortScaner
    {
        public static List<PortInfo> Scan()
        {
            {
                var res = new List<PortInfo>();
                Process process = new Process();
                process.StartInfo.FileName = "netstat";
                process.StartInfo.Arguments = "-ano";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    var pi = new PortInfo(line);
                    if (pi.LocalPort!=null)
                        res.Add(new PortInfo(line));
                }
                return res;
            }

        }

    }
}
