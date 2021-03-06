using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(iep);
        }

        public void StartServer()
        {
            Console.Write("Finalll");
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                //Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {

            // TODO: Create client socket 
            Socket clientSock = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSock.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.

            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] data = new byte[1024 * 1024];
                    int receivedLen = clientSock.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0)
                    {
                        //Console.WriteLine("Client: {0} ended the connection", clientSock.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data, 0, receivedLen));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSock.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSock.Close();
        }

        Response HandleRequest(Request request)
        { 
            //throw new Exception();
            try
            {

               
                string content;
                StatusCode code;
                string extension = "text/html";
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    code = StatusCode.BadRequest;
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string PhysicalPath;
                //TODO: check for redirect
                string Request_URI = request.relativeURI.Substring(1); //Functions take page name without the "/"
                if (Configuration.RedirectionRules.ContainsKey(Request_URI))
                {
                    PhysicalPath = Configuration.RootPath + "/" + Configuration.RedirectionRules[Request_URI];
                    if (File.Exists(PhysicalPath))
                    {
                        content = LoadDefaultPage(GetRedirectionPagePathIFExist(Request_URI));
                        return new Response(StatusCode.Redirect, extension, content, Configuration.RedirectionRules[Request_URI]);
                    }
                    else
                    {
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                        code = StatusCode.NotFound;
                    }
                }
                //TODO: check file exists
                PhysicalPath = Configuration.RootPath + request.relativeURI;
                if (File.Exists(PhysicalPath))
                {
                    // Create OK response
                    content = LoadDefaultPage(Request_URI);
                    code = StatusCode.OK;
                }
                else
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    code = StatusCode.NotFound;
                }
                //TODO: read the physical file

                // Create OK response
                return new Response(code, extension , content, null);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                return new Response(StatusCode.InternalServerError, "text/html", Configuration.InternalErrorDefaultPageName, null);
            }
            
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string redirectionPagePath = Configuration.RedirectionRules[relativePath];
            return redirectionPagePath;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string

            if (!File.Exists(filePath))
            {
                Logger.LogException(new FileNotFoundException());

            }
            else
            {
                StreamReader sr = new StreamReader(filePath);
                string fileContent = sr.ReadToEnd();
                sr.Close();
                return fileContent;
            }
            // else read file and return its content

            return string.Empty;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                StreamReader sr = new StreamReader(filePath);
                // then fill Configuration.RedirectionRules dictionary 
                while (!sr.EndOfStream)
                {
                    string read = sr.ReadLine();
                    string[] redirect = read.Split(',');
                    Configuration.RedirectionRules = new Dictionary<string, string>();
                    Configuration.RedirectionRules.Add(redirect[0], redirect[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
            }
        }

    }
}
