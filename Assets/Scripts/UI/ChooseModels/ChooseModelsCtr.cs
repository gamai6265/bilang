using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class ChooseModelsCtr : IController, IInfoListener {
        private ChooseModelsViewPresenter _view;
        private ILogicModule _logic;
        private IDesignWorld _designWorld;
        private INetworkMgr _networkMgr;

        public bool Init() {
            _logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (_logic == null) {
                LogSystem.Error("erro. logic module is null.");
                return false;
            }
            _designWorld = ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (_designWorld == null) {
                LogSystem.Error("erro. _designWorld module is null.");
                return false;
            }
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr == null) {
                LogSystem.Error("network is null.");
                return false;
            }
            _networkMgr.QueryTigger(TriggerNames.InfoTrigger).AddListener(this);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<ChooseModelsViewPresenter>(WindowId.ChooseModels);
                _view.ClickBack += OnClickBack;
                _view.ClickChoose += OnClickChoose;
            }
            UiManager.Instance.ShowWindow(WindowId.ChooseModels);
            CoroutineMgr.Instance.StartCoroutine(GetModelList());
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

        private void OnClickBack() {
            OnBack(-1);
        }

        private void OnClickChoose(int modelTypeId) {
            //var modelInfo = new ModelInfo();
            //modelInfo.modelTypeId = modelTypeId;
            //_designWorld.InitDesignModel(modelInfo);
            //Hide();
            //return;
            GetModelInfo(modelTypeId);
        }

        private IEnumerator GetModelList() {
//            var modesData = ModelsConfigProvider.Instance.GetData();
            var modesData = ModelsTypeConfigProvider.Instance.GetData();
            foreach (var item in modesData) {
                var www = new WWW(Constants.YangShiTuPian + item.Value.SampleImage); //TODO danie cache
                yield return www;
                Texture2D texture = null;
                if (www.error == null) {
                    texture = www.texture;
                }
                else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    texture = www.texture;
                }
                www.Dispose();
                _view.AddPrefabs(item.Value.Id, item.Value.TypeName, texture);
            }
        }

        public void Hide(MyAction onComplete = null) {
            _view.DeleteAll();
            UiControllerMgr.Instance.GetController(WindowId.Home).Show();
            UiManager.Instance.HideWindow(WindowId.ChooseModels);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.ChooseModels, "ui/ChooseModels"),
            };
        }

        private void GetModelInfo(int modelId) {
            _networkMgr.QuerySender<IInfoSender>().SendGetDefaultProductInfo(modelId.ToString());
        }

        public void OnGetNoticeListRsp(JsonData msg) {
        }

        public void OnFashionListSearchRsp(JsonData msg) {
        }

        public void OnFashionDetailRsp(JsonData msg) {
        }

        public void OnGetRecommendListRsp(JsonData msg) {
        }

        public void OnGetRecommendDetailRsp(JsonData msg) {
        }

        public void OnGetDefaultProductInfoRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    GetModelInfoCallBack(msg["data"]);
                } else {
                    LogSystem.Error("GetDefaultProductInfo req return returns==0");
                }
            } else {
                LogSystem.Error("GetDefaultProductInfo req network failed.");
            }
        }

        private void GetModelInfoCallBack(JsonData msg) {
            /*
            var bb = new List<PartInfo>();
            bb.Add(new PartInfo() {
                PartId = 1001,
    });
            var xx = new SerialModelInfo {
                LstPartInfo =bb,
                ModelTypeId = 1,//int.Parse(Utils.GetJsonStr(msg["modelId"]))
            };
            _designWorld.InitDesignModel(xx);
            return;
            //*/
            UiControllerMgr.Instance.ShowLodingTips(true);
            var modelInfo = new SerialModelInfo {
                LstPartInfo = new List<PartInfo>(),
                ModelTypeId = int.Parse(Utils.GetJsonStr(msg["modelId"]))
            };
            var lstPartInfo = SortPartsList(msg["parts"]);
            for (int i = 0; i < lstPartInfo.Count; i++) {
                var parInfo = new PartInfo {
                    PartId = int.Parse(Utils.GetJsonStr(lstPartInfo[i]["partId"])),
                    Fabric1 = Utils.GetJsonStr(lstPartInfo[i]["mateId1"]),
                    Fabric2 = Utils.GetJsonStr(lstPartInfo[i]["mateId2"]),
                    Fabric1Rotation = Utils.GetJsonStr(lstPartInfo[i]["mateRotate1"]),
                    Fabric2Rotation = Utils.GetJsonStr(lstPartInfo[i]["mateRotate2"]),
                };
                modelInfo.LstPartInfo.Add(parInfo);
            }
            
            _designWorld.GetFabricDetail(modelInfo, (result) => {
                if (result) {                  
                    _designWorld.InitDesignModel(modelInfo);
                    Constants.IsRecommend = true;
                }
                UiControllerMgr.Instance.ShowLodingTips(false);
            });
            //            Hide();//TODO 简化时先注释掉
        }

        private List<JsonData> SortPartsList(JsonData partsInfo) {
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