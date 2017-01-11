using System;
using System.Collections.Generic;
using Cloth3D.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cloth3D.Resource {
    public class ResourceMgr : IComponent {
        //已加载的预制件
        private readonly Dictionary<string, Object> _dicLoadedPrefabs = new Dictionary<string, Object>();

        //缓存的未激活的对象
        private readonly Dictionary<int, Queue<Object>> _dicUnusedObjects = new Dictionary<int, Queue<Object>>();
        //缓存的已激活的对象
        private readonly LinkedListDictionary<int, UsedObjectInfo> _dicUsedObjects =
            new LinkedListDictionary<int, UsedObjectInfo>();
        //预加载的预制件
        private readonly HashSet<int> _hashPreloadPrefabs = new HashSet<int>();

        private readonly ObjectPool<UsedObjectInfo> _usedObjectInfoPool =
            new ObjectPool<UsedObjectInfo>(); //使用对象信息的对象池

        private int _sceneId = -1;


        private float _lastTickTime = 0;


        public ResourceMgr() {
            _usedObjectInfoPool.Init(256);
        }

        public string Name { get; set; }

        public void Tick() {
            var curTime = Time.time;
            /*
            if (_lastTickTime <= 0) {
              _lastTickTime = curTime;
              return;
            }
            float delta = curTime - _lastTickTime;
            if (delta < 0.1f) {
              return;
            }
            _lastTickTime = curTime;
            */
            for (var node = _dicUsedObjects.FirstValue; null != node;) {
                var resInfo = node.Value;
                if (resInfo.RecycleTime > 0 && resInfo.RecycleTime < curTime) {
                    node = node.Next;
                    if (null != resInfo.Obj) {
                        _FinalizeObject(resInfo.Obj);
                        _PushToUnusedObjects(resInfo.ResId, resInfo.Obj);
                        _PopFromUsedObjects(resInfo.Id);
                        resInfo.Recycle();
                    }
                } else {
                    node = node.Next;
                }
            }
        }

        public bool Init() {
            return true;
        }

        //加载并实例化资源
        public void SpawnPreloadObjects(string res, int count) {
            var prefab = LoadPrefab(res);
            SpawnPreloadObjects(prefab, count);
        }

        internal void SpawnPreloadObjects(Object prefab, int count) {
            if (null != prefab) {
                if (!_hashPreloadPrefabs.Contains(prefab.GetInstanceID()))
                    _hashPreloadPrefabs.Add(prefab.GetInstanceID());
                for (var i = 0; i < count; ++i) {
                    var obj = Object.Instantiate(prefab);
                    _PushToUnusedObjects(prefab.GetInstanceID(), obj);
                }
            }
        }
        internal void PreloadPrefab(string res) {
            Object prefab = LoadPrefab(res);
            if (null != prefab) {
                if (!_hashPreloadPrefabs.Contains(prefab.GetInstanceID()))
                    _hashPreloadPrefabs.Add(prefab.GetInstanceID());
            }
        }

        //从未激活列表中取出物体，并激活
        internal Object NewObject(string res, float timeToRecycle) {
            var prefab = LoadPrefab(res);
            return NewObject(prefab, timeToRecycle);
        }

        internal Object NewObject(Object prefab, float timeToRecycle) {
            Object obj = null;
            if (null != prefab) {
                var curTime = Time.time;
                var time = timeToRecycle;
                if (timeToRecycle > 0)
                    time += curTime;
                var resId = prefab.GetInstanceID();
                obj = _PopFromUnusedObjects(resId);
                if (null == obj) {
                    obj = Object.Instantiate(prefab);
                }
                if (null != obj) {
                    _PushToUsedObjects(obj, resId, time);
                    _InitializeObject(obj);
                }
            }
            return obj;
        }

        //进入未激活状态，进入未使用列表
        internal bool RecycleObject(Object obj) {
            var ret = false;
            if (null != obj) {
                var gameObject = obj as GameObject;
                if (null != gameObject) {
                    //LogicSystem.LogicLog("RecycleObject {0} {1}", gameObject.name, gameObject.tag);
                }

                var objId = obj.GetInstanceID();
                if (_dicUsedObjects.Contains(objId)) {
                    var resInfo = _dicUsedObjects[objId];
                    if (null != resInfo) {
                        _FinalizeObject(resInfo.Obj);
                        _PopFromUsedObjects(objId);
                        _PushToUnusedObjects(resInfo.ResId, obj);
                        resInfo.Recycle();
                        ret = true;
                    }
                }
            }
            return ret;
        }

        public Object LoadPrefab(string res) {
            Object obj = null;
            if (string.IsNullOrEmpty(res)) {
                return null;
            }
            if (_dicLoadedPrefabs.ContainsKey(res)) {
                obj = _dicLoadedPrefabs[res];
            } else {
                if (GlobalVariables.IsPublish) {
                    obj = ResUpdateHandler.LoadAssetFromABWithoutExtention(res);
                }
                if (obj == null) {
                    obj = Resources.Load(res);
                }
                if (obj != null) {
                    _dicLoadedPrefabs.Add(res, obj);
                } else {
                    LogSystem.Error("LoadAsset failed:{0}",res);
                }
            }
            return obj;
        }

        //清除预加载列表和对象池
        internal void CleanupObjects() {
            for (var node = _dicUsedObjects.FirstValue; null != node;) {
                var resInfo = node.Value;
                if (!_hashPreloadPrefabs.Contains(resInfo.ResId)) {
                    node = node.Next;
                    _PopFromUsedObjects(resInfo.Id);
                    resInfo.Recycle();
                } else {
                    node = node.Next;
                }
            }

            foreach (var key in _dicUnusedObjects.Keys) {
                if (_hashPreloadPrefabs.Contains(key))
                    continue;
                var queue = _dicUnusedObjects[key];
                queue.Clear();
            }
            var waitDeleteLoadedPrefabEntrys = new List<string>();
            foreach (var key in _dicLoadedPrefabs.Keys) {
                var obj = _dicLoadedPrefabs[key];
                if (null != obj) {
                    int instId = obj.GetInstanceID();
                    if (!_hashPreloadPrefabs.Contains(instId)) {
                        waitDeleteLoadedPrefabEntrys.Add(key);
                    }
                } else {
                    waitDeleteLoadedPrefabEntrys.Add(key);
                }
            }
            foreach (var key in waitDeleteLoadedPrefabEntrys) {
                _dicLoadedPrefabs.Remove(key);
            }
            waitDeleteLoadedPrefabEntrys.Clear();

            Resources.UnloadUnusedAssets();
        }

        //取未激活对象
        private Object _PopFromUnusedObjects(int res) {
            Object obj = null;
            if (_dicUnusedObjects.ContainsKey(res)) {
                var queue = _dicUnusedObjects[res];
                if (queue.Count > 0)
                    obj = queue.Dequeue();
            }
            return obj;
        }

        private void _PushToUnusedObjects(int prefabId, Object obj) {
            if (_dicUnusedObjects.ContainsKey(prefabId)) {
                var queue = _dicUnusedObjects[prefabId];
                queue.Enqueue(obj);
            } else {
                var queue = new Queue<Object>();
                queue.Enqueue(obj);
                _dicUnusedObjects.Add(prefabId, queue);
            }
        }

        private void _PushToUsedObjects(Object obj, int resId, float recycleTime) {
            var objId = obj.GetInstanceID();
            if (!_dicUsedObjects.Contains(objId)) {
                var info = _usedObjectInfoPool.Alloc();
                info.Id = objId;
                info.Obj = obj;
                info.ResId = resId;
                info.RecycleTime = recycleTime;

                _dicUsedObjects.AddLast(objId, info);
            }
        }

        private void _PopFromUsedObjects(int objId) {
            _dicUsedObjects.Remove(objId);
        }

        //进入激活状态
        private void _InitializeObject(Object obj) {
            var gameObj = obj as GameObject;
            if (null != gameObj) {
                if (!gameObj.activeSelf)
                    gameObj.SetActive(true);
                /*ParticleSystem[] pss = gameObj.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in pss) {
                  if (null != ps && ps.playOnAwake) {
                    ps.Play();
                  }
                }*/
                var ps = gameObj.GetComponent<ParticleSystem>();
                if (null != ps && ps.playOnAwake) {
                    ps.Play();
                }
            }
        }

        //进入未激活状态
        private void _FinalizeObject(Object obj) {
            var gameObj = obj as GameObject;
            if (null != gameObj) {
                var ps0 = gameObj.GetComponent<ParticleSystem>();
                if (null != ps0 && ps0.playOnAwake) {
                    ps0.Stop();
                }
                var pss = gameObj.GetComponentsInChildren<ParticleSystem>();
                foreach (var ps in pss) {
                    if (null != ps) {
                        ps.Clear();
                    }
                }
                if (gameObj.transform != null && null != gameObj.transform.parent)
                    gameObj.transform.parent = null;
                if (gameObj.activeSelf)
                    gameObj.SetActive(false);
            }
        }

        private class UsedObjectInfo : IPoolAllocatedObject<UsedObjectInfo> {
            private ObjectPool<UsedObjectInfo> _pool;
            internal int Id;
            internal Object Obj;
            internal float RecycleTime;
            internal int ResId;

            public void InitPool(ObjectPool<UsedObjectInfo> pool) {
                _pool = pool;
            }

            public UsedObjectInfo Downcast() {
                return this;
            }

            internal void Recycle() {
                Obj = null;
                _pool.Recycle(this);
            }
        }
    }
}