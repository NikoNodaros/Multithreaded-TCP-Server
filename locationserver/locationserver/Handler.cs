using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
namespace locationserver
{
    internal class Handler
    {
        private string protocol, name, location, reqTyp, clientIP;
        private string[] requestLines;
        private bool isWhoIsProtocol = true, requestIsOk, isLookUp;
        private StreamReader sr; private StreamWriter sw;
        private string ReqStatus
        {
            get
            {
                if (requestIsOk)
                {
                    return (isWhoIsProtocol) ? "OK" : "200" + " " + "OK";
                }
                return (isWhoIsProtocol) ? "ERROR: no entries found" : "404" + " " + "Not Found";
            }
        }
        public static void Factory(object o)
        {
            Server.ConsoleWriteLine("Creating Request Handler", true);
            new Handler().Request((Socket)o);
        }
        private async void Request(Socket connection)
        {
            Server.ConsoleWriteLine("Handling Request", false);

            NetworkStream socketStream = new NetworkStream(connection);
            clientIP = connection.RemoteEndPoint.ToString().Split(':')[0];
            String Host = ((IPEndPoint)connection.RemoteEndPoint).Address.ToString();


            Server.ConsoleWriteLine("Connection Info: {0}", true, connection.ToString());
            Server.ConsoleWriteLine("Connection Received", false);
            Server.ConsoleWriteLine("Client IP: {0}", false, clientIP);

            sr = new StreamReader(socketStream);
            sw = new StreamWriter(socketStream);

            if (Server.TimeOut > 0)
            {
                socketStream.ReadTimeout = Server.TimeOut;
                socketStream.WriteTimeout = Server.TimeOut;
            }
            try
            {
                string firstLine = await TimeOutCheck(sr.ReadLineAsync()); 

                Server.ConsoleWriteLine("First Line received: '{0}'", true, firstLine);

                string[] firstLineSections = firstLine.Split(' ');
                name = firstLineSections[0];

                bool TwoOrMoreArgs = (firstLineSections.Length >= 2);

                if (TwoOrMoreArgs && !firstLineSections[1].StartsWith("/"))
                {
                    location = string.Join(" ", firstLineSections, 1, firstLineSections.Length -1);
                }
                else if (TwoOrMoreArgs)
                {
                    isWhoIsProtocol = false;
                    reqTyp = firstLineSections[0];
                    if (firstLineSections.Length == 2)// h9
                    {
                        protocol = "HTTP/0.9";
                        name = firstLineSections[1].Substring(1).Replace("\r\n", "");
                        if (reqTyp == "PUT")
                        {
                            RequestLinesReader(2);
                            location = requestLines[1].Replace("\r\n", "");
                        }
                    }
                    else if (firstLineSections[2] == "HTTP/1.0")// h0
                    {
                        protocol = firstLineSections[2];
                        if (reqTyp == "GET")
                        {
                            name = firstLineSections[1].Substring(2).Trim();
                            RequestLinesReader(0);
                        }
                        else if (reqTyp == "POST")
                        {
                            name = firstLineSections[1].Substring(1).Trim();
                            RequestLinesReader(1);
                        }
                    }
                    else if (firstLineSections[2] == "HTTP/1.1")// h1
                    {
                        protocol = firstLineSections[2];
                        if (reqTyp == "GET")
                        {
                            name = firstLineSections[1].Substring(7).Trim();//start at position 7
                            RequestLinesReader(1);
                        }
                        else if (reqTyp == "POST")
                        {
                            RequestLinesReader(2);
                        }
                    }
                }
                if (reqTyp == "POST" || reqTyp == "PUT" || ((location != null && location != "") && isWhoIsProtocol))//message client
                {
                    isLookUp = false;
                }
                else isLookUp = true;
                if (Server.NameLocation.ContainsKey(name))
                {
                    if (isLookUp) Server.NameLocation.TryGetValue(name, out location);
                    else
                    {
                        Server.NameLocation[name] = location;
                        Server.NameLocationChanged = true;
                    }
                    requestIsOk = true;
                }
                else
                {
                    if (isLookUp) { requestIsOk = false; }
                    else
                    {
                        requestIsOk = true;
                        Server.NameLocation.TryAdd(name, location);
                        Server.NameLocationChanged = true;
                    }
                }
                if (isWhoIsProtocol)
                {
                    if(!isLookUp) //If update
                    {
                        sw.WriteLine(ReqStatus);
                    }
                    else if (requestIsOk) //lookup successful
                    {
                        sw.WriteLine(location);
                    }
                    else
                    {
                        sw.WriteLine(ReqStatus);
                    }
                }
                else
                {
                    sw.WriteLine(protocol + " " + ReqStatus);
                    sw.WriteLine("Content-Type: text/plain");
                    sw.WriteLine();
                }
                sw.Flush();
                sw.Dispose();
                Server.ConsoleWriteLine("Response sent", false);
            }
            catch (Exception e)
            {
                Server.ConsoleWriteLine("Exception caught while attempting to handle client:", true);
                Server.ConsoleWriteLine(e.ToString(), true);
            }
            finally
            {
                connection.Disconnect(true);//socketStream.Close(); connection.Close();
                if (Server.Logging)
                {
                    string msg = reqTyp + " " + name; if (isLookUp) { msg += " " + location; }
                    string line = Host + " - - " + DateTime.Now.ToString("'['dd'/'MM'/'yyyy':'HH':'mm':'ss zz00']'") + "\"" + msg + "\" " + ReqStatus;
                    Server.RequestLogging = true;
                    Server.LogQueue.Enqueue(line);
                }
            }
        }
        private async Task<string> TimeOutCheck(Task<string> FirstLine)
        {
            Task outPut = null;
            if (Server.TimeOut > 0) outPut = await Task.WhenAny(FirstLine, Task.Delay(Server.TimeOut));
            else return await FirstLine;
            if (outPut == FirstLine)
            {
                return await FirstLine;
            }
            throw new TimeoutException();
        }
        private void RequestLinesReader(int pNumberOfLines)
        {
            requestLines = new string[pNumberOfLines];
            for (int i = 0; i < requestLines.Length; i++)
            {
                requestLines[i] = sr.ReadLine();
            }
            if (protocol != "HTTP/0.9")
            {
                string line = "";
                line = sr.ReadLine();
                while (line != "")
                {
                    line = sr.ReadLine();
                }
            }
            if (reqTyp == "POST")
            {
                int length;
                int.TryParse(requestLines[requestLines.Length - 1].Substring(15).Replace("\r\n", "").Trim(), out length);
                char[] buffer = new char[length];
                sr.ReadBlock(buffer, 0, length);
                if (pNumberOfLines < 2)
                {
                    location = String.Join("", buffer);
                }
                else
                {
                    string[] finalLineSections = String.Join("", buffer).Split(new char[] { '&' }, 2);
                    name = finalLineSections[0].Remove(0, 5);
                    location = finalLineSections[1].Remove(0, 9);
                }
            }
        }
    }
}
