using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

public enum PriceRange {
    upperLimit,
    lowerLimit,
}

namespace Cloth3D.Ui {
    public class DesignersRecommendSelectCtr : IController {
        private DesignersRecommendSelectViewPresenter _view;
        private IOrderCenter _orderCenter;

        private Dictionary<string, SelectInfo> _dicSelect = null;
        private bool _isSelecting = true;
        private string _Labels = string.Empty;
        private string _minPrice = string.Empty;
        private string _maxPrice = string.Empty;

        private int _page;
        private int _maxPage;
        private int _showCount = 8;
        public bool Init() {
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view =
                    UiManager.Instance.GetWindow<DesignersRecommendSelectViewPresenter>(WindowId.DesignersRecommendSelect);
                _view.SvDragFinished += OnSvDragFinished;
                _view.ClickHide += OnHideClick;
                _view.ClickSelect += OnSelectClick;
                _view.ClickConfirm += OnConfirmClick;
                _view.ClickChoose += OnChooseClick;
                _view.ClickReset += OnResetClick;
                _view.ClickClose += OnCloseClick;
                _view.ClickDeleteChoose += OnDeleteChooseClick;
                _view.ClickShirtPrefabs += OnShirtPrefabsClick;
            }
            UiManager.Instance.ShowWindow(WindowId.DesignersRecommendSelect);
            _dicSelect = new Dictionary<string, SelectInfo>();
            _view.IsSelecting(_isSelecting);
            GetData();
        }
        
        private void GetData() {
            _page = 0;
            _view.EmptyGrid();
            _orderCenter.GetLabelManageListReq("0", string.Empty, GetLabelManageListCallBack);
        }

        private void GetLabelManageListCallBack(bool result, JsonData msg) {
            if (result) {
                var data = msg["labelManagetList"];
                for (var i = 0; i < data.Count; i++) {
                    var type = Utils.GetJsonStr(data[i]["name"]);
                    var _lstSelectInfos = new List<SelectInfo>();
                    var detail = Utils.GetJsonStr(data[i]["detail"]);
                    string[] strs = detail.Split(new char[] { ',' });
                    for (var j = 0; j < strs.Length; j++) {
                        var selectInfo = new SelectInfo();
                        selectInfo.SelectType = type;
                        selectInfo.SelectValue = strs[j];
                        _lstSelectInfos.Add(selectInfo);
                    }
                    _view.InstantiationOptionPrefabs(type, _lstSelectInfos);
                }
            }
        }

        private void OnSvDragFinished() {
            if (_page < _maxPage) {
                UiControllerMgr.Instance.ShowLodingTips(true);
                _page++;
                _orderCenter.GetDesignerProductListReq(_page.ToString(), _showCount.ToString(), string.Empty, _Labels, _minPrice, _maxPrice, GetShirtDataCallback);
            }
        }

        private void OnCloseClick() {
            OnBack(-1);
        }

        private void OnDeleteChooseClick(SelectInfo selectInfo) {
            if (_dicSelect.ContainsKey(selectInfo.SelectType))
                _dicSelect.Remove(selectInfo.SelectType);
            _view.ResetAppointGrid(selectInfo);
            RequestShirtData();
        }

        private void OnShirtPrefabsClick(JsonData productJson) {
            UiControllerMgr.Instance.GetController(WindowId.DesignersRecommendPreview).Show();
            EventCenter.Instance.Broadcast(EventId.DesignersRecommendPreviewData, productJson);
        }

        private void OnChooseClick(bool isAdd, SelectInfo selectInfo) {
            if (isAdd) {
                if (!_dicSelect.ContainsKey(selectInfo.SelectType)) {
                    _dicSelect.Add(selectInfo.SelectType, selectInfo);
                }
                else {
                    _dicSelect[selectInfo.SelectType] = selectInfo;
                }
            }
            else {
                if (_dicSelect.ContainsKey(selectInfo.SelectType)) 
                    _dicSelect.Remove(selectInfo.SelectType);
            }
        }

        private void OnResetClick() {
            _dicSelect.Clear();
        }

        private void OnConfirmClick(string upperLimitStr, string lowerLimitStr) {
            if (upperLimitStr != string.Empty || lowerLimitStr != string.Empty || _dicSelect.Count != 0) {
                if (lowerLimitStr != string.Empty && _dicSelect.ContainsKey(PriceRange.lowerLimit.ToString())) {
                    var selectInfo = new SelectInfo();
                    selectInfo.SelectType = PriceRange.lowerLimit.ToString();
                    selectInfo.SelectValue = lowerLimitStr;
                    _dicSelect[PriceRange.lowerLimit.ToString()] = selectInfo;
                } else if (lowerLimitStr != string.Empty && !_dicSelect.ContainsKey(PriceRange.lowerLimit.ToString())) {
                    var selectInfo = new SelectInfo();
                    selectInfo.SelectType = PriceRange.lowerLimit.ToString();
                    selectInfo.SelectValue = lowerLimitStr;
                    _dicSelect.Add(PriceRange.lowerLimit.ToString(), selectInfo);
                }
                if (upperLimitStr != string.Empty && _dicSelect.ContainsKey(PriceRange.upperLimit.ToString())) {
                    var selectInfo = new SelectInfo();
                    selectInfo.SelectType = PriceRange.upperLimit.ToString();
                    selectInfo.SelectValue = upperLimitStr;
                    _dicSelect[PriceRange.upperLimit.ToString()] = selectInfo;
                } else if (upperLimitStr != string.Empty && !_dicSelect.ContainsKey(PriceRange.upperLimit.ToString())) {
                    var selectInfo = new SelectInfo();
                    selectInfo.SelectType = PriceRange.upperLimit.ToString();
                    selectInfo.SelectValue = upperLimitStr;
                    _dicSelect.Add(PriceRange.upperLimit.ToString(), selectInfo);
                }
                RequestShirtData();
            }
            else {
                UiControllerMgr.Instance.ShowTips("请至少选择一项选择项");
            }
        }

        private void OnHideClick() {
            if (_isSelecting) {
                OnBack(-1);
            }else {
                _view.IsSelecting(_isSelecting);
            }
        }

        private void OnSelectClick() {
            _view.IsSelecting(true);
        }

        private void RequestShirtData() {
            _page = 0;
            _isSelecting = false;
            _Labels = string.Empty;
            _view.InitAlreadySelect(_dicSelect, _isSelecting);
            var sb = new StringBuilder();
            _minPrice = string.Empty;
            _maxPrice = string.Empty;
            if (_dicSelect.Count == 0) {
                _view.HaveSelectItem(true);
            }else {
                var lstValue = new List<string>();
                foreach (var item in _dicSelect) {
                    if (item.Value.SelectType == PriceRange.upperLimit.ToString()) {
                        _maxPrice = item.Value.SelectValue;
                    }else if (item.Value.SelectType == PriceRange.lowerLimit.ToString()) {
                        _minPrice = item.Value.SelectValue;
                    }else {
                        lstValue.Add(item.Value.SelectValue);
                    }
                }
                for (var i = 0; i < lstValue.Count; i++) {
                    if (i != 0)
                        sb.Append(",");
                    sb.Append(lstValue[i]);
                }
            }
            _Labels = sb.ToString();
            UiControllerMgr.Instance.ShowLodingTips(true);
            _orderCenter.GetDesignerProductListReq(_page.ToString(), _showCount.ToString(), string.Empty, _Labels, _minPrice, _maxPrice, GetShirtDataCallback);
        }

        private void GetShirtDataCallback(bool result, JsonData msg) {
            if (result) {
                var designerProductList = msg["designerProductList"];
                if (designerProductList.Count > 0) {
                    var _pageCount = designerProductList.Count;
                    _maxPage = (_pageCount/_showCount != 0) ? (_pageCount/_showCount) + 1 : (_pageCount/_showCount);
                    _view.DeleteAllShirtItem();
                    CoroutineMgr.Instance.StartCoroutine(GetShirtData(designerProductList));
                }else {
                    _maxPage = 0;
                    _view.DeleteAllShirtItem();
                    UiControllerMgr.Instance.ShowTips("没有符合搜索条件的商品,请重新选择");
                    UiControllerMgr.Instance.ShowLodingTips(false);
                }
            }
        }

        IEnumerator GetShirtData(JsonData data) {
            for (var i = 0; i < data.Count; i++) {
                Texture2D texture2D = null;
                var item = data[i];
                var www = new WWW(Constants.KuanShiCanKao + Utils.GetJsonStr(item["mainPic"]));
                yield return www;
                if (www.error == null) {
                    texture2D = www.texture;
                } else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    texture2D = www.texture;
                }
                www.Dispose();
                _view.InitShirtPrefabs(item, texture2D);
            }
            UiControllerMgr.Instance.ShowLodingTips(false);
        }



        public void Update() {
        }

        public bool OnBack(int zorder) {
            _view.SetListPanelActive(false);
            _isSelecting = true;
            OnResetClick();
            Hide();
            return true;
        }

        public void Hide(MyAction onComplete = null) {
            UiManager.Instance.HideWindow(WindowId.DesignersRecommendSelect);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.DesignersRecommendSelect, "ui/DesignersRecommendSelect"),
            };
        }
    }
}