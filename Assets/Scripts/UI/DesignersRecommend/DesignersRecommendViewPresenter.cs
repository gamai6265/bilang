using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class DesignersRecommendViewPresenter : ViewPresenter {
        public UIButton BtnClose;
        public UILabel LblTitle;

        public UIScrollView DRScrollView;
        public UIGrid DRGrid;
        public GameObject DRPrefabs;

        public event MyAction SvDragFinished;
        public event MyAction ClickClose;
        public event MyAction<JsonData> ClickChoose;

        protected override void AwakeUnityMsg() {
            DRScrollView.onDragFinished += OnSelectionScrollViewDragFinished;
            BtnClose.onClick.Add(new EventDelegate(OnBtnCloseClick));
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void OnSelectionScrollViewDragFinished() {
            var constraint = DRScrollView.panel.CalculateConstrainOffset(DRScrollView.bounds.min,
                DRScrollView.bounds.min);
            if (constraint.y < 1f) {
                if (null != SvDragFinished) {
                    SvDragFinished();
                }
            }
        }

        private void OnBtnCloseClick() {
            if (null != ClickClose) {
                ClickClose();
            }
        }

        public void SetTitle(string str) {
            LblTitle.text = str;
        }

        public void AddRecommendPrefabs(JsonData jsonData, Texture2D texture2D) {
            var obj = NGUITools.AddChild(DRGrid.gameObject, DRPrefabs);
            RecommendPrefab rp = obj.GetComponent<RecommendPrefab>();
            rp.atid = Utils.GetJsonStr(jsonData["atid"]);
            rp.ProductJson = jsonData;
            rp.LblName.text = Utils.GetJsonStr(jsonData["name"]);
            var price = (Utils.GetJsonStr(jsonData["salesPrice"]) != string.Empty)
                    ? Utils.GetJsonStr(jsonData["salesPrice"])
                    : Utils.GetJsonStr(jsonData["originalPrice"]);
            rp.LblPrice.text = price;
            rp.TexRecommend.mainTexture = texture2D;
            UIEventListener.Get(obj).onClick += OnPrefabsClick;

            DRGrid.Reposition();
            DRGrid.repositionNow = true;
            NGUITools.SetDirty(DRGrid);
        }

        public void DeleteAllRecommendItem() {
            DRGrid.transform.DestroyChildren();
            var springPanel = DRScrollView.GetComponent<SpringPanel>();
            springPanel.target.y = -66.87f;
            springPanel.enabled = true;
        }

        private void OnPrefabsClick(GameObject go) {
            var rp = go.GetComponent<RecommendPrefab>();
            if (null != rp && null != ClickChoose) {
                ClickChoose(rp.ProductJson);
            }
        }
    }
}
