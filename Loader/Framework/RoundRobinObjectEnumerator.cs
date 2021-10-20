using System;
using System.Linq;
using System.Collections.Generic;

namespace NPPES.Loader.Framework
{
    public class RoundRobinObjectEnumerator<T> where T : new()
    {
        private readonly LinkedList<T> wl = new LinkedList<T>();

        private static readonly int itemCount = 7;
        public RoundRobinObjectEnumerator()
        {
            var start = wl.AddFirst(new T());
            var temp = start;
            for (int i = 0; i < itemCount - 1; i++)
            {
                temp = wl.AddAfter(temp, new T());
            }
            w = wl.First;
        }
        LinkedListNode<T> w;

        public T Next()
        {
            LinkedListNode<T> node;
            if (w == null)
            {
                w = wl.First;
                node = w;
            }
            else
            {
                node = w;
                w = w.Next;
            }

            return node.Value;
        }

        public IEnumerable<T> All()
        {
            return wl.AsEnumerable();
        }
    }
}
