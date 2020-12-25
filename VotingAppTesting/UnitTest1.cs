using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using VotingApp;

namespace VotingAppTesting
{
    [TestClass]
    public class UnitTest1
    {
        VotesAnalyzer vote = new VotesAnalyzer();
        List<int> years = new List<int>() { 2011, 2015, 2019 };

        [TestMethod]
        public void FindElectoralThresholdTest1()
        {
            List<int> expectedElectoralThresholdList = new List<int>() { 28756, 28707, 28057 };
            List<int> aclutalElectoralThresholdList = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                aclutalElectoralThresholdList.Add(vote.FindElectoralThreshold(years[i]));
            }
            CollectionAssert.AreEqual(expectedElectoralThresholdList, aclutalElectoralThresholdList);
        }

        [TestMethod]
        public void DiclosingAbbreviationTesting() // Testing if abbreviation is diclosed in full party's name
        {
            List<string> parties = new List<string>()
            {
                "Eesti Reformierakond",
                "Eesti Keskerakond",
                "Eesti Konservatiivne Rahvaerakond",
                "Isamaa Erakond",
                "Sotsiaaldemokraatlik Erakond",
                "Eesti 200",
                "Erakond Eestimaa Rohelised",
                "Elurikkuse Erakond",
                "Eesti Vabaerakond",
                "Üksikkandidaadid",
                "Eestimaa Ühendatud Vasakpartei"
            };
            vote._parties[2019] = vote.OverwriteFileToList(2019);
            for(int i = 0; i < parties.Count; i++)
            {
                Assert.AreEqual(vote._parties[2019][i]._name, parties[i]);
            }
        }

        [TestMethod]
        public void CalculateLosersLostPercentTesting() // Testing percent of loser parties in 2019
        {
            double lostPercent = vote.CalculateLosersLostPercent(2019);
            double expectedPercent = (double)50095 / (double)561141 * 100;
            Assert.IsTrue(lostPercent == expectedPercent);
        }

        [TestMethod]
        public void CalculateLosersLostPercentTesting2() // Testing percent of loser parties in 2015
        {
            double lostPercent = vote.CalculateLosersLostPercent(2015);
            double expectedPercent = (double)10180 / (double)574153 * 100;
            Assert.IsTrue(lostPercent == expectedPercent);
        }

        [TestMethod]
        public void CalculateLosersLostPercentTesting3() // Testing percent of loser parties in 2015
        {
            double lostPercent = vote.CalculateLosersLostPercent(2011);
            double expectedPercent = (double)60424 / (double)575133 * 100;
            Assert.IsTrue(lostPercent == expectedPercent);
        }

        [TestMethod]
        public void FindVoteSumTesting() // testing vote sum
        {
            Dictionary<int, int> sumsDict = new Dictionary<int, int>()
            {
                { 2011, 575133 },
                { 2015, 574153 },
                { 2019, 561141 },
            };
            foreach (int year in years)
            {
                vote._parties[year] = vote.OverwriteFileToList(year);
                vote.FindVoteSum(year);
            }
            CollectionAssert.AreEquivalent(vote._totalVotesSum, sumsDict);
        }

        [TestMethod]
        public void FindWinnersLosersDifferenceTesting1() // testing differences between winners and losers
        {
            int expectedDifference = 454285;
            int actualDifference = vote.FindWinnersLosersDifference(years[0]);
            CollectionAssert.Equals(expectedDifference, actualDifference);
        }

        [TestMethod]
        public void FindWinnersLosersDifferenceTesting2() // testing differences between winners and losers
        {
            int expectedDifference = 553793;
            int actualDifference = vote.FindWinnersLosersDifference(years[1]);
            CollectionAssert.Equals(expectedDifference, actualDifference);
        }

        [TestMethod]
        public void FindWinnersLosersDifferenceTesting3() // testing differences between winners and losers
        {
            int expectedDifference = 460951;
            int actualDifference = vote.FindWinnersLosersDifference(years[2]);
            CollectionAssert.Equals(expectedDifference, actualDifference);
      }

        [TestMethod]
        public void CalculateCoalitionOppositionPercentsTesting() // method for testing coalition and opposition percents
        {
            vote._parties[2019] = vote.OverwriteFileToList(2019);
            vote.FindVoteSum(2019);
            List<double> expectedDict = new List<double>() { 52.31, 38.77};
            List<double> coalitionOppositionDictionary = new List<double>();
            vote.FindExceededParties(years[2]);
            double coalitionPercent = vote.CalculateCoalitionOppositionPercents(true);
            double oppositionPercent = vote.CalculateCoalitionOppositionPercents(false);
            coalitionOppositionDictionary.Add(coalitionPercent);
            coalitionOppositionDictionary.Add(oppositionPercent);
            CollectionAssert.AreEquivalent(expectedDict, coalitionOppositionDictionary);
        }

        [TestMethod]
        public void FindQuotasTesting() // Method for testing list of quotas
        {
            List<double> expectedQuotaList = new List<double>() 
            { 
                5619.1, 5381.85, 6235.25, 6048.6, 5332.83, 5386.8, 3859, 5349.71, 5978.14, 5603.88, 5386.25, 5887.86
            };
            List<double> quotaList = vote.FindQuotas();
            CollectionAssert.AreEqual(expectedQuotaList, quotaList);
        }

        [TestMethod]
        public void FillDictionaryWithSumsTesting() // Method for testing dictionary with districts's votes's sums
        {
            Dictionary<int, int> expectedDistrictVotesSum = new Dictionary<int, int>() 
            {
                {1, 56191}, { 2, 69964}, { 3, 49882}, {4, 90729 }, {5, 31997 }, {6, 26934 },
                {7, 27013 }, {8, 37448 }, {9, 41847 }, {10, 44831 }, {11, 43090 }, {12, 41215 }
            };
            vote.FillDetailedPartiesDictionary();
            vote.FillDictionaryWithSums();
            CollectionAssert.AreEquivalent(expectedDistrictVotesSum, vote._districtVotesSum);
        }

        [TestMethod]
        public void FindMandateListTesting() // Method for testing mandate list for every district
        {
            List<int> expectedList = new List<int>()
            {
                9, 11, 6, 12, 3, 4, 5, 6, 5, 7, 7, 6
            };
            List<int> mandateList = vote.FindMandateList();
            CollectionAssert.Equals(expectedList, mandateList);
        }

        [TestMethod]
        public void FillCandidateDictionaryTesting() // Method for testing candidates's votes
        {
            vote.FillCandidateDictionary();
            Assert.IsTrue(vote._candidateDict[2][1]._votes == 77);
            Assert.IsTrue(vote._candidateDict[5][0]._votes == 366);
            Assert.IsTrue(vote._candidateDict[10][2]._votes == 106);
        }

        [TestMethod]
        public void FillCandidateDictionaryTesting2() // Method for testing candidate's party
        {
            vote.FillCandidateDictionary();
            StringAssert.Equals(vote._candidateDict[1][1]._candidateName, "ÕIE-MARI AASMÄE");
            StringAssert.Equals(vote._candidateDict[4][2]._candidateName, "TIIU KUURME");
            StringAssert.Equals(vote._candidateDict[7][3]._candidateName, "INNA ROSE");
        }

        [TestMethod]
        public void FillCandidateDictionaryTesting3() // Method for testing candidate's party
        {
            vote.FillCandidateDictionary();
            Assert.IsTrue(vote._candidateDict[1][5]._partyName == "Eesti Vabaerakond");
            Assert.IsTrue(vote._candidateDict[1][55]._partyName == "Eesti Konservatiivne Rahvaerakond");
            Assert.IsTrue(vote._candidateDict[1][100]._partyName == "Isamaa Erakond");
        }

        [TestMethod]
        public void DistrictNameTesting() // Method for testing District's name
        {
            List<string> districtName = new List<string>() 
            {
                "Tallinna Haabersti, Põhja-Tallinna ja Kristiine linnaosa", "Tallinna Kesklinna, Lasnamäe ja Pirita linnaosa",
                "Tallinna Mustamäe ja Nõmme linnaosa", "Harju- ja Raplamaa", "Hiiu-, Lääne- ja Saaremaa",
                "Lääne-Virumaa", "Ida-Virumaa", "Järva- ja Viljandimaa", "Jõgeva- ja Tartumaa", "Tartu linn",
                "Võru-, Valga- ja Põlvamaa", "Pärnumaa"
            };
            vote.FillDictionaryDistrictName();
            for(int i = 1; i < vote._districtVotesSum.Count; i++)
            {
                StringAssert.Equals(vote._districtName[i]._districtName, districtName[i - 1]);
            }
        }

        [TestMethod]
        public void DistrictMandatesTesting()
        {
            List<int> mandateCount = new List<int>() 
            { 
                10, 13, 8, 15, 6, 5, 7, 7, 7, 8, 8, 7
            };
            vote.FillDictionaryDistrictName();
            for(int i = 1; i <= 12; i++)
            {
                Assert.AreEqual(mandateCount[i - 1], vote._districtName[i]._mandates);
            }
        }
        
        [TestMethod]
        public void FindDistrictMandatesTesting() // Method for testing finding district mandates 
        {
            Dictionary<int, int> expectedDistrictMandates = new Dictionary<int, int>()
            {
                { 1, 7 },{ 2, 9 },{ 3, 5 },{ 4, 9 },
                { 5, 3 },{ 6, 4 },{ 7, 4 },{ 8, 4 },
                { 9, 5 },{ 10, 6 },{ 11, 7 },{ 12, 5 }
            };
            Dictionary<int, int> districtMandates = vote.FindDistrictMandates();
            CollectionAssert.AreEquivalent(districtMandates, expectedDistrictMandates);
        }
        [TestMethod]
        public void FillPersonalMandateDictTesting() // Method for testing personal mandates
        {
            Dictionary<int, int> expectedPersonalMandates = new Dictionary<int, int>()
            {
                {1, 2 }, {2, 2 }, {3, 1 }, {4, 3 }, {5, 0 }, {6, 0 },
                {7, 1 }, {8, 2 }, {9, 0 }, {10, 1 }, {11, 0 }, {12, 1 }
            };
            vote.FindQuotas();
            vote.FillCandidateDictionary();
            Dictionary<int, int> personalMandates = vote.FillPersonalMandateDict();
            CollectionAssert.AreEqual(personalMandates, expectedPersonalMandates);
        }
    }
}