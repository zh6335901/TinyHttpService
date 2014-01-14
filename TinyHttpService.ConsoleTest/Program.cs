using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.Core.Interface;
using TinyHttpService.Implement;
using TinyHttpService.RequestParser;


namespace TinyHttpService.ConsoleTest
{
    class Program : TinyHttpService.Core.TinyHttpServiceBase
    {
        public Program(IHttpServiceHandler handler)
            : base(handler)
        {
            
        }

        static void Main(string[] args)
        {
            Program program = new Program(new TinyHttpServiceHandler(new HttpRequestParser()));
            program.Bind(3000);
            Console.ReadLine();
        }
    }
}
