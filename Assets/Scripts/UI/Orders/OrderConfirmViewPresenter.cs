using UnityEngine;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class OrderConfirmViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public UIButton BtnPay;
        public UIButton BtnAddress;
//        public UIButton BtnRetract;
        public UIButton BtnChooseCoupon;

        public UILabel LblTitle;
        public UILabel LblOrderList;
        public UILabel LblCouponName;
        public UILabel LblTimeName;
        public UILabel LblMoney;
        public UILabel LblPay;
        public UILabel LblTotal;
        public UILabel LblDiscount;

        public UILabel LblName;
        public UILabel LblPhone;
        public UILabel LblAddress;
        public GameObject AddressObj;
        public GameObject NoAddressObj;

        public UILabel LblTime;

        public GameObject DiscountObj;
        public UILabel LblCoupon;
        public UILabel LblPrice;//实付款
        public UILabel LblTotalPrice;//商品总额
        public UILabel LblDiscountPrice;//扣减


        public UIScrollView OrderConfirmScrollView;
        public UIGrid OrderConfirmGrid;
        public GameObject OrderPrefabs;

        public event MyAction ClickBack;
        public event MyAction ClickPay;
        public event MyAction ClickAddress;

        public event MyAction<string, string> ClickChoose;
        public event MyAction<string, string> ClickChooseSize;

//        public event MyAction ClickRetract;
        public event MyAction ClickChooseCoupon;

        
        protected override void StartUnityMsg() {
            ChangeLanguage();
            BtnsListener();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("or_c_title");
            LblOrderList.text = StrConfigProvider.Instance.GetStr("or_c_orderlist");
            LblCouponName.text = StrConfigProvider.Instance.GetStr("or_c_coupon");
            LblCoupon.text = StrConfigProvider.Instance.GetStr("or_c_choosecoupon");
            LblTimeName.text = StrConfigProvider.Instance.GetStr("or_c_time");
//            LblMoney.text = StrConfigProvider.Instance.GetStr("or_c_money");
//            LblTotal.text = StrConfigProvider.Instance.GetStr("or_c_totalmoney");
//            LblDiscount.text = StrConfigProvider.Instance.GetStr("or_c_deduction");
            

            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblOrderList.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCouponName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCoupon.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblTimeName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
//            LblMoney.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
//            LblTotal.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
//            LblDiscount.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnPay.onClick.Add(new EventDelegate(OnBtnPayClick));
            BtnAddress.onClick.Add(new EventDelegate(OnBtnAddressClick));
//            BtnRetract.onClick.Add(new EventDelegate(OnBtnRetractClick));
            BtnChooseCoupon.onClick.Add(new EventDelegate(BtnChooseCouponClick));
        }

        private void OnBtnAddressClick() {
            if (null != ClickAddress) {
                ClickAddress();
            }
        }

        private void OnBtnPayClick() {
            if (null != ClickPay) {
                ClickPay();
            }
        }
        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

//        private void OnBtnRetractClick() {
//            if (null != ClickRetract) {
//                ClickRetract();
//            }
//        }

        private void BtnChooseCouponClick() {
            if (null != ClickChooseCoupon) {
                ClickChooseCoupon();
            }
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        public Address GetAddress() {
             var addr=new Address();
            addr.Addr = LblAddress.text;
            addr.Tel = LblPhone.text;
            addr.Contacts = LblName.text;
            return addr;
        }

        public void ClearAddress() {
            LblName.text = string.Empty;
            LblPhone.text = string.Empty;
            LblAddress.text = string.Empty;
        }

        public void SetAddress(Address addr) {
            LblName.text = addr.Contacts;
            LblPhone.text = addr.Tel;
            LblAddress.text = addr.Province+addr.City+addr.Area+addr.Addr;
            LblName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPhone.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblAddress.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        public void ShowAddress(bool tof) {
            AddressObj.SetActive(tof);
            NoAddressObj.SetActive(!tof);
        }

        public void SetDeliveryDate(string str) {
            LblTime.text = str;
            LblTime.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        public void SetPrice(string str) {
            LblPrice.text = str;
            LblPrice.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        public void SetDiscountPrice(string title, string price,string TotalPrice, string couponprice) {
            LblCoupon.text = title;
            DiscountObj.SetActive(true);
            LblTotalPrice.text = TotalPrice;
            LblPrice.text = price;
            LblDiscountPrice.text = couponprice;
        }

        public void ReSetDiscountPrice() {
            LblCoupon.text = string.Empty;
            LblDiscountPrice.text = string.Empty;
            LblTotalPrice.text = string.Empty;
            DiscountObj.SetActive(false);
        }

        //从购物车进入订单确认界面.无需再请求数据.使用购物车列表的
        public void AddOrderConfirmPrefabs(string modelId,string ocde, string orderName, string quantity, Texture2D _texture2D) {
            GameObject obj = NGUITools.AddChild(OrderConfirmGrid.gameObject, OrderPrefabs);

            OrderConfirmPrefabs ocp = obj.GetComponent<OrderConfirmPrefabs>();
            ocp.AddListener();
            obj.name = ocp.ClothCode = ocde;
            ocp.modelId = modelId;
            ocp.LbllClothName.text = orderName;
            ocp.LblClothQuantity.text = quantity;
            ocp.TexCloth.mainTexture = _texture2D;
            ocp.LbllClothName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            ocp.LblClothQuantity.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            ocp.ClickChoose += OnClickChoose;
            ocp.ClickChooseSize += OnClickChooseSize;
            OrderConfirmGrid.Reposition();
            OrderConfirmGrid.repositionNow = true;
            OrderConfirmScrollView.ResetPosition();
        }

        public void AdjustSizeItemDepth() {
            var depth = OrderConfirmScrollView.GetComponentInChildren<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(OrderConfirmScrollView.gameObject, depth + 1);
            GetSortedPanels(true);
        }

        private void OnClickChoose(string clothCode, string quantity) {
            if ( null != ClickChoose) {
                ClickChoose(clothCode, quantity);
            }
        }

        private void OnClickChooseSize(string modelId, string clothCode) {
            if (null != ClickChooseSize) {
                ClickChooseSize(modelId, clothCode);
            }
        }

        public void DeleteAll() {
            OrderConfirmGrid.transform.DestroyChildren();
        }

        public void SetLblPay(int count) {
            LblPay.text = StrConfigProvider.Instance.GetStr("or_c_pay");           
            LblPay.text = LblPay.text + "(" + count + ")";
            LblPay.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }
    }
}