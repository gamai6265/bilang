using System.Collections;
using System.Collections.Generic;
using Cloth3D.Interfaces;
using Cloth3D.Data;
using Cloth3D.Ui;
using LitJson;
using UnityEngine;

namespace Cloth3D.Order {
    public class OrderCenter : IOrderCenter, IOrderListener, IDesignClothesListener, IInfoListener {
        public string Name { get; set; }
        public Dictionary<string, SizeInfo> DicSizeInfo { get; set; }
        public string FabricFigureCode { get; set; }
        private INetworkMgr _networkMgr;
        private IUserCenter _userCenter;
        public JsonData CouponList { get; set; }

        private List<OrderInfo> _lstOrder = null;
        private event MyAction<bool, List<OrderInfo>, string> OnQueryOrder;

        private event MyAction<bool, string> OnCancleOrder;

        private string _modifyOrdId = string.Empty;
        private string _modifyStatus = string.Empty;
        private event MyAction<bool, List<OrderInfo>> OnModifyOrder;

        private string _submitOrder = string.Empty;
        private event MyAction<bool,JsonData> OnSubmitOrder;

        private event MyAction<bool, string> OnDeliveryDate;

        private string _orderDetailId = string.Empty;
        private event MyAction<bool, JsonData> OnOrderDetail;

        private string _orderParas = string.Empty;
        private event MyAction<bool> OnSubmitOrderEvaluate;
        private event MyAction<bool, List<ShopCarItem>> OnGetShopCarList;
        private event MyAction<bool, string> OnDeleteShopCarItems;

        private string _shopCarId = string.Empty;
        private event MyAction<bool, JsonData> OnShopCarDetail;

        private event MyAction<bool, JsonData> OnSaveShopCar;
        private event MyAction<bool, JsonData> OnOneKeyBuyProduct;
        private event MyAction<bool, JsonData> OnOrderEvaluationList;
        private event MyAction<bool, JsonData> OnGetCouponList;
        private event MyAction<bool, JsonData> OnGetMeasureByPropertiesCode;
        private event MyAction<bool, JsonData> OnGetGoodsInfo;
        private event MyAction<bool, JsonData> OnGetFabricDetail;
        private event MyAction<bool, JsonData> OnGetRecommendList;
        private event MyAction<bool, JsonData> OnGetClassifyManageList;
        private event MyAction<bool, JsonData> OnGetDesignerProductList;
        private event MyAction<bool, JsonData> OnGetLabelManageList;
        public void Tick() {
        }

        public bool Init() {
            DicSizeInfo = new Dictionary<string, SizeInfo>();
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (null == _networkMgr) {
                LogSystem.Warn("network is null.");
                return false;
            }
            _networkMgr.QueryTigger(TriggerNames.OrderTirgger).AddListener(this);
            _networkMgr.QueryTigger(TriggerNames.DesignClothesTrigger).AddListener(this);
            _networkMgr.QueryTigger(TriggerNames.InfoTrigger).AddListener(this);
            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Warn("user center is null.");
                return false;
            }
            return true;
        }

        public void QueryOrderList(string pageNo, string pageSize, string orderStatus, MyAction<bool, List<OrderInfo>, string> callback) {
            OnQueryOrder += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetOrderListReq(pageNo, pageSize, orderStatus);
        }

        public void CancleOrder(string orderId, MyAction<bool, string> callback) {
            OnCancleOrder += callback;
            _networkMgr.QuerySender<IOrderSender>().SendCancleOrderReq(orderId);
        }

        public void ModifyOrderStatus(string orderId, string status, MyAction<bool, List<OrderInfo>> callback) {
            if (_modifyOrdId == string.Empty) {
                OnModifyOrder += callback;
                _modifyOrdId = orderId;
                _modifyStatus = status;
                _networkMgr.QuerySender<IOrderSender>().SendModifyOrderStatuReq(orderId, status);
            } else {
                if (null != callback)
                    callback(false, _lstOrder);
            }
        }

        public void SubmitOrder(string orderInfoJson, string shopCartJson, string couponid, MyAction<bool,JsonData> callback) {
            if (_submitOrder == string.Empty) {
                _submitOrder = orderInfoJson;
                OnSubmitOrder += callback;
                _networkMgr.QuerySender<IOrderSender>().SendSubmitOrderReq(orderInfoJson, shopCartJson, couponid);
            } else {
                if (callback != null) {
                    JsonData json = new JsonData();
                    callback(false, json);
                }
            }
        }

        public void GetDeliveryDate(MyAction<bool, string> callback) {
            OnDeliveryDate += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetOrderDeliveryDateReq();
        }

        public void GetOrderDetail(string orderCode, MyAction<bool, JsonData> callback) {
            if (_orderDetailId == string.Empty) {
                _orderDetailId = orderCode;
                OnOrderDetail += callback;
                _networkMgr.QuerySender<IOrderSender>().SendGetOrderDetailReq(orderCode);
            } else {
                if (callback != null) {
                    JsonData json = new JsonData();
                    callback(false, json);
                }
            }
        }

        public void SubmitOrderEvaluate(string paras, IList<KeyValuePair<string, byte[]>> files, string fileExtension, MyAction<bool> callback){
            if (_orderParas == string.Empty) {
                _orderParas = paras;
                if(callback!=null)
                    OnSubmitOrderEvaluate += callback;
                _networkMgr.QuerySender<IOrderSender>().SendSubmitEvaluateReq(paras, files, fileExtension);
            } else {
                if (callback != null) {
                    callback(false);
                }
            }
        }

        public void GetShopCarList(MyAction<bool, List<ShopCarItem>> callback) {
            if (callback != null)
                OnGetShopCarList += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetShopCarListReq();
        }

        public void DeleteShorCarItem(string id, MyAction<bool, string> callback) {
            if (callback != null)
                OnDeleteShopCarItems += callback;
            _networkMgr.QuerySender<IOrderSender>().SendDeleteShopCarItemReq(id);
        }
        
        public void GetShopCarDetail(string shopCarId, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnShopCarDetail += callback;
            _networkMgr.QuerySender<IOrderSender>().SendShopCarDetail(shopCarId);
        }

        public void SaveShopCar(string goodsJson, IList<KeyValuePair<string, byte[]>> files, string shirtDesignerCode, MyAction<bool, JsonData> callback) {
            if (callback != null) 
                OnSaveShopCar += callback;
            _networkMgr.QuerySender<IOrderSender>().SendSaveShopCarReq(goodsJson, files, shirtDesignerCode);
        }

        public void OneKeyBuyProduct(string goodsJson, IList<KeyValuePair<string, byte[]>> files, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnOneKeyBuyProduct += callback;
            _networkMgr.QuerySender<IOrderSender>().SendOneKeyBuyProductReq(goodsJson,files);
        }

        public void GetOrderEvaluationList(MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnOrderEvaluationList += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetOrderEvaluationList();
        }


        public void GetCouponList(string amount, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnGetCouponList += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetCouponListReq(amount);
        }

        public void GetMeasureByPropertiesCode(string propertiesCode, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnGetMeasureByPropertiesCode += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetMeasureByPropertiesCodeReq(propertiesCode);
        }

        public void GetGoodsInfo(string goodsId, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnGetGoodsInfo += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetGoodsInfoReq(goodsId);
        }

        public void GetFabricDetail(string fabricCode, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnGetFabricDetail += callback;
            List<string> _lstFabricCode = new List<string>();
            _lstFabricCode.Add(fabricCode);
            _networkMgr.QuerySender<IDesignClothesSender>().SendGetFabricDetailReq(_lstFabricCode);
        }

        public void GetRecommendList(int pageNo, int pageSize, string modelId, string type, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnGetRecommendList += callback;
            _networkMgr.QuerySender<IInfoSender>().SendGetRecommendListReq(pageNo, pageSize, modelId, type);
        }

        public void GetClassifyManageListReq(string pageNo, string pageSize, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnGetClassifyManageList += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetClassifyManageListReq(pageNo, pageSize);
        }

        public void GetDesignerProductListReq(string pageNo, string pageSize, string type, string label,
            string minPrice, string maxPrice, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnGetDesignerProductList += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetDesignerProductListReq(pageNo, pageSize, type, label, minPrice, maxPrice);
        }

        public void GetLabelManageListReq(string pageNo, string pageSize, MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnGetLabelManageList += callback;
            _networkMgr.QuerySender<IOrderSender>().SendGetLabelManageListReq(pageNo, pageSize);
        }
        public void OnSaveShopCarRsp(JsonData msg) {
            var callback = OnSaveShopCar;
            OnSaveShopCar = null;
            IDictionary json = msg;
            if (json.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    if (callback != null) {
                        JsonData jsonData = new JsonData();
                        callback(false, jsonData);
                    }
                    LogSystem.Warn("Submit Order return failed.");
                }
            } else {
                if (callback != null) {
                    JsonData jsonData = new JsonData();
                    callback(false, jsonData);
                }
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnOneKeyBuyProductRsp(JsonData msg) {
            var callback = OnOneKeyBuyProduct;
            OnOneKeyBuyProduct = null;
            IDictionary json = msg;
            if (json.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    if (callback != null) {
                        JsonData jsonData = new JsonData();
                        callback(false, jsonData);
                    }
                    LogSystem.Warn("Submit Order return failed.");
                }
            } else {
                if (callback != null) {
                    JsonData jsonData = new JsonData();
                    callback(false, jsonData);
                }
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnGetShopCarListRsp(JsonData msg) {
            var callback = OnGetShopCarList;
            OnGetShopCarList = null;
            IDictionary json = msg;
            if (json.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    var data = msg["data"]["shopCart"];
                    List<ShopCarItem> lstInfo = new List<ShopCarItem>();
                    for (int i = 0; i < data.Count; i++) {
                        var item = new ShopCarItem();
                        item.ModelId = Utils.GetJsonStr(data[i]["modelId"]);
                        item.Id = Utils.GetJsonStr(data[i]["atid"]);
                        item.GoodsId = Utils.GetJsonStr(data[i]["goodsId"]);
                        item.GoodsPic = Utils.GetJsonStr(data[i]["image"]);
                        item.GoodsNum = int.Parse(Utils.GetJsonStr(data[i]["goodsNumber"])); ;
                        item.GoodsPrice = Utils.GetJsonStr(data[i]["goodsPrice"]);
                        item.GoodsName = Utils.GetJsonStr(data[i]["goodsName"]);
                        lstInfo.Add(item);
                    }
                    if (callback != null)
                        callback(true, lstInfo);
                } else {
                    if (callback != null)
                        callback(false, null);
                    LogSystem.Error("network requeset is failed.");
                }
            } else {
                if (callback != null)
                    callback(false, null);
                LogSystem.Error("network requeset is failed.");
            }
        }

        public void OnDeleteShopCarItemRsp(JsonData msg) {
            var callback = OnDeleteShopCarItems;
            OnDeleteShopCarItems = null;
            IDictionary json = msg;
            if (json.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true, Utils.GetJsonStr(msg["data"]["ids"][0]));
                } else {
                    if (callback != null)
                        callback(false, string.Empty);
                    LogSystem.Error("network requeset is failed.");
                }
            } else {
                if (callback != null)
                    callback(false, string.Empty);
                LogSystem.Error("network requeset is failed.");
            }
        }

        private void ParseOrder(JsonData jsonData) {
            var data = jsonData["order"];
            _lstOrder = new List<OrderInfo>();
            for (var i = 0; i < data.Count; i++) {
                var ord = new OrderInfo();
                ord.OrderCode = Utils.GetJsonStr(data[i]["orderCode"]);
                ord.OrderAmount = Utils.GetJsonStr(data[i]["actualAmount"]);
                ord.OrderStatus = Utils.GetJsonStr(data[i]["orderStatus"]);
                ord.OrderMemo = Utils.GetJsonStr(data[i]["remark"]);
                ord.OrderId = Utils.GetJsonStr(data[i]["atid"]);
                ord.ClothNumber = Utils.GetJsonStr(data[i]["totalNumber"]);
                ord.Store_id = Utils.GetJsonStr(data[i]["shopId"]);
                ord.Order_address = Utils.GetJsonStr(data[i]["address"]);
                ord.Order_tel = Utils.GetJsonStr(data[i]["telphone"]);
                ord.Order_contact = Utils.GetJsonStr(data[i]["consignee"]);
                ord.Order_time = Utils.GetJsonStr(data[i]["orderDate"]);
                ord.OrderPrice = Utils.GetJsonStr(data[i]["totalAmount"]);
                var json = data[i]["orderGoods"]["resultSet"];
                for (int j = 0; j < json.Count; j++) {
                    var goods = new Goods();
                    goods.OrderId = Utils.GetJsonStr(json[j]["orderId"]);
                    goods.GoodsId = Utils.GetJsonStr(json[j]["goodsId"]);
                    goods.GoodsName = Utils.GetJsonStr(json[j]["goodsName"]);
                    goods.GoodsNumber = Utils.GetJsonStr(json[j]["goodsNumber"]);
                    goods.GoodsPrice = Utils.GetJsonStr(json[j]["goodsPrice"]);
                    goods.AgoodsDescription = Utils.GetJsonStr(json[j]["goodsDescription"]);
                    ord.Order_Goods.Add(goods);
                }
                _lstOrder.Add(ord);
            }
        }

        private void ParseOrderListJson(JsonData jsonData) {
            var data = jsonData["order"];
            _lstOrder = new List<OrderInfo>();
            for (var i = 0; i < data.Count; i++) {
                var ord = new OrderInfo();
                ord.OrderCode = Utils.GetJsonStr(data[i]["orderCode"]);
                ord.OrderAmount = Utils.GetJsonStr(data[i]["actualAmount"]);
                ord.OrderStatus = Utils.GetJsonStr(data[i]["orderStatus"]);
                ord.OrderId = Utils.GetJsonStr(data[i]["atid"]);
                ord.ClothNumber = Utils.GetJsonStr(data[i]["totalNumber"]);
                ord.Store_id = Utils.GetJsonStr(data[i]["shopId"]);
                ord.OrderPrice = Utils.GetJsonStr(data[i]["totalAmount"]);
                ord.OrderImage = Utils.GetJsonStr(data[i]["image"]);
                _lstOrder.Add(ord);
            }
        }

        public void OnGetOrderListRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnQueryOrder;
            OnQueryOrder = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
//                    ParseOrder(msg["data"]);
                    ParseOrderListJson(msg["data"]);
                    if (callback != null) {
                        callback(true, _lstOrder, Utils.GetJsonStr(msg["data"]["count"]));
                    }
                } else {
                    LogSystem.Warn("get Order req return failed.");
                    if (callback != null) {
                        callback(false, _lstOrder, "0");
                    }
                }
            } else {
                LogSystem.Warn("require network is failed.");
                if (callback != null) {
                    callback(false, _lstOrder, "0");
                }
            }
        }

        public void OnCancleOrderRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnCancleOrder;
            OnCancleOrder = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    var json = msg["data"];
                    var id = Utils.GetJsonStr(json["ids"]);
                    for (int i = 0; i < _lstOrder.Count; i++) {
                        if (_lstOrder[i].OrderId == id) {
                            _lstOrder.RemoveAt(i);
                            break;
                        }
                    }
                    if (callback != null)
                        callback(true, id);
                } else {
                    if (callback != null)
                        callback(false, "-1");
                    LogSystem.Error("Cancle order return failed.");
                }
            } else {
                if (callback != null)
                    callback(false, "-1");
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnModifyOrderStatuRsp(JsonData msg) {
            IDictionary dic = msg;
            var id = _modifyOrdId;
            _modifyOrdId = string.Empty;
            var status = _modifyStatus;
            _modifyStatus = string.Empty;
            var callback = OnModifyOrder;
            OnModifyOrder = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    for (int i = 0; i < _lstOrder.Count; i++) {
                        if (_lstOrder[i].OrderId == id) {
                            _lstOrder[i].OrderStatus = status;
                            break;
                        }
                    }
                    if (callback != null)
                        callback(true, _lstOrder);
                } else {
                    if (callback != null)
                        callback(false, _lstOrder);
                    LogSystem.Warn("Modify Order Statu return failed.");
                }
            } else {
                if (callback != null)
                    callback(false, _lstOrder);
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnSubmitOrderRsp(JsonData msg) {
            IDictionary dic = msg;
            _submitOrder = string.Empty;
            var callback = OnSubmitOrder;
            OnSubmitOrder = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    if (callback != null) {
                        JsonData json = new JsonData();
                        callback(false, json);
                    }
                    UiControllerMgr.Instance.ShowTips(Utils.GetJsonStr(msg["messages"][0]));
                    LogSystem.Warn("Submit Order return failed.");
                }
            } else {
                if (callback != null) {
                    JsonData json = new JsonData();
                    callback(false, json);
                }
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnGetOrderDetailRsp(JsonData msg) {
            IDictionary dic = msg;
            _orderDetailId = string.Empty;
            var callback = OnOrderDetail;
            OnOrderDetail = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)                   
                     callback(true, msg["data"]);
                } else {
                    if (callback != null) {
                        JsonData json = new JsonData();
                        callback(false, json);
                    }
                    LogSystem.Warn("Get OrderDetail return failed.");
                }
            } else {
                if (callback != null) {
                    JsonData json = new JsonData();
                    callback(false, json);
                }
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnGetOrderDeliveryDateRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnDeliveryDate;
            OnDeliveryDate = null;
            if (dic.Contains("exit")) {
                if (Utils.GetJsonStr(msg["exit"]) == "0") {
                    if (callback != null)
                        callback(true, Utils.GetJsonStr(msg["data"]));
                } else {
                    if (callback != null)
                        callback(false, string.Empty);
                    LogSystem.Warn("Get Order DeliveryDate return failed.");
                }
            } else {
                if (callback != null)
                    callback(false, string.Empty);
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnSubmitEvaluateRsp(JsonData msg) {
            IDictionary dic = msg;
            _orderParas = string.Empty;
            var callback = OnSubmitOrderEvaluate;
            OnSubmitOrderEvaluate = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true);
                } else {
                    if (callback != null)
                        callback(false);
                    LogSystem.Warn("Submit Order Evaluate return failed.");
                }
            } else {
                if (callback != null)
                    callback(false);
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnGetReferenceSizeRsp(JsonData msg) {
        }
        
        public void OnGetShopCarDetail(JsonData msg) {
            IDictionary dic = msg;
            _shopCarId = string.Empty;
            var callback = OnShopCarDetail;
            OnShopCarDetail = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    if (callback != null) {
                        JsonData json = new JsonData();
                        callback(false, json);
                    }
                    LogSystem.Warn("Get ShopCarDetail return failed.");
                }
            } else {
                if (callback != null) {
                    JsonData json = new JsonData();
                    callback(false, json);
                }
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnGetOrderEvaluationList(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnOrderEvaluationList;
            OnOrderEvaluationList = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    if (callback != null) {
                        JsonData json = new JsonData();
                        callback(false, json);
                    }
                    LogSystem.Warn("Get OrderEvaluationList return failed.");
                }
            } else {
                if (callback != null) {
                    JsonData json = new JsonData();
                    callback(false, json);
                }
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnGetWishListRsp(JsonData msg) {
        }public void OnDeleteShirtTempletRsp(JsonData msg) {
        }public void OnGetCollectDetailRsp(JsonData msg) {
        }public void OnSaveWishiListRsp(JsonData msg) {
        }public void OnGetModelSizeListRsp(JsonData msg) {
        }public void OnGetUserSizeListRsp(JsonData msg) {
        }public void OnSaveUserSizeRsp(JsonData msg) {
        }public void OnDeleteUserSizeRsp(JsonData msg) {
        }public void OnQueryFabricListRsq(JsonData msg) {
        }

        public void OnGetCouponListRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnGetCouponList;
            OnGetCouponList = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    if (callback != null) {
                        JsonData json = new JsonData();
                        callback(false, json);
                    }
                    LogSystem.Warn("Get CouponList return failed.");
                }
            } else {
                if (callback != null) {
                    JsonData json = new JsonData();
                    callback(false, json);
                }
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnGetMeasureByPropertiesCodeRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnGetMeasureByPropertiesCode;
            OnGetMeasureByPropertiesCode = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    if (callback != null) {
                        callback(false, null);
                    }
                    LogSystem.Warn("Get Measure By PropertiesCode return failed.");
                }
            } else {
                if (callback != null) {
                    callback(false, null);
                }
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnGetGoodsInfoRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnGetGoodsInfo;
            OnGetGoodsInfo = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    if (callback != null) {
                        callback(false, null);
                    }
                    LogSystem.Warn("Get GoodsInfo return failed.");
                }
            } else {
                if (callback != null) {
                    callback(false, null);
                }
                LogSystem.Warn("network require failed.");
            }
        }

        public void OnGetFabricDetailRsq(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnGetFabricDetail;
            OnGetFabricDetail = null;
            if (dic.Contains("resultCode")) {
                if ((int) msg["resultCode"] == 1 && (int) msg["data"]["recordCount"] > 0) {
                    if (callback != null)
                        callback(true, msg["data"]["fabricDetail"][0]);
                }
                else {
                    LogSystem.Error("Get FabricDetail req return failed");
                    if (callback != null)
                        callback(false, null);
                }
            }
            else {
                LogSystem.Error("req return returns");
                if (callback != null)
                    callback(false, null);
            }
        }

        public void OnGetRecommendListRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnGetRecommendList;
            OnGetRecommendList = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    LogSystem.Error("Get FabricDetail req return failed");
                    if (callback != null)
                        callback(false, null);
                }
            } else {
                LogSystem.Error("req return returns");
                if (callback != null)
                    callback(false, null);
            }
        }

        public void OnCountProductInfoPriceRsq(JsonData msg) {
        }public void OnGetNoticeListRsp(JsonData msg) {
        }public void OnFashionListSearchRsp(JsonData msg) {
        }public void OnFashionDetailRsp(JsonData msg) {
        }public void OnGetRecommendDetailRsp(JsonData msg) {
        }public void OnGetDefaultProductInfoRsp(JsonData msg) {
        }
        public void OnGetClassifyManageListRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnGetClassifyManageList;
            OnGetClassifyManageList = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    LogSystem.Error("GetClassifyManageList req return failed");
                    if (callback != null)
                        callback(false, null);
                }
            } else {
                LogSystem.Error("req return returns");
                if (callback != null)
                    callback(false, null);
            }
        }

        public void OnGetDesignerProductListRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnGetDesignerProductList;
            OnGetDesignerProductList = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    if (callback != null) 
                        callback(true, msg["data"]);
                } else {
                    LogSystem.Error("OnGetDesignerProductListRsp req return failed");
                    if (callback != null)
                        callback(false, null);
                }
            } else {
                LogSystem.Error("req return returns");
                if (callback != null)
                    callback(false, null);
            }
        }

        public void OnGetLabelManageListRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnGetLabelManageList;
            OnGetLabelManageList = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    LogSystem.Error("OnGetLabelManageListRsp req return failed");
                    if (callback != null)
                        callback(false, null);
                }
            } else {
                LogSystem.Error("req return returns");
                if (callback != null)
                    callback(false, null);
            }
        }
    }
}