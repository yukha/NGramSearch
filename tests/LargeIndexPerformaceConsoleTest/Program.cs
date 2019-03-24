using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;

namespace LargeIndexPerformaceConsoleTest
{
    internal class Program
    {
        [Option(Description = "count of indexced item", LongName = "count", ShortName = "n")]
        public int IndexedItemCount { get; set; }


        public static int Main(string[] args)
        {
            return CommandLineApplication.Execute<Program>(args);
        }

        private void OnExecute()
        {
            IList<Actor> data = DataReader.ReadFromFile("data/actors.json");

            IntersectionCountTest test = new IntersectionCountTest(3);
            DateTime start = DateTime.Now;
            test.CreateIndex(data);
            test.RunSearch();

            Console.WriteLine($"ent of test {DateTime.Now - start}");
        }
    }
}
