using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyHttpService.HttpData
{
    public static class Mime
    {
        private static Dictionary<string, string> mimes = new Dictionary<string, string>();

        static Mime() 
        {
            mimes[".jpg"] = "image/jpeg";
            mimes[".png"] = "image/png";
            mimes[".ico"] = "image/ico";
            mimes[".txt"] = "text/plain";
            mimes[".css"] = "text/css";
            mimes[".html"] = "text/html";
            mimes[".htm"] = "text/html";
            mimes[".js"] = "application/javascript";
            mimes[".mp3"] = "audio/mpeg";
            mimes[".mp4"] = "video/mp4";
            mimes[".zip"] = "application/zip";
            mimes[".avi"] = "video/x-msvideo";
            mimes[".bmp"] = "image/bmp";
            mimes[".jpeg"] = "image/jpeg";
            mimes[".wma"] = "audio/x-ms-wma";
            mimes[".wmv"] = "video/x-ms-wmv";
        }

        public static string Get(string ext) 
        {
            if (mimes.ContainsKey(ext)) 
            {
                return mimes[ext];
            }
            return null;         
        }
    }
}
