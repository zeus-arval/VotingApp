using System;
using System.IO;
using System.Collections.Generic;

namespace VotingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> districts = new List<int>() { 2, 5, 7, 14 };
            List<int> years = new List<int>() { 2011, 2015, 2019 };
            VotesAnalyzer v = new VotesAnalyzer();
            foreach (int year in years)
            {
                v.FindElectoralThreshold(year);
                SleepAndCleanConsole();
                v.WriteOutExceededParties(year);
                SleepAndCleanConsole();
                v.PrintWinnerLoserInfo(year);
                SleepAndCleanConsole();
                v.CalculateLosersLostPercent(year);
                SleepAndCleanConsole();
                v.FindWinnersLosersDifference(year);
                SleepAndCleanConsole();
                v.WriteOutCalculations(year);
                SleepAndCleanConsole();
            }
            v.FindCoalitionOppositionPercent();
            SleepAndCleanConsole();
            v.FindQuotas();
            SleepAndCleanConsole();
            v.FindMandateList();
            SleepAndCleanConsole();
            v.WriteOutDistrictInfo();
            SleepAndCleanConsole();
            v.FindPersonalMandates();
            SleepAndCleanConsole();
            v.FindPersonalMandates(5);
            SleepAndCleanConsole();
            v.FindPersonalMandates(districts);
            SleepAndCleanConsole();
            v.FindDistrictMandates();
        }

        public static void SleepAndCleanConsole()
        {
            Console.WriteLine("\n\n\nPress Enter to go to the next method");
            ConsoleKeyInfo e;
            do
            {
                e = Console.ReadKey();
            } while (e.Key != ConsoleKey.Enter);
            Console.Clear();
        }
    }
}