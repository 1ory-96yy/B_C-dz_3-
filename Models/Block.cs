using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    public class Block
    {
        public int index { get; set; }
        public DateTime timestamp { get; set; }
        public string data { get; set; } = string.Empty;
        public string hash { get; set; }
        public string previousHash { get; set; }

        public long nonce { get; set; }

        public double miningDuration { get; set; }

        public int Difficulty { get; set; }
        public Block(int index, string data, string previousHash)
        {
            this.index = index;
            this.timestamp = DateTime.Now;
            this.data = data;
            this.previousHash = previousHash;
            this.hash = "";
        }
    }
}
