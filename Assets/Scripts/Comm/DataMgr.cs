using System.Collections.Generic;
using UnityEngine;
namespace Cloth3D {
    public interface IDataMgr<TData> where TData : IData {
        bool Load(string file);
        TData GetDataById(int id);
        int GetDataCount();
        IDictionary<int, TData> GetData();
        void Clear();
    }

    public class DataMgr<TData> : IDataMgr<TData> where TData : IData, new() {
        protected readonly Dictionary<int, TData> DicData = new Dictionary<int, TData>();

        public bool Load(string file) {
            var result = true;

            var dbc = new Dbc();
            if (!dbc.Load(file)) {
                return false;
            }
            for (var index = 0; index < dbc.RowNum; index++) {
                var row = dbc.GetRowByIndex(index);
                if (row != null) {
                    var data = new TData();
                    var ret = data.CollectDataFromDbc(row);
                    if (ret) {
                        if (!DicData.ContainsKey(data.GetId())) {
                            DicData.Add(data.GetId(), data);
                            AddRecord(data.GetId(), data);
                        } else {
                            LogSystem.Error("erro, has the same key.{0}", file + data.GetId());
                        }
                    } else {
                        LogSystem.Error("DataMgr.CollectDataFromDbc collectData Row:{0} failed!", index);
                        result = false;
                    }
                } else {
                    LogSystem.Error("dbc GetRowByIndex is null. index:{0}", index);
                    result = false;
                }
            }

            return result;
        }


        public TData GetDataById(int id) {
            if (DicData.ContainsKey(id)) {
                return (TData) DicData[id];
            }

            return default(TData);
        }


        public int GetDataCount() {
            return DicData.Count;
        }


        public IDictionary<int, TData> GetData() {
            return DicData;
        }

        public virtual void Clear() {
            DicData.Clear();
        }

        protected virtual void AddRecord(int id, TData data) {
            
        }
    }

    public class SingletonDataMgr<TData> : Singleton<SingletonDataMgr<TData>>, IDataMgr<TData>
        where TData : IData, new() {
        private static IDataMgr<TData> _dataMgrImpl = new DataMgr<TData>();

        public void SetDataMgr(IDataMgr<TData> dataMgr) {
            _dataMgrImpl = dataMgr;
        }

        public void Clear() {
            _dataMgrImpl.Clear();
        }

        public bool Load(string file) {
            return _dataMgrImpl.Load(file);
        }

        public IDictionary<int, TData> GetData() {
            return _dataMgrImpl.GetData();
        }

        public TData GetDataById(int id) {
            return _dataMgrImpl.GetDataById(id);
        }

        public int GetDataCount() {
            return _dataMgrImpl.GetDataCount();
        }
    }
}