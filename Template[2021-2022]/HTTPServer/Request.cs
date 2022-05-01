using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] HTTP_Request;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;

        public Request(string requestString)
        {
            headerLines = new Dictionary<string, string>();
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            bool _isValid = true;
            try
            {
                //TODO: parse the receivedRequest using the \r\n delimeter   
                string Parse="\r\n";
                 HTTP_Request = requestString.Split(new[] { Parse }, StringSplitOptions.None);
                Parse = "\r\n\r\n";
                string[] Request_Header = requestString.Split(new[] { Parse }, StringSplitOptions.None);
                // Parse Request line
                Parse = " ";
                string[] requestLines = HTTP_Request[0].Split(new[] { Parse }, StringSplitOptions.None);
                relativeURI = requestLines[1];
                httpVersion = HTTPVersion.HTTP11;
                method = RequestMethod.GET;
                // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
                if (HTTP_Request.Length < 3)
                {
                    _isValid = false;
                }
                // Parse Request line
                // Validate blank line exists
                if (HTTP_Request[HTTP_Request.Length - 2] == "")
                    _isValid = true;
                else _isValid = false;
                // Load header lines into HeaderLines dictionary
                for (int i = 1; i < HTTP_Request.Length - 2; i++)
                {
                    string[] lineWords = HTTP_Request[i].Split(':');
                    headerLines.Add(lineWords[0], lineWords[1]);
                }
                return _isValid;
            }
            
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
          
        }
       

    }
}
