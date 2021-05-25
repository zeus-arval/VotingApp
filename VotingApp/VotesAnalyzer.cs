using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace VotingApp
{
    public class VotesAnalyzer
    {
        private Dictionary<int, int> _electoralThresholds = new Dictionary<int, int>(); // Dictionary for electoral thresholds
        public Dictionary<int, string> _pathNames = new Dictionary<int, string>(); // Dictionary for using ElectionResultsXXXX.txt path's name by year
        public Dictionary<int, List<Party>> _parties = new Dictionary<int, List<Party>>(); // Dictionary for using Party's info by year
        public Dictionary<int, int> _totalVotesSum = new Dictionary<int, int>(); // Dictionary for using total sums of votes by year
        public Dictionary<int, int> _districtVotesSum = new Dictionary<int, int>(); // Dictionary for using sums of votes in district by number of district
        public Dictionary<int, List<Party>> _detailedParties = new Dictionary<int, List<Party>>(); //Dictionary for using Party's info by number of district 
        public Dictionary<int, District> _districtName = new Dictionary<int, District>(); // Dictionary for using district's names by number of district
        public Dictionary<int, List<Candidate>> _candidateDict = new Dictionary<int, List<Candidate>>(); // Dictionary for using candidate's info by candidate's district
        public VotesAnalyzer()
        {
            FillPathDictionary(); // Filling pathNames dictionary
        }

        public void FillCandidateDictionary() // (Test) one object from dictionary
        {   //Method , which creates list in dictionary(_candidateDict) with Candidate objects. Used file PersonalVotes.csv
            if (_candidateDict.Count == 0)
            {
                using (StreamReader reader = new StreamReader(@"C:\Users\37255\source\repos\VotingApp\VotingApp\bin\Debug\netcoreapp3.1\PersonalVotes.csv"))
                {
                    List<string> strList = new List<string>();
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line != "Valimisringkonna nr,Erakond,Kandidaat,Häälte arv") strList.Add(line);
                    }
                    for (int i = 0; i < 12; i++)
                    {
                        List<Candidate> list = new List<Candidate>();
                        foreach (string element in strList)
                        {
                            int distNum = int.Parse(element.Split(',').First());
                            if (distNum == i + 1) list.Add(new Candidate(element));
                        }
                        _candidateDict.Add(i + 1, list);
                    }
                }
            }
        }

        public void FillDetailedPartiesDictionary()
        {   // method for reading every line in file, adding new objects Party with party name and votes and districts to list and returing it as list
            // method can be used only for DetailedResults_2019.txt
            if (_districtName.Count == 0) FillDictionaryDistrictName();
            using (StreamReader reader = new StreamReader(@"C:\Users\37255\source\repos\VotingApp\VotingApp\bin\Debug\netcoreapp3.1\DetailedResults_2019.txt"))
            {
                List<string> strList = new List<string>();
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line != "Valimisringkond,Erakond,Häälte arv") strList.Add(line);
                }
                for (int i = 0; i < _districtName.Count; i++)
                {
                    List<Party> list = new List<Party>();
                    foreach (string element in strList)
                    {
                        int distNum = int.Parse(element.Split(',').First());
                        if (distNum == i + 1) list.Add(new Party(element, _districtName[i + 1]));
                    }
                    _detailedParties.Add(i + 1, list);
                }
            }
        }

        public void FillDictionaryDistrictName()
        {   // Method for reading every line in file, adding new objects District with number and name of district and amount of mandates to dict _districtName
            // Method uses file ElectionRegions.txt
            if (_districtName.Count == 0)
            {
                using (StreamReader reader = new StreamReader(@"C:\Users\37255\source\repos\VotingApp\VotingApp\bin\Debug\netcoreapp3.1\ElectionRegions.txt"))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        District district = new District(line);
                        _districtName.Add(district._districtNum, district);
                    }
                }
            }
        }

        private void FillPathDictionary()
        {   // Method for filling dictionary to facilitate getting pathName
            _pathNames.Add(2019, @"C:\Users\37255\source\repos\VotingApp\VotingApp\bin\Debug\netcoreapp3.1\ElectionResults2019.txt");
            _pathNames.Add(2015, @"C:\Users\37255\source\repos\VotingApp\VotingApp\bin\Debug\netcoreapp3.1\ElectionResults2015.txt");
            _pathNames.Add(2011, @"C:\Users\37255\source\repos\VotingApp\VotingApp\bin\Debug\netcoreapp3.1\ElectionResults2011.txt");
        }

        public List<Party> OverwriteFileToList(int year)
        {   // method for reading every line in file, adding new objects Party with party name and votes to list and returning list
            // method can be used only for ElectionResultsXXXX.txt
            List<Party> list = new List<Party>();
            using (StreamReader reader = new StreamReader(_pathNames[year]))
            {
                while (!reader.EndOfStream)
                {
                    list.Add(new Party(reader.ReadLine()));
                }
            }
            return list;
        }

        public int FindElectoralThreshold(int year) // Method 1 (Test)
        {   // Method for returning an amount of votes, which equals to 5%
            int electoralThreshold = 0;
            if (!_parties.ContainsKey(year)) _parties[year] = OverwriteFileToList(year); // List with Parties(Party include in votes and name)
            if (!_electoralThresholds.ContainsKey(year))
            {
                FindVoteSum(year);
                electoralThreshold = (int)(_totalVotesSum[year] * 0.05);
                _electoralThresholds.Add(year, electoralThreshold);
            }
            return electoralThreshold;
        }

        public void FindVoteSum(int year)
        {   //Method for finding sum of votes for all parties
            if (!_totalVotesSum.ContainsKey(year))
            {
                int sum = 0;
                foreach (Party party in _parties[year])
                {
                    sum += party._votes;
                }
                _totalVotesSum.Add(year, sum);
            }
        }

        public void WriteOutExceededParties(int year) // Method 2
        {   //Method for printing out parties, which exceeded electoral threshold
            FindExceededParties(year);
            Console.Write($"In {year} electoral threshold(5%) was ");
            MakeTextColored(_electoralThresholds[year].ToString(), 0);
            Console.Write("and exceeded parties were:\n");
            for (int i = 0; i < _parties[year].Count; i++)
            {
                if (_parties[year][i]._isWinner)
                {
                    MakeTextColored($"\t\t{_parties[year][i]._name}", 1);
                    Console.Write(" with ");
                    MakeTextColored($"{_parties[year][i]._votes}", 0);
                    Console.Write(" votes\n");
                }
            }
        }

        public void FindExceededParties(int year)
        {   // Method for defining, which parties exceeded electoral threshold (5%) 
            if (!_parties.ContainsKey(year)) _parties[year] = OverwriteFileToList(year); // if dictionary _parties does not have a key = year, create and add a List by key
            if (!_electoralThresholds.ContainsKey(year)) FindElectoralThreshold(year); // if dictionary _electoralThresholds does not contain a key == year, finding and adding it
            for (int i = 0; i < _parties[year].Count; i++)
            {
                if (_parties[year][i]._votes > _electoralThresholds[year]) _parties[year][i]._isWinner = true;
                else continue;
            }
        }

        public void PrintWinnerLoserInfo(int year) // Method 3
        {   //Method for printing out winners and losers
            if (!_parties.ContainsKey(year)) _parties[year] = OverwriteFileToList(year); // if dictionary _parties does not have a key = year, create and add a List by 
            Console.Write("In year ");
            MakeTextColored(year.ToString(), 0);
            Console.WriteLine("\nWinner party was:");
            PrintWinnerLoser(true, year);
            Console.WriteLine("\nLoser party was:");
            PrintWinnerLoser(false, year);
        }

        private void PrintWinnerLoser(bool isWinner, int year)
        {   //method for printing out information about winner and loser party 
            string partyName = _parties[year][0]._name;
            int votes = _parties[year][0]._votes;
            DefineWinnerLoser(partyName, votes, year, isWinner);
            MakeTextColored($"\t{partyName}", 1);
            Console.Write($" with ");
            MakeTextColored(votes.ToString(), 0);
            Console.Write($" votes\n");
        }

        private void DefineWinnerLoser(string partyName, int votes, int year, bool isWinner)
        {   //Method for defining min and max amount of votes and loser's and winner's names
            for (int i = 1; i < _parties[year].Count; i++)
            {
                if (isWinner)
                {
                    votes = Math.Max(_parties[year][i]._votes, votes);
                    partyName = DefinePartyLoserWinnerName(i, votes, year, partyName);
                }
                else
                {
                    votes = Math.Min(_parties[year][i]._votes, votes);
                    partyName = DefinePartyLoserWinnerName(i, votes, year, partyName);
                }
            }
        }

        private string DefinePartyLoserWinnerName(int i, int votes, int year, string partyName)
        {   //Method for returning one of two party's name
            if (votes == _parties[year][i]._votes) return _parties[year][i]._name;
            else return partyName;
        }

        public double CalculateLosersLostPercent(int year) // Method 4 (Test)
        {
            double lostPercent = 0;
            if (!_parties.ContainsKey(year)) _parties[year] = OverwriteFileToList(year); // if dictionary _parties does not have a key = year, create and add a List by key
            FindVoteSum(year);
            if (!_electoralThresholds.ContainsKey(year)) FindElectoralThreshold(year); // if dictionary _electoralThresholds does not contain a key == year, finding and adding 
            FindExceededParties(year);
            lostPercent = (double)FindLoserWinnerTotalSum(year, false) / (double)_totalVotesSum[year] * 100;
            return lostPercent;
        }

        public void PrintLosersLostPercent(int year)
        {
            double lostPercent = CalculateLosersLostPercent(year);
            Console.WriteLine($"{Math.Round(lostPercent, 2)}% were lost by loser parties");
        }

        private int FindLoserWinnerTotalSum(int year, bool isWinner) // (Test)
        {   // Method for finding total sum of winners/losers parties's votes
            int sum = 0;
            if (!_electoralThresholds.ContainsKey(year)) FindElectoralThreshold(year); // if dictionary _electoralThresholds does not contain a key == year, finding and adding it
            foreach (Party party in _parties[year])
            {
                if (isWinner == party._isWinner) sum += party._votes;
            }
            return sum;
        }

        public int FindWinnersLosersDifference(int year) // Method 5 (Test) 
        {   //Method for finding loasers and Winners difference in votes 
            int difference;
            double lostPercent;
            if (!_parties.ContainsKey(year)) _parties[year] = OverwriteFileToList(year); // if dictionary _parties does not have a key = year, create and add a List by key
            if (!_electoralThresholds.ContainsKey(year)) FindElectoralThreshold(year); // if dictionary _electoralThresholds does not contain a key == year, finding and adding it
            FindExceededParties(year);
            lostPercent = CalculateLosersLostPercent(year);
            difference = FindLoserWinnerTotalSum(year, true) - FindLoserWinnerTotalSum(year, false);
            Console.WriteLine($"Difference between winners and losers is {difference} votes");
            return difference;
        }

        public void WriteOutCalculations(int year) //Method 6
        {   //Method for printing out calculations from methods 1-3
            if (!_pathNames.ContainsKey(year))
            {
                CheckYear(year);
            }
            else
            {
                PrintWinnerLoserInfo(year);
            }
        }

        private void CheckYear(int year)
        {   //Method for printing what is wrong with year
            if (year % 4 == 3 && year > 2022) Console.WriteLine($"{year} year has not came yet");
            else if (year % 4 == 3 && year < 2008 && year > 1991) Console.WriteLine($"No election file with votes for {year} year");
            else
            {
                if (year >= 2020) Console.WriteLine($"In {year} election will not be organised");
                else if (year > 1992) Console.WriteLine($"In {year} election was not organised");
                else Console.WriteLine($"{year} is invalid format for year");
            }
        }

        public void FindCoalitionOppositionPercent() // Method 7 
        {   //Method (based on 2019 election results), which calculates percent of exceeded electoral threshold coalition(parties, which contain people , who are ministers) 
            //parties and of exceeded electoral threshold opposition(parties, which does not contain people, who work for government) parties.
            FindExceededParties(2019);
            double coalitionPercent = CalculateCoalitionOppositionPercents(true);
            double oppositionPercent = CalculateCoalitionOppositionPercents(false);
            Console.WriteLine($"Percent of exceeded coalition parties is {coalitionPercent}% , and of exceeded opposition parties is {oppositionPercent}%");
        }

        public double CalculateCoalitionOppositionPercents(bool isCoalition) // (Test)
        {   //Method for calculationg opposition and coalition parties's percent of total sum of votes
            List<string> coalitionList = new List<string>() { "Eesti Keskerakond", "Isamaa Erakond", "Eesti Konservatiivne Rahvaerakond" };
            int votes = 0;
            double percent;
            FindVoteSum(2019);
            foreach (Party party in _parties[2019])
            {
                if (party._isWinner && (isCoalition == coalitionList.Contains(party._name))) votes += party._votes; // Getting sum of coalition parties's votes
            }
            percent = Math.Round(((double)votes / (double)_totalVotesSum[2019] * 100), 2);
            return percent;
        }

        public List<double> FindQuotas() // Method 8 (Test)
        {
            if (_districtName.Count == 0) FillDictionaryDistrictName();
            if (_detailedParties.Count == 0) FillDetailedPartiesDictionary();
            List<double> quotaList = new List<double>();
            FillDictionaryWithSums();
            for (int i = 1; i <= _districtName.Count; i++)
            {
                _districtName[i]._quota = Math.Round((double)_districtVotesSum[i] / (double)_districtName[i]._mandates, 2);
                quotaList.Add(_districtName[i]._quota);
            }
            return quotaList;
        }

        public Dictionary<int, int> FillDictionaryWithSums() // (Test)
        {
            if (_districtVotesSum.Count == 0)
            {
                for (int disNum = 1; disNum <= _detailedParties.Count; disNum++)
                {
                    int sum = 0;
                    for (int party = 0; party < _detailedParties[disNum].Count; party++)
                    {
                        sum += _detailedParties[disNum][party]._votes;
                    }
                    _districtVotesSum.Add(disNum, sum);
                    Console.WriteLine(sum);
                }
            }
            return _districtVotesSum;
        }

        public List<int> FindMandateList() // Method 9
        {
            List<int> mandateList = new List<int>();
            FindQuotas();
            for (int i = 1; i <= _districtName.Count; i++)
            {
                int sum = 0;
                foreach (Party party in _detailedParties[i])
                {
                    int mandate = (int)(party._votes / _districtName[i]._quota);
                    double plus = (double)(party._votes / _districtName[i]._quota) - mandate;
                    party._mandates = (plus >= 0.75) ? mandate += 1 : mandate;
                    sum += party._mandates;
                }
                mandateList.Add(sum);
            }
            return mandateList;
        }

        public void WriteOutDistrictInfo() // Method 10
        {   // Method for printing out all information about parties in districts
            FindMandateList();
            for (int i = 1; i <= _districtName.Count; i++)
            {
                Console.Write($"In ");
                MakeTextColored(_districtName[i]._districtName, 1);
                Console.Write($" quota is ");
                MakeTextColored(_districtName[i]._quota.ToString(), 0);
                Console.Write(". Parties:\n");
                foreach (Party party in _detailedParties[i])
                {
                    Console.Write($"{party._votes}");
                    MakeTextColored($"\t{party._name}", 1);
                    Console.Write($" got ");
                    MakeTextColored(party._mandates.ToString(), 0);
                    string text = (party._mandates == 0 || party._mandates > 1) ? "mandates" : "mandate";
                    Console.Write($" {text}\n");
                }
                Console.WriteLine();
            }
        }

        public void FindPersonalMandates() // Method 11 (advanced)
        {   //Method for finding personal mandates in all districts by comparing votes for person with simple quota
            FindQuotas();
            FillCandidateDictionary();
            for (int i = 1; i <= _candidateDict.Count; i++)
            {
                FindPersonalMandates(i);
            }
        }

        private void PrintOutMandatesFromDistrict(int i)
        {
            Console.Write($"In District ");
            MakeTextColored($"{_districtName[i]._districtName}", 1);
            Console.Write(" mandates are:\n\n");
            foreach (Candidate candidate in _candidateDict[i])
            {
                if (candidate._isMandate)
                {
                    MakeTextColored($"{candidate._candidateName}", 0);
                    Console.Write($" from {candidate._partyName} with ");
                    MakeTextColored($"{candidate._votes}", 0);
                    Console.Write(" votes\n\n");
                }
            }
            Console.WriteLine("\n");
        }

        public void FindPersonalMandates(int districtNum) // Method 11 (advanced)
        {   //Method for finding personal mandates in certain district by comparing votes for person with simple quota
            if (districtNum > 0 && districtNum < 13)
            {
                if (_districtName.Count == 0) FindQuotas();
                if (_candidateDict.Count == 0) FillCandidateDictionary();
                int count = MakeCandidateMandate(districtNum);
                if (count > 0) PrintOutMandatesFromDistrict(districtNum);
            }
            else MakeTextColored($"{districtNum} is invalid number of district", 2);
        }

        private int MakeCandidateMandate(int districtNum)
        {
            int count = 0;
            foreach (Candidate candidate in _candidateDict[districtNum])
            {
                if (candidate._votes >= _districtName[districtNum]._quota)
                {
                    count++;
                    candidate._isMandate = true;
                }
            }
            return count;
        }

        public void FindPersonalMandates(List<int> districtNums) // Method 11 (advanced)
        {   //Method for finding personal mandates in list of districts by comparing votes for person with simple quota
            FindQuotas();
            FillCandidateDictionary();
            foreach(int districtNum in districtNums)
            {
                FindPersonalMandates(districtNum);
            }
        }

        public Dictionary<int, int> FindDistrictMandates() // Method 12 (advanced)
        {   //Method for finding district mandates and for subtracting amount of personal mandates from this district
            FindMandateList(); // filling _detailedParties dictionary, where key is district number and value is list of parties(which contains number of mandates)
            if (_candidateDict.Count == 0) FillCandidateDictionary();// filling _candidateDict dictionary, where key is dicstrict number and value is list of all candidates(which is mandate or not) from district
            Dictionary<int, int> districtMandates = FillMandatesDict();
            for (int i = 1; i < districtMandates.Count; i++)
            {
                if (districtMandates[i] != 0) 
                {
                    MakeTextColored(districtMandates[i].ToString(), 1);
                    Console.Write($" mandates are in district ");
                    MakeTextColored($"{_districtName[i]._districtName}\n", 1);
                }
            }
            return districtMandates;
        }

        private Dictionary<int, int> FillMandatesDict()
        {   //Method for finding total amount of district mandates without personal mandates
            Dictionary<int, int> districtMandates = new Dictionary<int, int>(); // key is district, value is amount of mandates
            Dictionary<int, int> personalMandateDict = FillPersonalMandateDict(); // key is district, value is amount of personal mandates from all parties in that district
            for(int districtNr = 1; districtNr <= personalMandateDict.Count; districtNr++)
            {
                int mandates = 0;
                foreach(Party party in _detailedParties[districtNr])
                {
                    mandates += party._mandates;
                }
                mandates -= personalMandateDict[districtNr];
                districtMandates.Add(districtNr, mandates);
            }
            return districtMandates;
        }

        public Dictionary<int, int> FillPersonalMandateDict() // (Test)
        {   //Method for filling dictionary with amounts of personal mandates for every district(key = district, value = amount of mandates)
            Dictionary<int, int> personalMandateDict = new Dictionary<int, int>();
            for(int i = 1; i <= _candidateDict.Count; i++)
            {
                int sum = MakeCandidateMandate(i);
                personalMandateDict.Add(i, sum);
            }
            return personalMandateDict;
        }


        private void MakeTextColored(string text, int designNum)
        {
            if (designNum == 0)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else if (designNum == 1) Console.ForegroundColor = ConsoleColor.Blue;
            else if (designNum == 2) Console.BackgroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}