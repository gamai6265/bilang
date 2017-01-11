using System.Collections.Generic;
using Cloth3D.Data;
using LitJson;

namespace Cloth3D.Interfaces {
    interface IOrderCenter : IComponent {
        string FabricFigureCode { get; set; }
        Dictionary<string, SizeInfo> DicSizeInfo { get; set; }
        JsonData CouponList { get; set; }
        void QueryOrderList(string pageNo, string pageSize, string orderStatus, MyAction<bool, List<OrderInfo>, string> callback);
        void CancleOrder(string orderId, MyAction<bool,string> callback);
        void ModifyOrderStatus(string orderId, string status, MyAction<bool, List<OrderInfo>> callback);
        void SubmitOrder(string orderInfoJson, string shopCartJson, string couponid, MyAction<bool,JsonData> callback);
        void GetDeliveryDate(MyAction<bool, string> callback);
        void GetOrderDetail(string orderCode, MyAction<bool, JsonData> callback);
        void SubmitOrderEvaluate(string paras, IList<KeyValuePair<string, byte[]>> files, string fileExtension,
            MyAction<bool> callback);

        void GetShopCarList(MyAction<bool, List<ShopCarItem>> callback);
        void DeleteShorCarItem(string id, MyAction<bool, string> callback);
        void GetShopCarDetail(string shopCarId, MyAction<bool, JsonData> callback);
        void SaveShopCar(string goodsJson, IList<KeyValuePair<string, byte[]>> files, string shirtDesignerCode, MyAction<bool, JsonData> callback);
        void OneKeyBuyProduct(string goodsJson, IList<KeyValuePair<string, byte[]>> files,
            MyAction<bool, JsonData> callback);
        void GetOrderEvaluationList(MyAction<bool, JsonData> callback);
        void GetCouponList(string amount, MyAction<bool, JsonData> callback);
        void GetMeasureByPropertiesCode(string propertiesCode, MyAction<bool, JsonData> callback);
        void GetGoodsInfo(string goodsId, MyAction<bool, JsonData> callback);
        void GetFabricDetail(string fabricCode, MyAction<bool, JsonData> callback);
        void GetRecommendList(int pageNo, int pageSize, string modelId, string type, MyAction<bool, JsonData> callback);
        void GetClassifyManageListReq(string pageNo, string pageSize, MyAction<bool, JsonData> callback);
        void GetDesignerProductListReq(string pageNo, string pageSize, string type, string label, string minPrice, string maxPrice, MyAction<bool, JsonData> callback);
        void GetLabelManageListReq(string pageNo, string pageSize, MyAction<bool, JsonData> callback);
    }
}
