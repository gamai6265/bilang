using System;
using Cloth3D.Data;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class OrdersDetailsViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public UIButton BtnPayment;
        public UIButton BtnReceiving;

        public UILabel LblAddress;
//        public UILabel LblClothesName;
//        public UILabel LblClothesSize;
        public UILabel LblClothNumber;
        public UILabel LblDeliveryType;
        public UILabel LblName;
        public UILabel LblorderCode;
        public UILabel LblOrderStatus;
        public UILabel LblPhone;
        public UILabel LblPrice;
        public GameObject OrderPrefabs;

        public UILabel LblTitle;
        public UILabel LblId;
        public UILabel LblQuantity;
        public UILabel LblSum;
        public UILabel LblDispatching;
//        public UILabel LblTestSize;
        public UILabel LblCancleOrder;
        public UILabel LblConfirm;
        public UILabel LblPayment;

        public UIGrid OrdersDetailsGrid;
        public UIScrollView OrdersDetailsScrollView;
//        public UITexture TexClothes;

        public event MyAction ClickBack;
        public event MyAction ClickReceiving;
        public event MyAction ClickPayment;
        public event MyAction ClickCancel;
        public event MyAction<string, string> ClickChoose;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }

        protected override void OnDestroyUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("or_d_title");
            LblId.text = StrConfigProvider.Instance.GetStr("or_d_id") + ": ";
            LblQuantity.text = StrConfigProvider.Instance.GetStr("or_d_quantity") + ": ";
            LblSum.text = StrConfigProvider.Instance.GetStr("or_d_sum") + ": ";
            LblDispatching.text = StrConfigProvider.Instance.GetStr("or_d_dispatching");
            LblDeliveryType.text = StrConfigProvider.Instance.GetStr("or_d_deliverytype");
//            LblTestSize.text = StrConfigProvider.Instance.GetStr("or_d_testsize");

            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblId.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblQuantity.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSum.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblDispatching.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblDeliveryType.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
//            LblTestSize.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnReceiving.onClick.Add(new EventDelegate(OnBtnReceivingClick));
            BtnPayment.onClick.Add(new EventDelegate(OnBtnPaymentClick));
           // BtnCancel.onClick.Add(new EventDelegate(OnBtnCancelClick));
        }

        private void OnBtnCancelClick() {
            if (null != ClickCancel) {
                ClickCancel();
            }
        }

        private void OnBtnPaymentClick() {
            if (null != ClickPayment) {
                ClickPayment();
            }
        }

        private void OnBtnReceivingClick() {
            if (null != ClickReceiving) {
                ClickReceiving();
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        public void SetClothesLabel(string[] strings) {
//            LblClothesName.text = strings[0];
            LblorderCode.text = strings[1];
            LblClothNumber.text = strings[2];
            LblPrice.text = strings[3];
            LblOrderStatus.text = strings[4];
//            TexClothes.mainTexture = texture2D;
        }

        public void SetAddressLabel(string[] strings) {
            LblAddress.text = strings[0];
            LblPhone.text = strings[1];
            LblName.text = strings[2];
//            LblClothesSize.text = strings[3];
        }

        public void AddOrderConfirmPrefabs(JsonData goodsJson, string orderId, string goodId, string goodsNumber, string remark, Texture2D texture2Ds) {
            var obj = NGUITools.AddChild(OrdersDetailsGrid.gameObject, OrderPrefabs);

            var cdpp = obj.GetComponent<ClothesPrefabs>();
            cdpp.OrderId = orderId;//订单id
            cdpp.LblName.text = goodId;//商品id
            cdpp.LblgoodsNumber.text = goodsNumber;
            cdpp.LblRemark.text = remark;
            cdpp.TexClothes.mainTexture = texture2Ds;

            UIEventListener.Get(obj).onClick += OnClickChoose;

            cdpp.LblName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            cdpp.LblgoodsNumber.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            cdpp.LblRemark.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            OrdersDetailsGrid.Reposition();
            OrdersDetailsGrid.repositionNow = true;
            AdjustItem();
        }

        private void AdjustItem() {
            var depth = OrdersDetailsScrollView.GetComponent<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(OrdersDetailsScrollView.gameObject, depth + 1);
            GetSortedPanels(true);
        }

        public void DeleteAll() {
            OrdersDetailsGrid.transform.DestroyChildren();
        }

        private void OnClickChoose(GameObject go) {
            var cp = go.GetComponent<ClothesPrefabs>();
            if (null != cp && null != ClickChoose) {
                ClickChoose(cp.OrderId, cp.LblName.text);
            }
        }

        public void SetBtns(string str) {
            if (str == StrConfigProvider.Instance.GetStr("or_p_payment")) {
                //BtnCancel.gameObject.SetActive(true);
               // LblCancleOrder.text = StrConfigProvider.Instance.GetStr("or_d_cancleorder");
                BtnPayment.gameObject.SetActive(true);
                LblPayment.text = StrConfigProvider.Instance.GetStr("or_d_payment");
            } else if (str == StrConfigProvider.Instance.GetStr("or_p_delivery")) {
                BtnReceiving.gameObject.SetActive(true);
                LblConfirm.text = StrConfigProvider.Instance.GetStr("or_d_confirm");
            }
            //switch (str) {
            //    case "待支付":
            //        BtnCancel.gameObject.SetActive(true);
            //        LblCancleOrder.text = StrConfigProvider.Instance.GetStr("or_d_cancleorder");
            //        BtnPayment.gameObject.SetActive(true);
            //        LblPayment.text = StrConfigProvider.Instance.GetStr("or_d_payment");
            //        break;
            //    case "已发货":
            //        BtnReceiving.gameObject.SetActive(true);
            //        LblConfirm.text = StrConfigProvider.Instance.GetStr("or_d_confirm");
            //        break;
            //}
        }

        public void CleanAll() {
            DeleteAll();
            BtnPayment.gameObject.SetActive(false);
            BtnReceiving.gameObject.SetActive(false);
            string[] str = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            SetClothesLabel(str);
            SetAddressLabel(str);
        }
    }
}