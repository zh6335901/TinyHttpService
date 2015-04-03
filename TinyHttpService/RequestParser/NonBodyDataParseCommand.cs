﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyHttpService.HttpData;
using TinyHttpService.RequestParser.Interface;

namespace TinyHttpService.RequestParser
{
    public class NonBodyDataParseCommand : RequestBodyDataParseCommand
    {
        public override async Task<HttpRequestBody> ExecuteAsync(Stream stream, Encoding e)
        {
            var source = new TaskCompletionSource<HttpRequestBody>();
            source.SetResult(new HttpRequestBody());
            return await source.Task;
        }
    }
}
