using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp1.Models;

namespace ConsoleApp1.Services
{
    public class MiningService
    {
        private readonly HashingService _hashingService;
        private readonly IConsensusRule _consensusRule;

        public MiningService(IConsensusRule consensusRule)
        {
            _hashingService = new HashingService();
            _consensusRule = consensusRule;
        }

        public void MineBlock(Block block, int difficulty)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            int processorCount = Environment.ProcessorCount;
            object lockObject = new object();

            Parallel.For(0, processorCount, (i, state) =>
            {
                long localNonce = i;
                while (!cancellationToken.IsCancellationRequested)
                {
                    string hash = _hashingService.ComputeSha256($"{block.index}{block.timestamp}{block.previousHash}{block.data}{localNonce}");
                    if (_consensusRule.IsValidHash(hash, difficulty))
                    {
                        lock (lockObject)
                        {
                            if (!cancellationToken.IsCancellationRequested)
                            {
                                block.nonce = localNonce;
                                block.hash = hash;
                                cancellationTokenSource.Cancel();
                            }
                        }
                    }
                    localNonce += processorCount;
                }
            });

            block.miningDuration = (DateTime.UtcNow - block.timestamp).TotalSeconds;
        }
    }
}
