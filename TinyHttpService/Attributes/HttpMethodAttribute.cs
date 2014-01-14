using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Attributes
{
    public enum HttpAction
    { 
        Get,
        Post,
        Put,
        Delete
    }

    public class HttpMethodAttribute : Attribute
    {
        public HttpAction HttpAction { get; set; }

        public HttpMethodAttribute(HttpAction httpAction)
        {
            this.HttpAction = httpAction;
        }
    }
}
