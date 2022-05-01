using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301

    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            string headerLinesString = "";
            if (redirectoinPath != null)
            {
                headerLinesString = "Content-Type: " + contentType + "\r\n" +
                    "Content-Length: " + content.Length + "\r\n" +
                    "Date: " + DateTime.Now.ToString() + "\r\n" +
                    "location: " + redirectoinPath + "\r\n";

            }
            else
            {
                headerLinesString = "Content-Type: " + contentType + "\r\n" +
                      "Content-Length: " + content.Length + "\r\n" +
                      "Date: " + DateTime.Now.ToString() + "\r\n";

            }

            // TODO: Create the request string
            responseString = GetStatusLine(code) + "\r\n" + headerLinesString + "\r\n\r\n" + content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string errorMessage;
            switch (code)
            {
                case StatusCode.BadRequest:
                    errorMessage = "Bad Request";
                    break;
                case StatusCode.OK:
                    errorMessage = "OK";
                    break;
                case StatusCode.InternalServerError:
                    errorMessage = "Internal Server Error";
                    break;
                case StatusCode.Redirect:
                    errorMessage = "Redirect";
                    break;
                case StatusCode.NotFound:
                    errorMessage = "Not Found";
                    break;
                default:
                    errorMessage = "";
                    break;

            }
            string statusLine = Configuration.ServerHTTPVersion + " " + code + " " + errorMessage;
            return statusLine;
        }
    }
}