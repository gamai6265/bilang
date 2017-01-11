using System.Collections.Generic;
using Cloth3D.Data;
using LitJson;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Cloth3D.Ui {
    public class DesignersRecommendSelectViewPresenter : ViewPresenter {
        public UIButton BtnHide;
        public UIButton BtnSelect;
        public GameObject NotSelect;
        public UIGrid HaveSelectGrid;
        public GameObject SelectDonePrefabs;

        public UIButton BtnConfirm;
        public UIButton BtnReset;

        public UIInput IpUpperLimit;
        public UIInput IpLowerLimit;

        public UIScrollView SelectScrollView;
        public UIGrid SelectGrid;
        public GameObject SelectListItem;
        public GameObject SelectPrefabs;

        public GameObject SelectPanel;
        public UIScrollView ListScrollView;
        public GameObject ListPanel;
        public UIGrid ListGrid;
        public GameObject ShirtPrefabs;
        public UIButton BtnClose;

        public event MyAction SvDragFinished;
        public event MyAction ClickHide;
        public event MyAction ClickSelect;
        public event MyAction<string, string> ClickConfirm;
        public event MyAction<bool, SelectInfo> ClickChoose;
        public event MyAction ClickReset;
        public event MyAction ClickClose;
        public event MyAction<SelectInfo> ClickDeleteChoose;
        public event MyAction<JsonData> ClickShirtPrefabs;

        protected override void AwakeUnityMsg() {
            ListScrollView.onDragFinished += OnSelectionScrollViewDragFinished;
            BtnHide.onClick.Add(new EventDelegate(OnBtnHideClick));
            BtnSelect.onClick.Add(new EventDelegate(OnBtnSelectClick));
            BtnConfirm.onClick.Add(new EventDelegate(OnBtnConfirmClick));
            BtnReset.onClick.Add(new EventDelegate(OnBtnResetClick));
            BtnClose.onClick.Add(new EventDelegate(OnBtnCloseClick));
        }

        private void OnSelectionScrollViewDragFinished() {
            var constraint = ListScrollView.panel.CalculateConstrainOffset(ListScrollView.bounds.min,
                ListScrollView.bounds.min);
            if (constraint.y < 1f) {
                if (null != SvDragFinished) {
                    SvDragFinished();
                }
            }
        }

        private void OnBtnHideClick() {
            if (null != ClickHide)
                ClickHide();
        }

        private void OnBtnCloseClick() {
            OnBtnResetClick();
            if (null != ClickClose)
                ClickClose();
        }

        private void OnBtnConfirmClick() {
            if (null != ClickConfirm) {
                if (IpUpperLimit.value != string.Empty && IpLowerLimit.value != string.Empty) {
                    if (Regex.IsMatch(IpUpperLimit.value, @"^[+-]?\d*[.]?\d*$") &&
                        Regex.IsMatch(IpLowerLimit.value, @"^[+-]?\d*[.]?\d*$")) {
                        if (float.Parse(IpUpperLimit.value) <= float.Parse(IpLowerLimit.value)) {
                            UiControllerMgr.Instance.ShowTips("价格区间有问题哦~");
                            return;
                        }
                    }
                    else {
                        UiControllerMgr.Instance.ShowTips("请输入正确的价钱格式");
                        return;
                    }
                }else {
                    if (IpUpperLimit.value != string.Empty && !Regex.IsMatch(IpUpperLimit.value, @"^[+-]?\d*[.]?\d*$")) {
                        UiControllerMgr.Instance.ShowTips("请输入正确的价钱格式");
                        return;
                    }
                    if (IpLowerLimit.value != string.Empty && !Regex.IsMatch(IpLowerLimit.value, @"^[+-]?\d*[.]?\d*$")) {
                        UiControllerMgr.Instance.ShowTips("请输入正确的价钱格式");
                        return;
                    }
                }
                ClickConfirm(IpUpperLimit.value, IpLowerLimit.value);
            }
        }

        private void OnBtnSelectClick() {
            if (null != ClickSelect)
                ClickSelect();
        }

        private void OnBtnResetClick() {
            IpUpperLimit.value = string.Empty;
            IpLowerLimit.value = string.Empty;
            var lstChildren = SelectGrid.transform.GetComponentsInChildren<SelectListItemCtr>(true);
            for (var i = 0; i < lstChildren.Length; i++) {
                var obj = lstChildren[i];
                var item = obj.ItemGrid.transform.GetComponentsInChildren<SelectPrefabsCtr>(true);
                for (var j = 0; j < item.Length; j++) {
                    item[j].SprSelect.spriteName = "button2";
                    item[j].LblText.color = new Color(0.2f, 0.2f, 0.2f, 1);
                }
            }
            if (null != ClickReset)
                ClickReset();
        }

        public void InstantiationOptionPrefabs(string type, List<SelectInfo> lstSelectInfos) {
            var obj = NGUITools.AddChild(SelectGrid.gameObject, SelectListItem);
            var sli = obj.GetComponent<SelectListItemCtr>();
            sli.LblLabel.text = type;
            SelectGrid.Reposition();
            SelectGrid.repositionNow = true;
            NGUITools.SetDirty(SelectGrid);

            for (var j = 0; j < lstSelectInfos.Count; j++) {
                var item = NGUITools.AddChild(sli.ItemGrid.gameObject, SelectPrefabs);
                var spc = item.GetComponent<SelectPrefabsCtr>();
                spc.LblText.text = lstSelectInfos[j].SelectValue;
                spc.SelectItemInfo = lstSelectInfos[j];
                spc.ClickChoose += OnClickChoose;
                sli.ItemGrid.Reposition();
                sli.ItemGrid.repositionNow = true;
                NGUITools.SetDirty(sli.ItemGrid);
                var depth = sli.ItemScrollView.GetComponentInChildren<UIPanel>().depth;
                NguiUtility.SetMinPanelDepth(sli.ItemScrollView.gameObject, depth + 1);
                GetSortedPanels(true);
            }
            var depth2 = SelectScrollView.GetComponentInChildren<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(SelectScrollView.gameObject, depth2 + 1);
            GetSortedPanels(true);
        }

        private void OnClickChoose(SelectInfo selectInfo) {
            var type = selectInfo.SelectType;
            var lstChildren = SelectGrid.transform.GetComponentsInChildren<SelectListItemCtr>(true);
            for (var i = 0; i < lstChildren.Length; i++) {
                var obj = lstChildren[i];
                if (type == obj.LblLabel.text) {
                    var value = selectInfo.SelectValue;
                    var item = obj.ItemGrid.transform.GetComponentsInChildren<SelectPrefabsCtr>(true);
                    for (var j = 0; j < item.Length; j++) {
                        if (value == item[j].LblText.text) {
                            if (item[j].SprSelect.spriteName != "btnsize") {
                                item[j].SprSelect.spriteName = "btnsize";
                                item[j].LblText.color = new Color(0.14f, 0.2f, 0.5f, 1);
                                if (null != ClickChoose)
                                    ClickChoose(true, selectInfo);
                            }else {
                                item[j].SprSelect.spriteName = "button2";
                                item[j].LblText.color = new Color(0.2f, 0.2f, 0.2f, 1);
                                if (null != ClickChoose)
                                    ClickChoose(false, selectInfo);
                            }
                        } else {
                            item[j].SprSelect.spriteName = "button2";
                            item[j].LblText.color = new Color(0.2f, 0.2f, 0.2f, 1);
                        }
                    }
                }
            }
        }
        
        public void EmptyGrid() {
            SelectGrid.transform.DestroyChildren();
            IpUpperLimit.value = string.Empty;
            IpLowerLimit.value = string.Empty;
            var springPanel = SelectScrollView.GetComponent<SpringPanel>();
            springPanel.target.y = 356.1f;
            springPanel.enabled = true;
        }

        public void ResetAppointGrid(SelectInfo selectInfo) {
            if (selectInfo.SelectType == PriceRange.upperLimit.ToString()) {
                IpUpperLimit.value = string.Empty;
            }else if (selectInfo.SelectType == PriceRange.lowerLimit.ToString()) {
                IpLowerLimit.value = string.Empty;
            }
            else {
                var type = selectInfo.SelectType;
                var lstChildren = SelectGrid.transform.GetComponentsInChildren<SelectListItemCtr>(true);
                for (var i = 0; i < lstChildren.Length; i++) {
                    var obj = lstChildren[i];
                    if (type == obj.LblLabel.text) {
                        var value = selectInfo.SelectValue;
                        var item = obj.ItemGrid.transform.GetComponentsInChildren<SelectPrefabsCtr>(true);
                        for (var j = 0; j < item.Length; j++) {
                            if (value == item[j].LblText.text) {
                                item[j].SprSelect.spriteName = "button2";
                                item[j].LblText.color = new Color(0.2f, 0.2f, 0.2f, 1);
                            }
                        }
                    }
                }
            }
        }

        public void InitAlreadySelect(Dictionary<string, SelectInfo> dic, bool isSelecting) {
            IsSelecting(isSelecting);
            SetListPanelActive(!isSelecting);
            HaveSelectGrid.transform.DestroyChildren();
            foreach (var item in dic) {
                var obj = NGUITools.AddChild(HaveSelectGrid.gameObject, SelectDonePrefabs);
                var sdpc = obj.GetComponent<SelectDonePrefabsCtr>();
                sdpc.LblText.text = item.Value.SelectValue;
                sdpc.SelectItemInfo = item.Value;

                sdpc.ClickChoose += OnDeleteChoose;
                RepositionSelectGrid();
            }
        }

        private void OnDeleteChoose(SelectInfo selectInfo) {
            var lstChildren = HaveSelectGrid.transform.GetComponentsInChildren<SelectDonePrefabsCtr>(true);
            for (var i = 0; i < lstChildren.Length; i++) {
                var obj = lstChildren[i];
                if (selectInfo.SelectValue == obj.LblText.text) {
                    Destroy(obj.gameObject);
                    RepositionSelectGrid();
                    if (null != ClickDeleteChoose)
                        ClickDeleteChoose(selectInfo);
                } 
            }
        }

        private void RepositionSelectGrid() {
            HaveSelectGrid.Reposition();
            HaveSelectGrid.repositionNow = true;
            NGUITools.SetDirty(HaveSelectGrid);
        }

        public void IsSelecting(bool isSelecting) {
            SelectPanel.SetActive(isSelecting);
            HaveSelectItem(isSelecting);
        }

        public void HaveSelectItem(bool isSelecting) {
            NotSelect.SetActive(isSelecting);
            HaveSelectGrid.gameObject.SetActive(!isSelecting);
        }

        public void SetListPanelActive(bool tof) {
            ListPanel.SetActive(tof);
        }

        public void InitShirtPrefabs(JsonData jsonData, Texture2D texture2D) {
            var obj = NGUITools.AddChild(ListGrid.gameObject, ShirtPrefabs);
            var rp = obj.GetComponent<RecommendPrefab>();
            rp.atid = Utils.GetJsonStr(jsonData["atid"]);
            rp.ProductJson = jsonData;
            rp.LblName.text = Utils.GetJsonStr(jsonData["name"]);
            var price = (Utils.GetJsonStr(jsonData["salesPrice"]) != string.Empty)
                ? Utils.GetJsonStr(jsonData["salesPrice"])
                : Utils.GetJsonStr(jsonData["originalPrice"]);
            rp.LblPrice.text = price;
            rp.TexRecommend.mainTexture = texture2D;
            UIEventListener.Get(obj).onClick += OnPrefabsClick;

            ListGrid.Reposition();
            ListGrid.repositionNow = true;
            NGUITools.SetDirty(ListGrid);
        }

        public void DeleteAllShirtItem() {
            ListGrid.transform.DestroyChildren();
        }

        private void OnPrefabsClick(GameObject go) {
            var rp = go.GetComponent<RecommendPrefab>();
            if (null != rp && null != ClickShirtPrefabs) {
                ClickShirtPrefabs(rp.ProductJson);
            }
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }
        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }
    }
}