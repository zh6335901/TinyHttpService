using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.HttpData
{
    public class HttpHeader
    {
        public Dictionary<string, string> Properties { get; set; }

        public string this[string key]
        {
            get 
            {
                if (this.Properties.ContainsKey(key))
                {
                    return this.Properties[key];
                }

                return null;
            }
            set 
            {
                this.Properties[key] = value;
            }
        }

        public HttpHeader()
        {
            this.Properties = new Dictionary<string,string>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var property in Properties)
            {
                sb.AppendFormat(@"{0}:{1}{2}", property.Key, property.Value, Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
