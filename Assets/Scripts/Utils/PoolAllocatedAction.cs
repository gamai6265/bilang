using System;
using System.Collections.Generic;

namespace Cloth3D{
    public sealed class PoolAllocatedAction : IPoolAllocatedObject<PoolAllocatedAction> {
        public void Init(Delegate action, params object[] args) {
            m_Action = action;
            m_Args = args;
        }
        public void Run() {
            m_Action.DynamicInvoke(m_Args);
            m_Action = null;
            m_Args = null;
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction> m_Pool;

        private Delegate m_Action;
        private object[] m_Args;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedAction<T1> : IPoolAllocatedObject<PoolAllocatedAction<T1>> {
        public void Init(MyAction<T1> action, T1 t1) {
            m_Action = action;
            m_T1 = t1;
        }
        public void Run() {
            m_Action(m_T1);
            m_Action = null;
            m_T1 = default(T1);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction<T1>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction<T1> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction<T1>> m_Pool;

        private MyAction<T1> m_Action;
        private T1 m_T1;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedAction<T1, T2> : IPoolAllocatedObject<PoolAllocatedAction<T1, T2>> {
        public void Init(MyAction<T1, T2> action, T1 t1, T2 t2) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
        }
        public void Run() {
            m_Action(m_T1, m_T2);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction<T1, T2>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction<T1, T2> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction<T1, T2>> m_Pool;

        private MyAction<T1, T2> m_Action;
        private T1 m_T1;
        private T2 m_T2;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedAction<T1, T2, T3> : IPoolAllocatedObject<PoolAllocatedAction<T1, T2, T3>> {
        public void Init(MyAction<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction<T1, T2, T3>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction<T1, T2, T3> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction<T1, T2, T3>> m_Pool;

        private MyAction<T1, T2, T3> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedAction<T1, T2, T3, T4> : IPoolAllocatedObject<PoolAllocatedAction<T1, T2, T3, T4>> {
        public void Init(MyAction<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction<T1, T2, T3, T4>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction<T1, T2, T3, T4> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction<T1, T2, T3, T4>> m_Pool;

        private MyAction<T1, T2, T3, T4> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedAction<T1, T2, T3, T4, T5> : IPoolAllocatedObject<PoolAllocatedAction<T1, T2, T3, T4, T5>> {
        public void Init(MyAction<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction<T1, T2, T3, T4, T5> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5>> m_Pool;

        private MyAction<T1, T2, T3, T4, T5> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedAction<T1, T2, T3, T4, T5, T6> : IPoolAllocatedObject<PoolAllocatedAction<T1, T2, T3, T4, T5, T6>> {
        public void Init(MyAction<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
            m_T6 = t6;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5, m_T6);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            m_T6 = default(T6);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction<T1, T2, T3, T4, T5, T6> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6>> m_Pool;

        private MyAction<T1, T2, T3, T4, T5, T6> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;
        private T6 m_T6;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7> : IPoolAllocatedObject<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7>> {
        public void Init(MyAction<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
            m_T6 = t6;
            m_T7 = t7;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5, m_T6, m_T7);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            m_T6 = default(T6);
            m_T7 = default(T7);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7>> m_Pool;

        private MyAction<T1, T2, T3, T4, T5, T6, T7> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;
        private T6 m_T6;
        private T7 m_T7;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8> : IPoolAllocatedObject<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8>> {
        public void Init(MyAction<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
            m_T6 = t6;
            m_T7 = t7;
            m_T8 = t8;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5, m_T6, m_T7, m_T8);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            m_T6 = default(T6);
            m_T7 = default(T7);
            m_T8 = default(T8);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8>> m_Pool;

        private MyAction<T1, T2, T3, T4, T5, T6, T7, T8> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;
        private T6 m_T6;
        private T7 m_T7;
        private T8 m_T8;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IPoolAllocatedObject<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>> {
        public void Init(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
            m_T6 = t6;
            m_T7 = t7;
            m_T8 = t8;
            m_T9 = t9;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5, m_T6, m_T7, m_T8, m_T9);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            m_T6 = default(T6);
            m_T7 = default(T7);
            m_T8 = default(T8);
            m_T9 = default(T9);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>> m_Pool;

        private MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;
        private T6 m_T6;
        private T7 m_T7;
        private T8 m_T8;
        private T9 m_T9;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<R> : IPoolAllocatedObject<PoolAllocatedFunc<R>> {
        public void Init(MyFunc<R> action) {
            m_Action = action;
        }
        public void Run() {
            m_Action();
            m_Action = null;
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<R>> m_Pool;

        private MyFunc<R> m_Action;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<T1, R> : IPoolAllocatedObject<PoolAllocatedFunc<T1, R>> {
        public void Init(MyFunc<T1, R> action, T1 t1) {
            m_Action = action;
            m_T1 = t1;
        }
        public void Run() {
            m_Action(m_T1);
            m_Action = null;
            m_T1 = default(T1);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<T1, R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<T1, R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<T1, R>> m_Pool;

        private MyFunc<T1, R> m_Action;
        private T1 m_T1;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<T1, T2, R> : IPoolAllocatedObject<PoolAllocatedFunc<T1, T2, R>> {
        public void Init(MyFunc<T1, T2, R> action, T1 t1, T2 t2) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
        }
        public void Run() {
            m_Action(m_T1, m_T2);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<T1, T2, R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<T1, T2, R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<T1, T2, R>> m_Pool;

        private MyFunc<T1, T2, R> m_Action;
        private T1 m_T1;
        private T2 m_T2;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<T1, T2, T3, R> : IPoolAllocatedObject<PoolAllocatedFunc<T1, T2, T3, R>> {
        public void Init(MyFunc<T1, T2, T3, R> action, T1 t1, T2 t2, T3 t3) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<T1, T2, T3, R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<T1, T2, T3, R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<T1, T2, T3, R>> m_Pool;

        private MyFunc<T1, T2, T3, R> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<T1, T2, T3, T4, R> : IPoolAllocatedObject<PoolAllocatedFunc<T1, T2, T3, T4, R>> {
        public void Init(MyFunc<T1, T2, T3, T4, R> action, T1 t1, T2 t2, T3 t3, T4 t4) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<T1, T2, T3, T4, R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, R>> m_Pool;

        private MyFunc<T1, T2, T3, T4, R> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<T1, T2, T3, T4, T5, R> : IPoolAllocatedObject<PoolAllocatedFunc<T1, T2, T3, T4, T5, R>> {
        public void Init(MyFunc<T1, T2, T3, T4, T5, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<T1, T2, T3, T4, T5, R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, R>> m_Pool;

        private MyFunc<T1, T2, T3, T4, T5, R> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, R> : IPoolAllocatedObject<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, R>> {
        public void Init(MyFunc<T1, T2, T3, T4, T5, T6, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
            m_T6 = t6;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5, m_T6);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            m_T6 = default(T6);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, R>> m_Pool;

        private MyFunc<T1, T2, T3, T4, T5, T6, R> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;
        private T6 m_T6;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, R> : IPoolAllocatedObject<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, R>> {
        public void Init(MyFunc<T1, T2, T3, T4, T5, T6, T7, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
            m_T6 = t6;
            m_T7 = t7;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5, m_T6, m_T7);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            m_T6 = default(T6);
            m_T7 = default(T7);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, R>> m_Pool;

        private MyFunc<T1, T2, T3, T4, T5, T6, T7, R> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;
        private T6 m_T6;
        private T7 m_T7;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, R> : IPoolAllocatedObject<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, R>> {
        public void Init(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
            m_T6 = t6;
            m_T7 = t7;
            m_T8 = t8;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5, m_T6, m_T7, m_T8);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            m_T6 = default(T6);
            m_T7 = default(T7);
            m_T8 = default(T8);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, R>> m_Pool;

        private MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, R> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;
        private T6 m_T6;
        private T7 m_T7;
        private T8 m_T8;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
    public sealed class PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> : IPoolAllocatedObject<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> {
        public void Init(MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) {
            m_Action = action;
            m_T1 = t1;
            m_T2 = t2;
            m_T3 = t3;
            m_T4 = t4;
            m_T5 = t5;
            m_T6 = t6;
            m_T7 = t7;
            m_T8 = t8;
            m_T9 = t9;
        }
        public void Run() {
            m_Action(m_T1, m_T2, m_T3, m_T4, m_T5, m_T6, m_T7, m_T8, m_T9);
            m_Action = null;
            m_T1 = default(T1);
            m_T2 = default(T2);
            m_T3 = default(T3);
            m_T4 = default(T4);
            m_T5 = default(T5);
            m_T6 = default(T6);
            m_T7 = default(T7);
            m_T8 = default(T8);
            m_T9 = default(T9);
            Recycle();
        }

        public void InitPool(ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> pool) {
            m_Pool = pool;
        }
        public PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> Downcast() {
            return this;
        }
        private ObjectPool<PoolAllocatedFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> m_Pool;

        private MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> m_Action;
        private T1 m_T1;
        private T2 m_T2;
        private T3 m_T3;
        private T4 m_T4;
        private T5 m_T5;
        private T6 m_T6;
        private T7 m_T7;
        private T8 m_T8;
        private T9 m_T9;

        public void SetPoolRecycleLock(object lockObj) {
            m_PoolLock = lockObj;
        }
        private void Recycle() {
            if (null != m_PoolLock) {
                lock (m_PoolLock) {
                    m_Pool.Recycle(this);
                }
            } else {
                m_Pool.Recycle(this);
            }
        }
        private object m_PoolLock;
    }
}
