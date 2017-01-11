using System.Collections.Generic;
using Cloth3D.Data;
using LitJson;

namespace Cloth3D.Interfaces{
    public class TriggerNames {
        public const string DesignClothesTrigger= "DesignClothesTrigger";
        public const string LoginTrigger = "LoginTrigger";
        public const string CodeSetTrigger = "CodeSetTrigger";
        public const string InfoTrigger = "InfoTigger";
        public const string OrderTirgger = "OrderTigger";
        public const string UserInfoTrigger = "UserInfoTrigger";
    }
    public interface IRequestHeader {
        void SetCookie(string cookie);
        void SetApiVersion(string apiVersion);
        void SetToken(string token);
    }
    //==========================================================================================
    //设计
    //==========================================================================================
    public class FabricQuery {
        public string QueryType = string.Empty;
        public string PageNo = "1";
        public string PageSize = "5";
        public string MateCode = string.Empty;
        public string FabricColor = string.Empty;
        public string FabricTexture = string.Empty;
        public string FabricFigure = string.Empty;
        public string FabricPly = string.Empty;
        public string FabricOrigin = string.Empty;
        public string Brand = string.Empty;
        public string YarnCount = string.Empty;
        public string MateType = string.Empty;
        public string MateName = string.Empty;
    }
    
    public interface IDesignClothesSender {
        void SendGetWishListReq();
        void SendDeleteShirtTempletReq(string templetCode);
        void SendGetCollectDetailReq(string id);
        void SendSaveWishListReq(string productWishJson, IList<KeyValuePair<string, byte[]>> files);
        void SendGetModelSizeListReq(string id , string modelid, string type, string height, string weight);
        void SendGetUserSizeListReq(string modelId);//, string id);
        void SendSaveUserSizeReq(string id,string atid, string buyerPropertiesJson);
        void SendDeleteUserSizeReq(string id);
        void SendQueryFabricListReq(FabricQuery queryInfo);
        void SendGetFabricDetailReq(List<string> lstFabricCode);
        void SendCountProductInfoPriceReq(string modelId, List<PartInfo> lstPartInfos);
    }

    public interface IDesignClothesListener {
        void OnGetWishListRsp(JsonData msg);
        void OnDeleteShirtTempletRsp(JsonData msg);
        void OnGetCollectDetailRsp(JsonData msg);
        void OnSaveWishiListRsp(JsonData msg);
        void OnGetModelSizeListRsp(JsonData msg);
        void OnGetUserSizeListRsp(JsonData msg);
        void OnSaveUserSizeRsp(JsonData msg);
        void OnDeleteUserSizeRsp(JsonData msg);
        void OnQueryFabricListRsq(JsonData msg);
        void OnGetFabricDetailRsq(JsonData msg);
        void OnCountProductInfoPriceRsq(JsonData msg);
    }
    //==========================================================================================
    //登录
    //==========================================================================================
    public interface ILoginSender {
        bool SendLogoutReq();
        bool SendLoginReq(string userName, string password);

        bool SendCheckRegisterCodeReq(string tel, string code, string password);
        void SendGetRegisterCodeReq(string tel);
        void SendRegisterReq(string code, string tel, string sex, string userAccount, string password, string realName);

        bool SendGetResetPwCodeReq(string tel);
        bool SendResetPasswordReq(string tel, string password, string code);
    }

    public interface ILoginListener {
        void OnLogoutRsp(JsonData msg);
        void OnLoginRsp(JsonData msg);
        
        bool OnGetRegisterCodeRsp(JsonData msg);
        void OnCheckRegisterCodeRsp(JsonData msg);
        void OnRegisterRsp(JsonData msg);

        void OnGetResetPwCodeRsp(JsonData msg);
        void OnResetPasswordRsp(JsonData msg);
    }
    //==========================================================================================
    //订单
    //==========================================================================================
    public interface IOrderSender {
        void SendSaveShopCarReq(string goodsJson, IList<KeyValuePair<string, byte[]>> files, string shirtDesignerCode);
        void SendOneKeyBuyProductReq(string goodsJson, IList<KeyValuePair<string, byte[]>> files);
        void SendGetShopCarListReq();
        void SendDeleteShopCarItemReq(string id);
        void SendGetOrderListReq(string pageNo, string pageSize, string orderStatus);
        void SendCancleOrderReq(string orderCode);
        void SendModifyOrderStatuReq(string id, string status);
        void SendSubmitOrderReq(string orderInfoJson, string shopCartJson, string couponid);
        void SendGetOrderDetailReq(string orderCode);
        void SendGetOrderDeliveryDateReq();
        void SendSubmitEvaluateReq(string paras, IList<KeyValuePair<string, byte[]>> files, string fileExtension);
        void SendGetReferenceSizeReq();
        void SendShopCarDetail(string shopCarId);
        void SendGetOrderEvaluationList();
        void SendGetCouponListReq(string amount);
        void SendGetMeasureByPropertiesCodeReq(string propertiesCode);
        void SendGetGoodsInfoReq(string goodsId);
        void SendGetClassifyManageListReq(string pageNo, string pageSize);
        void SendGetDesignerProductListReq(string pageNo, string pageSize, string type, string label, string minPrice, string maxPrice);
        void SendGetLabelManageListReq(string pageNo, string pageSize);
    }

    public interface IOrderListener {
        void OnSaveShopCarRsp(JsonData msg);
        void OnOneKeyBuyProductRsp(JsonData msg);
        void OnGetShopCarListRsp(JsonData msg);
        void OnDeleteShopCarItemRsp(JsonData msg);
        void OnGetOrderListRsp(JsonData msg);
        void OnCancleOrderRsp(JsonData msg);
        void OnModifyOrderStatuRsp(JsonData msg);
        void OnSubmitOrderRsp(JsonData msg);
        void OnGetOrderDetailRsp(JsonData msg);
        void OnGetOrderDeliveryDateRsp(JsonData msg);
        void OnSubmitEvaluateRsp(JsonData msg);
        void OnGetReferenceSizeRsp(JsonData msg);
        void OnGetShopCarDetail(JsonData msg);
        void OnGetOrderEvaluationList(JsonData msg);
        void OnGetCouponListRsp(JsonData msg);
        void OnGetMeasureByPropertiesCodeRsp(JsonData msg);
        void OnGetGoodsInfoRsp(JsonData msg);
        void OnGetClassifyManageListRsp(JsonData msg);
        void OnGetDesignerProductListRsp(JsonData msg);
        void OnGetLabelManageListRsp(JsonData msg);
    }

    //==========================================================================================
    //用户信息
    //==========================================================================================
    public interface IUserInfoSender {
        void SendQueryUserInfoReq(string token);
        void SendModifyUserInfoReq(string userName, IList<KeyValuePair<string, byte[]>> files);
        void SendGetAddressReq(int pageNo=0,int pageSize=0);
        void SendDeleteAddressReq(string addressId);
        void SendSetDefaultAddressReq(string addreId);
        void SendModifyAddressReq(Address addr, string userId,string addr_id);
        void SendNewAddressReq(string addr);
        void SendGetUserSizeReq(string token);
        void SendGetHistorySizeReq(string customerPhone);
        void SendModifyUserSize(string type, string val);
        void SendShareUrl();
        void SendGetCityListReq(string parentCode);
        void SendGetAreaReq(string parentCode);
        void SendAddOrUpdateModelSizeReq(string modelId, string modelSizeJson);
    }
    public interface IUserInfoListener {
        void OnQueryUserInfoRsp(JsonData msg);
        void OnModifyUserInfoRsp(JsonData msg);
        void OnGetAddressRsp(JsonData msg);
        void OnDeleteAddressRsp(JsonData msg);
        void OnSetDefaultAddressRsp(JsonData msg);
        void OnModifyAddressRsp(JsonData msg);
        void OnNewAddressRsp(JsonData msg);
        void OnGetUserSizeRsp(JsonData msg);
        void OnGetHistorySizeRsp(JsonData msg);
        void OnModifyUserSizeRsp(JsonData msg);
        void OnShareUrl(JsonData msg);
        void OnGetCityListRsp(JsonData msg);
        void OnGetAreaRsp(JsonData msg);
        void OnAddOrUpdateModelSizeRsq(JsonData msg);
    }

    //==========================================================================================
    //系统集
    //==========================================================================================
    public interface ICodeSetSender {
        void SendGetPricePropertysReq();
        void SendGetCodeSetReq(string codeSet);
    }

    public interface ICodeSetListener {
        void OnGetPricePropertysRsp(JsonData msg);
        void OnGetCodesetRsp(JsonData msg);
    }
    //==========================================================================================
    //资讯
    //==========================================================================================
    public interface IInfoSender {
        void SendGetNoticeListReq();
        void SendFashionListSearchReq();
        void SendFashionDetailReq(string id);
        void SendGetRecommendListReq(int pageNo = 0, int pageSize = 0, string modelId = null, string type = null);
        void SendGetRecommendDetailReq(string id);
        void SendGetDefaultProductInfo(string modelId);
    }

    public interface IInfoListener {
        void OnGetNoticeListRsp(JsonData msg);
        void OnFashionListSearchRsp(JsonData msg);
        void OnFashionDetailRsp(JsonData msg);
        void OnGetRecommendListRsp(JsonData msg);
        void OnGetRecommendDetailRsp(JsonData msg);
        void OnGetDefaultProductInfoRsp(JsonData msg);
    }
}
