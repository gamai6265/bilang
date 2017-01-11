using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class DesignViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public UIButton BtnNext;
        public UIButton BtnSave;
        public UIButton BtnShopCar;
        public UIButton BtnShopCar2;
        //public UIButton BtnShare;
        public UIButton BtnBuyNow;
        public UIButton BtnBuyNow2;

        public GameObject DesignObj;
        public GameObject RecommendObj;

        public UILabel LblTitle;
        public UILabel Lblprice;
        public UILabel LblShirtName;

        public UITweener BtnSaveTweener;

        public event MyAction ClickBack;
        public event MyAction ClickNext;
        public event MyAction ClickSave;
        public event MyAction ClickShopCar;
        public event MyAction ClickShare;
        public event MyAction ClickBuyNow;

        protected override void AwakeUnityMsg() {
            //_tweeAlpha = BackGroud.gameObject.GetComponent<TweenAlpha>();
        }
        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        protected override void OnDestroyUnityMsg() {
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnNext.onClick.Add(new EventDelegate(OnBtnNextClick));
            BtnSave.onClick.Add(new EventDelegate(OnBtnSaveClick));
            BtnShopCar.onClick.Add(new EventDelegate(OnBtnShopCarClick));
            BtnShopCar2.onClick.Add(new EventDelegate(OnBtnShopCarClick));
            BtnBuyNow.onClick.Add(new EventDelegate(OnBtnBuyNowClick));
            BtnBuyNow2.onClick.Add(new EventDelegate(OnBtnBuyNowClick));
            //BtnShare.onClick.Add(new EventDelegate(OnBtnShareClick));
        }

        private void OnBtnBuyNowClick() {
            if (null!= ClickBuyNow) {
                ClickBuyNow();
            }
        }

        private void OnBtnShopCarClick() {
            if (null != ClickShopCar) {
                ClickShopCar();
            }
        }

        private void OnBtnSaveClick() {
            if (null != ClickSave) {
                ClickSave();
            }
        }

        private void OnBtnNextClick() {
            if (null != ClickNext) {
                ClickNext();
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        private void OnBtnShareClick() {
            if (null != ClickShare)
                ClickShare();
        }

        public void PlayAnimation(bool tof) {
            if (tof) {
                BtnSaveTweener.ResetToBeginning();
                BtnSaveTweener.PlayForward();
            } else {
                BtnSaveTweener.PlayReverse();
            }
        }

        public void SetDesignTitle(string str) {
            LblTitle.text = str;
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        public void SetPrice(string display, string price) {
            Lblprice.text = display + " " + price;
        }
        
        public void ChangeBtnToDesign(bool design) {
            if (design) {
                //BtnSave.gameObject.SetActive(false);
                //BtnShare.gameObject.SetActive(false);
                BtnShopCar.gameObject.SetActive(false);
                //BtnNext.gameObject.SetActive(false);
                DesignObj.SetActive(false);
                RecommendObj.SetActive(false);
            } else {
                //BtnNext.gameObject.SetActive(Constants.IsRecommend);
                //BtnShare.gameObject.SetActive(true);
                BtnShopCar.gameObject.SetActive(true);
                //BtnSave.gameObject.SetActive(true);
                DesignObj.SetActive(Constants.IsRecommend);
                RecommendObj.SetActive(!Constants.IsRecommend);
            }
        }

        public void SetShirtName(string str) {
            LblShirtName.text = str;
        }
    }
}