using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.Router
{
    public class RouteData
    {
        public Dictionary<string, string> DataTokens { get; private set; }

        public string RouteUri { get; set; }

        public RouteData() 
        {
            this.DataTokens = new Dictionary<string, string>();
        }

        public string this[string key] 
        {
            get 
            {
                if (DataTokens.ContainsKey(key))
                {
                    return DataTokens[key];
                }

                return null;
            }
            set 
            {
                DataTokens[key] = value;
            }
        }
    }
}
