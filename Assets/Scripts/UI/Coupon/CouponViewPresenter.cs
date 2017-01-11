using UnityEngine;
using System.Collections;
using Cloth3D.Data;
using LitJson;

namespace Cloth3D.Ui {
    public class CouponViewPresenter : ViewPresenter {
        public UILabel LblTitle;
        public UILabel LblExplain;//TODO 国际化

        public UIButton BtnBack;
        public UIScrollView CouponScrollView;
        public UIGrid CouponGrid;
        public GameObject CouponItemPrefabs;

        public event MyAction ClickBack;
        public event MyAction<string, string, string> ClickChoose;

        protected override void AwakeUnityMsg() {
        }

        protected override void StartUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
        }
        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        public void AddPrefabs(JsonData jsonData, Texture2D texture2D) {
            var obj = NGUITools.AddChild(CouponGrid.gameObject, CouponItemPrefabs);

            var cip = obj.GetComponent<CouponItemPrefab>();
            obj.name = cip.CouponId = Utils.GetJsonStr(jsonData["id"]);
//            cip.LblTitle.text = Utils.GetJsonStr(jsonData["title"]);
//            cip.LblPrice.text = Utils.GetJsonStr(jsonData["price"]);
            cip.CouponPrice = Utils.GetJsonStr(jsonData["couponPrice"]);
            cip.Price = Utils.GetJsonStr(jsonData["price"]);
//            cip.LblStart.text = Utils.GetJsonStr(jsonData["startTime"]);
//            cip.LblEnd.text = Utils.GetJsonStr(jsonData["endTime"]);
            cip.TexCoupon.mainTexture = texture2D;
            cip.ClickChoose += OnClickChoose;

            CouponGrid.Reposition();
            CouponGrid.repositionNow = true;
            NGUITools.SetDirty(CouponGrid);

            var depth = CouponScrollView.GetComponentInChildren<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(CouponScrollView.gameObject, depth + 1);
            GetSortedPanels(true);
        }

        public void DeleteAll() {
            CouponGrid.transform.DestroyChildren();
        }

        private void OnClickChoose(string couponid, string couponPrice, string title) {
            if (null != ClickChoose) {
                ClickChoose(couponid, couponPrice, title);
            }
        }
    }
}
