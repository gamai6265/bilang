using System.Collections.Generic;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {

    public class MeasurementCtr : IController {
        private MeasurementViewPresenter _view;
        private IDesignWorld _designWorld;
//        private INetworkMgr _networkMgr;
        private IUserCenter _userCenter;
        private IOrderCenter _orderCenter;
        private string _modelId = string.Empty;
        private string _atid = string.Empty;
        private string _clothCode = string.Empty; 
        private MysizeEntry[] _mysizeEntry = new[] { MysizeEntry.Height, MysizeEntry.Weight };
        public bool Init() {
//            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
//            if (null == _networkMgr) {
//                LogSystem.Error("net workmgr is null");
//                return false;
//            }
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
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("usercenter is null");
                return false;
            }
            EventCenter.Instance.AddListener<string, string, string>(EventId.EditorSize, SetData);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<MeasurementViewPresenter>(WindowId.Measurement);
                _view.ClickBack += OnBackClick;
                _view.ClickSave += OnSaveClick;
                _view.ClickAdaptation += OnAdaptationClick;
                _view.ClickService += OnServiceClick;
                _view.OnClickChooseSize += OnChooseClickSize;
            }
            UiManager.Instance.ShowWindow(WindowId.Measurement);
            LoadSizeData();
            LoadUserSize();
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                Hide();
                UiControllerMgr.Instance.GetController(WindowId.EditorSize).Show();
                EventCenter.Instance.Broadcast(EventId.ShowEditorSizeByModelId, _modelId, _clothCode);
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete = null) {
            _view.DeleteAllPrefabs();
            UiManager.Instance.HideWindow(WindowId.Measurement);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Measurement, "ui/Measurement"),
            };
        }

        void OnBackClick() {
            OnBack(-1);
        }

        private void SetData(string modelId, string atid, string clothCode) {
            _modelId = modelId;
            _atid = atid;
            _clothCode = clothCode;
        }

        private void OnSaveClick() {
            JsonData jD = new JsonData();
            foreach (var item in _orderCenter.DicSizeInfo) {
                if (item.Value.ShowValue == "-") {
                    UiControllerMgr.Instance.ShowTips("请完善尺寸");
                    return;
                }

                var sizeItem = new JsonData();
                if(_atid!=string.Empty)
                    sizeItem["atid"] = item.Value.Atid;
                sizeItem["propertiesName"] = item.Value.SizeName;
                sizeItem["propertiesCode"] = item.Value.PropertiesCode;
                sizeItem["propertiesValue"] = item.Value.SizeValue;
                sizeItem["propertiesType"] = item.Value.PropertiesType;
                sizeItem["description"] = item.Value.Description;
                sizeItem["sortNo"] = item.Value.SortNo;
                jD.Add(sizeItem);
            }
            _designWorld.SaveModelSize(_modelId, _atid, jD.ToJson(), SaveModelSizeCallBack);
        }

        private void SaveModelSizeCallBack(bool result, JsonData msg) {
            if (result) {
                UiControllerMgr.Instance.GetController(WindowId.EditorSize).Show();
                EventCenter.Instance.Broadcast(EventId.ShowEditorSizeByModelId, _modelId, _clothCode);
                Hide();
            }
        }

        private void LoadSizeData() {
            _view.DeleteAll();
            foreach (var sizeItem in _orderCenter.DicSizeInfo) {
                _view.AddPrefabs(sizeItem.Value);
            }
        }

        private void LoadUserSize() {
            var userSize = _userCenter.UserInfo.UserSize;
            string[] height = userSize.Height.Split(new char[] {'.'});
            string[] weight = userSize.Weight.Split(new char[] {'.'});
            _view.SetHightAndWeight(height[0], weight[0]);
            var userInfo = _userCenter.UserInfo;
            _view.SetUserInfo(userInfo.UserName, userInfo.Photo, userInfo.Head);
        }

        private void OnAdaptationClick(string hight, string weight) {
            string[] str = {hight, weight};
            for (var i = 0; i < _mysizeEntry.Length; i++) {
                _userCenter.ModifyUserSize(_mysizeEntry[i], str[i], (result) => {
                if (result) {
                    _userCenter.GetModelSizeList(string.Empty, _modelId, string.Empty, hight, weight, GetModelSizeListRsp);
                    }
                });
            }
        }

        private void GetModelSizeListRsp(bool result, JsonData msg) {
            if (result) {
                var properties = msg["properties"];
                for (var i = 0; i < properties.Count; i++) {
                    var sizeData = properties[i]["sizeData"];
                    for (var k = 0; k < sizeData.Count; k++) {
                        var sizeItem = sizeData[k];
                        foreach (var dicSizeItem in _orderCenter.DicSizeInfo) {
                            if (dicSizeItem.Key == Utils.GetJsonStr(sizeItem["propertiesCode"])) {
                                dicSizeItem.Value.SizeValue = dicSizeItem.Value.ShowValue = Utils.GetJsonStr(sizeItem["defaultValue"]);
                                dicSizeItem.Value.MinSize = int.Parse(Utils.GetJsonStr(sizeItem["minValue"]));
                                dicSizeItem.Value.MaxSize = int.Parse(Utils.GetJsonStr(sizeItem["maxValue"]));
                                dicSizeItem.Value.SizePicture = Utils.GetJsonStr(sizeItem["image"]);
                                dicSizeItem.Value.unit = Utils.GetJsonStr(sizeItem["unit"]);
                                break;
                            }
                        }
                    }
                }
                LoadSizeData();
            }
        }

        private
            void OnServiceClick() {
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (null != logic) {
                if (null != logic.OnBeforeStageEnter) {
                    logic.OnBeforeStageEnter(StageType.StageNone);
                }
                SDKContorl.Instance.CallPhone(logic.Config.GetSetting("Misc", "phone"));
            }
        }

        private void OnChooseClickSize(string propertiesCode) {
            EventCenter.Instance.Broadcast(EventId.Measurement, propertiesCode);
            Hide();
        }
    }
}
