namespace Keystone.Collections;

public class Tree<T>(T root) where T : INode<T>
{
    public T Root { get; } = root;

    public IEnumerable<T> DepthFirst()
    {
        var stack = new Stack<T>();
        stack.Push(Root);
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            yield return node;
            foreach (var child in node.Children.Reverse())
                stack.Push(child);
        }
    }

    public IEnumerable<T> BreadthFirst()
    {
        var queue = new Queue<T>();
        queue.Enqueue(Root);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node;
            foreach (var child in node.Children)
                queue.Enqueue(child);
        }
    }

    public IEnumerable<T> Leaves() => DepthFirst().Where(n => n.Children.Count == 0);
}