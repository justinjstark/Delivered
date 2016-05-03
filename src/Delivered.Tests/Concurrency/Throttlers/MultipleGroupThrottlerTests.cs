using System.Threading.Tasks;
using Delivered.Concurrency.Throttlers;
using NUnit.Framework;
using Shouldly;

namespace Delivered.Tests.Concurrency.Throttlers
{
    [TestFixture]
    public class MultipleGroupThrottlerTests
    {
        [Test]
        public void Do_CallsTheAsyncFunc()
        {
            var throttler = new MultipleGroupThrottler<int>();

            var asyncFuncCalled = false;
            throttler.Do(async () =>
            {
                asyncFuncCalled = true;
                await Task.FromResult(0);
            }, 1).Wait();

            asyncFuncCalled.ShouldBeTrue();
        }

        [Test]
        public void Do_WaitsOnThrottlersForTheSameGrouping()
        {
            var throttler = new MultipleGroupThrottler<int>();

            throttler.AddConcurrencyLimiter(e => 1, 1);

            var task1 = throttler.Do(async () => await Task.Delay(400), 1);
            var task2 = throttler.Do(async () => await Task.Delay(200), 1);
            var task3 = throttler.Do(async () => await Task.Delay(100), 1);

            task1.Wait();

            task2.IsCompleted.ShouldBeFalse();

            task2.Wait();

            task3.IsCompleted.ShouldBeFalse();
        }

        [Test]
        public void Do_DoesNotWaitOnThrottlersForDifferentGroupings()
        {
            var throttler = new MultipleGroupThrottler<int>();

            throttler.AddConcurrencyLimiter(e => e % 3, 1);

            var task1 = throttler.Do(async () => await Task.Delay(300), 0);
            var task2 = throttler.Do(async () => await Task.Delay(200), 1);
            var task3 = throttler.Do(async () => await Task.Delay(200), 2);

            task1.Wait();

            task2.IsCompleted.ShouldBeTrue();
            task3.IsCompleted.ShouldBeTrue();
        }

        [Test]
        public void Do_WaitsForThrottlersForSameGroupingsAndDoesNotWaitForDifferentGroupings()
        {
            var throttler = new MultipleGroupThrottler<int>();

            throttler.AddConcurrencyLimiter(e => e % 2, 1);

            var task1 = throttler.Do(async () => await Task.Delay(300), 0);
            var task2 = throttler.Do(async () => await Task.Delay(200), 1);
            var task3 = throttler.Do(async () => await Task.Delay(200), 1);

            task1.Wait();

            task2.IsCompleted.ShouldBeTrue();
            task3.IsCompleted.ShouldBeFalse();
        }
    }
}
