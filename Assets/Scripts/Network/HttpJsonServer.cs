using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using HttpRequest;
using LitJson;

namespace Cloth3D.Network {
    //TODO danie 缓存  ReqInfo,RequestHttpWebRequest,
    public class HttpJsonServer : IDesignClothesSender, ILoginSender, IProtolProcessor, IRequestHeader, ICodeSetSender,
        IInfoSender, IOrderSender, IUserInfoSender{
        private readonly NetworkMgr _networkMgr;
        private Dictionary<string, string> _dicParam = new Dictionary<string, string>();
        private string _cookie = string.Empty; // not null
        private string _tokeon = string.Empty;
        private string _apiVersion = "";

        public HttpJsonServer(NetworkMgr networkMgr) {
            _networkMgr = networkMgr;
        }

        public void SendGetWishListReq() {
            LogSystem.Debug("SendGetWishListReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "product/productwish/getWishList") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData =
                    new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsGetShirtTempletList).ToString()}}
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendDeleteShirtTempletReq(string id) {
            LogSystem.Debug("SendDeleteShirtTempletReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "product/productwish/deleteWishList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsDeleteShirtTemplet).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("ids", id);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetCollectDetailReq(string id) {
            LogSystem.Debug("SendGetCollectDetailReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "product/productwish/queryWishListDetail") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetCollectDetail).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("id", id);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendSaveWishListReq(string productWishJson, IList<KeyValuePair<string, byte[]>> files) {
            LogSystem.Debug("SendSaveWishListReq");
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var http = new RequestHttpWebRequest();
             var reqInfo = new RequestInfo(Constants.OrdersysURL + "product/productwish/addProductToWishList") {
                 Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } ,
                    { "ContentType", "multipart/form-data; boundary=" + boundary } },
                 ExternalData =
                     new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsCollectDesign).ToString() } }
             };

             _dicParam.Clear();
             _dicParam.Add("token", _tokeon);
             _dicParam.Add("productWishJson", productWishJson);

            reqInfo.PostData = SerialUploadFiles(_dicParam, files, ".jpg", boundary);
            //             reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetModelSizeListReq(string id, string modelid, string type, string height, string weight) {
            LogSystem.Debug("SendGetModelSizeListReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "template/templateproperties/getModelSizeList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetModelSize).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("id", id);
            _dicParam.Add("modelId", modelid);
            _dicParam.Add("type", type);
            _dicParam.Add("height", height);
            _dicParam.Add("weight", weight);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetUserSizeListReq(string modelId) {//, string atid) {
            LogSystem.Debug("SendGetUserSizeListReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "buyer/buyerproperties/getBuyerSizeList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetUserSizeList).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("id", string.Empty);
            _dicParam.Add("modelId", modelId);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendSaveUserSizeReq(string id, string atid, string buyerPropertiesJson) {
            LogSystem.Debug("SendSaveModelSizeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "buyer/buyerproperties/addOrUpdateBuyerSize") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsSaveModelSize).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("modelId", id);
            _dicParam.Add("id", atid);
//            _dicParam.Add("propertiesType", propertiesType);
            _dicParam.Add("buyerPropertiesJson", buyerPropertiesJson);
            
            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendDeleteUserSizeReq(string id) {
            LogSystem.Debug("SendDeleteModelSizeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "buyer/buyerproperties/deleteBuyerSize") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsDeleteModelSize).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("ids", id);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendQueryFabricListReq(FabricQuery queryInfo) {
            LogSystem.Debug("SendQueryFabricListReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL2 + "apis/FabricInformationApiAction!queryFabricList.do") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsQueryFabricList).ToString() } }
            };
            JsonData data = new JsonData();
            data["queryType"] = queryInfo.QueryType;
            data["pageNo"] = queryInfo.PageNo;
            data["pageSize"] = queryInfo.PageSize;
            data["mateCode"] = queryInfo.MateCode;
            data["fabricColor"] = queryInfo.FabricColor;
            data["fabricTexture"] = queryInfo.FabricTexture;
            data["fabricFigure"] = queryInfo.FabricFigure;
            data["fabricPly"] = queryInfo.FabricPly;
            data["fabricOrigin"] = queryInfo.FabricOrigin;
            data["brand"] = queryInfo.Brand;
            data["yarnCount"] = queryInfo.YarnCount;
            data["mateType"] = queryInfo.MateType;
            data["mateName"] = queryInfo.MateName;
            data["custmerId"] = "20";

            /*
            LogSystem.Debug("~~~~~~~~~~~~~~~~~~~~~");
            LogSystem.Debug("queryInfo.QueryType="+queryInfo.QueryType);
            LogSystem.Debug("queryInfo.PageNo=" + queryInfo.PageNo);
            LogSystem.Debug("queryInfo.FabricFigure=" + queryInfo.FabricFigure);
            LogSystem.Debug("queryInfo.Brand=" + queryInfo.Brand);
            LogSystem.Debug("queryInfo.MateType=" + queryInfo.MateType);
            LogSystem.Debug("queryInfo.mateName=" + queryInfo.MateName);
            LogSystem.Debug("~~~~~~~~~~~~~~~~~~~~~");
            */

            _dicParam.Clear();
            _dicParam.Add("paras", data.ToJson());
            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetFabricDetailReq(List<string> lstFabricCode) {
            LogSystem.Debug("SendGetFabricDetailReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL2 + "apis/FabricInformationApiAction!getFabricDetail.do") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData =
                    new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsGetFabricDetail).ToString()}}
            };

            _dicParam.Clear();

            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < lstFabricCode.Count; i++) {
                if (i != 0)
                    sb.Append(",");
                sb.Append(lstFabricCode[i]);
            }
            JsonData js = new JsonData();
            js["mateCode"] = sb.ToString();
            _dicParam.Add("paras", js.ToJson());
            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendCountProductInfoPriceReq(string modelId, List<PartInfo> lstPartInfos) {
            LogSystem.Debug("SendCountProductInfoPriceReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "product/productinfo/countProductInfoPrice") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsCountProductInfoPrice).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("modelId", modelId);
            JsonData partJson = new JsonData();
            foreach (var partItem in lstPartInfos) {
                var item = new JsonData(); //TODO 
//                item["partId"] = partItem.PartId;
//                if (!string.IsNullOrEmpty(partItem.PartType))
//                item["partType"] = partItem.PartType;
                if (!string.IsNullOrEmpty(partItem.PartWeight)) {
                    item["partWeight"] = partItem.PartWeight;
                    if (!string.IsNullOrEmpty(partItem.Fabric2))
                        item["mateId2"] = partItem.Fabric2;
                    if (!string.IsNullOrEmpty(partItem.MateId1Price))
                        item["mateId1Price"] = partItem.MateId1Price;
                    if (!string.IsNullOrEmpty(partItem.MateId2Price))
                        item["mateId2Price"] = partItem.MateId2Price;
                }
                
//                if (!string.IsNullOrEmpty(partItem.Fabric1))
//                item["mateId1"] = partItem.Fabric1;
                
//                if (!string.IsNullOrEmpty(partItem.MateId1SuppType))
//                item["mateId1SuppType"] = partItem.MateId1SuppType;
//                if (!string.IsNullOrEmpty(partItem.MateId2SuppType))
//                item["mateId2SuppType"] = partItem.MateId2SuppType;
                partJson.Add(item);
            }
            _dicParam.Add("partJson", partJson.ToJson());
//            Debug.Log(modelId);
//            Debug.Log("--------------------------------"+partJson.ToJson());
            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendAddOrUpdateModelSizeReq(string modelId, string modelSizeJson) {
            LogSystem.Debug("SendAddOrUpdateModelSizeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "template/templateproperties/addOrUpdateModelSize") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsAddOrUpdateModelSize).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("modelId", modelId);
            _dicParam.Add("modelSizeJson", modelSizeJson);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }
        
        public bool SendLogoutReq() {
            LogSystem.Debug("SendLogoutReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + "logout.php") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsLogout).ToString() } }
            };

            _dicParam.Clear();

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
            return true;
        }

        public bool SendLoginReq(string userName, string password) {
            LogSystem.Debug("send login req");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.WSCURL + "?action=user&control=login") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData = new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsLogin).ToString()}}
            };

            _dicParam.Clear();
            _dicParam.Add("tel", userName);
            _dicParam.Add("password", password);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
            return true;
        }

        public bool SendCheckRegisterCodeReq(string tel, string code, string password) {
            LogSystem.Debug("SendCheckRegisterCodeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.WSCURL + "?action=user&control=operating") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData =
                    new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsCheckRegisterCode).ToString()}}
            };
            _dicParam.Clear();
            _dicParam.Add("tel", tel);
            _dicParam.Add("code", code);
            _dicParam.Add("password", password);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
            return true;
        }

        public void SendGetRegisterCodeReq(string tel) {
            LogSystem.Debug("SendGetVerifyCodeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.WSCURL + "?action=user&control=sms") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData = new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsGetRegisterCode).ToString()}}
            };
            _dicParam.Clear();
            _dicParam.Add("tel", tel);
            _dicParam.Add("type", "sms");

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendRegisterReq(string code, string tel, string sex, string userAccount, string password,
            string realName) {
                LogSystem.Debug("SendRegisterReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + "?action=user&control=operating") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData = new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsRegister).ToString()}}
            };
            var us = new JsonData();
            us["tel"] = tel;
            us["sex"] = sex;
            us["user_account"] = userAccount;
            us["user_password"] = password;
            us["user_name"] = realName;
            var user = us.ToJson();

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "register");
            _dicParam.Add("code", code);
            _dicParam.Add("paras", user);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public bool SendGetResetPwCodeReq(string tel) {
            LogSystem.Debug("SendGetResetPwCodeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.WSCURL + "?action=user&control=sms") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData = new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsGetResetPwCode).ToString()}}
            };
            _dicParam.Clear();
            _dicParam.Add("tel", tel);
            _dicParam.Add("type", "forget");

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
            return true;
        }

        public bool SendResetPasswordReq(string tel, string password, string code) {
            LogSystem.Debug("SendGetResetPwCodeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.WSCURL + "?action=user&control=forgetPassword") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData = new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsResetPassword).ToString()}}
            };
            _dicParam.Clear();
            _dicParam.Add("tel", tel);
            _dicParam.Add("password", password);
            _dicParam.Add("code", code);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
            return true;
        }

        public void SendQueryUserInfoReq(string token) {
            LogSystem.Debug("SendQueryUserInfoReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.WSCURL + "?action=user&control=info") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData = new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsQueryUserInfo).ToString()}}
            };
            _dicParam.Clear();
            _dicParam.Add("token", token);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendModifyUserInfoReq(string userName, IList<KeyValuePair<string, byte[]>> files) {
            LogSystem.Debug("SendModifyUserInfoReq ");
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "user/updateUser") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie},
                    { "ContentType", "multipart/form-data; boundary=" + boundary } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsModifyUserInfo).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("name", userName);

            reqInfo.PostData = SerialUploadFiles(_dicParam, files, ".jpg", boundary);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetAddressReq(int pageNo=0,int pageSize=0) {
            LogSystem.Debug("SendGetAddressReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "buyer/buyershippingaddress/getAddressList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetAddress).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
           // _dicParam.Add("PageNo", PageNo.ToString());
           // _dicParam.Add("PageSize", PageSize.ToString());

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendDeleteAddressReq(string addressId) {
            LogSystem.Debug("SendDeleteAddressReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "buyer/buyershippingaddress/deleteAddress") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsDeleteAddress).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("ids", addressId);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
           
        }

        public void SendSetDefaultAddressReq(string addressId) {
            LogSystem.Debug("SendSetDefaultAddressReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "buyer/buyershippingaddress/setDefaultAddress") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsSetDefaultAddress).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("id", addressId);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);           
        }

        public void SendModifyAddressReq(Address addr, string userId,string addr_id) {
            LogSystem.Debug("SendModifyAddressReq");
           /* var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + Constants.XIUGAIDIZHI) {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsModifyAddress).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "address");
            _dicParam.Add("paras", addr.ToJson(userId));

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);*/
            JsonData data = new JsonData();
            data["returns"] = "1";
            data["message"] = "";
            var addrId = data["data"] = new JsonData();
            addrId["addr_id"] = addr_id;
            var msg = Encoding.UTF8.GetBytes(data.ToJson());
            var stream = new MemoryStream();
            stream.Write(msg, 0, msg.Length);
            RecMsg(new ResponseInfo() {
                StatusCode = HttpStatusCode.OK,
                RequestInfo = new RequestInfo("") {
                    ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsModifyAddress).ToString() } }
                },
                ResponseContent = stream,
            });
        }

        public void SendNewAddressReq(string shippingAddressJson) {  
            LogSystem.Debug("SendNewAddressReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "buyer/buyershippingaddress/addOrUpdateAddress") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsNewAddress).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("shippingAddressJson", shippingAddressJson);


            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetUserSizeReq(string tel) {
            LogSystem.Debug("SendGetUserSizeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.WSCURL + "?action=size&control=userSize") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetUserSize).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetHistorySizeReq(string customerPhone) {
            LogSystem.Debug("SendGetHistorySizeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "template/templateproperties/getOrderClothSizeList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetHistorySize).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("phoneNo", customerPhone);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendModifyUserSize(string type,string val) {
            LogSystem.Debug("SendModifyUserSize");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.WSCURL + "?action=size&control=editSize") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsModifyUserSize).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("type", type);
            _dicParam.Add("val", val);

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendShareUrl() {
            LogSystem.Debug("SendShareUrl");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.WSCURL + "?action=user&control=share") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData = new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsShareUrl).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        private void RecMsg(ResponseInfo respInfo) {
            //LogSystem.Debug("recve msg");
            JsonData jdata = null;
            if (respInfo.StatusCode == HttpStatusCode.OK) {
                var str = respInfo.GetString(Encoding.UTF8);
                try {
                    jdata = JsonMapper.ToObject(str);
                    if (respInfo.Headers != null) {
                        var cookie = respInfo.Headers.Get("set-cookie");
                        if (cookie != null)
                            jdata["set-cookie"] = cookie;
                        else
                            jdata["set-cookie"] = string.Empty;
                    }
                    
                } catch (Exception e) {
                    LogSystem.Error("rec msg to json failed.{0}",e.Message);
                }
            } else {
                LogSystem.Error("!!!!!http request failed:{0}",respInfo.GetString(Encoding.UTF8));
            }
            if (jdata == null) {
                jdata = new JsonData();
            }
            LogSystem.Debug("rec msg ({0}):{1}",Enum.GetName(typeof(Protocol), int.Parse(respInfo.RequestInfo.ExternalData["MsgId"])),jdata.ToJson());
            _networkMgr.PushMsg(new NetMessage() {
                Id = int.Parse(respInfo.RequestInfo.ExternalData["MsgId"]),
                Content = jdata
            });
        }
        
        private byte[] SerialUploadFiles(IDictionary<string, string> parameters, IList<KeyValuePair<string, byte[]>> files,
            string fileExtension, string boundary) {
            byte[] boundarybytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endbytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            //request.ContentType = "multipart/form-data; boundary=" + boundary;
            var stream= new MemoryStream();
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in parameters.Keys) {
                stream.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, parameters[key]);
                byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                stream.Write(formitembytes, 0, formitembytes.Length);
            }

            string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
            for (int i = 0; i < files.Count; i++) {
                stream.Write(boundarybytes, 0, boundarybytes.Length);
                string header = string.Format(headerTemplate, files[i].Key, files[i].Key+ fileExtension);
                byte[] headerbytes = Encoding.UTF8.GetBytes(header);
                stream.Write(headerbytes, 0, headerbytes.Length);
                stream.Write(files[i].Value, 0, files[i].Value.Length);
            }
            stream.Write(endbytes, 0, endbytes.Length);
            return stream.ToArray();
        }

        private byte[] SerialParameters(IDictionary<string, string> parameters) {
            StringBuilder buffer = new StringBuilder();
            bool isFirst = true;
            foreach (string key in parameters.Keys) {
                if (!isFirst) {
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                } else {
                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    isFirst = false;
                }
            }
            byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
            return data;
        }

        public void SetCookie(string cookie) {
            _cookie = cookie;
        }

        public void SetApiVersion(string apiVersion) {
            _apiVersion = apiVersion;
        }

        public void SetToken(string token) {
            _tokeon = token;
        }

        public void SendGetPricePropertysReq() {
            LogSystem.Debug("send GetPricePropertys Req");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + "System.php") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData =
                    new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsGetPricePropertys).ToString()}}
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "getPriceCountPropertys");

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetCodeSetReq(string codeSet) {
            LogSystem.Debug("send GetCodeset Req");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + "System.php") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData = new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsGetCodeSet).ToString()}}
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "getCodeset");
            _dicParam.Add("codeset", "'" + codeSet + "'"); //TODO 类似int值

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetNoticeListReq() {
            LogSystem.Debug("send SendGetNoticeListReq Req");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + "salesNotice.php") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData = new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsGetNoticeList).ToString()}}
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "getNoticeList");

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendFashionListSearchReq() {
            LogSystem.Debug("SendFashionListSearchReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL2 + "getData.php") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData =
                    new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsGetFasionListSearch).ToString()}}
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "fashionListSearch");

            reqInfo.PostData = SerialParameters(_dicParam);

            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendFashionDetailReq(string id) {
            LogSystem.Debug("SendFashionDetailReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL2 + "getData.php") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData = new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsFashionDetail).ToString()}}
            };

            var us = new JsonData();
            us["subject_id"] = "\"" + id + "\"";
            var user = us.ToJson();

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "fashionDetail");
            _dicParam.Add("paras", user);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetRecommendListReq(int pageNo = 0, int pageSize = 0, string modelId = null, string recommendType = null) {
            LogSystem.Debug("SendGetRecommendListReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "product/productinfo/getProductInfoList") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie}},
                ExternalData =
                    new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsGetRecommendList).ToString()}}
            };

            _dicParam.Clear();
            if (pageNo != 0) {
                _dicParam.Add("PageNo", pageNo.ToString());
            }
            if (pageSize!=0) {
                _dicParam.Add("PageSize", pageSize.ToString());
            }
            if (modelId!=null) {
                _dicParam.Add("modelId", modelId);
            }
            if (recommendType!=null) {
                _dicParam.Add("type", recommendType);
            }

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);              
        }

        public void SendGetRecommendDetailReq(string atid) {
            LogSystem.Debug("SendGetRecommendDetailReq");

            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "product/productinfo/getProductInfoDetail") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetRecommendDetail).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("id", atid);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetDefaultProductInfo(string modelId) {
            LogSystem.Debug("SendGetDefaultProductInfo");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "product/productinfo/getDefaultProductInfo") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetDefaultProductInfo).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("modelId", modelId);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetCityListReq(string parentCode) {
            LogSystem.Debug("SendGetCityListReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + "addressArea.php") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetCityList).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "getCityList");
            _dicParam.Add("parentCode", parentCode);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetAreaReq(string parentCode) {
            LogSystem.Debug("SendGetCityListReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + "addressArea.php") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetArea).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "getCountryList");
            _dicParam.Add("parentCode", parentCode);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendSaveShopCarReq(string goodsJson, IList<KeyValuePair<string, byte[]>> files, string shirtDesignerCode) {
            LogSystem.Debug("SendSaveShopCarReq ");
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/shoppingcart/addProductToShopCart") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie},
                    { "ContentType", "multipart/form-data; boundary=" + boundary } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsSaveShopCar).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("goodsJson", goodsJson);
            _dicParam.Add("shirtDesignerCode", shirtDesignerCode);
            reqInfo.PostData = SerialUploadFiles(_dicParam, files, ".jpg", boundary);
            //            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendOneKeyBuyProductReq(string goodsJson, IList<KeyValuePair<string, byte[]>> files) {
            LogSystem.Debug("OneKeyBuyProduct ");
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/shoppingcart/oneKeyBuyProduct") {
                Headers = new WebHeaderCollection {{"API_VERSION", Constants.VERSION}, {"Cookie", _cookie},
                    { "ContentType", "multipart/form-data; boundary=" + boundary } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsOneKeyBuyProduct).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("goodsJson", goodsJson);

            reqInfo.PostData = SerialUploadFiles(_dicParam, files, ".jpg", boundary);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetShopCarListReq() {
            LogSystem.Debug("SendGetShopCarListReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/shoppingcart/getShopCartList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetShopCarList).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendShopCarDetail(string shopCarId) {
            LogSystem.Debug("SendShopCarDetail ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/shoppingcart/getShopCartDetail") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetShopCarDetail).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("id", shopCarId);
            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetOrderEvaluationList() {
            LogSystem.Debug("SendGetOrderEvaluationList ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/orderevaluation/getOrderEvaluationList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetOrderEvaluationList).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetCouponListReq(string amount) {
            LogSystem.Debug("SendGetCouponListReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/orderpayment/getCouponList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetCouponListReq).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("amount", amount);
            
            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetMeasureByPropertiesCodeReq(string propertiesCode) {
            LogSystem.Debug("SendGetMeasureByPropertiesCodeReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "template/templateproperties/getMeasureByPropertiesCode") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetMeasureByPropertiesCode).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("propertiesCode", propertiesCode);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetGoodsInfoReq(string goodsId) {
            LogSystem.Debug("SendGetGoodsInfoReq");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "goods/goodsinfo/getGoodsInfo") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetGoodsInfo).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("id", goodsId);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendDeleteShopCarItemReq(string id) {
            LogSystem.Debug("SendDeleteShopCarItemReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/shoppingcart/deleteShopCartItem") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsDeleteShopCarItem).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("ids", id);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetOrderListReq(string pageNo, string pageSize, string orderStatus) {
            LogSystem.Debug("SendGetOrderListReq ");
            var http = new RequestHttpWebRequest();
//            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/order/getOrderList") {
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/order/getOrderListNew") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetOrderList).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("pageNo", pageNo);
            _dicParam.Add("pageSize", pageSize);
            _dicParam.Add("orderStatus", orderStatus);
            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendCancleOrderReq(string orderCode) {
            LogSystem.Debug("SendCancleOrderReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/order/deleteOrder") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsCancleOrder).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("id", orderCode);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendModifyOrderStatuReq(string id, string status) {
            LogSystem.Debug("SendModifyOrderStatuReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/order/updateOrderStatus") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsModifyOrderStatu).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("id", id);
            _dicParam.Add("orderStatus", status);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendSubmitOrderReq(string orderInfoJson, string shopCartJson, string couponid) {
            LogSystem.Debug("SendSubmitOrderReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/shoppingcart/confirmShopCartItems") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsSubmitOrder).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("orderInfoJson", orderInfoJson);
            _dicParam.Add("shopCartJson", shopCartJson);
            _dicParam.Add("couponid", couponid);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetOrderDetailReq(string orderId) {
            LogSystem.Debug("SendGetOrderDetailReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/order/getOrderDetail") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetOrderDetail).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("id", orderId);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
            
        }

        public void SendGetOrderDeliveryDateReq() {
            LogSystem.Debug("SendGetOrderDeliveryDateReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + "order/orderDeliveryDate.php") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetOrderDeliveryDate).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "date");

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendSubmitEvaluateReq(string evaluationJson, IList<KeyValuePair<string, byte[]>> files, string fileExtension) {
            LogSystem.Debug("SendGetOrderDetailReq ");
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "order/orderevaluation/saveOrderEvaluation") {
                Headers =
                    new WebHeaderCollection {
                        {"API_VERSION", Constants.VERSION},
                        {"Cookie", _cookie},
                        {"ContentType", "multipart/form-data; boundary=" + boundary}
                    },
                ExternalData =
                    new Dictionary<string, string> {{"MsgId", ((int) Protocol.CsSubmitEvaluate).ToString()}}
            };

            _dicParam.Clear();
            _dicParam.Add("token",_tokeon);
            _dicParam.Add("evaluationJson", evaluationJson);
            //TODO 图片上传为空
            reqInfo.PostData = SerialUploadFiles(_dicParam, files, fileExtension, boundary);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetReferenceSizeReq() {
            LogSystem.Debug("SendGetReferenceSizeReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.URL + "shoppingCart/queryShirtSize.php") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetReferenceSize).ToString() } }
            };

            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("action", "shopCart");
            _dicParam.Add("type", "0");

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetClassifyManageListReq(string pageNo, string pageSize) {
            LogSystem.Debug("SendGetClassifyManageListReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "designer/getClassifyManageList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetClassifyManageList).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("pageNo", pageNo);
            _dicParam.Add("pageSize", pageSize);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetDesignerProductListReq(string pageNo, string pageSize, string type, string label, string minPrice, string maxPrice) {
            LogSystem.Debug("SendGetDesignerProductListReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "designer/getDesignerProductList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetDesignerProductList).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("pageNo", pageNo);
            _dicParam.Add("pageSize", pageSize);
            _dicParam.Add("type", type);
            _dicParam.Add("label", label);
            _dicParam.Add("minPrice", minPrice);
            _dicParam.Add("maxPrice", maxPrice);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public void SendGetLabelManageListReq(string pageNo, string pageSize) {
            LogSystem.Debug("SendGetLabelManageListReq ");
            var http = new RequestHttpWebRequest();
            var reqInfo = new RequestInfo(Constants.OrdersysURL + "designer/getLabelManageList") {
                Headers = new WebHeaderCollection { { "API_VERSION", Constants.VERSION }, { "Cookie", _cookie } },
                ExternalData =
                    new Dictionary<string, string> { { "MsgId", ((int)Protocol.CsGetLabelManageList).ToString() } }
            };
            _dicParam.Clear();
            _dicParam.Add("token", _tokeon);
            _dicParam.Add("pageNo", pageNo);
            _dicParam.Add("pageSize", pageSize);

            reqInfo.PostData = SerialParameters(_dicParam);
            http.GetResponseAsync(reqInfo, RecMsg);
        }

        public List<Type> GetSenderList() {
            return new List<Type> {
                typeof(IRequestHeader),
                typeof(IDesignClothesSender),
                typeof(ILoginSender),
                typeof(ICodeSetSender),
                typeof(IInfoSender),
                typeof(IOrderSender),
                typeof(IUserInfoSender)
            };
        }

        public List<KeyValuePair<string, ITrigger>> GetTriggerList() {
            return new List<KeyValuePair<string, ITrigger>> {
                new KeyValuePair<string, ITrigger>(TriggerNames.DesignClothesTrigger, new DesignClothesTrigger()),
                new KeyValuePair<string, ITrigger>(TriggerNames.LoginTrigger, new LoginTrigger()),
                new KeyValuePair<string, ITrigger>(TriggerNames.CodeSetTrigger, new CodeSetTrigger()),
                new KeyValuePair<string, ITrigger>(TriggerNames.InfoTrigger, new InfoTrigger()),
                new KeyValuePair<string, ITrigger>(TriggerNames.OrderTirgger, new OrderTrigger()),
                new KeyValuePair<string, ITrigger>(TriggerNames.UserInfoTrigger, new UserInfoTrigger()),
            };
        }
    }

    internal class DesignClothesTrigger : AbstractTrigger<IDesignClothesListener> {
        public override bool FireOneListener(IDesignClothesListener listener, NetMessage evt) {
            var ret = true;
            var jdata = (JsonData) evt.Content;
            switch ((Protocol) evt.Id) {
                case Protocol.CsGetShirtTempletList:
                    listener.OnGetWishListRsp(jdata);
                    break;
                case Protocol.CsDeleteShirtTemplet:
                    listener.OnDeleteShirtTempletRsp(jdata);
                    break;
                case Protocol.CsGetCollectDetail:
                    listener.OnGetCollectDetailRsp(jdata);
                    break;
                case Protocol.CsCollectDesign:
                    listener.OnSaveWishiListRsp(jdata);
                    break;
                case Protocol.CsGetModelSize:
                    listener.OnGetModelSizeListRsp(jdata);
                    break;
                case Protocol.CsGetUserSizeList:
                    listener.OnGetUserSizeListRsp(jdata);
                    break;
                case Protocol.CsSaveModelSize:
                    listener.OnSaveUserSizeRsp(jdata);
                    break;
                case Protocol.CsDeleteModelSize:
                    listener.OnDeleteUserSizeRsp(jdata);
                    break;
                case Protocol.CsQueryFabricList:
                    listener.OnQueryFabricListRsq(jdata);
                    break;
                case Protocol.CsGetFabricDetail:
                    listener.OnGetFabricDetailRsq(jdata);
                    break;
                case Protocol.CsCountProductInfoPrice:
                    listener.OnCountProductInfoPriceRsq(jdata);
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        public override List<int> GetEventList() {
            return new List<int> {
                (int)Protocol.CsGetShirtTempletList,
                (int)Protocol.CsDeleteShirtTemplet,
                (int)Protocol.CsGetCollectDetail,
                (int)Protocol.CsCollectDesign,
                (int)Protocol.CsGetModelSize,
                (int)Protocol.CsGetUserSizeList,
                (int)Protocol.CsSaveModelSize,
                (int)Protocol.CsDeleteModelSize,
                (int)Protocol.CsQueryFabricList,
                (int)Protocol.CsGetFabricDetail,
                (int)Protocol.CsCountProductInfoPrice,
            };
        }
    }

    internal class LoginTrigger : AbstractTrigger<ILoginListener> {
        public override bool FireOneListener(ILoginListener listener, NetMessage evt) {
            var ret = true;
            var jdata = (JsonData) evt.Content;
            switch ((Protocol) evt.Id) {
                case Protocol.CsLogin:
                    listener.OnLoginRsp(jdata);
                    break;
                case Protocol.CsLogout:
                    listener.OnLogoutRsp(jdata);
                    break;
                case Protocol.CsGetRegisterCode:
                    listener.OnGetRegisterCodeRsp(jdata);
                    break;
                case Protocol.CsCheckRegisterCode:
                    listener.OnCheckRegisterCodeRsp(jdata);
                    break;
                case Protocol.CsRegister:
                    listener.OnRegisterRsp(jdata);
                    break;
                case Protocol.CsGetResetPwCode:
                    listener.OnGetResetPwCodeRsp(jdata);
                    break;
                case Protocol.CsResetPassword:
                    listener.OnResetPasswordRsp(jdata);
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        public override List<int> GetEventList() {
            return new List<int>() {
               (int)Protocol.CsLogin,
               (int)Protocol.CsLogout,
               (int)Protocol.CsGetRegisterCode,
               (int)Protocol.CsCheckRegisterCode,
               (int)Protocol.CsRegister,
               (int)Protocol.CsGetResetPwCode,
               (int)Protocol.CsResetPassword,
            };
        }
    }

    internal class CodeSetTrigger : AbstractTrigger<ICodeSetListener> {
        public override bool FireOneListener(ICodeSetListener listener, NetMessage evt) {
            var ret = true;
            var jdata = (JsonData)evt.Content;
            switch ((Protocol)evt.Id) {
                case Protocol.CsGetPricePropertys:
                    listener.OnGetPricePropertysRsp(jdata);
                    break;
                case Protocol.CsGetCodeSet:
                    listener.OnGetCodesetRsp(jdata);
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        public override List<int> GetEventList() {
            return new List<int>() {
               (int)Protocol.CsGetPricePropertys,
               (int)Protocol.CsGetCodeSet,
            };
        }
    }

    internal class InfoTrigger : AbstractTrigger<IInfoListener> {
        public override bool FireOneListener(IInfoListener listener, NetMessage evt) {
            var ret = true;
            var jdata = (JsonData) evt.Content;
            switch ((Protocol) evt.Id) {
                case Protocol.CsGetNoticeList:
                    listener.OnGetNoticeListRsp(jdata);
                    break;
                case Protocol.CsGetFasionListSearch:
                    listener.OnFashionListSearchRsp(jdata);
                    break;
                case Protocol.CsFashionDetail:
                    listener.OnFashionDetailRsp(jdata);
                    break;
                case Protocol.CsGetRecommendList:
                    listener.OnGetRecommendListRsp(jdata);
                    break;
                case Protocol.CsGetRecommendDetail:
                    listener.OnGetRecommendDetailRsp(jdata);
                    break;
                case Protocol.CsGetDefaultProductInfo:
                    listener.OnGetDefaultProductInfoRsp(jdata);
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        public override List<int> GetEventList() {
            return new List<int>() {
               (int)Protocol.CsGetNoticeList,
               (int)Protocol.CsGetFasionListSearch,
               (int)Protocol.CsFashionDetail,
               (int)Protocol.CsGetRecommendList,
               (int)Protocol.CsGetRecommendDetail,
               (int)Protocol.CsGetDefaultProductInfo,
            };
        }
    }

    internal class OrderTrigger : AbstractTrigger<IOrderListener> {
        public override bool FireOneListener(IOrderListener listener, NetMessage evt) {
            var ret = true;
            var jdata = (JsonData)evt.Content;
            switch ((Protocol) evt.Id) {
                case Protocol.CsSaveShopCar:
                    listener.OnSaveShopCarRsp(jdata);
                    break;
                case Protocol.CsOneKeyBuyProduct:
                    listener.OnOneKeyBuyProductRsp(jdata);
                    break;
                case Protocol.CsGetShopCarList:
                    listener.OnGetShopCarListRsp(jdata);
                    break;
                case Protocol.CsDeleteShopCarItem:
                    listener.OnDeleteShopCarItemRsp(jdata);
                    break;
                case Protocol.CsGetOrderList:
                    listener.OnGetOrderListRsp(jdata);
                    break;
                case Protocol.CsCancleOrder:
                    listener.OnCancleOrderRsp(jdata);
                    break;
                case Protocol.CsModifyOrderStatu:
                    listener.OnModifyOrderStatuRsp(jdata);
                    break;
                case Protocol.CsSubmitOrder:
                    listener.OnSubmitOrderRsp(jdata);
                    break;
                case Protocol.CsGetOrderDetail:
                    listener.OnGetOrderDetailRsp(jdata);
                    break;
                case Protocol.CsGetOrderDeliveryDate:
                    listener.OnGetOrderDeliveryDateRsp(jdata);
                    break;
                case Protocol.CsSubmitEvaluate:
                    listener.OnSubmitEvaluateRsp(jdata);
                    break;
                case Protocol.CsGetReferenceSize:
                    listener.OnGetReferenceSizeRsp(jdata);
                    break;
                case Protocol.CsGetShopCarDetail:
                    listener.OnGetShopCarDetail(jdata);
                    break;
                case Protocol.CsGetOrderEvaluationList:
                    listener.OnGetOrderEvaluationList(jdata);
                    break;
                case Protocol.CsGetCouponListReq:
                    listener.OnGetCouponListRsp(jdata);
                    break;
                case Protocol.CsGetMeasureByPropertiesCode:
                    listener.OnGetMeasureByPropertiesCodeRsp(jdata);
                    break;
                case Protocol.CsGetGoodsInfo:
                    listener.OnGetGoodsInfoRsp(jdata);
                    break;
                case Protocol.CsGetClassifyManageList:
                    listener.OnGetClassifyManageListRsp(jdata);
                    break;
                case Protocol.CsGetDesignerProductList:
                    listener.OnGetDesignerProductListRsp(jdata);
                    break;
                case Protocol.CsGetLabelManageList:
                    listener.OnGetLabelManageListRsp(jdata);
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        public override List<int> GetEventList() {
            return new List<int>() {
                (int) Protocol.CsSaveShopCar,
                (int) Protocol.CsOneKeyBuyProduct,
                (int) Protocol.CsGetShopCarList,
                (int) Protocol.CsDeleteShopCarItem,
                (int) Protocol.CsGetOrderList,
                (int) Protocol.CsCancleOrder,
                (int) Protocol.CsModifyOrderStatu,
                (int) Protocol.CsSubmitOrder,
                (int) Protocol.CsGetOrderDetail,
                (int) Protocol.CsGetOrderDeliveryDate,
                (int) Protocol.CsSubmitEvaluate,
                (int) Protocol.CsGetReferenceSize,
                (int) Protocol.CsGetShopCarDetail,
                (int) Protocol.CsGetCouponListReq,
                (int) Protocol.CsGetOrderEvaluationList,
                (int) Protocol.CsGetMeasureByPropertiesCode,
                (int) Protocol.CsGetGoodsInfo,
                (int) Protocol.CsGetClassifyManageList,
                (int) Protocol.CsGetDesignerProductList,
                (int) Protocol.CsGetLabelManageList,
            };
        }
    }

    internal class UserInfoTrigger : AbstractTrigger<IUserInfoListener> {
        public override bool FireOneListener(IUserInfoListener listener, NetMessage evt) {
            var ret = true;
            var jdata = (JsonData) evt.Content;
            switch ((Protocol) evt.Id) {
                case Protocol.CsQueryUserInfo:
                    listener.OnQueryUserInfoRsp(jdata);
                    break;
                case Protocol.CsModifyUserInfo:
                    listener.OnModifyUserInfoRsp(jdata);
                    break;
                case Protocol.CsGetAddress:
                    listener.OnGetAddressRsp(jdata);
                    break;
                case Protocol.CsDeleteAddress:
                    listener.OnDeleteAddressRsp(jdata);
                    break;
                case Protocol.CsSetDefaultAddress:
                    listener.OnSetDefaultAddressRsp(jdata);
                    break;
                case Protocol.CsModifyAddress:
                    listener.OnModifyAddressRsp(jdata);
                    break;
                case Protocol.CsNewAddress:
                    listener.OnNewAddressRsp(jdata);
                    break;
                case Protocol.CsGetUserSize:
                    listener.OnGetUserSizeRsp(jdata);
                    break;
                case Protocol.CsGetHistorySize:
                    listener.OnGetHistorySizeRsp(jdata);
                    break;
                case Protocol.CsModifyUserSize:
                    listener.OnModifyUserSizeRsp(jdata);
                    break;
                case Protocol.CsShareUrl:
                    listener.OnShareUrl(jdata);
                    break;
                case Protocol.CsGetCityList:
                    listener.OnGetCityListRsp(jdata);
                    break;
                case Protocol.CsGetArea:
                    listener.OnGetAreaRsp(jdata);
                    break;
                case Protocol.CsAddOrUpdateModelSize:
                    listener.OnAddOrUpdateModelSizeRsq(jdata);
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        public override List<int> GetEventList() {
            return new List<int>() {
                (int)Protocol.CsQueryUserInfo,
                (int)Protocol.CsModifyUserInfo,
                (int)Protocol.CsGetAddress,
                (int)Protocol.CsDeleteAddress,
                (int)Protocol.CsSetDefaultAddress,
                (int)Protocol.CsModifyAddress,
                (int)Protocol.CsNewAddress,
                (int)Protocol.CsGetUserSize,
                (int)Protocol.CsGetHistorySize,
                (int)Protocol.CsModifyUserSize,
                (int)Protocol.CsShareUrl,
                (int)Protocol.CsGetCityList,
                (int)Protocol.CsGetArea,
                (int)Protocol.CsAddOrUpdateModelSize,
            };
        }
    }
}