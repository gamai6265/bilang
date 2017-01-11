using System.Collections;
using System.Collections.Generic;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class TrendyCtr : IController,IInfoListener {
        private TrendyViewPresenter _view;
        private INetworkMgr _networkMgr;
        public bool Init() {
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_networkMgr == null) {
                LogSystem.Error("network is null.");
                return false;
            }
            _networkMgr.QueryTigger(TriggerNames.InfoTrigger).AddListener(this);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<TrendyViewPresenter>(WindowId.Trendy);
                _view.ClickBack += OnBackClick;
                _view.ClickChoose += OnChoose;
            }
            UiManager.Instance.ShowWindow(WindowId.Trendy);
            UiControllerMgr.Instance.ShowLodingTips(true);
            _networkMgr.QuerySender<IInfoSender>().SendFashionListSearchReq();
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                _view.ResetScrollView();
                Hide();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiControllerMgr.Instance.GetController(WindowId.Home).Show();
            UiManager.Instance.HideWindow(WindowId.Trendy);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Trendy, "ui/Trendy"),
            };
        }

        void OnDestroy() {
            var network = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (network != null) {
                network.QueryTigger(TriggerNames.InfoTrigger).RemoveListener(this);
            }
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        public void OnChoose(string trendyItemId) {
            UiControllerMgr.Instance.GetController(WindowId.TrendyDetails).Show();
            EventCenter.Instance.Broadcast(EventId.ShowTrendyDetail, trendyItemId);
        }

        private IEnumerator GetTrendyList(JsonData data) {
            for (var i = 0; i < data.Count; i++) {
                var www = new WWW(Constants.ChaoLiuZiXun + data[i]["subject_img"]);
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
                _view.AddPrefabs(Utils.GetJsonStr(data[i]["id"]), texture, Utils.GetJsonStr(data[i]["subject_desc"]));
            }
            UiControllerMgr.Instance.ShowLodingTips(false);
        }

        public void OnGetNoticeListRsp(JsonData msg) {
        }

        public void OnFashionListSearchRsp(JsonData msg) {
//            IDictionary dic = msg;
//            if (dic.Contains("exit")) {
//                if (msg["exit"].ToString() != "1") {
//                    _view.DeleteAll();
//                    var data = msg["data"];
//                    CoroutineMgr.Instance.StartCoroutine(GetTrendyList(data));
//                } else {
//                    LogSystem.Error("OnFashionListSearchRsp return failed");
//                }
//            } else {
//                LogSystem.Error("network require failed.");
//            }
        }

        public void OnFashionDetailRsp(JsonData msg) {
        }

        public void OnGetRecommendListRsp(JsonData msg) {
        }

        public void OnGetRecommendDetailRsp(JsonData msg) {
        }

        public void OnGetDefaultProductInfoRsp(JsonData msg) {
        }
    }
}