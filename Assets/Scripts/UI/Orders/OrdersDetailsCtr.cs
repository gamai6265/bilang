using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class OrdersDetailsCtr : IController{
        private IOrderCenter _orderCenter;
        
        private string _price;
        private OrdersDetailsViewPresenter _view;
        private CodeSet _codesetList;
        private OrderInfo _orderInfo;

        public bool Init() {
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }

            EventCenter.Instance.AddListener<OrderInfo, List<Texture2D>>(EventId.GetOrdersDetails, GetOrdersDetails);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<OrdersDetailsViewPresenter>(WindowId.OrdersDetails);
                _view.ClickBack += OnBackClick;
                _view.ClickReceiving += OnReceivngClick;
                _view.ClickPayment += OnPaymentClick;
                _view.ClickCancel += OnCancelClick;
                _view.ClickChoose += OnChooseClick;
            }
            UiManager.Instance.ShowWindow(WindowId.OrdersDetails);
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
            _view.CleanAll();
            UiManager.Instance.HideWindow(WindowId.OrdersDetails);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.OrdersDetails, "ui/OrdersDetails"),
            };
        }

        private void OnBackClick() {
            OnBack(-1);
        }

        private void OnReceivngClick() {
            SetOrderStatus(_orderInfo.OrderId, _codesetList.GetName2Data()[StrConfigProvider.Instance.GetStr("or_p_recipient")]);
        }

        private void OnPaymentClick() {
            UiControllerMgr.Instance.GetController(WindowId.Payment).Show();
            EventCenter.Instance.Broadcast(EventId.SetPrice, _price);
            var subject = string.Empty;
            if (_orderInfo.Order_Goods.Count > 1) {
                subject = _orderInfo.Order_Goods.Count.ToString() + "件衬衣";
            } else {
                subject = _orderInfo.Order_Goods[0].GoodsName;
            }
            EventCenter.Instance.Broadcast(EventId.SubmitOrder,
                _orderInfo.OrderCode, _orderInfo.OrderId, subject);
        }

        private void OnCancelClick() {
            SetOrderStatus(_orderInfo.OrderId, _codesetList.GetName2Data()[StrConfigProvider.Instance.GetStr("or_p_hbcancle")]);
        }

        private void OnChooseClick(string orderId, string goodId) {
            UiControllerMgr.Instance.GetController(WindowId.ClothesDetails).Show();
            EventCenter.Instance.Broadcast(EventId.GetClothesDetailsData, orderId, goodId, "Order", string.Empty);
        }

        private void GetOrdersDetails(OrderInfo orderInfo, List<Texture2D> texture2Ds) {
            _view.DeleteAll();
            //            InitOrderdetails(orderInfo, texture2Ds);
            _orderCenter.GetOrderDetail(orderInfo.OrderId, OrdersDetails);
        }

        private void OrdersDetails(bool result, JsonData json) {
            if (result) {
                CoroutineMgr.Instance.StartCoroutine(Orderdetails(json));
            }
        }

        private IEnumerator Orderdetails(JsonData json) {
            var orderName = Utils.GetJsonStr(json["atid"]); //orderInfo.OrderId;
            var number = Utils.GetJsonStr(json["totalNumber"]);
            var buyerName = Utils.GetJsonStr(json["crps"]);
            var Tel = Utils.GetJsonStr(json["telphone"]);
            var addr = Utils.GetJsonStr(json["address"]);
            _price = Utils.GetJsonStr(json["actualAmount"]);
            var status = _codesetList.GetData()[Utils.GetJsonStr(json["orderStatus"])];
            _view.SetBtns(status);
            string[] strs = { orderName, orderName, number, _price, status };
            _view.SetClothesLabel(strs);
            var orderGoods = json["orderGoods"];
            for (var i = 0; i < orderGoods.Count; i++) {
                var item = orderGoods[i];
                var goodsId = Utils.GetJsonStr(item["goodsId"]);
                var goodsNumber = Utils.GetJsonStr(item["goodsNumber"]);
                var goodsDescription = Utils.GetJsonStr(item["goodsDescription"]);
                var www = new WWW(Constants.ImageURL + Utils.GetJsonStr(item["image"]));
                yield return www;
                Texture2D clothesTexture = null;
                if (www.error == null) {
                    if (www.isDone) {
                        clothesTexture = www.texture;
                    }
                } else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    clothesTexture = www.texture;
                }
                www.Dispose();
                _view.AddOrderConfirmPrefabs(item, orderName, goodsId, goodsNumber, goodsDescription, clothesTexture);
            }
            string[] str = { addr, Tel, buyerName };
            _view.SetAddressLabel(str);
        }

        /*
        private void InitOrderdetails(OrderInfo orderInfo, List<Texture2D> texture2Ds) {
            _orderInfo = orderInfo;
            var orderName = orderInfo.OrderId;
            var number = orderInfo.ClothNumber;
            var buyerName = orderInfo.Order_contact;
            var Tel = orderInfo.Order_tel;
            var addr = orderInfo.Order_address;
            _price = orderInfo.OrderPrice;
            var status = _codesetList.GetData()[orderInfo.OrderStatus];
            _view.SetBtns(status);
            string[] strs = { orderName, orderInfo.OrderId, number, _price, status };
            _view.SetClothesLabel(strs);
            for (int i = 0; i < orderInfo.Order_Goods.Count; i++) {
                var item = orderInfo.Order_Goods[i];
                _view.AddOrderConfirmPrefabs(orderInfo.OrderId, item.GoodsId, item.GoodsNumber, item.AgoodsDescription,
                    texture2Ds[i]);
            }
            string[] str = { addr, Tel, buyerName };
            _view.SetAddressLabel(str);
        }
        */

        private void GetCodeSet() {
            CodeSetProvider.Instance.QueryCodeset("ORDER_STATUS", OnGetCodeset);
        }
        private void OnGetCodeset(CodeSet codeset) {
            _codesetList = codeset;
        }

        private void SetOrderStatus(string id, string status) {
            _orderCenter.ModifyOrderStatus(id, status, ModifyStatus);
        }
        private void ModifyStatus(bool result, List<OrderInfo> lstOrder) {
            if (result) {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_modifysuccess"));
            } else {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_modifyfail"));
            }
        }
    }
}