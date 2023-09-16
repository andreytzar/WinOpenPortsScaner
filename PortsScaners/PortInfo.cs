
using System.Diagnostics;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.Sockets;

namespace WinOpenPortsScaner
{
    internal class PortInfo
    {
        public string InputText;
        public string Protocol { get; set; }
        public int? LocalPort { get { return LocaEndPoint?.Port; } }
        public IPEndPoint? LocaEndPoint { get; set; }
        public IPEndPoint? RemoteEndPoint { get; set; }
        public string Status { get; set; }
        public int? PID { get; set; }
        public string? Process { get; set; }
        public string? RemoteInfo { get; set; }


        public PortInfo() { }
        public PortInfo(string text)
        {
            InputText = text;
            try
            {
                string replacedString = Regex.Replace(text, @"\s+", "|");
                var arr = replacedString.Split("|");
                Protocol = arr[1];
                try
                {
                    LocaEndPoint = GetIPEndPoint(arr[2]);
                }
                catch { }
                try
                {
                    RemoteEndPoint = GetIPEndPoint(arr[3]);
                }
                catch { }
                Status = Protocol == "UDP" ?"": arr[4];
                try
                {
                    PID = Protocol=="UDP"?int.Parse(arr[4]): int.Parse(arr[5]);
                }catch { }
            }
            catch { }
        }

        public void InitProcess() => Process = GetProcessByPID(PID);
        public void InitRemoteInfo() => RemoteInfo = GetRemoteInfo(RemoteEndPoint);

        static IPEndPoint? GetIPEndPoint(string endPointString)
        {
            try
            {
                if (endPointString=="*.*" || endPointString== "[::]:0") return null;
                string pattern = @"\[([^\]]+)\]:(\d+)";
                Match match = Regex.Match(endPointString, pattern);

                if (match.Success)
                {
                    int p = int.Parse(match.Groups[2].Value);
                    return new IPEndPoint(IPAddress.Parse("0.0.0.0"), p);
                }
                string[] parts = endPointString.Split(':');
                if (parts.Length != 2) return null;
                string ipAddress = parts[0];
                int port;
                if (!int.TryParse(parts[1], out port))
                    return null;
                IPAddress address = IPAddress.Parse(ipAddress);
                return new IPEndPoint(address, port);
            }
            catch
            {
                return null;
            }
        }

        internal static string? GetProcessByPID(int? PID)
        {
            if (PID==null) return null;
            try
            {
                return System.Diagnostics.Process.GetProcesses().FirstOrDefault(x => x.Id == PID)?.ProcessName;
            }
            catch
            {
                return null;
            }
        }


        internal static string? GetRemoteInfo(IPEndPoint? endPoint)
        {
            if (endPoint==null || endPoint.ToString().Contains("0.0.0.0")) return  null;
            string serverIpAddress = endPoint.Address.ToString();
            int serverPort = endPoint.Port;
            try
            {
                TcpClient tcpClient = new TcpClient(serverIpAddress, serverPort);
                IPEndPoint remoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                IPHostEntry hostEntry = Dns.GetHostEntry(remoteEndPoint.Address);
                string remoteHostName = hostEntry.HostName;
                string remoteHostLocalization = string.Join(";", hostEntry.Aliases);
                var row3 = string.Empty;
                if (hostEntry.AddressList.Length > 1) row3 = $"\n{string.Join("\n", hostEntry.AddressList.ToList())}";
                tcpClient.Close();
                var row2 = string.Empty;
                if (remoteHostName != Dns.GetHostEntry(serverIpAddress).HostName) row2 = $"\n{Dns.GetHostEntry(serverIpAddress).HostName}";
                return $"{remoteHostName} {remoteHostLocalization}{row2}";
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    

}
}
