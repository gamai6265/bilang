using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class CollectCtr : IController, IDesignClothesListener{
        private CollectViewPresenter _view;
        private INetworkMgr _networkMgr;
        private ILoginMgr _loginMgr;
        private IUserCenter _userCenter;
        private IDesignWorld _designWorld;

        private string _collectId = string.Empty;
        public bool Init() {
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if(null == _networkMgr) {
                LogSystem.Error("net workmgr is null");
                return false;
            }
            _networkMgr.QueryTigger(TriggerNames.DesignClothesTrigger).AddListener(this);
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (null == _loginMgr) {
                LogSystem.Error("login mgr is null");
                return false;
            }
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (null == _userCenter) {
                LogSystem.Error("userCenter is null");
                return false;
            }
            _designWorld=ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (_designWorld == null) {
                LogSystem.Error("_designWorld is null");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<CollectViewPresenter>(WindowId.Collect);
                _view.ClickBack += OnBackClick;
                _view.ClickSure += OnDeleteClick;
                _view.ClickChoose += OnClickChoose;
            }
            UiManager.Instance.ShowWindow(WindowId.Collect);
            UiControllerMgr.Instance.ShowLodingTips(true);
            _networkMgr.QuerySender<IDesignClothesSender>().SendGetWishListReq();
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
            UiManager.Instance.HideWindow(WindowId.Collect);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Collect, "ui/Collect"),
            };
        }


        private void OnDeleteClick(string templetCode) {
            UiControllerMgr.Instance.ShowMessageBox(
                "",
                StrConfigProvider.Instance.GetStr("mb_deletecollect"),
               StrConfigProvider.Instance.GetStr("mb_cancle"),
                () => {
                },
                StrConfigProvider.Instance.GetStr("mb_confirm"),
                () => {
                    _networkMgr.QuerySender<IDesignClothesSender>().SendDeleteShirtTempletReq(templetCode);
                }
            );
        }

        private void OnClickChoose(string templetCode) {
            _collectId = templetCode;
            _networkMgr.QuerySender<IDesignClothesSender>().SendGetCollectDetailReq(templetCode);
        }
        
        void OnBackClick() {
            OnBack(-1);
        }

        IEnumerator IntCollectItem(JsonData jData) {
            _view.DeleteAllPrefabs();
            var wishList = jData["wishList"];
            for (int i = 0; i < wishList.Count; i++) {
                var www = new WWW(Constants.KuanShiCanKao + Utils.GetJsonStr(wishList[i]["picture"]));
                yield return www;
                Texture2D texture2D;
                if (www.error == null) {
                    texture2D = www.texture;
                }
                else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    texture2D = www.texture;
                }
                www.Dispose();
                var code = Utils.GetJsonStr(wishList[i]["atid"]);
                var name = Utils.GetJsonStr(wishList[i]["name"]);
                var price = Utils.GetJsonStr(wishList[i]["price"]);
                var time = Utils.GetJsonStr(wishList[i]["crdt"]);
                _view.AddPrefabs(code, name, price, time, texture2D);
                _view.AdjustItem();

            }
            UiControllerMgr.Instance.ShowLodingTips(false);
        }

        public void OnGetWishListRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    CoroutineMgr.Instance.StartCoroutine(IntCollectItem(msg["data"]));
                } else {
                    LogSystem.Error("req is failed.");
                }
            } else {
                LogSystem.Error("net work is failed.");
            }
        }

        public void OnDeleteShirtTempletRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    var json = msg["data"]["ids"];
                    for (var i = 0; i < json.Count; i++) {
                        _view.DeletePrefabs(Utils.GetJsonStr(json[i]));
                    }
                } else {
                    LogSystem.Error("req return failed.");
                }
            } else {
                LogSystem.Error("net work requre failed.");
            }
        }
        public void OnGetCollectDetailRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {                   
                    UiManager.Instance.HideAllShownWindow();
                    Hide();
                    EventCenter.Instance.Broadcast(EventId.ClosePersonalCenter);                   
                    OnClickChooseBack(msg["data"]);
                } else {
                    LogSystem.Error("req return failed");
                }
            } else {
                LogSystem.Error("req return returns=0");
            }
        }

        private void OnClickChooseBack(JsonData msg) {
            UiControllerMgr.Instance.ShowLodingTips(true);
            JsonData wishList = msg["wishList"];
            for (var i = 0; i < wishList.Count; i++) {
                if (_collectId == Utils.GetJsonStr(wishList[i]["atid"])) {
                    var modelInfo = new SerialModelInfo();
                    modelInfo.LstPartInfo = new List<PartInfo>();
                    modelInfo.ModelTypeId = int.Parse(Utils.GetJsonStr(wishList[i]["modelId"]));

                    var lstPartInfo = SortPartsList(wishList[i]["parts"]);
                    for (var index = 0; index < lstPartInfo.Count; index++) {
                        var parInfo = new PartInfo();
                        parInfo.PartId = int.Parse(Utils.GetJsonStr(lstPartInfo[index]["partId"]));
                        parInfo.Fabric1 = Utils.GetJsonStr(lstPartInfo[index]["mateId1"]);
                        parInfo.Fabric2 = Utils.GetJsonStr(lstPartInfo[index]["mateId2"]);
                        parInfo.Fabric1Rotation = Utils.GetJsonStr(lstPartInfo[index]["mateRotate1"]);
                        parInfo.Fabric2Rotation = Utils.GetJsonStr(lstPartInfo[index]["mateRotate2"]);
                        modelInfo.LstPartInfo.Add(parInfo);
                    }
                    _designWorld.GetFabricDetail(modelInfo, (result) => {
                        if (result) {
                            _designWorld.InitDesignModel(modelInfo);
                            Constants.IsRecommend = true;
                        }
                        UiControllerMgr.Instance.ShowLodingTips(false);
                    });
                }
            }
        }

        private List<JsonData> SortPartsList(JsonData partsInfo) {// TODO LOW B的做法
            var lstPartsInfo = new List<JsonData>();
            for (var i = 0; i < partsInfo.Count; i++) {
                if (Utils.GetJsonStr(partsInfo[i]["partId"]) == "1273" || Utils.GetJsonStr(partsInfo[i]["partId"]) == "1251") {//前幅排第一
                    if(!lstPartsInfo.Contains(partsInfo[i]))
                        lstPartsInfo.Add(partsInfo[i]);
                    break;
                }
            }
            for (var i = 0; i < partsInfo.Count; i++) {
                if (Utils.GetJsonStr(partsInfo[i]["partId"]) == "1014" || Utils.GetJsonStr(partsInfo[i]["partId"]) == "1035") {//后幅
                    if (!lstPartsInfo.Contains(partsInfo[i]))
                        lstPartsInfo.Add(partsInfo[i]);
                    break;
                }
            }
            for (var i = 0; i < partsInfo.Count; i++) {
                if (!lstPartsInfo.Contains(partsInfo[i]))
                    lstPartsInfo.Add(partsInfo[i]);
            }
            return lstPartsInfo;
        }


        public void OnSaveWishiListRsp(JsonData msg) {
        }public void OnGetModelSizeListRsp(JsonData msg) {
        }public void OnGetUserSizeListRsp(JsonData msg) {
        }public void OnSaveUserSizeRsp(JsonData msg) {
        }public void OnDeleteUserSizeRsp(JsonData msg) {
        }public void OnQueryFabricListRsq(JsonData msg) {
        }public void OnGetFabricDetailRsq(JsonData msg) {
        }public void OnCountProductInfoPriceRsq(JsonData msg) {
        }
    }
}
