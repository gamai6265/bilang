using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class DesignersRecommendCtr : IController {
        private DesignersRecommendViewPresenter _view;
        private IOrderCenter _orderCenter;

        private string _type = string.Empty;
        private int _page;
        private int _maxPage;
        private int _showCount = 8;
        public bool Init() {
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            EventCenter.Instance.AddListener<string>(EventId.DesignersRecommendType, GetData);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<DesignersRecommendViewPresenter>(WindowId.DesignersRecommend);
                _view.ClickClose += OnCloseClick;
                _view.ClickChoose += OnChooseClick;
                _view.SvDragFinished += OnSvDragFinished;
            }
            UiManager.Instance.ShowWindow(WindowId.DesignersRecommend);
        }

        private void GetData(string type) {
            _page = 0;
            _type = string.Empty;
            _type = type;
            _view.DeleteAllRecommendItem();
            _view.SetTitle(_type);
            UiControllerMgr.Instance.ShowLodingTips(true);
            _orderCenter.GetDesignerProductListReq(_page.ToString(), _showCount.ToString(), _type, string.Empty, string.Empty, string.Empty, RecommendListRsp);
        }

        private void OnSvDragFinished() {
            if (_page < _maxPage) {
                UiControllerMgr.Instance.ShowLodingTips(true);
                _page++;
                _orderCenter.GetDesignerProductListReq(_page.ToString(), _showCount.ToString(), _type, string.Empty, string.Empty, string.Empty, RecommendListRsp);
            }
        }

        private void OnCloseClick() {
            OnBack(-1);
        }

        private void OnChooseClick(JsonData productJson) {
            UiControllerMgr.Instance.GetController(WindowId.DesignersRecommendPreview).Show();
            EventCenter.Instance.Broadcast(EventId.DesignersRecommendPreviewData, productJson);
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            Hide();
            return true;
        }

        public void Hide(MyAction onComplete = null) {
            UiManager.Instance.HideWindow(WindowId.DesignersRecommend);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.DesignersRecommend, "ui/DesignersRecommend"),
            };
        }

        private void RecommendListRsp(bool result, JsonData msg) {
            if (result) {
                var designerProductList = msg["designerProductList"];
                if (designerProductList.Count > 0) {
                    var _pageCount = designerProductList.Count;
                    _maxPage = (_pageCount / _showCount != 0) ? (_pageCount / _showCount) + 1 : (_pageCount / _showCount);
                    CoroutineMgr.Instance.StartCoroutine(GetRecommendStyle(designerProductList));
                }else {
                    _maxPage = 0;
                }
            }
        }

        IEnumerator GetRecommendStyle(JsonData data) {
            for (var i = 0; i < data.Count; i++) {
                var productInfo = data[i];
                var www = new WWW(Constants.KuanShiCanKao + Utils.GetJsonStr(productInfo["mainPic"]));
                yield return www;
                Texture2D texture2D = null;
                if (www.error == null) {
                    texture2D = www.texture;
                } else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    texture2D = www.texture;
                }
                www.Dispose();
                _view.AddRecommendPrefabs(productInfo, texture2D);
            }
            UiControllerMgr.Instance.ShowLodingTips(false);
        }
    }
}
