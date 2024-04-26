using System.Collections.Generic;
using System.Linq;

public class PriorityQueue<TElement, TPriority>
{
    private List<(TElement Element, TPriority Priority)> elements = new List<(TElement, TPriority)>();

    public int Count => elements.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        elements.Add((element, priority));
        elements = elements.OrderBy(x => x.Priority).ToList();
    }

    public TElement Dequeue()
    {
        var item = elements.First().Element;
        elements.RemoveAt(0);
        return item;
    }

    public bool Contains(TElement element)
    {
        return elements.Any(e => e.Element.Equals(element));
    }
}