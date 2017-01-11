using System;
using System.Collections.Generic;

namespace Cloth3D{
    public interface IPoolAllocatedObject<T> where T : IPoolAllocatedObject<T>, new() {
        void InitPool(ObjectPool<T> pool);
        T Downcast();
    }
    public class ObjectPool<T> where T : IPoolAllocatedObject<T>, new() {
        public void Init(int initSize) {
            for (var i = 0; i < initSize; ++i) {
                T t = new T();
                t.InitPool(this);
                _unusedObjects.Enqueue(t);
            }
        }
        public T Alloc() {
            if (_unusedObjects.Count > 0)
                return _unusedObjects.Dequeue();
            T t = new T();
            t.InitPool(this);
            return t;
        }
        public void Recycle(IPoolAllocatedObject<T> t) {
            if (null != t) {
                _unusedObjects.Enqueue(t.Downcast());
            }
        }
        public int Count {
            get {
                return _unusedObjects.Count;
            }
        }
        private readonly Queue<T> _unusedObjects = new Queue<T>();
    }
}
