using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VotingApp
{
    public class Candidate
    {
        internal int _districtNum;
        public int _votes;
        public string _partyName;
        public string _candidateName;
        internal bool _isMandate;
        public Candidate(string line)
        {
            _districtNum = ReturnDistrictNum(line);
            _votes = ReturnVotes(line);
            _partyName = ReturnPartyName(line);
            _candidateName = ReturnCandidateName(line);
            _isMandate = false; // false - candidate is not personal mandate, true - candidate is personal mandate
        }

        private string ReturnCandidateName(string line)
        {
            string[] lineInfo = line.Split(',');
            return lineInfo[2];
        }

        private string ReturnPartyName(string line)
        {
            string[] lineInfo = line.Split(',');
            return lineInfo[1];
        }

        private int ReturnVotes(string line)
        {
            return int.Parse(line.Split(',').Last());
        }

        private int ReturnDistrictNum(string line)
        {
            return int.Parse(line.Split(',').First());
        }
    }
}
