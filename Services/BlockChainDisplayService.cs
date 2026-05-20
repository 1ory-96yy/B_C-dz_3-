using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Services
{
    public class BlockChainDisplayService
    {
        public void printChain(List<Models.Block> chain)
        {
            foreach (var block in chain)
            {
                Console.WriteLine($"Index: {block.index}");
                Console.WriteLine($"Timestamp: {block.timestamp}");
                Console.WriteLine($"Data: {block.data}");
                Console.WriteLine($"Hash: {block.hash}");
                Console.WriteLine($"Nonce: {block.nonce}");
                Console.WriteLine($"Mining Duration: {block.Difficulty}");
                Console.WriteLine($"Previous Hash: {block.previousHash}");
                Console.WriteLine(new string('-', 20));
            }
        }

        public void printChainValidity(bool isValid)
        {
            if (isValid)
                Console.WriteLine("The blockchain is valid.");
            else
                Console.WriteLine("The blockchain is invalid.");
        }
    }
}
