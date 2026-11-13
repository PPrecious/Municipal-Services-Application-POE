
using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public class BasicTreeNode<T>
    {
        public T Value { get; set; }
        public List<BasicTreeNode<T>> Children { get; private set; }

        public BasicTreeNode(T value)
        {
            Value = value;
            Children = new List<BasicTreeNode<T>>();
        }

        public void AddChild(BasicTreeNode<T> child)
        {
            if (child != null)
                Children.Add(child);
        }

        public bool Contains(T value)
        {
            if (EqualityComparer<T>.Default.Equals(Value, value))
                return true;

            foreach (BasicTreeNode<T> child in Children)
            {
                if (child.Contains(value))
                    return true;
            }

            return false;
        }

        public List<T> PreOrder()
        {
            List<T> result = new List<T>();
            result.Add(Value);

            foreach (BasicTreeNode<T> child in Children)
                result.AddRange(child.PreOrder());

            return result;
        }
    }

    public class BinaryTreeNode<T>
    {
        public T Value { get; set; }
        public BinaryTreeNode<T> Left { get; set; }
        public BinaryTreeNode<T> Right { get; set; }

        public BinaryTreeNode(T value)
        {
            Value = value;
        }

        public bool Contains(T value)
        {
            if (EqualityComparer<T>.Default.Equals(Value, value))
                return true;

            return (Left != null && Left.Contains(value)) ||
                   (Right != null && Right.Contains(value));
        }

        public List<T> PreOrder()
        {
            List<T> result = new List<T>();
            result.Add(Value);

            if (Left != null)
                result.AddRange(Left.PreOrder());

            if (Right != null)
                result.AddRange(Right.PreOrder());

            return result;
        }
    }

    public class BinarySearchTree<T>
    {
        private class Node
        {
            public int Key;
            public T Value;
            public Node Left;
            public Node Right;

            public Node(int key, T value)
            {
                Key = key;
                Value = value;
            }
        }

        private Node root;

        public void Insert(int key, T value)
        {
            root = InsertNode(root, key, value);
        }

        private Node InsertNode(Node node, int key, T value)
        {
            if (node == null)
                return new Node(key, value);

            if (key < node.Key)
                node.Left = InsertNode(node.Left, key, value);
            else if (key > node.Key)
                node.Right = InsertNode(node.Right, key, value);
            else
                node.Value = value;

            return node;
        }

        public T Search(int key)
        {
            Node current = root;

            while (current != null)
            {
                if (key == current.Key)
                    return current.Value;

                current = key < current.Key
                    ? current.Left
                    : current.Right;
            }

            return default(T);
        }
    }

    public enum AvlRotation
    {
        None,
        LL,
        RR,
        LR,
        RL
    }

    public class AvlTree<T>
    {
        private class Node
        {
            public int Key;
            public int Height;
            public T Value;
            public Node Left;
            public Node Right;

            public Node(int key, T value)
            {
                Key = key;
                Value = value;
                Height = 1;
            }
        }

        private Node root;

        public int LLRotations { get; private set; }
        public int RRRotations { get; private set; }
        public int LRRotations { get; private set; }
        public int RLRotations { get; private set; }

        public AvlRotation LastRotation { get; private set; }

        private int Height(Node node)
        {
            return node == null ? 0 : node.Height;
        }

        private int Balance(Node node)
        {
            return node == null
                ? 0
                : Height(node.Left) - Height(node.Right);
        }

        private void UpdateHeight(Node node)
        {
            node.Height = 1 + Math.Max(
                Height(node.Left),
                Height(node.Right));
        }

        private Node RotateRight(Node y)
        {
            Node x = y.Left;
            Node temp = x.Right;

            x.Right = y;
            y.Left = temp;

            UpdateHeight(y);
            UpdateHeight(x);

            return x;
        }

        private Node RotateLeft(Node x)
        {
            Node y = x.Right;
            Node temp = y.Left;

            y.Left = x;
            x.Right = temp;

            UpdateHeight(x);
            UpdateHeight(y);

            return y;
        }

        public void Insert(int key, T value)
        {
            LastRotation = AvlRotation.None;
            root = InsertNode(root, key, value);
        }

        private Node InsertNode(Node node, int key, T value)
        {
            if (node == null)
                return new Node(key, value);

            if (key < node.Key)
                node.Left = InsertNode(node.Left, key, value);
            else if (key > node.Key)
                node.Right = InsertNode(node.Right, key, value);
            else
            {
                node.Value = value;
                return node;
            }

            UpdateHeight(node);

            int balance = Balance(node);

            if (balance > 1 && key < node.Left.Key)
            {
                LLRotations++;
                LastRotation = AvlRotation.LL;
                return RotateRight(node);
            }

            if (balance < -1 && key > node.Right.Key)
            {
                RRRotations++;
                LastRotation = AvlRotation.RR;
                return RotateLeft(node);
            }

            if (balance > 1 && key > node.Left.Key)
            {
                LRRotations++;
                LastRotation = AvlRotation.LR;
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            if (balance < -1 && key < node.Right.Key)
            {
                RLRotations++;
                LastRotation = AvlRotation.RL;
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        public T Search(int key)
        {
            Node current = root;

            while (current != null)
            {
                if (key == current.Key)
                    return current.Value;

                current = key < current.Key
                    ? current.Left
                    : current.Right;
            }

            return default(T);
        }
    }

    public class RedBlackRequestIndex
    {
        private enum Colour
        {
            Red,
            Black
        }

        private class Node
        {
            public int Key;
            public ServiceRequest Value;
            public Colour Colour;
            public Node Parent;
            public Node Left;
            public Node Right;

            public Node(int key, ServiceRequest value)
            {
                Key = key;
                Value = value;
                Colour = Colour.Red;
            }
        }

        private Node root;

        public int LeftRotations { get; private set; }
        public int RightRotations { get; private set; }
        public int Recolourings { get; private set; }

        public void Insert(ServiceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            Node existing = FindNode(request.RequestId);

            if (existing != null)
            {
                existing.Value = request;
                return;
            }

            Node inserted = new Node(request.RequestId, request);
            Node parent = null;
            Node current = root;

            while (current != null)
            {
                parent = current;

                current = inserted.Key < current.Key
                    ? current.Left
                    : current.Right;
            }

            inserted.Parent = parent;

            if (parent == null)
                root = inserted;
            else if (inserted.Key < parent.Key)
                parent.Left = inserted;
            else
                parent.Right = inserted;

            FixInsertion(inserted);
        }

        private void FixInsertion(Node node)
        {
            while (node != root &&
                   node.Parent != null &&
                   node.Parent.Colour == Colour.Red)
            {
                Node parent = node.Parent;
                Node grandparent = parent.Parent;

                if (parent == grandparent.Left)
                {
                    Node uncle = grandparent.Right;

                    if (uncle != null && uncle.Colour == Colour.Red)
                    {
                        parent.Colour = Colour.Black;
                        uncle.Colour = Colour.Black;
                        grandparent.Colour = Colour.Red;
                        Recolourings += 3;
                        node = grandparent;
                    }
                    else
                    {
                        if (node == parent.Right)
                        {
                            node = parent;
                            RotateLeft(node);
                            parent = node.Parent;
                            grandparent = parent.Parent;
                        }

                        parent.Colour = Colour.Black;
                        grandparent.Colour = Colour.Red;
                        Recolourings += 2;
                        RotateRight(grandparent);
                    }
                }
                else
                {
                    Node uncle = grandparent.Left;

                    if (uncle != null && uncle.Colour == Colour.Red)
                    {
                        parent.Colour = Colour.Black;
                        uncle.Colour = Colour.Black;
                        grandparent.Colour = Colour.Red;
                        Recolourings += 3;
                        node = grandparent;
                    }
                    else
                    {
                        if (node == parent.Left)
                        {
                            node = parent;
                            RotateRight(node);
                            parent = node.Parent;
                            grandparent = parent.Parent;
                        }

                        parent.Colour = Colour.Black;
                        grandparent.Colour = Colour.Red;
                        Recolourings += 2;
                        RotateLeft(grandparent);
                    }
                }
            }

            if (root != null && root.Colour != Colour.Black)
            {
                root.Colour = Colour.Black;
                Recolourings++;
            }
        }

        private void RotateLeft(Node node)
        {
            Node pivot = node.Right;

            node.Right = pivot.Left;

            if (pivot.Left != null)
                pivot.Left.Parent = node;

            pivot.Parent = node.Parent;

            if (node.Parent == null)
                root = pivot;
            else if (node == node.Parent.Left)
                node.Parent.Left = pivot;
            else
                node.Parent.Right = pivot;

            pivot.Left = node;
            node.Parent = pivot;

            LeftRotations++;
        }

        private void RotateRight(Node node)
        {
            Node pivot = node.Left;

            node.Left = pivot.Right;

            if (pivot.Right != null)
                pivot.Right.Parent = node;

            pivot.Parent = node.Parent;

            if (node.Parent == null)
                root = pivot;
            else if (node == node.Parent.Right)
                node.Parent.Right = pivot;
            else
                node.Parent.Left = pivot;

            pivot.Right = node;
            node.Parent = pivot;

            RightRotations++;
        }

        private Node FindNode(int id)
        {
            Node current = root;

            while (current != null)
            {
                if (id == current.Key)
                    return current;

                current = id < current.Key
                    ? current.Left
                    : current.Right;
            }

            return null;
        }

        public ServiceRequest Search(int id)
        {
            Node node = FindNode(id);
            return node == null ? null : node.Value;
        }

        public List<ServiceRequest> InOrder()
        {
            List<ServiceRequest> result =
                new List<ServiceRequest>();

            InOrder(root, result);
            return result;
        }

        private void InOrder(
            Node node,
            List<ServiceRequest> result)
        {
            if (node == null)
                return;

            InOrder(node.Left, result);
            result.Add(node.Value);
            InOrder(node.Right, result);
        }

        public bool Validate()
        {
            if (root == null)
                return true;

            if (root.Colour != Colour.Black)
                return false;

            return ValidateNode(root, int.MinValue, int.MaxValue) >= 0;
        }

        private int ValidateNode(Node node, int min, int max)
        {
            if (node == null)
                return 1;

            if (node.Key <= min || node.Key >= max)
                return -1;

            if (node.Colour == Colour.Red)
            {
                if ((node.Left != null &&
                     node.Left.Colour == Colour.Red) ||
                    (node.Right != null &&
                     node.Right.Colour == Colour.Red))
                    return -1;
            }

            int leftHeight = ValidateNode(node.Left, min, node.Key);
            int rightHeight = ValidateNode(node.Right, node.Key, max);

            if (leftHeight < 0 ||
                rightHeight < 0 ||
                leftHeight != rightHeight)
                return -1;

            return leftHeight +
                (node.Colour == Colour.Black ? 1 : 0);
        }
    }

    public class MinHeap<T>
    {
        private class Entry
        {
            public int Priority;
            public long Sequence;
            public T Value;

            public Entry(int priority, long sequence, T value)
            {
                Priority = priority;
                Sequence = sequence;
                Value = value;
            }
        }

        private readonly List<Entry> items = new List<Entry>();
        private long nextSequence;

        public int Count
        {
            get { return items.Count; }
        }

        private bool ComesBefore(Entry first, Entry second)
        {
            if (first.Priority != second.Priority)
                return first.Priority < second.Priority;

            return first.Sequence < second.Sequence;
        }

        public void Enqueue(T value, int priority)
        {
            items.Add(new Entry(priority, nextSequence++, value));

            int index = items.Count - 1;

            while (index > 0)
            {
                int parent = (index - 1) / 2;

                if (!ComesBefore(items[index], items[parent]))
                    break;

                Swap(index, parent);
                index = parent;
            }
        }

        public T Dequeue()
        {
            if (items.Count == 0)
                throw new InvalidOperationException("Heap is empty.");

            T result = items[0].Value;
            Entry last = items[items.Count - 1];

            items.RemoveAt(items.Count - 1);

            if (items.Count > 0)
            {
                items[0] = last;
                HeapifyDown(0);
            }

            return result;
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int left = index * 2 + 1;
                int right = index * 2 + 2;
                int smallest = index;

                if (left < items.Count &&
                    ComesBefore(items[left], items[smallest]))
                    smallest = left;

                if (right < items.Count &&
                    ComesBefore(items[right], items[smallest]))
                    smallest = right;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int first, int second)
        {
            Entry temp = items[first];
            items[first] = items[second];
            items[second] = temp;
        }
    }

    public class GraphEdge
    {
        public string From { get; set; }
        public string To { get; set; }
        public int Weight { get; set; }

        public override string ToString()
        {
            return From + " - " + To + " (weight " + Weight + ")";
        }
    }

    public class ServiceLocationGraph
    {
        private readonly Dictionary<string, List<GraphEdge>> edges =
            new Dictionary<string, List<GraphEdge>>(
                StringComparer.OrdinalIgnoreCase);

        public void AddLocation(string location)
        {
            if (!edges.ContainsKey(location))
                edges[location] = new List<GraphEdge>();
        }

        public void AddRoad(string first, string second, int weight)
        {
            if (string.IsNullOrWhiteSpace(first) ||
                string.IsNullOrWhiteSpace(second))
                throw new ArgumentException("Locations are required.");

            if (weight < 0)
                throw new ArgumentOutOfRangeException("weight");

            AddLocation(first);
            AddLocation(second);

            edges[first].Add(new GraphEdge
            {
                From = first,
                To = second,
                Weight = weight
            });

            edges[second].Add(new GraphEdge
            {
                From = second,
                To = first,
                Weight = weight
            });
        }

        public List<string> BFS(string start)
        {
            List<string> result = new List<string>();

            if (!edges.ContainsKey(start))
                return result;

            Queue<string> queue = new Queue<string>();
            HashSet<string> visited =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                result.Add(current);

                foreach (GraphEdge edge in edges[current])
                {
                    if (visited.Add(edge.To))
                        queue.Enqueue(edge.To);
                }
            }

            return result;
        }

        public List<string> DFS(string start)
        {
            List<string> result = new List<string>();

            if (!edges.ContainsKey(start))
                return result;

            Stack<string> stack = new Stack<string>();
            HashSet<string> visited =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            stack.Push(start);

            while (stack.Count > 0)
            {
                string current = stack.Pop();

                if (!visited.Add(current))
                    continue;

                result.Add(current);

                foreach (GraphEdge edge in edges[current].AsEnumerable().Reverse())
                {
                    if (!visited.Contains(edge.To))
                        stack.Push(edge.To);
                }
            }

            return result;
        }

        public List<GraphEdge> MinimumSpanningTree(
            string start,
            out int totalWeight)
        {
            totalWeight = 0;
            List<GraphEdge> result = new List<GraphEdge>();

            if (!edges.ContainsKey(start))
                return result;

            HashSet<string> visited =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            MinHeap<GraphEdge> heap = new MinHeap<GraphEdge>();

            visited.Add(start);
            AddAvailableEdges(start, visited, heap);

            while (heap.Count > 0 && visited.Count < edges.Count)
            {
                GraphEdge edge = heap.Dequeue();

                if (visited.Contains(edge.To))
                    continue;

                visited.Add(edge.To);
                result.Add(edge);
                totalWeight += edge.Weight;

                AddAvailableEdges(edge.To, visited, heap);
            }

            return result;
        }

        private void AddAvailableEdges(
            string location,
            HashSet<string> visited,
            MinHeap<GraphEdge> heap)
        {
            foreach (GraphEdge edge in edges[location])
            {
                if (!visited.Contains(edge.To))
                    heap.Enqueue(edge, edge.Weight);
            }
        }
    }
}
