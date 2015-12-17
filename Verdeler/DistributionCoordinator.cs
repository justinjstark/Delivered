using System;
using System.Threading.Tasks;

namespace Verdeler
{
    public class DistributionCoordinator<TDistributable, TRecipient>
        where TDistributable : Distributable
        where TRecipient : Recipient
    {
        private readonly Distributor<TDistributable, TRecipient> _distributor;

        public DistributionCoordinator(Distributor<TDistributable, TRecipient> distributor)
        {
            _distributor = distributor;
        }

        public async Task DistributeFile(TDistributable distributable, TRecipient recipient)
        {
            var distributionTask = _distributor.DistributeAsync(distributable, recipient);

            try
            {
                await distributionTask;

                //Success
                //distributionRepository.MarkDistributionComplete(file.Id, destination.Name, DateTime.Now);
                
                //Do something
            }
            catch (AggregateException aggregateException)
            {
                //Failure
                //Flatten the aggregate exception returned from the async method
                var exceptions = aggregateException.Flatten().InnerExceptions;

                //Do something
            }
        }
    }
}
