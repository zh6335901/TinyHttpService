using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.HttpData
{
    public class FilePart
    {
        public string Filename { get; set; }

        public string Name { get; set; }

        public MemoryStream Data { get; set; }   
    }
}
