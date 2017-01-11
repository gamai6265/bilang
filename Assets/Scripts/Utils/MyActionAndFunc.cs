using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloth3D{
    public delegate void MyAction();
    public delegate void MyAction<T1>(T1 t1);
    public delegate void MyAction<T1, T2>(T1 t1, T2 t2);
    public delegate void MyAction<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
    public delegate void MyAction<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate void MyAction<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate void MyAction<T1, T2, T3, T4, T5, T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate void MyAction<T1, T2, T3, T4, T5, T6, T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    public delegate void MyAction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);
    public delegate void MyAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);
    public delegate R MyFunc<out R>();
    public delegate R MyFunc<T1, out R>(T1 t1);
    public delegate R MyFunc<T1, T2, out R>(T1 t1, T2 t2);
    public delegate R MyFunc<T1, T2, T3, out R>(T1 t1, T2 t2, T3 t3);
    public delegate R MyFunc<T1, T2, T3, T4, out R>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate R MyFunc<T1, T2, T3, T4, T5, out R>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate R MyFunc<T1, T2, T3, T4, T5, T6, out R>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate R MyFunc<T1, T2, T3, T4, T5, T6, T7, out R>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    public delegate R MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, out R>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);
    public delegate R MyFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, out R>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9);
}
