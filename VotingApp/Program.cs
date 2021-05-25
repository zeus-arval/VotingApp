using System;
using System.IO;
using System.Collections.Generic;

namespace VotingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            VotesAnalyzer v = new VotesAnalyzer();
            //v.WriteOutExceededParties(2011);
            List<int> districts = new List<int>() {2,5,7,14};
            //v.FindPersonalMandates();
            //SleepAndCleanConsole();
            //v.FindPersonalMandates();
            v.FindQuotas();
        }
        public static void SleepAndCleanConsole()
        {
            ConsoleKeyInfo e;
            do
            {
                e = Console.ReadKey();
            } while (e.Key != ConsoleKey.Enter);
            Console.Clear();
        }
    }
}