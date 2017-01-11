using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class OrdersCtr : IController{
        private OrdersViewPresenter _view;
        private IOrderCenter _orderCenter;

        private string _chooseOrderStatus = "all";//选择加载订单类型
        private int _page;
        private int _pageCount;
        private int _showCount = 5; //每页加载个数
        private CodeSet _codesetList;

        public bool Init() {
            _orderCenter=ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter)as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            EventCenter.Instance.AddListener<string>(EventId.OrderEvaluateDone, (str) => { _view.DeleteItem(str); });
            EventCenter.Instance.AddListener(EventId.PaySucceed, ReGetOrdersList);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<OrdersViewPresenter>(WindowId.Orders);
                _view.SvDragFinished += OnSvDragFinished;

                _view.ClickBack += OnBackClick;
                _view.ClickPayment += OnPaymentClick;
                _view.ClickReceiving += OnReceivingClick;
                _view.ClickComment += OnCommentClick;
                _view.ClickAll += OnAllClick;
                _view.ClickRemind += OnRemindClick;

                _view.ClickSure += OnSureClick;
                _view.ClickPay += OnClickPay;
                _view.ClickAppraise += OnClickAppraise;
                _view.ClickReceipt += OnClickReceipt;
                _view.ClickChooce += OnClickChooce;
            }
            UiManager.Instance.ShowWindow(WindowId.Orders);
            UiControllerMgr.Instance.ShowLodingTips(true);
            GetCodeSet();
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                Hide();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            _page = 0;
            _view.DeleteAllPrefabs();
            UiManager.Instance.HideWindow(WindowId.Orders);
        }


        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Orders, "ui/Orders"),
            };
        }

        private void OnSvDragFinished() {
            var maxPage = (_pageCount/_showCount != 0) ? (_pageCount/_showCount) + 1 : (_pageCount/_showCount);
            if (_page < maxPage) {
                UiControllerMgr.Instance.ShowLodingTips(true);
                _page++;
                _orderCenter.QueryOrderList(_page.ToString(), _showCount.ToString(), _chooseOrderStatus, QueryOrderList);
            }
        }

        private void OnClickChooce(OrderInfo orderInfo, List<Texture2D> texture2Ds) {//订单详情
            UiControllerMgr.Instance.GetController(WindowId.OrdersDetails).Show();
            EventCenter.Instance.Broadcast(EventId.GetOrdersDetails, orderInfo, texture2Ds);
        }

        private void OnClickReceipt(string ordersId) {//收货
            SetOrderStatus(ordersId, "06");
        }

        private void OnClickAppraise(List<string> lstStrs, List<Texture2D> texTure2D, List<string> lstGoodId) {//评价
               UiControllerMgr.Instance.GetController(WindowId.OrderEvaluate).Show();
               EventCenter.Instance.Broadcast(EventId.OrderEvaluate, lstStrs, texTure2D,lstGoodId);
        }

        private void OnClickPay(string orderCode, string orderId, string price, string subject) {//付款
            UiControllerMgr.Instance.GetController(WindowId.Payment).Show();
            EventCenter.Instance.Broadcast(EventId.SetPrice, price);
            EventCenter.Instance.Broadcast(EventId.SubmitOrder, orderCode, orderId, subject);
        }

        private void OnSureClick(string ordersId) {//取消订单
            UiControllerMgr.Instance.ShowMessageBox(
               "",
               StrConfigProvider.Instance.GetStr("mb_promptorder"),
               StrConfigProvider.Instance.GetStr("mb_cancle"),
                () => {
                },
                StrConfigProvider.Instance.GetStr("mb_confirm"),
                () => {
                    _orderCenter.CancleOrder(ordersId, CancleOrder);
                }
            );
        }

        private void OnRemindClick() {
            UiControllerMgr.Instance.ShowTips("已提醒发货,请耐心等候");
        }

        private void OnAllClick() {
            _chooseOrderStatus = "all";
            ReGetOrdersList();
        }

        private void OnCommentClick() {
            _chooseOrderStatus = "06";//TODO
            ReGetOrdersList();
        }

        private void OnReceivingClick() {
            _chooseOrderStatus = "05";
            ReGetOrdersList();
        }

        private void OnPaymentClick() {
            _chooseOrderStatus = "11";
            ReGetOrdersList();
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        private void ReGetOrdersList() {
            _view.DeleteAllPrefabs();
            _page = 0;
            _view.SpringPanel(-143);
            UiControllerMgr.Instance.ShowLodingTips(true);
            _orderCenter.QueryOrderList(_page.ToString(), _showCount.ToString(), _chooseOrderStatus, QueryOrderList);
        }

        private void QueryOrderList(bool result, List<OrderInfo> lstOrder, string pageCount) {
            if (result) {
                _pageCount = int.Parse(pageCount);
                CoroutineMgr.Instance.StartCoroutine(IntOrdersItem(lstOrder));
            }
        }

        ///*
        private IEnumerator IntOrdersItem(List<OrderInfo> lstorder) {
            for (int i = 0; i < lstorder.Count; i++) {
                var orderStatus = Utils.GetJsonStr(lstorder[i].OrderStatus);
                var lstTexture2Ds = new List<Texture2D>();
                var lstGoodId = new List<string>();
                if (lstorder[i].ClothNumber != string.Empty) {
//                    var clothNumber = int.Parse(lstorder[i].ClothNumber);
//                    for (var j = 0; j < clothNumber; j++) {
                    var www = new WWW(Constants.ImageURL + lstorder[i].OrderImage);
                    yield return www;
                    Texture2D texture2D = null;
                    if (www.error == null) {
                        if (www.isDone) {
                            texture2D = www.texture;
                        }
                    }
                    else {
                        www = new WWW(Constants.DefaultImage);
                        yield return www;
                        texture2D = www.texture;
                    }
                    lstTexture2Ds.Add(texture2D);
                    lstGoodId.Add(lstorder[i].OrderCode);
                    www.Dispose();
//                    }
                    var state = _codesetList.GetData()[orderStatus];
                    _view.AddPrefabs(lstorder[i], state, lstTexture2Ds, orderStatus, lstGoodId);
                    _view.AdjustSizeItemDepth();
                }
            }
            UiControllerMgr.Instance.ShowLodingTips(false);
        }

        private void GetCodeSet() {
            CodeSetProvider.Instance.QueryCodeset("ORDER_STATUS", OnGetCodeset);
        }

        private void OnGetCodeset(CodeSet codeset) {
            _codesetList = codeset;
            _orderCenter.QueryOrderList(_page.ToString(), _showCount.ToString(), _chooseOrderStatus, QueryOrderList);
        }

        private void SetOrderStatus(string id, string status) {
            _orderCenter.ModifyOrderStatus(id, status, ModifyStatus);
        }

        private void ModifyStatus(bool result, List<OrderInfo> lstOrder) {
            if (result) {
                ReGetOrdersList();
            } else {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_modifyfail"));
            }
        }

        private void CancleOrder(bool result, string orderId) {
            if (result) {
                _view.DeleteItem(orderId);
            } else {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_canclefail"));
            }
        }

    }
}