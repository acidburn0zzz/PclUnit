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
using PclUnit.Util;
namespace PclUnit.Runner
{
    public class TestMeta : IMeta
    {
        public TestMeta()
        {
            Category = new List<string>();
        }

        public FixtureMeta Fixture { get; set; }

 
        public string UniqueName { get; protected set; }
        public string Name { get; protected set; }


        public string ToListJson()
        {
              return String.Format("{{Name:\"{0}\", UniqueName:\"{1}\", Description:\"{2}\", Category:{3}}}",
                                 Name, UniqueName, Description, Category.ToListJson());
        }

        public string ToItemJson()
        {
            return String.Format("{{Fixture:{4}, Description:\"{2}\", Category:{3}, UniqueName:\"{1}\", Name:\"{0}\"}}",
                                 Name, UniqueName, Description, Category.ToListJson(), Fixture.ToItemJson());

        }


        public string Description { get; set; }
        public IList<string> Category { get; set; }
    }

    internal class DummyHelper : IAssertionHelper
    {
        public ILog Log { get; set; }
        public IAssert Assert { get; set; }
    }
}