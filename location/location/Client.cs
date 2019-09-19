using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace location
{
    public enum RequestType
    {
        Lookup = 0, Update = 1
    }
    static class Client
    {
        //-h threethinggame.com page/08140 -p 80 -h1 -nt
        #region Variables
        /// <summary>
        /// Class Variables with default values
        /// </summary>
        private static RequestType mRequestType;
        private static TcpClient mClient;
        private static string mName = String.Empty;
        private static string mLocation = String.Empty;
        private static string mHostName = "whois.net.dcs.hull.ac.uk";
        private static int mPort = 43, mTimeout = 1000, mContentLength = 0;
        private static string mProtocol = "whois";
        private static StreamWriter sw;
        private static StreamReader sr;
        private static string mServerResponse = String.Empty;
        private static bool mDebug = false;
        private static char[] mBuffer = null;
        private static List<string> mOptionalHeaderLines = null;
        static bool mNoTimeOut { get { return (mTimeout == 0); } }
        /// <summary>
        /// Data Binding Properties
        /// </summary>
        public static RequestType RequestType_B { get { return mRequestType; } set { mRequestType = value; } }
        public static bool DebugMode_B { get { return mDebug; } set { mDebug = value; } }
        public static int Timeout_B { get { return mTimeout; } set { mTimeout = value; } }
        public static string HostName_B { get { return mHostName; } set { mHostName = value; } }
        public static string Name_B { get { return mName; } set { mName = value; } }
        public static string Location_B { get { return mLocation; } set { mLocation = value; } }
        public static int Port_B { get { return mPort; } set { mPort = value; } }
        public static string ServerResponse_B { get { return mServerResponse; } set { mServerResponse = value; } }
        public static string Protocol_B { get { return mProtocol; } set { mProtocol = value; } }
        #endregion
        public static void ExecuteClient()
        {
            try
            {
                mClient = new TcpClient();
                mClient.Connect(mHostName, mPort);
                if (!mNoTimeOut)
                {
                    mClient.ReceiveTimeout = mTimeout;
                    mClient.SendTimeout = mTimeout;
                }
                sw = new StreamWriter(mClient.GetStream());
                sr = new StreamReader(mClient.GetStream());
                RunProtocol();
                Whois.GUI.NotifyPropertyChanged(String.Empty);
            }
            catch (Exception e)
            {
                ConsoleWriteLine("Failed to update GUI. Reason: {0}", true, e);
                Console.WriteLine("Something went wrong");
            }
            finally
            {
                mClient.Close();
            }
        }
        #region Helper Functions
        private static void ConsoleWriteLine(string pOutput, bool pDebugOnly, params object[] pOtherParams)
        {
            if ((pDebugOnly && mDebug) || !pDebugOnly) Console.WriteLine(String.Format(pOutput, pOtherParams));
        }
        private static string ContentLength()
        {
            int length = 0;
            switch (mProtocol[mProtocol.Length - 1])
            {
                case '0':
                    length = mLocation.Length;
                    break;
                case '1':
                    length = mName.Length + mLocation.Length + 15;
                    break;
                default:
                    break;
            }
            return length.ToString().Trim();
        }
        private static void RunProtocol()
        {
            try
            {
                switch (mProtocol[mProtocol.Length - 1])
                {
                    case 's':
                        P_whois();
                        break;
                    case '0':
                        P_0();
                        break;
                    case '1':
                        P_1();
                        break;
                    case '9':
                        P_9();
                        break;
                    default:
                        P_whois();
                        break;
                }
                ConsoleWriteLine("Sending {0} Request with {1}", true, mRequestType.ToString(), mProtocol);//DEBUG PRINT ONLY
            }
            catch (Exception)
            {
                ConsoleWriteLine("Something went wrong", false);
            }
        }
        #endregion
        #region Protocols
        private static void P_whois()
        {
            if (mRequestType == RequestType.Lookup)
            {
                sw.WriteLine(mName);
                sw.Flush();
                mServerResponse = sr.ReadLine();
                if (mServerResponse == "ERROR: no entries found")
                {
                    ConsoleWriteLine(mServerResponse, false);//ALWAYS PRINT
                }
                else
                {
                    ConsoleWriteLine("{0} is {1}", false, mName, mServerResponse);//ALWAYS PRINT
                }
            }
            else
            {
                sw.WriteLine(mName + " " + mLocation);
                sw.Flush();
                if (sr.ReadLine().Trim() == "OK")
                {
                    ConsoleWriteLine("{0} location changed to be {1}", false, mName, mLocation);//ALWAYS PRINT
                }
            }
        }
        /// <summary>
        /// HTTP 1.0
        /// </summary>
        private static void P_0()
        {
            if (mRequestType == RequestType.Lookup)
            {
                sw.WriteLine("GET /?" + mName + " " + "HTTP/1.0" + "\r\n");
                sw.Flush();
                mServerResponse = sr.ReadToEnd().Replace("\r\n","");

                if (mServerResponse.Substring(9, 3) == "200")
                {
                    ConsoleWriteLine("{0} is {1}", false, mName, mServerResponse.Substring(39).Trim());//ALWAYS PRINT
                }
                else if (mServerResponse.Substring(9, 3) == "404")
                {
                    ConsoleWriteLine("ERROR: no entries found", false);//ALWAYS PRINT
                }
            }
            else
            {
                sw.Write("POST" + " " + "/" + mName + " " + "HTTP/1.0" + "\r\n"
                    + "Content-Length:" + " " + ContentLength() + "\r\n" 
                    + "\r\n"
                    + mLocation);
                sw.Flush();
                mServerResponse = sr.ReadToEnd().Replace("\r\n", "");
                if (mServerResponse.Substring(9, 3) == "200")
                {
                    ConsoleWriteLine("{0} location changed to be {1}", false,  mName, mLocation);//ALWAYS PRINT
                }
                else
                {
                    ConsoleWriteLine("ERROR: too many arguments", false);//ALWAYS PRINT
                }
            }
        }
        /// <summary>
        /// HTTP 1.1
        /// </summary>
        private static void P_1()
        {
            if (mRequestType == RequestType.Lookup)
            {
                sw.WriteLine("GET" + " " + "/?name=" + mName + " " + "HTTP/1.1" + "\r\n"
                    + "Host:" + " " + mHostName + "\r\n");

                sw.Flush();

                string firstLine = sr.ReadLine();

                try
                {
                    if (!OptionalHeaderReader())
                    {
                        mServerResponse = sr.ReadLine();
                    }
                    else
                    {
                        sr.ReadBlock(mBuffer = new char[mContentLength], 0, mContentLength);
                        mServerResponse = new string(mBuffer);
                    }
                }
                catch (Exception e)
                {
                    ConsoleWriteLine("Exception \n{0}", true, e);
                }
                ConsoleWriteLine("Managed to read: \n\n {0}", true, mServerResponse);
                ConsoleWriteLine("----------------", true);
                ConsoleWriteLine(mServerResponse, true);// DEBUG PRINT ONLY

                if (firstLine.Substring(9, 3) == "200")
                {
                    ConsoleWriteLine("{0} is {1}", false, mName, mServerResponse);//ALWAYS PRINT
                }
                else
                {
                    ConsoleWriteLine("ERROR: no entries found", false);// ALWAYS PRINT
                }
            }
            else
            {

                sw.Write("POST" + " " + "/" + " " + "HTTP/1.1" + "\r\n"
                    + "Host:" + " " + mHostName + "\r\n"
                    + "Content-Length:" + " " + ContentLength() + "\r\n"
                    + "\r\n"
                    + "name=" + mName + "&location=" + mLocation);
                sw.Flush();
                try
                {
                    if (!OptionalHeaderReader())
                    {
                        mServerResponse = sr.ReadToEnd().Replace("\r\n", "");
                    }
                    else
                    {
                        sr.ReadBlock(mBuffer = new char[mContentLength], 0, mContentLength);
                        mServerResponse = new string(mBuffer);
                    }
                }
                catch (Exception e)
                {
                    ConsoleWriteLine("Exception \n{0}", true, e);
                }
                if (mOptionalHeaderLines[0].Substring(9, 3) == "200")
                {
                    ConsoleWriteLine("{0} location changed to be {1}", false, mName, mLocation);//ALWAYS PRINT
                }
                else
                {
                    ConsoleWriteLine("ERROR: too many arguments", false);//ALWAYS PRINT
                }
            }
        }
        /// <summary>
        /// HTTP 0.9
        /// </summary>
        private static void P_9()
        {
            if (mRequestType == RequestType.Lookup)
            {
                sw.WriteLine("GET /" + mName);
                sw.Flush();
                mServerResponse = sr.ReadToEnd().Replace("\r\n", "");
                if (mServerResponse.Substring(9, 3) == "200")
                {
                    ConsoleWriteLine("{0} is {1}", false, mName, mServerResponse.Substring(39).Trim());// ALWAYS PRINT
                }
                else if (mServerResponse.Substring(9, 3) == "404")
                {
                    ConsoleWriteLine("ERROR: no entries found", false);//ALWAYS PRINT
                }
            }
            else
            {
                sw.WriteLine("PUT" + " " + "/" + mName + "\r\n"
                    + "\r\n" + mLocation);
                sw.Flush();
                mServerResponse = sr.ReadToEnd().Replace("\r\n", "");
                if (mServerResponse.Substring(13, 2) == "OK")
                {
                    ConsoleWriteLine("{0} location changed to be {1}", false, mName, mLocation);//ALWAYS PRINT
                }
                else
                {
                    ConsoleWriteLine("ERROR: too many arguments", false);//ALWAYS PRINT
                }
            }
        }
        private static bool OptionalHeaderReader()
        {
            mOptionalHeaderLines = new List<string>();
            bool found = false;
            string line;
            while ((line = sr.ReadLine()) != "")
            {
                mOptionalHeaderLines.Add(line);
                if (line.Contains("Content-Length: "))
                {
                    Int32.TryParse(new string(line.Where(c => (char.IsDigit(c))).ToArray()), out mContentLength);
                    if (mContentLength != 0) found = true;
                    else mLocation = ""; found = false;
                }
            }
            return found;
        }
        #endregion
    }
}
