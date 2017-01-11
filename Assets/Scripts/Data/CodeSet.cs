using System.Collections;
using System.Collections.Generic;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Data {
    public delegate void CodeSetUpdate(CodeSet codeSet);
    public class CodeSet {
        private Dictionary<string, string> _dicData = new Dictionary<string, string>();
        private Dictionary<string, string> _dicName2Data = new Dictionary<string, string>();
        public string CodeName;
        public IDictionary<string, string> GetData() {
            return _dicData;
        }
        public IDictionary<string, string> GetName2Data() {
            return _dicName2Data;
        }
        public void Parse(JsonData data) {
            CodeName = data[0]["codeset"].ToString();
            for (int i = 0; i < data.Count; i++) {
                _dicData.Add(data[i]["codeValue"].ToString(), data[i]["codeName"].ToString());
                _dicName2Data.Add(data[i]["codeName"].ToString(), data[i]["codeValue"].ToString());
            }
        }
    }

    public class CodeSetProvider : Singleton<CodeSetProvider> , ICodeSetListener{
        private Dictionary<string, CodeSet> _dicCodesets = new Dictionary<string, CodeSet>();
        private List<KeyValuePair<string, MyAction<CodeSet>>>  _lstActions = new List<KeyValuePair<string, MyAction<CodeSet>>>();
        public void Init() {
            var network = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (null != network) {
                network.QueryTigger(TriggerNames.CodeSetTrigger).AddListener(this);
            }
        }
        public void QueryCodeset(string name, MyAction<CodeSet> action) {
            if (_dicCodesets.ContainsKey(name)) {
                if (action != null)
                    action(_dicCodesets[name]);
            } else {
                var network = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
                if (null != network) {
                    _lstActions.Add(new KeyValuePair<string, MyAction<CodeSet>>(name, action));
                    network.QuerySender<ICodeSetSender>().SendGetCodeSetReq(name);
                } else {
                    LogSystem.Warn("CodeSetProvider network is null.");
                }
            }
        }

        private void NotifyAction(CodeSet codeSet) {
            List<int> removeList = new List<int>();
            for (int i = 0; i < _lstActions.Count; i++) {
                if (_lstActions[i].Key == codeSet.CodeName) {
                    _lstActions[i].Value(codeSet);
                    removeList.Add(i);
                }
            }
            for (int i = removeList.Count-1; i >=0; i--) {
                _lstActions.RemoveAt(i);
            }
            removeList.Clear();
        }
        public void OnGetCodesetRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("exit")) {
                if (msg["exit"].ToString() != "1") {
                    JsonData data = msg["data"];
                    if (data.Count > 0) {
                        var key = data[0]["codeset"].ToString();
                        var codeSet = new CodeSet();
                        if (_dicCodesets.ContainsKey(key)) {
                            _dicCodesets[key] = codeSet;
                        } else {
                            _dicCodesets.Add(key, codeSet);
                        }
                        codeSet.Parse(data);
                        //notify update
                        NotifyAction(codeSet);
                    }
                } else {
                    LogSystem.Error("OnGetCodesetRsp exit==1");
                }
            } else {
                LogSystem.Warn("OnGetCodesetRsp network require failed");
            }
        }
        public void OnGetPricePropertysRsp(JsonData msg) {
        }
    }
}
