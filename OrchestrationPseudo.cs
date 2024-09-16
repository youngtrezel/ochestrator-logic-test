using System;
using System.Collections.Generic;

public class Event
{
    public int Timestamp { get; set; }
    public string Data { get; set; }
}

public class EventOrchestrator
{
    private List<Queue<Event>> incomingQueues = new List<Queue<Event>>();
    private Queue<Event> outgoingQueue = new Queue<Event>();

    // Maximum number of events orchestrator can process at once
    private const int MAX_EVENTS = 5;

    // Constructor initializes 5 incoming queues with 5 events each
    public EventOrchestrator(List<List<Event>> incomingEvents)
    {
        foreach (var events in incomingEvents)
        {
            Queue<Event> queue = new Queue<Event>(events);
            incomingQueues.Add(queue);
        }
    }

    public void ProcessMostRecentEvents()
    {
        // A priority queue (min-heap) to keep track of the 3 most recent events
        PriorityQueue<Event, int> minHeap = new PriorityQueue<Event, int>(3);

        while (outgoingQueue.Count < 3)
        {
            // Get the top event from each of the 5 queues (peek, don't dequeue)
            List<Event> currentEvents = new List<Event>();
            for (int i = 0; i < 5; i++)
            {
                if (incomingQueues[i].Count > 0)
                {
                    currentEvents.Add(incomingQueues[i].Peek()); // Peek to inspect the event
                }
            }

            // Sort the current events by timestamp in descending order (most recent first)
            currentEvents.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));

            // Add events to the min-heap, ensuring the size doesn't exceed 3
            foreach (var e in currentEvents)
            {
                if (minHeap.Count < 3)
                {
                    minHeap.Enqueue(e, e.Timestamp); // Add to heap if less than 3
                }
                else if (e.Timestamp > minHeap.Peek().Timestamp)
                {
                    minHeap.Dequeue();  // Remove the oldest
                    minHeap.Enqueue(e, e.Timestamp);  // Add the newer event
                }
            }

            // Now we need to process and dequeue one event from one of the queues
            // Remove the most recent event across all queues
            Event mostRecentEvent = currentEvents[0];
            for (int i = 0; i < 5; i++)
            {
                if (incomingQueues[i].Count > 0 && incomingQueues[i].Peek() == mostRecentEvent)
                {
                    incomingQueues[i].Dequeue(); // Remove from the queue
                    break;
                }
            }

            // Add the processed event to the outgoing queue
            outgoingQueue.Enqueue(mostRecentEvent);
        }
    }

    public void PrintOutgoingQueue()
    {
        while (outgoingQueue.Count > 0)
        {
            Event e = outgoingQueue.Dequeue();
            Console.WriteLine($"Event Timestamp: {e.Timestamp}, Data: {e.Data}");
        }
    }
}

// Example usage
public class Program
{
    public static void Main()
    {
        List<Event> queue1 = new List<Event> {
            new Event { Timestamp = 1, Data = "Event1-1" },
            new Event { Timestamp = 2, Data = "Event1-2" },
            new Event { Timestamp = 5, Data = "Event1-3" },
            new Event { Timestamp = 9, Data = "Event1-4" },
            new Event { Timestamp = 14, Data = "Event1-5" }
        };

        List<Event> queue2 = new List<Event> {
            new Event { Timestamp = 3, Data = "Event2-1" },
            new Event { Timestamp = 6, Data = "Event2-2" },
            new Event { Timestamp = 7, Data = "Event2-3" },
            new Event { Timestamp = 10, Data = "Event2-4" },
            new Event { Timestamp = 15, Data = "Event2-5" }
        };

        // Assume more queues like queue1 and queue2 are created...

        List<List<Event>> incomingEvents = new List<List<Event>> { queue1, queue2 /*...more queues*/ };

        EventOrchestrator orchestrator = new EventOrchestrator(incomingEvents);
        orchestrator.ProcessMostRecentEvents();
        orchestrator.PrintOutgoingQueue();
    }
}
