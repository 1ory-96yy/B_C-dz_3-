using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp1.Models;

namespace ConsoleApp1.Services
{
    public class MiningService
    {
        private readonly HashingService _hashingService;
        private readonly Func<string, bool> _isValidHash;

        public MiningService(Func<string, bool> isValidHash)
        {
            _hashingService = new HashingService();
            _isValidHash = isValidHash;
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
                    if (_isValidHash(hash))
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
