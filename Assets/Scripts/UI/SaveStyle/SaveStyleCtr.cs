using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class SaveStyleCtr : IController,IDesignClothesListener {
        private SaveStyleViewPresenter _view;
        private INetworkMgr _network;
        private IDesignWorld _designWorld;
        private IDesignWorld _idesiWorld;
        private IUserCenter _userCenter;

        private string tempStr = "";
        private static string[] _codesetNameList ={ "QUARTER", "TEMPLET_STYLE"};
        private CodeSet[] _codesetList=new CodeSet[2];
        private bool[] _isCodeSetDone = new[] {false, false};

        private string _name = string.Empty;
        private string _idea = string.Empty;
        private string _tag = string.Empty;
        private string _seasons = string.Empty;
        private string _style = string.Empty;
        private string _price = string.Empty;

        public bool Init() {
            _network = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (null == _network) {
                 LogSystem.Error("network is null.");
                return false;
            }
            _network.QueryTigger(TriggerNames.DesignClothesTrigger).AddListener(this);
//            EventCenter.Instance.AddListener(EventId.ScreenShotDone, ScreenShotDone);
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("user center is null.");
                return false;
            }
            _designWorld=ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (_designWorld == null) {
              LogSystem.Error("_designWorld is null");
                return false;
            }
            EventCenter.Instance.AddListener<string>(EventId.SaveStylePrice, SetPrice);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<SaveStyleViewPresenter>(WindowId.SaveStyle);
                _view.ClickSave += OnClickSave;
                _view.ClickBack += OnClickBack;
                _view.ClickSeasons += OnClickSeasons;
                _view.ClickStyle += OnClickStyle;
                _view.ClickStyleBtn += OnClickStyleBtn;
            }
            GetCodeSet();
            UiManager.Instance.ShowWindow(WindowId.SaveStyle);
            GetShirtName();
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            if (zorder==0 || zorder == -1) {
                Hide();
                return true;
            }
            //TODO 选择季节和风格返回有问题
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            _price = string.Empty;
            UiManager.Instance.HideWindow(WindowId.SaveStyle);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.SaveStyle, "ui/SaveStyle"),
            };
        }

        void OnDestroy() {
            //            EventCenter.Instance.RemoveListener(EventId.ScreenShotDone, ScreenShotDone);
            EventCenter.Instance.RemoveListener<string>(EventId.SaveStylePrice, SetPrice);
            _isCodeSetDone = new[] { false, false, false, false };
        }

        private void OnClickStyleBtn(string codeName) {
            if (tempStr == "风格") {
                _view.SetStyleLabel(codeName);
                _style = _codesetList[1].GetName2Data()[codeName];
            } else {
                _view.SetSeasonsLabel(codeName);
                _seasons = _codesetList[0].GetName2Data()[codeName];
            }
            _view.PlayTweener(false);
        }
        
        private void OnClickStyle() {
            if (IsCodeSetDone()) {
                _view.DeleteAll();
                _view.SetLbExplain(StrConfigProvider.Instance.GetStr("ss_showstyles"));
                tempStr = StrConfigProvider.Instance.GetStr("ss_style");
                foreach (var item in _codesetList[1].GetData()) {
                    _view.AddPrefabs(item.Value, item.Key);
                }
                _view.PlayTweener(true);
            }
        }

        private void OnClickSeasons() {
            if (IsCodeSetDone()) {
                _view.DeleteAll();
                _view.SetLbExplain(StrConfigProvider.Instance.GetStr("ss_showseason"));
                tempStr = StrConfigProvider.Instance.GetStr("ss_season");
                foreach (var item in _codesetList[0].GetData()) {
                    _view.AddPrefabs(item.Value, item.Key);
                }
                _view.PlayTweener(true);
            }
        }

        private void OnClickBack() {
            OnBack(-1);
        }

        private void OnClickSave() {      
//            EventCenter.Instance.Broadcast(EventId.ScreenShot);
            CollectDesign();
        }

        private void SetPrice(string price) {
            _price = price;
        }

        private void CollectDesign() {
            List<PartInfo> partInfos = new List<PartInfo>();
            var modelId = _designWorld.GetModelId();

            JsonData collect = new JsonData();
            collect["modelId"] = modelId.ToString();
            collect["seson"] = _seasons;
            collect["style"] = _style;
            collect["tags"] = _view.GetTags();
            collect["name"] = _name;
            collect["memo"] = _view.GetMemo();
            collect["price"] = _price;
            var parts = collect["parts"] = new JsonData();
            partInfos = _designWorld.GetPartInfo();
            for (int i = 0; i < partInfos.Count; i++) {
                var partItem = new JsonData();
                partItem["partId"] = partInfos[i].PartId.ToString();
                partItem["mateId1"] = partInfos[i].Fabric1;
                partItem["mateId2"] = partInfos[i].Fabric2;
                partItem["mateRotate1"] = partInfos[i].Fabric1Rotation;
                partItem["mateRotate2"] = partInfos[i].Fabric2Rotation;
                parts.Add(partItem);
            }

            IList<KeyValuePair<string, byte[]>> files = new List<KeyValuePair<string, byte[]>>();
            byte[] bytes = null;
            _designWorld.GetScreenShot((texture2D, texture2D2) => {
                bytes = texture2D.EncodeToJPG();
            });
            files.Add(new KeyValuePair<string, byte[]>("image", bytes));
//            Debug.Log(collect.ToJson());
            _network.QuerySender<IDesignClothesSender>().SendSaveWishListReq(collect.ToJson(), files);
        }

//        private void ScreenShotDone() {
//            SaveShirtTemplet();
//        }

        private void GetShirtName() {
            var fabricInfo = _designWorld.GetMainFabbric();
            var model = ModelsTypeConfigProvider.Instance.GetDataById(_designWorld.GetModelId());
            _name = fabricInfo.FabricColorName + fabricInfo.FabricFigureName + fabricInfo.FabricTextureName + model.TypeName;
            _view.SetShirtName(_name);
        }

        private bool IsCodeSetDone() {
            bool result = true;
            for (int i = 0; i < _isCodeSetDone.Length; i++) {
                if (!_isCodeSetDone[i]) {
                    result = false;
                    break;
                }
            }
            return result;
        }
        private void OnGetCodeset(CodeSet codeset) {
            for (int i = 0; i < _codesetNameList.Length; i++) {
                if (_codesetNameList[i] == codeset.CodeName) {
                    _codesetList[i] = codeset;
                    _isCodeSetDone[i] = true;
                }
            }
        }

        private void GetCodeSet() {
            for (int i = 0; i < _codesetNameList.Length; i++) {
                CodeSetProvider.Instance.QueryCodeset(_codesetNameList[i], OnGetCodeset);
            }
        }

//        void SaveShirtTemplet() {
//            IList<KeyValuePair<string, byte[]>> files = new List<KeyValuePair<string, byte[]>>();
//            files.Add(new KeyValuePair<string, byte[]>("screenshot", ScreenShotControl.screenShotQian));
//        }

        public void OnGetWishListRsp(JsonData msg) {
        }

        public void OnDeleteShirtTempletRsp(JsonData msg) {
        }

        public void OnGetCollectDetailRsp(JsonData msg) {
        }

        public void OnSaveWishiListRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("successful")) {
                if ((bool) msg["successful"]) {
                    _view.ClearData();
                    Hide();
                    //                    var json = msg["data"];
                    //TODO 获取返回id json["id"]
                    UiControllerMgr.Instance.ShowTips("保存我的收藏成功");
                }
                else {
                    LogSystem.Error("req is failed.");
                }
            }
            else {
                LogSystem.Error("net work is failed.");
            }
        }

        public void OnGetModelSizeListRsp(JsonData msg) {
        }public void OnGetUserSizeListRsp(JsonData msg) {
        }public void OnSaveUserSizeRsp(JsonData msg) {
        }public void OnDeleteUserSizeRsp(JsonData msg) {
        }public void OnQueryFabricListRsq(JsonData msg) {
        }public void OnGetFabricDetailRsq(JsonData msg) {
        }public void OnCountProductInfoPriceRsq(JsonData msg) {
        }
    }
}