using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TinyHttpService.RequestParser.Interface;

namespace TinyHttpService.RequestParser
{
    public class BodyParseCommandFactory
    {
        public static RequestBodyDataParseCommand GetBodyParseCommand(string contentType) 
        {
            contentType = contentType ?? string.Empty;
            contentType = contentType.ToLower();

            if (contentType.Contains("application/json")) 
            {
                return new JsonBodyDataParseCommand();
            }
            else if (contentType.Contains("multipart/form-data")) 
            {
                return new MultiPartFormDataParseCommand();
            }
            else if (contentType.Contains("application/x-www-form-urlencoded"))
            {
                return new UrlEncodedBodyDataParseCommand();
            }
            else 
            {
                return new NonBodyDataParseCommand();
            }
        }
    }
}
