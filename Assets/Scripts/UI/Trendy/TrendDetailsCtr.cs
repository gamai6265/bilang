using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class TrendDetailsCtr : IController,IInfoListener{
        private TrendyDetailsViewPresenter _view;
        private INetworkMgr _networkMgr;

        public bool Init() {
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (null != _networkMgr) {
                _networkMgr.QueryTigger(TriggerNames.InfoTrigger).AddListener(this);
            } else {
                LogSystem.Error("_networkMgr is null.");
            }
            EventCenter.Instance.AddListener<string>(EventId.ShowTrendyDetail, ShowTrendyDetail);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<TrendyDetailsViewPresenter>(WindowId.TrendyDetails);
                _view.ClickBack += OnBackClick;
            }
            UiManager.Instance.ShowWindow(WindowId.TrendyDetails);
            UiControllerMgr.Instance.ShowLodingTips(true);
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
            UiManager.Instance.HideWindow(WindowId.TrendyDetails);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.TrendyDetails, "ui/TrendyDetails"),
            };
        }

        void OnDestroy() {
            _networkMgr.QueryTigger(TriggerNames.InfoTrigger).RemoveListener(this);
            EventCenter.Instance.RemoveListener<string>(EventId.ShowTrendyDetail, ShowTrendyDetail);
        }
        private void OnBackClick() {
            OnBack(-1);
        }

        private IEnumerator GetTrendDetailsList(JsonData data) {
            for (var i = 0; i < data.Count; i++) {
                var www = new WWW(Constants.ChaoLiuZiXun + data[i]["item_img"]);
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
                _view.AddPrefabs(data[i]["description"].ToString(), texture);
                www.Dispose();
            }
            UiControllerMgr.Instance.ShowLodingTips(false);
        }

        public void OnGetNoticeListRsp(JsonData msg) {
        }

        public void OnFashionListSearchRsp(JsonData msg) {
        }

        public void OnFashionDetailRsp(JsonData msg) {
            LogSystem.Debug("OnFashionDetailRsp");
            IDictionary dic = msg;
            if (dic.Contains("exit")) {
                if (msg["exit"].ToString() != "1") {
                    var data = msg["data"];
                    CoroutineMgr.Instance.StartCoroutine(GetTrendDetailsList(data));
                } else {
                    LogSystem.Error("OnFashionDetailRsp return faild.");
                }
            } else {
                LogSystem.Error("OnFashionDetailRsp network faild.");
            }
        }

        public void OnGetRecommendListRsp(JsonData msg) {
        }

        public void OnGetRecommendDetailRsp(JsonData msg) {
        }

        public void OnGetDefaultProductInfoRsp(JsonData msg) {
        }

        private void ShowTrendyDetail(string id) {
            LogSystem.Debug("ShowTrendyDetail");
            _view.DeleteAllPrefabs();
            _networkMgr.QuerySender<IInfoSender>().SendFashionDetailReq(id);
        }
    }
}