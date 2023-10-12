using System;
using System.Linq;

using Xunit;
using Assert = Xunit.Assert;

using Themis.Index.KdTree;

namespace Themis.Index.Tests.KdTree;

public class PriorityQueueTests : IDisposable
{
    private PriorityQueue<string, float>? Queue;

    private const int InitialCapacity = 2;
    private static readonly FloatMath TypeMath = new();

    public PriorityQueueTests()
    {
        Queue = new PriorityQueue<string, float>(TypeMath, InitialCapacity);
    }

    [Fact]
    public void PriorityQueueTest()
    {
        if (Queue == null) throw new ArgumentNullException(nameof(Queue));

        Assert.Equal(InitialCapacity, Queue.Capacity);

        int ExpectedFinalCount = 0;

        foreach (var person in TestItems.People) { Queue.Enqueue(person.Name, person.Age); }

        var peopleByAgeDesc = TestItems.People.OrderByDescending(p => p.Age).ToArray();

        float ExpectedHighestPriority = peopleByAgeDesc.First().Age;
        float ActualHighestPriority = Queue.GetHighestPriority();
        Assert.Equal(ExpectedHighestPriority, ActualHighestPriority);

        foreach (int index in Enumerable.Range(0, peopleByAgeDesc.Length))
        {
            var ExpectedPerson = peopleByAgeDesc[index];
            int ExpectedQueueCount = peopleByAgeDesc.Length - index - 1;

            var ActualPersonName = Queue.Dequeue();

            Assert.Equal(ExpectedPerson.Name, ActualPersonName);
            Assert.Equal(ExpectedQueueCount, Queue.Count);
        }

        Assert.Equal(ExpectedFinalCount, Queue.Count);
    }

    public void Dispose()
    {
        Queue = null;
    }
}