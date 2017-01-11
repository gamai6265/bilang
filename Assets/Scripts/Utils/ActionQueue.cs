using System;
using System.Collections.Generic;

namespace Cloth3D {
    public sealed class ActionQueue : IActionQueue {
        public int CurActionNum {
            get {
                return m_Actions.Count;
            }
        }

        public void QueueActionWithDelegation(Delegate action, params object[] args) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, args);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action.DynamicInvoke(args); });
                LogSystem.Warn("QueueActionWithDelegation {0} use lambda expression, maybe out of memory.", action.Method.ToString());
            }
        }

        public void QueueAction(MyAction action) {
            m_Actions.Enqueue(action);
        }

        public void QueueAction<T1>(MyAction<T1> action, T1 t1) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction<T1>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction<T1> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1); });
                LogSystem.Warn("QueueAction {0}({1}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1);
            }
        }

        public void QueueAction<T1, T2>(MyAction<T1, T2> action, T1 t1, T2 t2) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction<T1, T2>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction<T1, T2> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2); });
                LogSystem.Warn("QueueAction {0}({1},{2}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2);
            }
        }

        public void QueueAction<T1, T2, T3>(MyAction<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction<T1, T2, T3>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction<T1, T2, T3> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2, t3);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2, t3); });
                LogSystem.Warn("QueueAction {0}({1},{2},{3}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2, t3);
            }
        }
        public void QueueAction<T1, T2, T3, T4>(MyAction<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction<T1, T2, T3, T4>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction<T1, T2, T3, T4> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2, t3, t4);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2, t3, t4); });
                LogSystem.Warn("QueueAction {0}({1},{2},{3},{4}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2, t3, t4);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5>(MyAction<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction<T1, T2, T3, T4, T5> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2, t3, t4, t5);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2, t3, t4, t5); });
                LogSystem.Warn("QueueAction {0}({1},{2},{3},{4},{5}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2, t3, t4, t5);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6>(MyAction<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction<T1, T2, T3, T4, T5, T6> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2, t3, t4, t5, t6);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2, t3, t4, t5, t6); });
                LogSystem.Warn("QueueAction {0}({1},{2},{3},{4},{5},{6}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2, t3, t4, t5, t6);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7>(MyAction<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2, t3, t4, t5, t6, t7);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2, t3, t4, t5, t6, t7); });
                LogSystem.Warn("QueueAction {0}({1},{2},{3},{4},{5},{6},{7}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2, t3, t4, t5, t6, t7);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2, t3, t4, t5, t6, t7, t8);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2, t3, t4, t5, t6, t7, t8); });
                LogSystem.Warn("QueueAction {0}({1},{2},{3},{4},{5},{6},{7},{8}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2, t3, t4, t5, t6, t7, t8);
            }
        }

        public void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2, t3, t4, t5, t6, t7, t8, t9);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2, t3, t4, t5, t6, t7, t8, t9); });
                LogSystem.Warn("QueueAction {0}({1},{2},{3},{4},{5},{6},{7},{8},{9}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2, t3, t4, t5, t6, t7, t8, t9);
            }
        }

        public void QueueAction<R>(MyFunc<R> action) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedFunc<R>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedFunc<R> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(); });
                LogSystem.Warn("QueueAction {0}() use lambda expression, maybe out of memory.", action.Method.ToString());
            }
        }

        public void QueueAction<T1, R>(MyFunc<T1, R> action, T1 t1) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedFunc<T1, R>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedFunc<T1, R> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1); });
                LogSystem.Warn("QueueAction {0}({1}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1);
            }
        }

        public void QueueAction<T1, T2, R>(MyFunc<T1, T2, R> action, T1 t1, T2 t2) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedFunc<T1, T2, R>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedFunc<T1, T2, R> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2); });
                LogSystem.Warn("QueueAction {0}({1},{2}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2);
            }
        }

        public void QueueAction<T1, T2, T3, R>(MyFunc<T1, T2, T3, R> action, T1 t1, T2 t2, T3 t3) {
            bool needUseLambda = true;
            ObjectPool<PoolAllocatedFunc<T1, T2, T3, R>> pool;
            m_ActionPools.GetOrNewData(out pool);
            if (null != pool) {
                PoolAllocatedFunc<T1, T2, T3, R> helper = pool.Alloc();
                if (null != helper) {
                    helper.SetPoolRecycleLock(m_LockForAsyncActionRun);
                    helper.Init(action, t1, t2, t3);
                    m_Actions.Enqueue(helper.Run);
                    needUseLambda = false;
                }
            }
            if (needUseLambda) {
                m_Actions.Enqueue(() => { action(t1, t2, t3); });
                LogSystem.Warn("QueueAction {0}({1},{2},{3}) use lambda expression, maybe out of memory.", action.Method.ToString(), t1, t2, t3);
            }
        }

        public MyAction DequeueAction() {
            MyAction action = null;
            if (m_Actions.Count > 0) {
                if (null != m_LockForAsyncActionRun) {
                    lock (m_LockForAsyncActionRun) {
                        if (m_Actions.Count > 0) {
                            action = m_Actions.Dequeue();
                        }
                    }
                } else {
                    action = m_Actions.Dequeue();
                }
            }
            return action;
        }

        public void HandleActions(int maxCount) {
            try {
                for (int i = 0; i < maxCount; ++i) {
                    if (m_Actions.Count > 0) {
                        MyAction action = null;
                        if (null != m_LockForAsyncActionRun) {
                            lock (m_LockForAsyncActionRun) {
                                if (m_Actions.Count > 0) {
                                    action = m_Actions.Dequeue();
                                }
                            }
                        } else {
                            action = m_Actions.Dequeue();
                        }
                        if (null != action)
                            action();
                    } else {
                        break;
                    }
                }
            } catch (Exception ex) {
                LogSystem.Error("ActionQueue.HandleActions throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        public void Reset() {
            m_Actions.Clear();
            m_ActionPools.Clear();
        }

        public void DebugPoolCount(MyAction<string> output) {
            m_ActionPools.Visit((object type, object pool) => {
                Type t = type as Type;
                if (null != t) {
                    object ret = t.InvokeMember("Count", System.Reflection.BindingFlags.GetProperty, null, pool, null);
                    if (null != ret) {
                        output(string.Format("ActionPool {0} buffered {1} objects", pool.GetType().ToString(), (int)ret));
                    }
                } else {
                    output(string.Format("ActionPool contain a null pool ({0}) ..", pool.GetType().ToString()));
                }
            });
        }

        public ActionQueue() {
        }

        internal ActionQueue(object lockObj) {
            m_LockForAsyncActionRun = lockObj;
        }

        private object m_LockForAsyncActionRun = null;
        private Queue<MyAction> m_Actions = new Queue<MyAction>();
        private TypedDataCollection m_ActionPools = new TypedDataCollection();
    }
}