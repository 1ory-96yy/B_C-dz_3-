using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Services
{
    public class ConsensusRule : IConsensusRule
    {
        private readonly string _signature;

        public ConsensusRule(string signature)
        {
            _signature = signature.ToLower();
        }

        public bool IsValidHash(string hash, int difficulty)
        {
            if (string.IsNullOrEmpty(hash))
                return false;
            var prefix = new string('0', difficulty);
            var requiredStart = prefix + _signature;
            return hash.StartsWith(requiredStart, StringComparison.OrdinalIgnoreCase);
        }
    }
}
