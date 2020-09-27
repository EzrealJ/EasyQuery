using BenchmarkDotNet.Running;
using System;
using BenchmarkDotNet.Reports;

namespace Ezreal.EasyQuery.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Summary summary = BenchmarkRunner.Run<WhereConditionArgumentsInterpret_Test>();
            Console.ReadKey();
        }
    }
}
