using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VotingApp
{
    public class District
    {
        internal int _districtNum;
        public int _mandates;
        public string _districtName;
        internal double _quota;
        public District(string pathLine)
        {
            _quota = 0;
            _districtName = ReturnName(pathLine);
            _mandates = ReturnMandates(pathLine);
            _districtNum = ReturnDistrictNumber(pathLine);
        }

        private string ReturnName(string pathLine)
        {
            string[] details = pathLine.Split(')');
            int index = details[0].IndexOf('(') + 1;
            return details[0].Substring(index, details[0].Length - index);
        }

        private int ReturnMandates(string pathLine)
        {
            string mandatesInfo = pathLine.Split('–').Last();
            mandatesInfo = mandatesInfo.Trim();
            string[] mandates = mandatesInfo.Split(' ');
            return int.Parse(mandates[0]);
        }

        private int ReturnDistrictNumber(string pathLine)
        {
            string[] infoList = pathLine.Split(' ');
            return int.Parse(infoList[2]);
        }
    }
}
