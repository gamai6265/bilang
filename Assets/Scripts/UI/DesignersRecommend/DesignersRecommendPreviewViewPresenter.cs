using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class DesignersRecommendPreviewViewPresenter : ViewPresenter {
        public UIButton BtnClose;
        public UIButton BtnBuyNow;
        public UIButton BtnSaveShopCar;
        public UIButton BtnGoShopCar;

        public UIScrollView BannerScrollView;
        public UIGrid BannerGrid;
        public GameObject BannerItem;

        public UILabel LblShirtName;
        public UILabel LblSalesPrice;
        public GameObject LblCurrency2;
        public UILabel LblOriginalPrice;

        public UIScrollView ContentScrollView;
        public UIGrid DRPreviewGrid;
        public GameObject PreviewTextureItem;

        public event MyAction ClickClose;
        public event MyAction ClickBuyNow;
        public event MyAction ClickSaveShopCar;
        public event MyAction ClickGoShopCar;

        protected override void AwakeUnityMsg() {
            BtnClose.onClick.Add(new EventDelegate(OnBtnCloseClick));
            BtnBuyNow.onClick.Add(new EventDelegate(OnBtnBuyNowClick));
            BtnSaveShopCar.onClick.Add(new EventDelegate(OnBtnSaveShopCarClick));
            BtnGoShopCar.onClick.Add(new EventDelegate(OnBtnGoShopCarClick));
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }
        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }
        private void OnBtnCloseClick() {
            if (null != ClickClose) 
                ClickClose();
        }
        private void OnBtnBuyNowClick() {
            if (null != ClickBuyNow)
                ClickBuyNow();
        }
        private void OnBtnSaveShopCarClick() {
            if (null != ClickSaveShopCar)
                ClickSaveShopCar();
        }
        private void OnBtnGoShopCarClick() {
            if (null!=ClickGoShopCar)
                ClickGoShopCar();
        }

        public void SetProductLabel(JsonData jsonData) {
            LblShirtName.text = Utils.GetJsonStr(jsonData["name"]);
            if (Utils.GetJsonStr(jsonData["salesPrice"]) != string.Empty) {
                LblSalesPrice.text = Utils.GetJsonStr(jsonData["salesPrice"]);
                LblCurrency2.SetActive(true);
                LblOriginalPrice.text = Utils.GetJsonStr(jsonData["originalPrice"]);
            }else {
                LblCurrency2.SetActive(false);
                LblSalesPrice.text = Utils.GetJsonStr(jsonData["originalPrice"]);
            }
        }

        public void ReSetBanner() {
            BannerGrid.transform.DestroyChildren();
            var springPanel = BannerScrollView.GetComponent<SpringPanel>();
            springPanel.target.x = 0;
            springPanel.enabled = true;
        }

        public void InitBannerPic(Texture2D texture2D) {
            var obj = NGUITools.AddChild(BannerGrid.gameObject, BannerItem);
            DRPreviewBannerItemCtr drpb = obj.GetComponent<DRPreviewBannerItemCtr>();
            drpb.DRPreviewBannerItem.mainTexture = texture2D;
            BannerGrid.Reposition();
            BannerGrid.repositionNow = true;
            NGUITools.SetDirty(BannerGrid);
        }
        public void ReSetPreview() {
            DRPreviewGrid.transform.DestroyChildren();
            var springPanel = ContentScrollView.GetComponent<SpringPanel>();
            springPanel.target.y = 10.97f;
            springPanel.enabled = true;
        }

        public void InitDetailPic(Texture2D texture2D) {
            var obj = NGUITools.AddChild(DRPreviewGrid.gameObject, PreviewTextureItem);
            DRPreviewTextureItemCtr drpt = obj.GetComponent<DRPreviewTextureItemCtr>();
            drpt.DRPreviewTextureItem.mainTexture = texture2D;
            DRPreviewGrid.Reposition();
            DRPreviewGrid.repositionNow = true;
            NGUITools.SetDirty(DRPreviewGrid);
        }
    }
}