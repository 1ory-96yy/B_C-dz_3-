using ConsoleApp1.Services;
using System;
using System.Collections.Generic;
using ConsoleApp1.Models;
using System.Linq;
using System.Threading;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string uniqueSignature = "ace";
            var consensusRule = new ConsoleApp1.Services.ConsensusRule(uniqueSignature);

            var blockchain = new BlockChaineService(consensusRule);

            blockchain.AddBlock("Block1 Data");
            blockchain.AddBlock("Block2 Data");
            blockchain.AddBlock("Block3 Data");
            blockchain.AddBlock("Block4 Data");

            Console.WriteLine("Main chain:");
            foreach (var block in blockchain.chain)
            {
                Console.WriteLine($"Index: {block.index}, Hash: {block.hash}, Data: {block.data}");
            }

            var attackerChain = new List<Block>(blockchain.chain.GetRange(0, 2));

            var attackerBlock2 = new Block(2, "Attacker stole1000 coins", attackerChain[1].hash);
            var miningService = new MiningService(consensusRule);
            miningService.MineBlock(attackerBlock2, blockchain.Difficulty);
            attackerChain.Add(attackerBlock2);

            for (int i = 3; i <= 5; i++)
            {
                var previousBlock = attackerChain[^1];
                var newBlock = new Block(i, $"Attacker Block {i}", previousBlock.hash);
                miningService.MineBlock(newBlock, blockchain.Difficulty);
                attackerChain.Add(newBlock);
            }

            Console.WriteLine("\nAttacker chain:");
            foreach (var block in attackerChain)
            {
                Console.WriteLine($"Index: {block.index}, Hash: {block.hash}, Data: {block.data}");
            }

            bool replaced = blockchain.ResolveConsensus(attackerChain);

            Console.WriteLine("\nAfter resolving consensus:");
            foreach (var block in blockchain.chain)
            {
                Console.WriteLine($"Index: {block.index}, Hash: {block.hash}, Data: {block.data}");
            }

            Console.WriteLine(replaced ? "The chain was replaced by the attacker's chain." : "The main chain remains intact.");


            var displayService = new BlockChainDisplayService();

            for (int i = 0; i < 5; i++)
            {
                blockchain.AddBlock("First block data");
                displayService.printChain(blockchain.chain.TakeLast(5).ToList());
                displayService.printChainValidity(blockchain.IsChainValid());
            }
        }
    }
}
