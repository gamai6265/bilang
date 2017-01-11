using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloth3D{
    public interface IActionQueue {
        int CurActionNum {
            get;
        }
        void QueueActionWithDelegation(Delegate action, params object[] args);
        void QueueAction(MyAction action);
        void QueueAction<T1>(MyAction<T1> action, T1 t1);
        void QueueAction<T1, T2>(MyAction<T1, T2> action, T1 t1, T2 t2);
        void QueueAction<T1, T2, T3>(MyAction<T1, T2, T3> action, T1 t1, T2 t2, T3 t3);
        void QueueAction<T1, T2, T3, T4>(MyAction<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4);
        void QueueAction<T1, T2, T3, T4, T5>(MyAction<T1, T2, T3, T4, T5> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
        void QueueAction<T1, T2, T3, T4, T5, T6>(MyAction<T1, T2, T3, T4, T5, T6> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
        void QueueAction<T1, T2, T3, T4, T5, T6, T7>(MyAction<T1, T2, T3, T4, T5, T6, T7> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
        void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);
        void QueueAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);
        void QueueAction<R>(MyFunc<R> action);
        void QueueAction<T1, R>(MyFunc<T1, R> action, T1 t1);
        void QueueAction<T1, T2, R>(MyFunc<T1, T2, R> action, T1 t1, T2 t2);
        void QueueAction<T1, T2, T3, R>(MyFunc<T1, T2, T3, R> action, T1 t1, T2 t2, T3 t3);
    }
}
