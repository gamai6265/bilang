using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class RecommendCtr : IController,IInfoListener{
        private RecommendViewPresenter _view;
        private INetworkMgr _networkMgr;
        private IDesignWorld _designWorld;
        public bool Init() {
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr != null) {
                _networkMgr.QueryTigger(TriggerNames.InfoTrigger).AddListener(this);
            } else {
                LogSystem.Error("network is null.");
            }
            _designWorld=ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (_designWorld == null) {
               LogSystem.Error("error. _designWorld is null");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<RecommendViewPresenter>(WindowId.Recommend);
                _view.ClickBack += OnBackClick;
                _view.ClickChoose += OnChoose;
            }
            UiManager.Instance.ShowWindow(WindowId.Recommend);
            UiControllerMgr.Instance.ShowLodingTips(true);
            _networkMgr.QuerySender<IInfoSender>().SendGetRecommendListReq(0,0,null,"1"); //TODO 做分页
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
            UiControllerMgr.Instance.GetController(WindowId.Home).Show();
            UiManager.Instance.HideWindow(WindowId.Recommend);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Recommend, "ui/Recommend"),
            };
        }

        void OnDestroy() {
            _networkMgr.QueryTigger(TriggerNames.InfoTrigger).RemoveListener(this);
        }

        void OnBackClick() {
            OnBack(-1);
        }

        private void OnChoose(string atid) {
            _networkMgr.QuerySender<IInfoSender>().SendGetRecommendDetailReq(atid);
        }
        
        public void OnGetNoticeListRsp(JsonData msg) {
        }

        public void OnFashionListSearchRsp(JsonData msg) {
        }

        public void OnFashionDetailRsp(JsonData msg) {
        }
        public void OnGetDefaultProductInfoRsp(JsonData msg) {
        }

        public void OnGetRecommendListRsp(JsonData msg) {
//            IDictionary dic = msg;
//            if (dic.Contains("successful")) {
//                if (msg["successful"].IsBoolean) {
//                    _view.DeleteAll();
//                    CoroutineMgr.Instance.StartCoroutine(GetRecommendStyle(msg["data"]));
//                } else {
//                    LogSystem.Error("GetRecommendList req return returns==0");
//                }
//            } else {
//                LogSystem.Error("GetRecommendList req network failed.");
//            }
        }

//        IEnumerator GetRecommendStyle(JsonData data) {
//            for (int i = 0; i < int.Parse(Utils.GetJsonStr(data["count"])); i++) {              
//                var productInfo = data["productInfo"];
//                string templetCode = Utils.GetJsonStr(productInfo[i]["modelId"]);
//                string atid = Utils.GetJsonStr(productInfo[i]["atid"]);
//                WWW www = new WWW(Constants.KuanShiCanKao + Utils.GetJsonStr(productInfo[i]["picture"]));
//                yield return www;
//                Texture2D texture2D = www.texture;
//                www.Dispose();
//                _view.AddPrefabs(templetCode, atid, texture2D);
//            }
//            UiControllerMgr.Instance.ShowLodingTips(false);
//        }

        public void OnGetRecommendDetailRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    Hide();
                    OnChooseCallBack(msg["data"]);
                } else {
                    LogSystem.Error("GetRecommendDetail req return returns==0");
                }
            } else {
                LogSystem.Error("GetRecommendDetail req network failed.");
            }
        }
        
        private void OnChooseCallBack(JsonData msg) {
            UiControllerMgr.Instance.ShowLodingTips(true);
            var modelInfo = new SerialModelInfo();
            modelInfo.LstPartInfo = new List<PartInfo>();
            modelInfo.ModelTypeId = int.Parse(Utils.GetJsonStr(msg["modelId"]));
            
            var lstPartInfo = SortPartsList(msg["parts"]);
            for (int i = 0; i < lstPartInfo.Count; i++) {
                var parInfo = new PartInfo();
                parInfo.PartId = int.Parse(Utils.GetJsonStr(lstPartInfo[i]["partId"]));
                parInfo.Fabric1 = Utils.GetJsonStr(lstPartInfo[i]["mateId1"]);
                parInfo.Fabric2 = Utils.GetJsonStr(lstPartInfo[i]["mateId2"]);
                parInfo.Fabric1Rotation = Utils.GetJsonStr(lstPartInfo[i]["mateRotate1"]);
                parInfo.Fabric2Rotation = Utils.GetJsonStr(lstPartInfo[i]["mateRotate2"]);
                modelInfo.LstPartInfo.Add(parInfo);
            }
            _designWorld.GetFabricDetail(modelInfo, (result) => {
                if (result) {
                    _designWorld.InitDesignModel(modelInfo);
                    Constants.IsRecommend = false;
                }
                UiControllerMgr.Instance.ShowLodingTips(false);
            });
        }

        private List<JsonData> SortPartsList(JsonData partsInfo) {// TODO LOW B的做法
            var lstPartsInfo = new List<JsonData>();
            for (var i = 0; i < partsInfo.Count; i++) {
                if (Utils.GetJsonStr(partsInfo[i]["partId"]) == "1273" || Utils.GetJsonStr(partsInfo[i]["partId"]) == "1251") {//前幅
                    if (!lstPartsInfo.Contains(partsInfo[i]))
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
    }
}
