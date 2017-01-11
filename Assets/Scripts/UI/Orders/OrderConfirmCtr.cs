using System;
using System.Collections.Generic;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {

    public class OrderConfirmCtr : IController{
        private OrderConfirmViewPresenter _view;
        private IOrderCenter _orderCenter;
        private IUserCenter _userCenter;
        
        private Address _address = new  Address();
        private List<JsonData> _shopCartJson = new List<JsonData>();
        private List<ShopItems> _shopItemses = new List<ShopItems>();
        private Dictionary<string, string> _shopItemsNum = new Dictionary<string, string>();
        private string _price = string.Empty;
        private string _totalprice = string.Empty;
        private string _couponid = string.Empty;
        private static Dictionary<string, Dictionary<string, SizeInfo>> _dicClothsSize;
        public bool Init() {
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }

            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("user center is null.");
                return false;
            }
            EventCenter.Instance.AddListener<string>(EventId.SetOrderConfirmPrice, SetPrice);
            EventCenter.Instance.AddListener<List<ShopItems>>(EventId.GetShopCarData, InitShopCarData);
            EventCenter.Instance.AddListener<string>(EventId.ChooseAddress, ChangeAddress);
            EventCenter.Instance.AddListener(EventId.ShowAddress, ShowAddress);
            EventCenter.Instance.AddListener<string, string, string>(EventId.ChooseCoupon, ChangeDiscountPrice);
            EventCenter.Instance.AddListener<string, Dictionary<string, SizeInfo>>(EventId.SetClothSize, SetClothSize);
            EventCenter.Instance.AddListener<Address>(EventId.ChangeOrderConfirmAddress, _ChangeAddress);
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<OrderConfirmViewPresenter>(WindowId.OrderConfirm);
                _view.ClickBack += OnBackClick;
//                _view.ClickSize += OnSizeClick;
                _view.ClickPay += OnPayClick;
                _view.ClickAddress += OnAddressClick;

                _view.ClickChoose += OnClickChoose;
                _view.ClickChooseSize += OnClickChooseSize;

//                _view.ClickRetract += OnClickRetract;
                _view.ClickChooseCoupon += OnClickChooseCoupon;
            }
            UiManager.Instance.ShowWindow(WindowId.OrderConfirm);
            ShowAddress();
            GetDeliveryDate();
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            if (zorder == 0 || zorder == -1) {
                Hide();
                Constants.HomeShow = true;
                Constants.HomeBtnsShow = true;
                UiControllerMgr.Instance.GetController(WindowId.HomeBtns).Show();
                UiControllerMgr.Instance.GetController(WindowId.ShopCar).Show();
                return true;
            }
            return false;
        }

        public void Hide(MyAction onComplete=null) {
            _price = string.Empty;
            _totalprice = string.Empty;
            _couponid = string.Empty;
            _view.ReSetDiscountPrice();
            _dicClothsSize.Clear();
            UiManager.Instance.HideWindow(WindowId.OrderConfirm);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
               new KeyValuePair<WindowId, string>(WindowId.OrderConfirm, "ui/OrderConfirm"),
            };
        }

        void OnDestroy() {
            EventCenter.Instance.RemoveListener<string>(EventId.SetOrderConfirmPrice, SetPrice);
            EventCenter.Instance.RemoveListener<List<ShopItems>>(EventId.GetShopCarData, InitShopCarData);
            EventCenter.Instance.RemoveListener<string, string, string>(EventId.ChooseCoupon, ChangeDiscountPrice);
            EventCenter.Instance.RemoveListener(EventId.ShowAddress, ShowAddress);
            EventCenter.Instance.RemoveListener<string, string, string>(EventId.ChooseCoupon, ChangeDiscountPrice);
            EventCenter.Instance.RemoveListener<string, Dictionary<string, SizeInfo>>(EventId.SetClothSize, SetClothSize);
            EventCenter.Instance.RemoveListener<Address>(EventId.ChangeOrderConfirmAddress, _ChangeAddress);
        }

        private void OnAddressClick() {
            UiControllerMgr.Instance.GetController(WindowId.Address).Show();
           // EventCenter.Instance.Broadcast(EventId.ShowAddress);
        }

        //衣服详情
        private void OnClickChoose(string clothCode, string quantity) {
            UiControllerMgr.Instance.GetController(WindowId.ClothesDetails).Show();
            EventCenter.Instance.Broadcast(EventId.GetClothesDetailsData, clothCode, clothCode, "ShoppingCart", quantity);
        }

        //衣服尺寸
        private void OnClickChooseSize(string modelId, string clothCode) {
            UiControllerMgr.Instance.GetController(WindowId.EditorSize).Show();            
            EventCenter.Instance.Broadcast(EventId.ShowEditorSizeByModelId, modelId, clothCode);
            
        }

//        private void OnClickRetract() {
//           //TODO 收起
//        }

        //优惠卷
        private void OnClickChooseCoupon() {
            var tmpprice = (_totalprice == string.Empty) ? _price : _totalprice;
            _orderCenter.GetCouponList(tmpprice, GetCouponList);
        }

        void OnBackClick() {
            OnBack(-1);
        }

        //提交订单
        void OnPayClick() {
            if (_dicClothsSize.Count == 0 || _dicClothsSize.Count !=_shopItemses.Count) {
                UiControllerMgr.Instance.ShowTips("请选择衣服尺寸");
                return;
            }
            if (_address == null || _address.Contacts == string.Empty || _address.Province == string.Empty
                || _address.City == string.Empty || _address.Tel == string.Empty) {
                UiControllerMgr.Instance.ShowTips("收货信息不能为空");
                return;
            }

            JsonData orderInfoJson = new JsonData();
            orderInfoJson["actualAmount"] = _price;
            orderInfoJson["consignee"] = _address.Contacts;
            orderInfoJson["address"] = _address.Province + _address.City + _address.Area + _address.Addr;
            orderInfoJson["telphone"] = _address.Tel;
            
            JsonData data = new JsonData();
            for (int i = 0; i < _shopItemses.Count; i++) {
                var item = new JsonData();
                data.Add(item);
                item["atid"] = _shopItemses[i].Id;
                item["number"] = _shopItemsNum[_shopItemses[i].Id];
                var size = item["size"] = new JsonData();
                if (_dicClothsSize.ContainsKey(_shopItemses[i].Id)) {
                    foreach (var tempItem in _dicClothsSize[_shopItemses[i].Id]) {
                        var sizeitem = new JsonData();
                        sizeitem["propertiesCode"] = tempItem.Value.PropertiesCode;
                        sizeitem["propertiesValue"] = tempItem.Value.SizeValue;
                        sizeitem["propertiesType"] = tempItem.Value.PropertiesType;
//                    Debug.Log(tempItem.Value.PropertiesCode+"/"+"/"+ tempItem.Value.SizeValue+ "/" +tempItem.Value.PropertiesType);
                        size.Add(sizeitem);
                    }
                }
            }
//            Debug.Log(orderInfoJson.ToJson());
//            Debug.Log(data.ToJson());
            _orderCenter.SubmitOrder(orderInfoJson.ToJson(), data.ToJson(), _couponid, SubmitOrder);
        }
        
        //初始实例衣服按钮
        private void InitShopCarData(List<ShopItems> shopitems) {
            _dicClothsSize = new Dictionary<string, Dictionary<string, SizeInfo>>();
            _shopItemses.Clear();
            _shopItemsNum.Clear();
            _shopItemses = shopitems;
            _view.DeleteAll();
            var num = 0;
            foreach (var item in _shopItemses) {
                num += int.Parse(item.Num);
                _view.AddOrderConfirmPrefabs(item.ModelId, item.Id, item.GoodsName, item.Num, item.GoodPic);
                if (!_shopItemsNum.ContainsKey(item.Id))
                    _shopItemsNum.Add(item.Id, item.Num);
                _view.AdjustSizeItemDepth();
            }
            _view.SetLblPay(num);
        }

        private void SetPrice(string str) {
            _price = str;
            _totalprice = _price;
            _view.SetPrice(str);
        }

        //获取交货日期
        private void GetDeliveryDate() {
            DateTime dateTime = DateTime.Now.AddDays(5);//TODO 先默认5天后
            var str = dateTime.Year.ToString() +" - "+ dateTime.Month.ToString() + " - " + dateTime.Day.ToString();
            _view.SetDeliveryDate(str);
            //_orderCenter.GetDeliveryDate(SetDeliveryDate);
        }
        private void SetDeliveryDate(bool result, string data) {
            if (result) {
                _view.SetDeliveryDate(data);
            }
        }
        
        //提交订单
        private void SubmitOrder(bool result, JsonData json) {
            if (result) {
                _couponid = string.Empty;
                _totalprice = string.Empty;
                _shopCartJson.Clear();
                _orderCenter.DicSizeInfo.Clear();
                UiControllerMgr.Instance.GetController(WindowId.Home).Show();
                UiControllerMgr.Instance.GetController(WindowId.Payment).Show();
                EventCenter.Instance.Broadcast(EventId.SetPrice, _price);

                var subject = string.Empty;
                JsonData orderGoods = json["orderGoods"];
                if (orderGoods.Count > 1) {
                    subject = orderGoods.Count.ToString() + "件衬衣";
                } else {
                    subject = Utils.GetJsonStr(orderGoods[0]["goodsName"]);
                }
                EventCenter.Instance.Broadcast(EventId.SubmitOrder, Utils.GetJsonStr(json["orderCode"]), Utils.GetJsonStr(json["atid"]), subject);//TODO 提交订单后调用支付需要修改
                Hide();
            }
        }
       
        private void ChangeAddress(string addrId) {
            Show();
            MyAction<bool, List<Address>> chooseAction = null;
            chooseAction = (rsult, lstAddr) => {
                Address foundAddr = lstAddr.Find((Address addr) => {
                    return addr.Id == addrId;
                });
                if (foundAddr != null)
                    _ChangeAddress(foundAddr);
            };
            _userCenter.QueryAddress(chooseAction);
        }

        private void _ChangeAddress(Address add) {
            _address = add;
            _view.SetAddress(add);
        }

        private void ChangeDiscountPrice(string couponid, string couponPrice, string title) {
            _couponid = couponid;
            var tem = (_totalprice == string.Empty) ? _price : _totalprice;
            var price = float.Parse(tem);
            var couponprice = float.Parse(couponPrice);
            var temp = price - couponprice;
            if (temp < 0)
                temp = 0;
            _price = string.Format("{0:F}", temp);//temp.ToString();
            _view.SetDiscountPrice(title, _price, tem, couponPrice);
        }


        private void ShowAddress() {
            _userCenter.QueryAddress(QueryAddress);
        }

        private void QueryAddress(bool result, List<Address> lstAddress) {
            if (result) {
                if (lstAddress.Count == 0) {
                    _view.ShowAddress(false);
                    _address = null;
                    _view.ClearAddress();
                }
                for (var i = 0; i < lstAddress.Count; i++) {
                    _view.ShowAddress(true);
                    if (_address != null && lstAddress[i].Id == _address.Id) {
                        _ChangeAddress(lstAddress[i]);
                        break;
                    }
                    _ChangeAddress(lstAddress[i]);
                }
            }
        }

        private void GetCouponList(bool result, JsonData json) {
            if (result) {
                _orderCenter.CouponList = null;
                _orderCenter.CouponList = json;
                UiControllerMgr.Instance.GetController(WindowId.Coupon).Show();
            }
        }

        private void SetClothSize(string aaa, Dictionary<string, SizeInfo> bbb) {
            if (!_dicClothsSize.ContainsKey(aaa)) {
                _dicClothsSize.Add(aaa,bbb);
            }
            else {
                _dicClothsSize[aaa] = bbb;
            }
        }
    }
}
