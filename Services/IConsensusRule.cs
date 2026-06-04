using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Services
{
    public interface IConsensusRule
    {
        bool IsValidHash(string hash, int difficulty);
    }
}
