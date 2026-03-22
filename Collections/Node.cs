using Keystone.Reactivity;

namespace Keystone.Collections;

public interface INode<T> where T : INode<T>
{
    IReadOnlyProperty<T?> Parent { get; }
    IReadOnlyList<T> Children { get; }
}

public abstract class Node<T> : INode<T> where T : Node<T>
{
    private readonly List<T> _children = [];
    private readonly Property<T?> _parent = new(null);

    public IReadOnlyProperty<T?> Parent => _parent;
    public IReadOnlyList<T> Children => _children;

    protected void Add(T child)
    {
        if (child.Parent.Value is not null)
            throw new InvalidOperationException("Node already has a parent.");
        child._parent.Value = (T)this;
        _children.Add(child);
    }

    protected void Remove(T child)
    {
        child._parent.Value = null;
        _children.Remove(child);
    }
}