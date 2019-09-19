using System;
using System.Net.Sockets;
using System.IO;
using location;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
public class Whois
{
    public static ClientInterface GUI = new ClientInterface();
    [STAThread] static void Main(string[] args)
    {
        if (args.Length == 0) { new Application().Run(GUI = new ClientInterface()); }
        else
        {
            List<string> mArgsList = args.ToList();

            for (int i = 0; i < mArgsList.Count; i++)
            {
                switch (mArgsList[i])
                {
                    case "-nt":
                        Client.Timeout_B = 0;
                        mArgsList.RemoveAt(i);
                        i = -1;
                        continue;
                    case "-l":
                        Client.HostName_B = "localhost";
                        mArgsList.RemoveAt(i);
                        i = -1;
                        continue;
                    case "-d":
                        Client.DebugMode_B = true;
                        mArgsList.RemoveAt(i);
                        i = -1;
                        continue;
                    case "-h":
                        if (mArgsList.Count > i + 1)
                        {
                            Client.HostName_B = mArgsList[i + 1];
                            mArgsList.RemoveAt(i);
                        }
                        mArgsList.RemoveAt(i);
                        i = -1;
                        continue;
                    case "-p":
                        if (mArgsList.Count > i + 1)
                        {
                            Int32.TryParse(mArgsList[i + 1], out int f);
                            Client.Port_B = f;
                            mArgsList.RemoveAt(i);
                        }
                        mArgsList.RemoveAt(i);
                        i = -1;
                        continue;
                    case "-h0":
                        Client.Protocol_B = "0";
                        mArgsList.RemoveAt(i);
                        i = -1;
                        continue;
                    case "-h1":
                        Client.Protocol_B = "1";
                        mArgsList.RemoveAt(i);
                        i = -1;
                        continue;
                    case "-h9":
                        Client.Protocol_B = "9";
                        mArgsList.RemoveAt(i);
                        i = -1;
                        continue;
                    default:
                        continue;
                }
            }
            switch (mArgsList.Count)
            {//DOES IT HANDLE MULTIPLE ARGS?????????????????????????
                case 0:
                    Client.Name_B = "";
                    Client.RequestType_B = RequestType.Lookup;
                    break;
                case 1:
                    Client.Name_B = mArgsList[0];
                    Client.RequestType_B = RequestType.Lookup;
                    break;
                default:
                    Client.Name_B = mArgsList[0];
                    Client.Location_B = String.Join(" ", mArgsList.ToArray(), 1, mArgsList.Count - 1);
                    Client.RequestType_B = RequestType.Update;
                    break;
            }
            Client.ExecuteClient();
        }
    }
}