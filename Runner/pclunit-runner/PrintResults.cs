﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PclUnit.Runner;

namespace pclunit_runner
{
    public static class PrintResults
    {
        public static bool TeamCity;

        public static void PrintStart()
        {
            if (TeamCity)
            {
                Console.WriteLine("##teamcity[testSuiteStarted name='{0}']", "all");
            }
            else
            {
                Console.WriteLine("Starting Tests");
            }
        }

        public static void PrintEnd(ResultsFile file)
        {


            if (TeamCity)
            {
                Console.WriteLine("##teamcity[testSuiteStarted name='{0}']", "all");
            }
            else
            {

                PrintCount(PlatformResult.Errors, "Errors:");
                PrintCount(PlatformResult.Failures, "Failures:");
                PrintCount(PlatformResult.Ignores, "Ignores:");
                PrintCount(PlatformResult.NoErrors, "NoErrors:");
                PrintTotaledCount(PlatformResult.Success, "Success:");

                Console.WriteLine("Final Total");
                Console.WriteLine();
                var resultCount = file.ResultCount;
                foreach (var kp in resultCount.OrderBy(it => it.Key))
                {
                    Console.WriteLine("  {0,-15}{1,4}", kp.Key, kp.Value);
                }
                Console.WriteLine("{0,-17}{1,4}", "Total", resultCount.Select(r => r.Value).Sum());
            }
        }


        public static void PrintTotaledCount(IEnumerable<Result> results, string header)
        {

            lock (PlatformResult.ExpectedTests)
            {
                Console.WriteLine(header);
                var totalCount = PlatformResult.ExpectedTests.SelectMany(it => it.Value)
                                               .Select(it => it.Result)
                                               .ToLookup(it => it.Platform)
                                               .ToDictionary(k => k.Key, v => v.Count());
                foreach (var result in results.GroupBy(it => it.Platform))
                {
                    Console.Write("  {0,-15}{1,6}/{2}" ,
                        result.Key,
                        result.Count(), 
                        totalCount.ContainsKey(result.Key) 
                            ? totalCount[result.Key]
                            : 0
                        );
                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }

        public static void PrintCount(IEnumerable<Result> results, string header)
        {
            if (results.Any())
            {
                Console.WriteLine(header);
                foreach (var result in results.GroupBy(it => it.Platform))
                {
                    Console.Write("  {0,-15}{1,6}",
                         result.Key,
                         result.Count()
                         ); 
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }

        public static void PrintResult(IDictionary<string, PlatformResult> dict)
        {
            if (dict.All(it => it.Value.Result != null))
            {
                var result = dict.Select(it => it.Value.Result).First();
                if (TeamCity)
                {
                    Console.WriteLine("##teamcity[testStarted name='{2}.{1}.{0}' captureStandardOutput='true']",
                                      result.Test.Name, result.Test.Fixture.Name, result.Test.Fixture.Assembly.Name);
                }
                else
                {
                    Console.Write(result.Test.Fixture.Assembly.Name + ".");
                    Console.Write(result.Test.Fixture.Name + ".");
                }
                Console.WriteLine(result.Test.Name);
                foreach (var grpResult in dict.GroupBy(it => it.Value.Result.Kind))
                {
                    Console.Write("{0}:", grpResult.Key);
                    foreach (var keyValuePair in grpResult)
                    {
                        Console.Write(" ");
                        Console.Write(keyValuePair.Value.Platform);
                    }
                    if (TeamCity)
                    {
                        switch (grpResult.Key)
                        {
                            case ResultKind.Fail:
                            case ResultKind.Error:
                                Console.WriteLine(
                                    "##teamcity[testFailed name='{2}.{1}.{0}' message='See log or details']", result.Test.Name, result.Test.Fixture.Name, result.Test.Fixture.Assembly.Name);
                                break;
                            case ResultKind.Ignore:
                                Console.WriteLine(
                                    "##teamcity[testIgnored name='{2}.{1}.{0}' message='See log or details']", result.Test.Name, result.Test.Fixture.Name, result.Test.Fixture.Assembly.Name);
                                break;
                        }

                    }
                    Console.WriteLine();
                }
                var span = new TimeSpan();
                foreach (var r in dict.Select(it => it.Value.Result))
                {
                    span += (r.EndTime - r.StartTime);
                }
                Console.WriteLine("avg time:{0}", new TimeSpan(span.Ticks / dict.Count));


                foreach (var lup in dict.ToLookup(it => it.Value.Result.Output))
                {
                    var name = String.Join(",", lup.Select(it => it.Value.Platform));

                    Console.WriteLine("{0}:", name);
                    Console.WriteLine(lup.Key);
                }

                if (TeamCity)
                {
                    Console.WriteLine("##teamcity[testFinished name='{2}.{1}.{0}' duration='{3}']",
                        result.Test.Name,
                        result.Test.Fixture.Name,
                        result.Test.Fixture.Assembly.Name,
                        (result.EndTime - result.StartTime).TotalMilliseconds);
                }
                else
                {

                    if (dict.Any(it => it.Value.Result.Kind == ResultKind.Fail))
                    {
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!");
                    }
                    else if (dict.Any(it => it.Value.Result.Kind == ResultKind.Error))
                    {
                        Console.WriteLine("EEEEEEEEEEEEEEEEEEEEEEEEE");
                    }
                    else if (dict.Any(it => it.Value.Result.Kind == ResultKind.Ignore))
                    {
                        Console.WriteLine("?????????????????????????");
                    }
                    else if (dict.All(it => it.Value.Result.Kind == ResultKind.Success))
                    {
                        Console.WriteLine("-------------------------");
                    }
                    else
                    {
                        Console.WriteLine(".........................");
                    }

                    Console.WriteLine(String.Empty);
                }
            }
        }
    }
}