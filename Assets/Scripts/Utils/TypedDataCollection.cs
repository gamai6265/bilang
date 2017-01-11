using System;
using System.Collections;

namespace Cloth3D {
    /// <remarks>
    /// 这种方式主要用于数据层面不好抽象的场合，如果从业务角度能够对数据定义进行抽象，就尽量不要用这种动态的聚合数据的方式而尽量使用
    /// 静态类型的关联。
    /// 这个类仅用于管理若干不同类型的数据，如之前C++里用union管理不同数据的情形，使用限制：
    /// 1、这个集合里每种类型的数据只能有一个实例。
    /// 2、由于使用运行时类型信息及非强类型集合，运行效率较低。
    /// </remarks>
    public sealed class TypedDataCollection {
        public void GetOrNewData<T>(out T t) where T : new() {
            t = GetData<T>();
            if (null == t) { //TODO 这里不严谨，结构体不是null
                t = new T();
                AddData(t);
            }
        }
        public void AddData<T>(T data) {
            Type t = typeof(T);
            if (null != data) {
                if (_hashDatas.Contains(t)) {
                    _hashDatas[t] = data;
                } else {
                    _hashDatas.Add(t, data);
                }
            }
        }
        public void RemoveData<T>(T t) {
            RemoveData<T>();
        }
        public void RemoveData<T>() {
            Type t = typeof(T);
            if (_hashDatas.Contains(t)) {
                _hashDatas.Remove(t);
            }
        }
        public T GetData<T>() {
            T ret = default(T);
            Type t = typeof(T);
            if (_hashDatas.Contains(t)) {
                ret = (T)_hashDatas[t];
            }
            return ret;
        }
        public void Clear() {
            _hashDatas.Clear();
        }
        public void Visit(MyAction<object, object> visitor) {
            foreach (DictionaryEntry dict in _hashDatas) {
                visitor(dict.Key, dict.Value);
            }
        }
        private readonly Hashtable _hashDatas = new Hashtable();
    }
}
