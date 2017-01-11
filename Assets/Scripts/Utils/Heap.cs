﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloth3D{
    public sealed class Heap<ElementT> {
        public ElementT Root {
            get {
                ElementT t = default(ElementT);
                if (m_Tree.Count > 0)
                    t = m_Tree[0];
                return t;
            }
        }
        public int Count {
            get { return m_Tree.Count; }
        }
        public ElementT this[int index] {
            get {
                ElementT val = default(ElementT);
                if (index >= 0 && index < m_Tree.Count)
                    val = m_Tree[index];
                return val;
            }
        }
        public void Clear() {
            m_Tree.Clear();
        }
        public void Build(params ElementT[] vals) {
            m_Tree.Clear();
            m_Tree.AddRange(vals);
            int currentSize = Count;
            for (int i = currentSize / 2; i >= 1; --i) {
                ElementT val = m_Tree[i - 1];//子树的根
                                             //寻找放置y的位置
                int c = 2 * i;//c的父结点是y的目标位置
                while (c <= currentSize) {
                    //m_Head[c]应是较大的同胞结点
                    if (c < currentSize && m_Compare.Compare(m_Tree[c - 1], m_Tree[c]) < 0)
                        ++c;
                    if (m_Compare.Compare(val, m_Tree[c - 1]) >= 0)
                        break;
                    m_Tree[c / 2 - 1] = m_Tree[c - 1];//将孩子结点上移
                    c *= 2;//下移一层
                }
                m_Tree[c / 2 - 1] = val;
            }
        }
        public void Push(ElementT val) {
            m_Tree.Add(default(ElementT));
            int i = Count;
            while (i > 1 && m_Compare.Compare(m_Tree[i / 2 - 1], val) < 0) {
                m_Tree[i - 1] = m_Tree[i / 2 - 1];
                i /= 2;
            }
            m_Tree[i - 1] = val;
        }
        public ElementT Pop() {
            ElementT root = default(ElementT);
            int currentSize = Count;
            if (currentSize > 0) {
                root = m_Tree[0];
                ElementT last = m_Tree[currentSize - 1];//最后一个元素
                int i = 1;//堆的当前结点
                int ci = 2;//i的孩子结点
                while (ci <= currentSize) {
                    //m_Heap[ci]应是i的较大的孩子
                    if (ci < currentSize && m_Compare.Compare(m_Tree[ci - 1], m_Tree[ci]) < 0)
                        ++ci;
                    if (m_Compare.Compare(last, m_Tree[ci - 1]) >= 0)
                        break;
                    m_Tree[i - 1] = m_Tree[ci - 1];//将孩子结点上移
                    i = ci;
                    ci *= 2;//下移一层
                }
                m_Tree[i - 1] = last;
                m_Tree.RemoveAt(currentSize - 1);
            }
            return root;
        }
        public int IndexOf(ElementT val) {
            return m_Tree.IndexOf(val);
        }
        public void Update(int index, ElementT val) {
            int currentSize = Count;
            if (index >= 0 && index < currentSize) {
                //先删除
                ElementT last = m_Tree[currentSize - 1];//最后一个元素
                int i = index + 1;//要修改的堆的当前结点
                int ci = i * 2;//i的孩子结点
                while (ci <= currentSize) {
                    //m_Heap[ci]应是i的较大的孩子
                    if (ci < currentSize && m_Compare.Compare(m_Tree[ci - 1], m_Tree[ci]) < 0)
                        ++ci;
                    if (m_Compare.Compare(last, m_Tree[ci - 1]) >= 0)
                        break;
                    m_Tree[i - 1] = m_Tree[ci - 1];//将孩子结点上移
                    i = ci;
                    ci *= 2;//下移一层
                }
                m_Tree[i - 1] = last;
                //再添加
                i = currentSize;
                while (i > 1 && m_Compare.Compare(m_Tree[i / 2 - 1], val) < 0) {
                    m_Tree[i - 1] = m_Tree[i / 2 - 1];
                    i /= 2;
                }
                m_Tree[i - 1] = val;
            }
        }
        public Heap() {
            Init(null);
        }
        public Heap(IComparer<ElementT> comparer) {
            Init(comparer);
        }
        private void Init(IComparer<ElementT> comparer) {
            if (null == comparer) {
                m_Compare = Comparer<ElementT>.Default;
            } else {
                m_Compare = comparer;
            }
        }

        private List<ElementT> m_Tree = new List<ElementT>();
        private IComparer<ElementT> m_Compare = null;
    }
    public sealed class DefaultReverseComparer<T> : IComparer<T> {
        public int Compare(T x, T y) {
            if (x == null) {
                return (y != null) ? 1 : 0;
            }
            if (y == null) {
                return -1;
            }
            if (x is IComparable<T>) {
                return -((IComparable<T>)((object)x)).CompareTo(y);
            }
            if (x is IComparable) {
                return -((IComparable)((object)x)).CompareTo(y);
            }
            throw new ArgumentException("does not implement right interface");
        }
    }
}
