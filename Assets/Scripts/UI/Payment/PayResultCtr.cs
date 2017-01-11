using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class PayResultCtr : IController{
        private PayResultViewPresenter _view;
        private IOrderCenter _orderCenter;
        private string[] _strings;
        private bool _status = false;
        public bool Init() {
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            EventCenter.Instance.AddListener<string[], bool>(EventId.PayResult, GetPayResult);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<PayResultViewPresenter>(WindowId.PayResult);
                _view.ClickBack += OnBackClick;
                _view.ClickSure += OnSureClick;
                _view.ClickBackHome += OnBackHome;
            }
            UiManager.Instance.ShowWindow(WindowId.PayResult);
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                //TODO 这里的界面显隐关系还需细调,简直恶心
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
                if (_status)
                    EventCenter.Instance.Broadcast(EventId.PaySucceed);
                Hide();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            _status = false;
            UiManager.Instance.HideWindow(WindowId.PayResult);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.PayResult,"ui/PayResult"),
            };
        }

        private void GetPayResult(string[] strings, bool status) {
            _strings = strings;
            if (status) {
                _status = status;
                _orderCenter.GetOrderDetail(_strings[0], OrdersDetails);
                UiControllerMgr.Instance.GetController(WindowId.Payment).Hide();
            }
            _view.SetResult(status);
            _view.SetLabel(_strings[0], _strings[3], _strings[4]);
        }

        private void OrdersDetails(bool result, JsonData json) {
            if (result) {
                LogSystem.Debug("Order Status ="+ Utils.GetJsonStr(json["orderStatus"]));
//                var isPayment = false;
//                if (Utils.GetJsonStr(json["orderStatus"]) == "01") {
//                    isPayment = true;
//                }
//                _view.SetResult(isPayment);
//                _view.SetLabel(_strings[0], _strings[3], _strings[4]);
            }
        }

        void OnBackClick() {
            OnBack(-1);
        }

        void OnSureClick() {
            //TODO 这里的界面显隐关系还需细调
            Hide();
            SDKContorl.Instance.Pay(_strings[0], _strings[1], _strings[2], _strings[3], _strings[4]);
        }

        void OnBackHome() {
            OnBack(-1);
//            UiManager.Instance.HideAllShownWindow();
//           UiManager.Instance.ShowWindow(WindowId.Home);
//           UiManager.Instance.ShowWindow(WindowId.HomeBtns);
        }

    }
}
