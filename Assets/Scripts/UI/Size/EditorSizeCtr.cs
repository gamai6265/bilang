using System.Collections.Generic;
using System.Linq;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class EditorSizeCtr : IController  {
        private IOrderCenter _orderCenter;
        private EditorSizeViewPresenter _view;
        private IUserCenter _userCenter;
        private IDesignWorld _designWorld;
        private string _modelId = string.Empty;
        private List<ModelPart> _lstParts = new List<ModelPart>();//衣服部件列表
        private Dictionary<string, JsonData> _dicModelSize = new Dictionary<string, JsonData>();//模型部件默认尺寸
        private string _clothCode = string.Empty;
        private Dictionary<string, SizeInfo> _clothSize;


        public bool Init() {
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("error usercenter is null");
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
            EventCenter.Instance.AddListener<string, string>(EventId.ShowEditorSizeByModelId, GetModelSizeById);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<EditorSizeViewPresenter>(WindowId.EditorSize);
                _view.ClickAddSize += OnClickAddNewSize;
                _view.ClickBack += OnBackClick;
                _view.ClickChoose += OnChooseSize;
                _view.ClickDelete += OnDeleteSize;
                _view.ClickEditor += OnEditorSize;
                _view.ClickHistory += OnHistory;
            }
            _view.DeleteAllPrefabs();
            UiManager.Instance.ShowWindow(WindowId.EditorSize);
        }


        void GetModelSizeById(string modelId, string clothCode) {
            _modelId = modelId ;
            _clothCode = clothCode;
            _view.DeleteAllPrefabs();
            _designWorld.GetUserSize(_modelId, GetModelSizeCallBack);
            _orderCenter.GetShopCarDetail(_clothCode, GetShopCarDetail);
        }

        private void GetModelSizeCallBack(bool result,JsonData msg) {
            if (result) {                          
                SetupEditorSize(msg);
            }
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

        public void Hide(MyAction onComplete = null) {
            UiManager.Instance.HideWindow(WindowId.EditorSize);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.EditorSize, "ui/EditorSize"),
            };
        }

        private void SetupEditorSize(JsonData data) {   
            for (int i = 0; i < int.Parse(Utils.GetJsonStr(data["count"])); i++) {               
                var json = data["properties"][i];                
                string atid = Utils.GetJsonStr(json["atid"]);
                var lstUserSizeInfos = new List<SizeInfo>();
                for (int j = 0; j < json["sizeData"].Count; j++) {
                    var msg = json["sizeData"][j];
                    SizeInfo sizeInfo = new SizeInfo();                   
                    sizeInfo.Atid = Utils.GetJsonStr(msg["atid"]);
                    sizeInfo.SizeName = Utils.GetJsonStr(msg["propertiesName"]);
                    sizeInfo.SizeValue = sizeInfo.ShowValue = Utils.GetJsonStr(msg["propertiesValue"]);
                    sizeInfo.PropertiesType = Utils.GetJsonStr(msg["propertiesType"]);
                    sizeInfo.PropertiesCode = Utils.GetJsonStr(msg["propertiesCode"]);
                    sizeInfo.Description = Utils.GetJsonStr(msg["description"]);
                    sizeInfo.SortNo = Utils.GetJsonStr(msg["sortNo"]);
                    lstUserSizeInfos.Add(sizeInfo);
                }               
                _view.AddEditorSizePrefabs(atid, lstUserSizeInfos);
                _view.AdjustSizeItemDepth();
            }
        }

        private void OnClickAddNewSize() {
            foreach (var size in _orderCenter.DicSizeInfo) {
                size.Value.ShowValue = "-";
            }
            UiControllerMgr.Instance.GetController(WindowId.Measurement).Show();
            EventCenter.Instance.Broadcast(EventId.EditorSize, _modelId, string.Empty, _clothCode);
            Hide();
        }

        void OnBackClick() {
            OnBack(-1);
        }

        private void OnHistory() {
            UiControllerMgr.Instance.GetController(WindowId.HistorySize).Show();
            EventCenter.Instance.Broadcast(EventId.ShowHistorySize, _clothCode);
            Hide();
        }

        private void OnEditorSize(string atid, List<SizeInfo> sizeInfo) {
            _orderCenter.DicSizeInfo.Clear();
            foreach (var userSizeLst in sizeInfo) {
                if (!_orderCenter.DicSizeInfo.ContainsKey(userSizeLst.PropertiesCode))
                    _orderCenter.DicSizeInfo.Add(userSizeLst.PropertiesCode, userSizeLst);
            }
            foreach (var size in _orderCenter.DicSizeInfo) {
                foreach (var modelSizeItem in _dicModelSize) {
                    if (size.Key == Utils.GetJsonStr(modelSizeItem.Value["propertiesCode"])) {
                        size.Value.MinSize = int.Parse(Utils.GetJsonStr(modelSizeItem.Value["minValue"]));
                        size.Value.MaxSize = int.Parse(Utils.GetJsonStr(modelSizeItem.Value["maxValue"]));
                        size.Value.SizePicture = Utils.GetJsonStr(modelSizeItem.Value["image"]);
                        size.Value.unit = Utils.GetJsonStr(modelSizeItem.Value["unit"]);
                        break;
                    }
                }
            }
            UiControllerMgr.Instance.GetController(WindowId.Measurement).Show();
            EventCenter.Instance.Broadcast(EventId.EditorSize, _modelId, atid, _clothCode);
            Hide();
        }

        private void OnChooseSize(string atid, List<SizeInfo> sizeInfo) {
            //缺少尺寸=>添加修改尺寸
            if (!IsEqual(sizeInfo)) {
                foreach (var userSizeLst in sizeInfo) {//用户列表尺寸
                    foreach (var dicSizeInfo in _orderCenter.DicSizeInfo) {//模板尺寸
                        if (dicSizeInfo.Value.PropertiesCode == userSizeLst.PropertiesCode) {
                            dicSizeInfo.Value.Atid = userSizeLst.Atid;
                            dicSizeInfo.Value.SizeValue = dicSizeInfo.Value.ShowValue = userSizeLst.SizeValue;
                            dicSizeInfo.Value.SortNo = userSizeLst.SortNo;
                        }
                    }
                }
                UiControllerMgr.Instance.GetController(WindowId.Measurement).Show();
                EventCenter.Instance.Broadcast(EventId.EditorSize, _modelId, atid, _clothCode);
            } 
            //尺寸相当=>选择尺寸.
            else {
                _orderCenter.DicSizeInfo.Clear();
                foreach (var userSizeLst in sizeInfo) {
                    if (!_orderCenter.DicSizeInfo.ContainsKey(userSizeLst.PropertiesCode))
                        _orderCenter.DicSizeInfo.Add(userSizeLst.PropertiesCode, userSizeLst);
                }
                _clothSize=new Dictionary<string, SizeInfo>();
                _clothSize.Clear();
                foreach (var userSizeLst in sizeInfo) {
                    if (!_clothSize.ContainsKey(userSizeLst.PropertiesCode))
                        _clothSize.Add(userSizeLst.PropertiesCode, userSizeLst);
                }
                EventCenter.Instance.Broadcast(EventId.SetClothSize,_clothCode,_clothSize);
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

        private void GetShopCarDetail(bool result, JsonData json) {
            if (result) {
                _lstParts.Clear();
                for (var i = 0; i < json["goods"]["parts"].Count; i++) {
                    var goods = json["goods"]["parts"][i];
                    var part = ModelPartProvider.Instance.GetModelPart(int.Parse(Utils.GetJsonStr(goods["modelId"])),
                        int.Parse(Utils.GetJsonStr(goods["partId"])));
                    if (part != null) {
                        _lstParts.Add(part);
                    }
                }
                var userSize = _userCenter.UserInfo.UserSize;
                _userCenter.GetModelSizeList(string.Empty, _modelId, string.Empty, userSize.Height, userSize.Weight,
                    GetModelSizeListRsp);
            }
        }

        private void OnDeleteSize(string id) {
            UiControllerMgr.Instance.ShowMessageBox(
              "",
              "是否删除尺寸",
              "取消",
               () => {
               },
              "确定",
               () => {
                   _designWorld.DeleteUserSize(id, OnDeleteSizeCallBack);
               }
           );
        }

        private void OnDeleteSizeCallBack(bool result,JsonData msg) {
            if (result) {
                string atid = Utils.GetJsonStr(msg["ids"][0]);
                _view.DeletePrefabs(atid);
            }
        }

        private void GetModelSizeListRsp(bool result, JsonData msg) {
            if (result) {
                var properties = msg["properties"];
                for (var i = 0; i < properties.Count; i++) {
                    if (Utils.GetJsonStr(properties[i]["modelId"]) == _modelId) {
                        _orderCenter.DicSizeInfo.Clear();
                        for (var j = 0; j < _lstParts.Count; j++) {//衣服部件信息
                            var patrItem = _lstParts[j];
                            var sizeData = properties[i]["sizeData"];
                            for (var k = 0; k < sizeData.Count; k++) {//衣服模板尺寸
                                var sizeItem = sizeData[k];
                                var size = new SizeInfo();
                                var propertiesCode = Utils.GetJsonStr(sizeItem["propertiesCode"]);
                                if (!_dicModelSize.ContainsKey(propertiesCode))
                                    _dicModelSize.Add(propertiesCode, sizeItem);
                                var partLstModelSize = patrItem.LstModelSizes;
                                if (partLstModelSize != null) {
                                    for (var l = 0; l < partLstModelSize.Count; l++) {
                                        if (propertiesCode == partLstModelSize[l]) {
                                            size.Atid = string.Empty;
                                            size.SizeName = Utils.GetJsonStr(sizeItem["propertiesName"]);
                                            size.SizeValue = size.ShowValue = Utils.GetJsonStr(sizeItem["defaultValue"]);
                                            if (Utils.GetJsonStr(sizeItem["minValue"]).Length > 0)
                                                size.MinSize = int.Parse(Utils.GetJsonStr(sizeItem["minValue"]));
                                            else
                                                size.MinSize = 1;
                                            if (Utils.GetJsonStr(sizeItem["maxValue"]).Length > 0)
                                                size.MaxSize = int.Parse(Utils.GetJsonStr(sizeItem["maxValue"]));
                                            else
                                                size.MaxSize = 100;
                                            size.SizePicture = Utils.GetJsonStr(sizeItem["image"]);
                                            size.TestMeathod = Utils.GetJsonStr(sizeItem["description"]);
                                            size.unit = Utils.GetJsonStr(sizeItem["unit"]);
                                            size.PropertiesType = Utils.GetJsonStr(sizeItem["propertiesType"]);
                                            size.ModelId = Utils.GetJsonStr(sizeItem["modelId"]);
                                            size.PropertiesCode = propertiesCode;
                                            size.SortNo = Utils.GetJsonStr(sizeItem["sortNo"]);

                                            if (!_orderCenter.DicSizeInfo.ContainsKey(propertiesCode)) {
                                                _orderCenter.DicSizeInfo.Add(propertiesCode, size);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
