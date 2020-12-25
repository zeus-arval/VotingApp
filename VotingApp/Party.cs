using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VotingApp
{
    public class Party
    {
        internal int _votes;
        internal int _districtNumber;
        public string _name;
        internal bool _isWinner; // true is winner, false is loser
        internal int _mandates;
        private Dictionary<string, string> shortFullPartyName = new Dictionary<string, string>()
            {
                {"REF", "Eesti Reformierakond"},
                {"KESK", "Eesti Keskerakond"},
                {"EKRE", "Eesti Konservatiivne Rahvaerakond"},
                {"IE", "Isamaa Erakond"},
                {"SDE", "Sotsiaaldemokraatlik Erakond"},
                {"EE200", "Eesti 200"},
                {"ROH", "Erakond Eestimaa Rohelised"},
                {"ERE", "Elurikkuse Erakond"},
                {"EVA", "Eesti Vabaerakond"},
                {"ÜKSIK.", "Üksikkandidaadid"},
                {"EÜVP", "Eestimaa Ühendatud Vasakpartei"}
            };
        public Party(string fileLine)
        {
            _name = ReturnPartyName(fileLine);
            _votes = ReturnVotes(fileLine);
            _isWinner = false; 
        }
        public Party(string fileLine, District district)
        {
            _name = ReturnPartyName(fileLine, district);
            _votes = ReturnVotes(fileLine, district);
            _districtNumber = ReturnDistrict(fileLine);
            _mandates = 0;
        }

        private int ReturnDistrict(string fileLine)
        {
            return int.Parse(fileLine.Split(',').First());
        }

        private int ReturnVotes(string fileLine)
        {   // Method , which returns amount fof votes
            return int.Parse(fileLine.Split(' ').Last());
        }
        private int ReturnVotes(string fileLine, District district)
        {   // Method , which returns amount fof votes
            return int.Parse(fileLine.Split(',').Last());
        }
        private string ReturnPartyName(string line)
        {   //Method which take full line from path and returns full party name
            string[] partyInfo = line.Split(' ');
            if(partyInfo.Length < 3 && partyInfo[0] != "Üksikkandidaadid")
            {
                return shortFullPartyName[partyInfo[0]];
            }
            else
            {
                string partyName = "";
                for (int i = 0; i < partyInfo.Length - 1; i++)
                {
                    partyName += $"{partyInfo[i]} ";
                }
                return partyName;
            }            
        }
        private string ReturnPartyName(string line, District district)
        {   //Method returns name from line from path
            string[] details = line.Split(',');
            return details[1];
        }
    }
}
