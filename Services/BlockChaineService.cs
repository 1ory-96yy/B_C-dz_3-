using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Services
{
    public class BlockChaineService
    {
        public List<Block> chain { get; set; }
        public int Difficulty { get; set; } = 10;
        private readonly HashingService hashingService;
        private readonly int _adjustmentInterval = 2;
        private readonly int _blockGenerationInterval = 5;


        private readonly MiningService _miningService;
        private readonly IConsensusRule _consensusRule;

        public BlockChaineService(IConsensusRule consensusRule)
        {
            this.chain = new List<Block>();
            this.hashingService = new HashingService();
            this._consensusRule = consensusRule;
            this._miningService = new MiningService(consensusRule);
            this.CreateGenesisBlock();
        }

        private void CreateGenesisBlock()
        {
            var genesisBlock = new Block(0, "0","");
            genesisBlock.hash = this.hashingService.ComputeHash(genesisBlock);
            this.chain.Add(genesisBlock);
        }

        public void AddBlock(string data)
        {
            var previousBlock = this.chain.Last();
            var newBlock = new Block(previousBlock.index + 1, data, previousBlock.hash);
            newBlock.Difficulty = this.Difficulty;
            _miningService.MineBlock(newBlock, Difficulty);

            this.chain.Add(newBlock);
            if (newBlock.index % _adjustmentInterval == 0)
            {
                AdjustDifficulty();
            }
        }


        private void AdjustDifficulty()
        {
            var recentBlocks = this.chain.Where(b => b.index > 0).TakeLast(_adjustmentInterval).ToList();
            if (recentBlocks.Count ==0)
                return;
            double averageMiningTime = recentBlocks.Average(x =>(x.timestamp - this.chain[x.index - 1].timestamp).TotalSeconds);
            if (averageMiningTime < _blockGenerationInterval / 2)
            {
                Difficulty++;
            }
            else if (averageMiningTime > _blockGenerationInterval * 2 && Difficulty > 1)
            {
                Difficulty= Math.Max(1, Difficulty - 1);
            }
        }
        
        public bool IsChainValid()
        {
            for (int i = 1; i < this.chain.Count; i++)
            {
                var currentBlock = this.chain[i];
                var previousBlock = this.chain[i - 1];
                if (currentBlock.hash != this.hashingService.ComputeHash(currentBlock))
                    return false;
                if (currentBlock.previousHash != previousBlock.hash)
                    return false;
                if (!_consensusRule.IsValidHash(currentBlock.hash, currentBlock.Difficulty))
                    return false;
            }
            return true;
        }

        public bool ResolveConsensus(List<Block> competingChain)
        {
            if (!competingChain.Any())
                return false;
            for (int i = 1; i < competingChain.Count; i++)
            {
                var currentBlock = competingChain[i];
                var previousBlock = competingChain[i - 1];
                var hashingService = new HashingService();
                if (currentBlock.hash != hashingService.ComputeHash(currentBlock))
                    return false;
                if (currentBlock.previousHash != previousBlock.hash)
                    return false;
                if (!_consensusRule.IsValidHash(currentBlock.hash, currentBlock.Difficulty))
                    return false;
            }
            if (competingChain.Count > this.chain.Count)
            {
                this.chain = competingChain;
                return true;
            }

            return false;
        }
    }
}
