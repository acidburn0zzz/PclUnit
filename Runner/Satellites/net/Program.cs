﻿// 
//  Copyright 2013 PclUnit Contributors
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using PclUnit.Runner;


namespace net_runner
{
    class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

     
        static void Main(string[] args)
        {
            var hidden = args.First();
            var id = args.Skip(1).First();
            var url = args.Skip(2).First();
            var sharedpath = args.Skip(3).First();
            var dlls = args.Skip(4);
           new Runner.Shared.RunTests().Run(id,url, dlls.ToArray());
        }



        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var baseUri = new Uri(args.RequestingAssembly.CodeBase); 
            var shortName = new AssemblyName(args.Name).Name;

            Console.WriteLine(args.RequestingAssembly.CodeBase);
            Console.WriteLine(shortName);
            string fullName = Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(baseUri.AbsolutePath)), string.Format("{0}.dll", shortName));
            Console.WriteLine(fullName);
            Assembly asm = null;
            try
            {

                asm = Assembly.LoadFile(fullName);
            }
            catch(Exception ex)
            {
                
                Console.WriteLine(ex);
            }
            return asm;
        }
    }
}