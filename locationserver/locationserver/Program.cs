using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace locationserver
{
    class Program
    {
        public static Application UI { get; private set; } public static Window MainWindow { get; private set; }
        [STAThread]static void Main(string[] args)
        {
            bool windowedMode = false;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-w": windowedMode = true;
                        continue;
                    case "-l":
                        if (args.Length > i + 1)
                        {
                            if (Uri.IsWellFormedUriString(@args[i + 1], UriKind.RelativeOrAbsolute))
                            {
                                Server.LogFilePath = args[i + 1];
                                Server.Logging = true;
                            }
                            else
                            {
                                Console.WriteLine("Invalid path");
                            }
                        }
                        continue;
                    case "-f":
                        if (args.Length > i + 1)
                        {
                            if (Uri.IsWellFormedUriString(@args[i + 1], UriKind.RelativeOrAbsolute))
                            {
                                Server.NameLocationFilePath = args[i + 1];
                                Server.Saving = true;
                            }
                            else
                            {
                                Console.WriteLine("Invalid path");
                            }
                        }
                        continue;
                    case "-p": if (args.Length > i + 1) Int32.TryParse(args[i+1],out Server.Port);
                        continue;
                    case "-t": if (args.Length > i + 1) Int32.TryParse(args[i + 1], out Server.TimeOut);
                        continue; 
                    case "-nt": Server.TimeOut = 0;
                        continue;
                    case "-d": Server.Debug = true;
                        continue;
                    default:
                        continue;
                }
            }
            if (windowedMode) {new Application().Run(new ServerInterface(Server.Debug, Server.LogFilePath, Server.NameLocationFilePath, Server.TimeOut)); }
            else Server.RunServer();
        }
    }
}
