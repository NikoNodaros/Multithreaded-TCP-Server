using System;
using System.Reflection;
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

//[assembly: AssemblyVersion("4.3.2.1")]
namespace locationserver
{
    public class Server
    {
        //Create thred for writing to console
        internal static ConcurrentDictionary<string, string> NameLocation = new ConcurrentDictionary<string, string>();
        internal static string LogFilePath = null, NameLocationFilePath = null;
        internal static string BackUpPath {get { return NameLocationFilePath.Remove(NameLocationFilePath.Length - 4) + "-temp.dat"; } }
        internal static ConcurrentQueue<string> LogQueue = new ConcurrentQueue<string>(), ConsoleQueue = new ConcurrentQueue<string>();
        private static FileStream fs;
        private static BinaryFormatter formatter = new BinaryFormatter();
        internal static bool  Debug = false, KeepListening = true, Logging = false, Saving = false;
        internal static volatile bool NameLocationChanged = false, RequestLogging = false;
        internal static int Port = 43, TimeOut = 1000;
        internal static TcpListener listener = null;

        public static void Factory(bool pDebug, string pLogFilePath, string pSaveFilePath, int pTimeOut)
        {
            Debug = pDebug; LogFilePath = pLogFilePath; NameLocationFilePath = pSaveFilePath; TimeOut = pTimeOut;
        }
        internal static void ConsoleWriteLine(string pOutput, bool pDebugOnly, params object[] pOtherParams)
        {
            if ((pDebugOnly && Debug) || !pDebugOnly) ConsoleQueue.Enqueue(String.Format(pOutput, pOtherParams));
        }
        static void OnExit(object sender, EventArgs e) { Server.ConsoleWriteLine("Exiting", false); Console.ReadKey(); }
        public static void RunServer()
        {
            KeepListening = true;
            new Thread(()=> 
            {
                while (true)
                {
                    if (ConsoleQueue.TryDequeue(out string line))
                    {
                        Console.WriteLine(line);
                    }
                }
            }).Start();
            if (Saving)
            {
                try { fs = new FileStream(File.Exists(NameLocationFilePath)? NameLocationFilePath : BackUpPath, FileMode.OpenOrCreate); } catch (FileLoadException) { Server.ConsoleWriteLine("Could not load file", false); }
                if (fs.Length != 0) try { NameLocation = (ConcurrentDictionary<string, string>)formatter.Deserialize(fs); } catch (SerializationException e) { Server.ConsoleWriteLine("Failed to deserialize. Reason: {1}", true, e.Message); }
                if (fs != null)  fs.Close();
                if (NameLocation != null && NameLocation.Count() > 0)
                {
                    Server.ConsoleWriteLine("Data successfully loaded: ", false);
                    if (Debug) foreach (KeyValuePair<string, string> entry in NameLocation)
                    {
                        Server.ConsoleWriteLine("{0} is in {1}",true , entry.Key, entry.Value);
                    }
                }
                else Server.ConsoleWriteLine("No records found", false);
            }
            try
            {
                listener = new TcpListener(IPAddress.Any, Port); listener.Start(); Server.ConsoleWriteLine("Server started Listening", false);
                if(Logging)new Thread(() =>
                {   //if (!File.Exists(LogFilePath)) Directory.CreateDirectory(LogFilePath);
                    while (KeepListening)
                    {
                        if (LogFilePath != null && RequestLogging)
                        {
                            Server.ConsoleWriteLine("Logging to file", true);
                            try
                            {
                                StreamWriter SW = File.AppendText(LogFilePath);
                                RequestLogging = false;
                                while (LogQueue.TryDequeue(out string line))
                                {
                                    Server.ConsoleWriteLine(line, true);
                                    SW.WriteLine(line);
                                }
                                SW.Close();
                            }
                            catch (Exception e)
                            {
                                Server.ConsoleWriteLine("Unable to Write Log File {0} \n {1}", true, LogFilePath, e);
                            }
                        }
                    }
                }).Start();//Log
                if(Saving)new Thread(() =>
                {
                    while (KeepListening)
                    {
                        if (NameLocationChanged)
                        {
                            NameLocationChanged = false;
                            try
                            {
                                fs = File.Create(BackUpPath);
                                formatter.Serialize(fs, NameLocation); fs.Close();
                                if (File.Exists(NameLocationFilePath)) File.Delete(NameLocationFilePath);
                                File.Move(BackUpPath, NameLocationFilePath);
                                //if (debugMode && NameLocation.Count > 0) Server.ConsoleWriteLine("Things to save");
                            }
                            catch (SerializationException e)
                            {
                                Server.ConsoleWriteLine("Failed to serialize. Reason: {0}", true, e.Message);
                            }
                        }
                    }
                }).Start();//Save dictionary to file
                ThreadPool.SetMaxThreads(1000, 2000);
                int counter = 0;
                while (KeepListening)
                {
                    if (listener.Pending())
                    {
                        Server.ConsoleWriteLine("------------------", false);
                        Server.ConsoleWriteLine("Connection Pending", false);
                        ((Action)(async () =>
                        {
                            Server.ConsoleWriteLine("-- Connecting Socket #{0}", false, ++counter);
                            ThreadPool.QueueUserWorkItem(Handler.Factory, await listener.AcceptSocketAsync());
                        }))();
                    }
                }
            }
            catch (Exception e)
            {
                Server.ConsoleWriteLine("Exception: {0}", true, e.ToString());
            }
        }
        public static void StopServer()
        {
            KeepListening = false;
            listener.Stop();
            Server.ConsoleWriteLine("Server stopped listening", false);
        }
    }
}
