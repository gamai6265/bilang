using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class HistorySizeCtr : IController,IUserInfoListener{
        private HistorySizeViewPresenter _view;
        private INetworkMgr _networkMgr;
        private IUserCenter _userCenter;
        private IDesignWorld _designWorld;
        private IOrderCenter _orderCenter;
        private string _modelId = string.Empty;
        private string _clothCode = string.Empty;
        private Dictionary<string, SizeInfo> _clothSize;

        public bool Init() {
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr == null) {
                LogSystem.Error("eror, network is null");
                return false;
            } 
            _networkMgr.QueryTigger(TriggerNames.UserInfoTrigger).AddListener(this);
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("usercenter is null");
                return false;
            }
            _designWorld = ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (_designWorld == null) {
                LogSystem.Error("_designWorld is null");
                return false;
            }
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            EventCenter.Instance.AddListener<string>(EventId.ShowHistorySize, SetClothCode);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<HistorySizeViewPresenter>(WindowId.HistorySize);
                _view.ClickBack += OnBackClick;
                _view.ClickChoose += OnChooseSize;
                _view.ClickSave += OnSave;
            }
            _view.DeleteAllPrefabs();
            var userInfo = _userCenter.UserInfo;
            if (userInfo != null) {
                _networkMgr.QuerySender<IUserInfoSender>().SendGetHistorySizeReq(userInfo.Tel);
            }
            UiManager.Instance.ShowWindow(WindowId.HistorySize);
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                Hide();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            _modelId = string.Empty;
            _clothCode = string.Empty;
            UiManager.Instance.HideWindow(WindowId.HistorySize);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.HistorySize, "ui/HistorySize"),
            };
        }

        void OnDestroy() {
            EventCenter.Instance.RemoveListener<string>(EventId.ShowHistorySize, SetClothCode);
        }

        private void SetClothCode(string clothCode) {
            _clothCode = clothCode;
        }

        private void SetupHistorySize(JsonData data) {
            if (data["properties"].Count > 0) {
                JsonData jsonData = data["properties"]["sizeData"];
                _modelId= Utils.GetJsonStr(data["properties"]["modelId"]);
                for (int i = 0; i < jsonData.Count; i++) {
                    var json = jsonData[i];
                    var lstUserSizeInfos = new List<SizeInfo>();
                    for (int j = 0; j < json.Count; j++) {
                        var msg = json[j];
                        SizeInfo sizeInfo = new SizeInfo();
                        sizeInfo.SizeName = Utils.GetJsonStr(msg["propertiesName"]);
                        sizeInfo.SizeValue = Utils.GetJsonStr(msg["defaultValue"]);
                        sizeInfo.PropertiesType = Utils.GetJsonStr(msg["propertiesType"]);
                        sizeInfo.PropertiesCode = Utils.GetJsonStr(msg["propertiesCode"]);
                        sizeInfo.SortNo = Utils.GetJsonStr(msg["sortNo"]);
                        lstUserSizeInfos.Add(sizeInfo);
                    }
                    _view.AddHistorySizePrefabs(lstUserSizeInfos);
                    _view.AdjustSizeItemDepth();
                }
            }
        }
        void OnBackClick() {
            OnBack(-1);
        }

        private void OnChooseSize(List<SizeInfo> sizeInfo) {
            //缺少尺寸=>添加修改尺寸
            if (!IsEqual(sizeInfo)) {
                foreach (var userSizeLst in sizeInfo) {//用户列表尺寸
                    foreach (var dicSizeInfo in _orderCenter.DicSizeInfo) {//模板尺寸
                        if (dicSizeInfo.Value.PropertiesCode == userSizeLst.PropertiesCode) {
                            dicSizeInfo.Value.Atid = userSizeLst.Atid;
                            dicSizeInfo.Value.SizeValue = userSizeLst.SizeValue;
                            dicSizeInfo.Value.SortNo = userSizeLst.SortNo;
                        }
                    }
                }
                UiControllerMgr.Instance.GetController(WindowId.Measurement).Show();
                EventCenter.Instance.Broadcast(EventId.EditorSize, _modelId, string.Empty, _clothCode);
            }
            //尺寸相当=>选择尺寸.
            else {
                _orderCenter.DicSizeInfo.Clear();
                foreach (var userSizeLst in sizeInfo) {
                    if (!_orderCenter.DicSizeInfo.ContainsKey(userSizeLst.PropertiesCode))
                        _orderCenter.DicSizeInfo.Add(userSizeLst.PropertiesCode, userSizeLst);
                }
                _clothSize = new Dictionary<string, SizeInfo>();
                _clothSize.Clear();
                foreach (var userSizeLst in sizeInfo) {
                    if (!_clothSize.ContainsKey(userSizeLst.PropertiesCode))
                        _clothSize.Add(userSizeLst.PropertiesCode, userSizeLst);
                }
                EventCenter.Instance.Broadcast(EventId.SetClothSize, _clothCode, _clothSize);
            }
            Hide();
        }

        private bool IsEqual(List<SizeInfo> sizeInfo) {
            List<string> ListA = new List<string>();
            foreach (var userSizeLst in sizeInfo) {
                if (!ListA.Contains(userSizeLst.PropertiesCode)) {
                    ListA.Add(userSizeLst.PropertiesCode);
                }
            }
            List<string> ListB = new List<string>();
            foreach (var dicSizeInfo in _orderCenter.DicSizeInfo) {
                if (!ListB.Contains(dicSizeInfo.Value.PropertiesCode)) {
                    ListB.Add(dicSizeInfo.Value.PropertiesCode);
                }
            }
            return ListB.All(b => ListA.Any(a => a.Equals(b)));
        }

        private void OnSave(List<SizeInfo> sizeInfos) {
            JsonData jD = new JsonData();
            foreach (var item in sizeInfos) {
                var sizeItem = new JsonData();
                sizeItem["propertiesName"] = item.SizeName;
                sizeItem["propertiesCode"] = item.PropertiesCode;
                sizeItem["propertiesValue"] = item.SizeValue;
                sizeItem["propertiesType"] = item.PropertiesType;
                sizeItem["sortNo"] = item.SortNo;
                jD.Add(sizeItem);
            }
            _designWorld.SaveModelSize(_modelId, string.Empty, jD.ToJson(), SaveModelSizeCallBack);
        }
        private void SaveModelSizeCallBack(bool result, JsonData msg) {
            if (result) {
                UiControllerMgr.Instance.GetController(WindowId.EditorSize).Show();
                EventCenter.Instance.Broadcast(EventId.ShowEditorSizeByModelId, _modelId, _clothCode);
                Hide();
            }
        }

        void IUserInfoListener.OnGetHistorySizeRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    SetupHistorySize(msg["data"]);
                } else {
                    LogSystem.Error("req return failed.");
                }
            } else {
                LogSystem.Error("network require failed.");
            }
        }

        void IUserInfoListener.OnModifyUserSizeRsp(JsonData msg) {
        }void IUserInfoListener.OnShareUrl(JsonData msg) {
        }void IUserInfoListener.OnGetCityListRsp(JsonData msg) {
        }void IUserInfoListener.OnGetAreaRsp(JsonData msg) {
        }public void OnAddOrUpdateModelSizeRsq(JsonData msg) {
        }void IUserInfoListener.OnQueryUserInfoRsp(JsonData msg) {
        }void IUserInfoListener.OnModifyUserInfoRsp(JsonData msg) {
        }void IUserInfoListener.OnGetAddressRsp(JsonData msg) {
        }void IUserInfoListener.OnDeleteAddressRsp(JsonData msg) {
        }void IUserInfoListener.OnSetDefaultAddressRsp(JsonData msg) {
        }void IUserInfoListener.OnModifyAddressRsp(JsonData msg) {
        }void IUserInfoListener.OnNewAddressRsp(JsonData msg) {
        }void IUserInfoListener.OnGetUserSizeRsp(JsonData msg) {
        }
    }
}