using System.Collections.Generic;
using Cloth3D.Interfaces;

namespace Cloth3D.Ui {
    public class PaymentCtr : IController {
        private PaymentViewPresenter _view;
        private IUserCenter _userCenter;
        private string _type = PaymenType.alipay.ToString();
        private float _price;

        private string _firCode = string.Empty;
        private string _tempStr = string.Empty;
        private string _body= string.Empty;

        public bool Init() {
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("user center is null.");
                return false;
            }
            EventCenter.Instance.AddListener<string>(EventId.SetPrice, SetPrice);
            EventCenter.Instance.AddListener<string, string, string>(EventId.SubmitOrder, SubmitOrderId);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<PaymentViewPresenter>(WindowId.Payment);
                _view.ClickBack += OnBackClick;
                _view.ClickWeChat += OnWebchatClick;
                _view.ClickUnionpay += OnUnionpayClick;
                _view.ClickAlipay += OnAlipayClick;
                _view.ClickPay += OnClickPay;
            }
            UiManager.Instance.ShowWindow(WindowId.Payment);
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
               Hide();
                if (!Constants.HomeShow || !Constants.HomeBtnsShow) {
                    Constants.HomeShow = true;
                    Constants.HomeBtnsShow = true;
                    UiControllerMgr.Instance.GetController(WindowId.HomeBtns).Show();
                    UiControllerMgr.Instance.GetController(WindowId.ShopCar).Show();
                }
                if (Constants.IsShopCar) {
                    Constants.IsShopCar = false;
                    UiControllerMgr.Instance.GetController(WindowId.ShopCar).Show();
                }
                // UiControllerMgr.Instance.GetController(WindowId.OrderConfirm).Show();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.Payment);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Payment, "ui/Payment"),
            };
        }

        void OnDestroy() {
            EventCenter.Instance.RemoveListener<string>(EventId.SetPrice, SetPrice);
        }

        private void SetPrice(string str) {
            float tem;
            float.TryParse(str, out tem);
            _price = tem;
            _view.SetPrice(str);
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnWebchatClick() {
            _view.SetSprite(PaymenType.wx);
            _type = PaymenType.wx.ToString();;
        }

        private void OnUnionpayClick() {
            _view.SetSprite(PaymenType.upacp);
            _type = PaymenType.upacp.ToString();
        }

        private void OnAlipayClick() {
            _view.SetSprite(PaymenType.alipay);
            _type = PaymenType.alipay.ToString();
        }

        private void SubmitOrderId(string orderNo,  string body, string subject) {
            _firCode = orderNo;
            _body = body;
            _tempStr = _userCenter.UserInfo.UserName + "的" + subject;
        }

        private void OnClickPay() {
            SDKContorl.Instance.Pay(_firCode, _tempStr, _body, (_price * 100).ToString(), _type);
        }
    }
}