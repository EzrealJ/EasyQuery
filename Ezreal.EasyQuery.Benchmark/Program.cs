using BenchmarkDotNet.Running;
using System;

namespace Ezreal.EasyQuery.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<WhereConditionArgumentsInterpret_Test>();
            Console.ReadKey();
        }
    }
}
