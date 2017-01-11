using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using cn.sharesdk.unity3d;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.User {
    public class UserCenter:IUserCenter, IUserInfoListener, IDesignClothesListener {
        private INetworkMgr _networkMgr;
        private ILoginMgr _loginMgr;
        private UserInfo _userInfo = new UserInfo();
        private ClothSize _clothsize;
        private List<Address> _lstAddress=null;
        private Address _modifyAddress=null;
        private Address _newAddress = null;
//        private UserSize _modifySize=null;
        public string Name { get; set; }

        private event MyAction<bool, List<Address>> OnQueryAddress;
        private event MyAction<bool, string> OnDeleteAddress;
        public event MyAction<bool, string> OnSetDefaultAddress;

        public ClothSize ClothSize {
            get { return _clothsize; }
            private set { _clothsize = value; }
        }

        public Dictionary<string, Dictionary<string, string>> DicCityList { get;  set; }
        public Dictionary<string, Dictionary<string, string>> DicAreaList { get;  set; }

        public event MyAction<UserInfo> OnUserInfoUpdate;
        private event MyAction<bool, string> OnModifyAddress;
        private event MyAction<bool,JsonData> OnNewAddress;
        private event MyAction<bool> OnModifyUserSize;
        private event MyAction<bool, Dictionary<string, string>> OnGetCity;
        private event MyAction<bool, Dictionary<string, string>> OnGetArea;
        private event MyAction<bool, JsonData> OnAddOrUpdateModelSize;
        private event MyAction<bool, JsonData> OnModifyUserInfo;
        private event MyAction<bool, JsonData> OnGetModelSizeList;

        public void Tick() {
        }

        public bool Init() {
            DicCityList = new Dictionary<string, Dictionary<string, string>>();
            DicAreaList = new Dictionary<string, Dictionary<string, string>>();
            _networkMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (null == _networkMgr) {
                LogSystem.Error("network is null.");
                return false;
            }
            _networkMgr.QueryTigger(TriggerNames.UserInfoTrigger).AddListener(this);
            _networkMgr.QueryTigger(TriggerNames.DesignClothesTrigger).AddListener(this);
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (_loginMgr == null) {
                LogSystem.Error("login mgr is null.");
                return false;
            }
            _loginMgr.OnLoginSucc += OnLoginSucc;
            _loginMgr.OnLogout += OnLogout;
            _clothsize = new ClothSize() {
                SizeName = "41/M",
                SizeLingW = "41.0",
                SizeYiC = "73.5",
                SizeJianK = "46.5",
                SizeXiongW = "109.0",
                SizeYaoW = "105.0",
                SizeXiuCLong = "62",
                SizeXiuKLong = "11.5",
                SizeXiuF = "42.5"
            };
            return true;
        }
       
        private void OnLogout(bool result) {
            if (result) {
                UserInfo = new UserInfo();
                _networkMgr.QuerySender<IRequestHeader>().SetToken(string.Empty);
            }
        }

        private void OnLoginSucc(string token) {
            UserInfo = new UserInfo {
                Token = token,
            };
            _QueryUserInfo();
            _QueryUserSize();
        }


        public UserInfo UserInfo {
            get {
                return _userInfo;
            }
            private set {
                _userInfo = value;
            }
        }

        public void QueryAddress(MyAction<bool, List<Address>> callback) {
//            if (_lstAddress != null) {
//                if (callback != null)
//                    callback(true, _lstAddress);
//            } else {
                OnQueryAddress += callback;
                _networkMgr.QuerySender<IUserInfoSender>().SendGetAddressReq();
//            }
        }

        public void DeleteAddress(string addrId, MyAction<bool, string> callback) {
                OnDeleteAddress += callback;
                _networkMgr.QuerySender<IUserInfoSender>().SendDeleteAddressReq(addrId);        
        }

        public void GetModelSizeList(string id, string modelid, string type, string height, string weight,
            MyAction<bool, JsonData> callback) {
            OnGetModelSizeList += callback;
            _networkMgr.QuerySender<IDesignClothesSender>().SendGetModelSizeListReq(string.Empty, modelid, string.Empty, height, weight);
        }

        public void ModifyUserInfo(string usrName, IList<KeyValuePair<string, byte[]>> files,
            MyAction<bool, JsonData> callback) {
            if (callback != null)
                OnModifyUserInfo += callback;
            _networkMgr.QuerySender<IUserInfoSender>().SendModifyUserInfoReq(usrName, files);
        }


        public event MyAction<bool, string> OnDefaultAddress; 
        public void SetDefaultAddress(string addrId,MyAction<bool,string> callback) {
                OnDefaultAddress = callback;
                _networkMgr.QuerySender<IUserInfoSender>().SendSetDefaultAddressReq(addrId);            
        }

        public void NewAddress(string addr, MyAction<bool,JsonData> callback) {
           // if (_newAddress == null) {
                OnNewAddress += callback;
              //  _newAddress = addr;
                _networkMgr.QuerySender<IUserInfoSender>().SendNewAddressReq(addr);
          //  } else {
            //    if (null != callback)
              //      callback(false);
          //  }
        }

        public void ModifyUserSize(MysizeEntry entry, string value, MyAction<bool> callback) {
//            if (_modifySize != null) {
//                if (null != callback)
//                    callback(false);
//            } else {
                OnModifyUserSize += callback;
                var _modifySize = new UserSize() {
                    Height = UserInfo.UserSize.Height,
                    Weight = UserInfo.UserSize.Weight,
                    Shoulder = UserInfo.UserSize.Shoulder,
                    Bust = UserInfo.UserSize.Bust,
                    WaistLine = UserInfo.UserSize.WaistLine,
                    HipLine = UserInfo.UserSize.HipLine,
                };
                string type = string.Empty;
                switch (entry) {
                    case MysizeEntry.Height:
                        type = "customer_height";
                        _modifySize.Height = value;
                        break;
                    case MysizeEntry.Weight:
                        type = "customer_weight";
                        _modifySize.Weight = value;
                        break;
                    case MysizeEntry.Shoulder:
                        type = "size_jian_k";
                        _modifySize.Shoulder = value;
                        break;
                    case MysizeEntry.Bust:
                        type = "size_xiong_w";
                        _modifySize.Bust = value;
                        break;
                    case MysizeEntry.Waistline:
                        type = "size_yao_w";
                        _modifySize.WaistLine = value;
                        break;
                    case MysizeEntry.Hipline:
                        type = "size_dun_w";
                        _modifySize.HipLine = value;
                        break;
                }
                _networkMgr.QuerySender<IUserInfoSender>().SendModifyUserSize(type, value);
//            }
        }

        public void ShareUrl() {
            _networkMgr.QuerySender<IUserInfoSender>().SendShareUrl();
        }

        public void GetCityList(string provinceCode, MyAction<bool, Dictionary<string, string>> callback) {
            OnGetCity += callback;
            _networkMgr.QuerySender<IUserInfoSender>().SendGetCityListReq(provinceCode);
        }

        public void GetAreaList(string cityCode, MyAction<bool, Dictionary<string, string>> callback) {
            OnGetArea += callback;
            _networkMgr.QuerySender<IUserInfoSender>().SendGetAreaReq(cityCode);
        }

        public Address GetDefaultAddress() {
            Address ret = null;
            if (_lstAddress != null) {
                for (int i = 0; i < _lstAddress.Count; i++) {
                    if (_lstAddress[i].IsDefault) {
                        ret = _lstAddress[i];
                        break;
                    }
                }
            }
            return ret;
        }

        public void AddOrUpdateModelSize(string modelId, string modelSizeJson, MyAction<bool, JsonData> callback) {
            OnAddOrUpdateModelSize += callback;
           _networkMgr.QuerySender<IUserInfoSender>().SendAddOrUpdateModelSizeReq(modelId, modelSizeJson);
        }

        private void _QueryUserInfo() {
            var sender = _networkMgr.QuerySender<IUserInfoSender>();
            if (sender != null) {
                sender.SendQueryUserInfoReq(UserInfo.Token);
            } else {
                LogSystem.Error("ILoginSender is null.");
            }
        }

        public void OnQueryUserInfoRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    JsonData data = msg["info"];
//                    UserInfo.UserId = Utils.GetJsonStr(data["user_id"]);
                    UserInfo.UserName = Utils.GetJsonStr(data["name"]);
                    UserInfo.Tel = Utils.GetJsonStr(data["tel"]);
//                    UserInfo.WeiXin = Utils.GetJsonStr(data["weixin"]);
//                    UserInfo.OpenId = Utils.GetJsonStr(data["open_id"]);
//                    UserInfo.WxId = Utils.GetJsonStr(data["wxid"]);
//                    UserInfo.Parent = Utils.GetJsonStr(data["parent_1"]);
                    UserInfo.Photo = Utils.GetJsonStr(data["photo"]);
//                    UserInfo.ShopPhoto = Utils.GetJsonStr(data["shop_photo"]);
//                    UserInfo.Level = Utils.GetJsonStr(data["level"]);
//                    JsonData data2 = msg["account"];
//                    UserInfo.TiXian = Utils.GetJsonStr(data2["tixian"]);
//                    UserInfo.TiXianDqr = Utils.GetJsonStr(data2["tixian_dqr"]);
//                    UserInfo.YuE = Utils.GetJsonStr(data2["yue"]);

                    if (null != OnUserInfoUpdate)
                        OnUserInfoUpdate(UserInfo);
                } else {
                    LogSystem.Error("QueryUserInfo failed.");
                }
            } else {
                LogSystem.Error("request QueryUserInfo network failed.");
            }
        }

        private void _QueryUserSize() {
            var sender = _networkMgr.QuerySender<IUserInfoSender>();
            if (sender != null) {
                sender.SendGetUserSizeReq(UserInfo.Token);
            } else {
                LogSystem.Error("ILoginSender is null.");
            }
        }


        public void OnModifyUserInfoRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnModifyUserInfo;
            OnModifyUserInfo = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    if (callback != null)
                        callback(true, msg["data"]);
                } else {
                    if (callback != null)
                        callback(false, null);
                    LogSystem.Error("Modify UserInfo return failed.");
                }
            } else {
                if (callback != null)
                    callback(false, null);
                LogSystem.Error("network require failed.");
            }

        }

        private void ParseAddress(JsonData data) {
            _lstAddress = new List<Address>();
            var count = int.Parse(Utils.GetJsonStr(data["count"]));
            for (var i = 0; i < count; i++) {
                var json = data["shippingAddress"][i];
                var addr = new Address();
                addr.Id = Utils.GetJsonStr(json["atid"]);                
                if (Utils.GetJsonStr(json["defaultIdentifier"]) == "1") {
                    addr.IsDefault = true;
                }
                addr.Province = Utils.GetJsonStr(json["province"]);
                addr.ProvinceCode = Utils.GetJsonStr(json["provinceCode"]);
                addr.Area = Utils.GetJsonStr(json["area"]);
                addr.AreaCode = Utils.GetJsonStr(json["areaCode"]);
                addr.City = Utils.GetJsonStr(json["city"]);
                addr.CityCode = Utils.GetJsonStr(json["cityCode"]);
                addr.Addr = Utils.GetJsonStr(json["detailedAddress"]);
                addr.Contacts = Utils.GetJsonStr(json["contacts"]);
                addr.Tel = Utils.GetJsonStr(json["telphone"]);
                addr.PostCode = Utils.GetJsonStr(json["postCode"]);
                _lstAddress.Add(addr);
            }
            //if (count == 1) {
            //    _networkMgr.QuerySender<IUserInfoSender>().SendSetDefaultAddressReq(_lstAddress[0].Id);
            //}
        }

        public void OnGetAddressRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnQueryAddress;
            OnQueryAddress = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    ParseAddress(msg["data"]);
                    if (callback != null) {
                        callback(true, _lstAddress);
                    }
                } else {
                    LogSystem.Error("get address req return failed.");
                    if (callback != null) {
                        callback(false, _lstAddress);
                    }
                }
            } else {
                LogSystem.Error("require network is failed.");
                if (callback != null) {
                    callback(false, _lstAddress);
                }
            }
        }

        public void OnDeleteAddressRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnDeleteAddress;
            OnDeleteAddress = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    var json = msg["data"];
                    var addr_id =Utils.GetJsonStr(json["ids"][0]);
                    for (int i = 0; i < _lstAddress.Count; i++) {
                        if (_lstAddress[i].Id == addr_id) {                          
                            _lstAddress.RemoveAt(i);
                            break;
                        }
                    }
                    if (callback != null)
                        callback(true, addr_id);
                } else {
                    if (callback != null)
                        callback(false,"-1");
                    LogSystem.Error("Delete address return failed.");
                }
            } else {
                if (callback != null)
                    callback(false,"-1");
                LogSystem.Error("network require failed.");
            }
        }

        public void OnSetDefaultAddressRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnDefaultAddress;
            OnDefaultAddress = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    JsonData json = msg["data"];
                    var id = Utils.GetJsonStr(json["addressId"]);
                    for (int i = 0; i < _lstAddress.Count; i++) {
                        if (_lstAddress[i] != null) {
                            if (_lstAddress[i].Id == id) {
                                _lstAddress[i].IsDefault = true;
                            } else {
                                _lstAddress[i].IsDefault = false;
                            }
                        }
                    }
                    if (callback != null)
                        callback(true, id);
                } else {
                    if (callback != null)
                        callback(false, "-1");
                    LogSystem.Error("set address default return failed.");
                }
            } else {
                if (callback != null)
                    callback(false, "-1");
                LogSystem.Error("network require failed.");
            }
        }

        public void OnModifyAddressRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    //TODO you hua
                    var json = msg["data"];
                    var id = Utils.GetJsonStr(json["addr_id"]);
                    Debug.Log("xxxxxxxxxxx:"+id);
                    var foundAddr = _lstAddress.Find((Address otherAddr) => { return id == otherAddr.Id; });
                 
                    if (foundAddr != null) {
                        _lstAddress.Remove(foundAddr);
                    }
                    _lstAddress.Add(_modifyAddress);
                    _modifyAddress = null;
                    if (OnModifyAddress != null)
                        OnModifyAddress(true, id);
                } else {
                    if (OnModifyAddress != null)
                        OnModifyAddress(false, "-1");
                    LogSystem.Error("network require failed.");
                }
            } else {
                if (OnModifyAddress != null)
                    OnModifyAddress(false, "-1");
                LogSystem.Error("network require failed.");
            }
        }

        public void OnNewAddressRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnNewAddress;
            OnNewAddress = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                  //  var json = msg["data"];
                   // var addr_id = Utils.GetJsonStr(json["addressId"]);
                    _lstAddress = null;                 
                   // QueryAddress(null);
                    if (callback != null)
                        callback(true,msg["data"]);
                } else {
                    var json=new JsonData();
                    if (callback != null)
                        callback(false,json);
                    LogSystem.Error("network require failed.");
                }
            } else {
                var json = new JsonData();
                if (callback != null)
                    callback(false,json);
                LogSystem.Error("network require failed.");
            }
        }

        public void OnGetUserSizeRsp(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    if (Utils.GetJsonStr(msg["data"]).ToUpper() != "FALSE") {
                        JsonData data = msg["data"];
                        UserInfo.UserSize.Height = Utils.GetJsonStr(data["customer_height"]);
                        UserInfo.UserSize.Weight = Utils.GetJsonStr(data["customer_weight"]);
                        UserInfo.UserSize.Shoulder = Utils.GetJsonStr(data["size_jian_k"]);
                        UserInfo.UserSize.Bust = Utils.GetJsonStr(data["size_xiong_w"]);
                        UserInfo.UserSize.WaistLine = Utils.GetJsonStr(data["size_yao_w"]);
                        UserInfo.UserSize.HipLine = Utils.GetJsonStr(data["size_dun_w"]);
                    } else {
                        UserInfo.UserSize.Height = "175";
                        UserInfo.UserSize.Weight = "55";
                        UserInfo.UserSize.Shoulder = "46.5";
                        UserInfo.UserSize.Bust = "109";
                        UserInfo.UserSize.WaistLine = "105";
                        UserInfo.UserSize.HipLine = "109";
                    }
                    if (null != OnUserInfoUpdate)
                        OnUserInfoUpdate(UserInfo);
                } else {
                    LogSystem.Error("QueryUserSize failed.");
                }
            } else {
                LogSystem.Error("request QueryUserSize network failed.");
            }
        }

        public void OnGetHistorySizeRsp(JsonData msg) {
            //TODO
        }

        private void ModifyUserSize(JsonData jsonData) {
            var type = Utils.GetJsonStr(jsonData["type"]);
            var value = Utils.GetJsonStr(jsonData["val"]);
            switch (type) {
                case "customer_height":
                    UserInfo.UserSize.Height = value;
                    break;
                case "customer_weight":
                    UserInfo.UserSize.Weight = value;
                    break;
                case "size_jian_k":
                    UserInfo.UserSize.Shoulder = value;
                    break;
                case "size_xiong_w":
                    UserInfo.UserSize.Bust = value;
                    break;
                case "size_yao_w":
                    UserInfo.UserSize.WaistLine = value;
                    break;
                case "size_dun_w":
                    UserInfo.UserSize.HipLine = value;
                    break;
            }
        }

        public void OnModifyUserSizeRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnModifyUserSize;
            OnModifyUserSize = null;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    ModifyUserSize(msg);
                    if (callback != null)
                        callback(true);
                } else {
                    LogSystem.Error("ModifyUserSize failed.");
                    if (callback != null)
                        callback(false);
                }
            } else {
                LogSystem.Error("request ModifyUserSize network failed.");
                if (callback != null)
                    callback(false);
            }
        }

        public void OnShareUrl(JsonData msg) {
            IDictionary dic = msg;
            if (dic.Contains("returns")) {
                if (Utils.GetJsonStr(msg["returns"]) == "1") {
                    ShareContent content = new ShareContent();
                    content.SetTitle(StrConfigProvider.Instance.GetStr("tip_blmajor"));
                    content.SetText(StrConfigProvider.Instance.GetStr("tip_blmc"));
                    content.SetImageUrl("");
                    content.SetUrl(Utils.GetJsonStr(msg["shareUrl"]));
                    content.SetShareType(ContentType.Webpage);
                    ShareSDK.Instance.ShareContent(PlatformType.WeChat, content);
                } else {
                    LogSystem.Error("ShareUrl failed.");
                }
            } else {
                LogSystem.Error("request ShareUrl network failed.");
            }
        }

        public void OnGetCityListRsp(JsonData msg) {
            var callback = OnGetCity;
            OnGetCity = null;
            IDictionary dic = msg;
            if (dic.Contains("exit")) {
                if (Utils.GetJsonStr(msg["exit"]) == "0") {
                    JsonData data = msg["data"];
                    if (data.Count > 0) {
                        var parentCode = Utils.GetJsonStr(data[0]["parentCode"]);
                        var dicCode = new Dictionary<string, string>();
                        for (int i = 0; i < data.Count; i++) {
                            dicCode.Add(Utils.GetJsonStr(data[i]["areaCode"]), Utils.GetJsonStr(data[i]["areaName"]));
                        }
                        if (DicCityList.ContainsKey(parentCode))
                        DicCityList.Add(parentCode, dicCode);
                        if (callback != null)
                            callback(true, dicCode);
                    }
                } else {
                    LogSystem.Error("get city list return failed.");
                }
            } else {
                LogSystem.Error("network require is failed.");
            }
        }

        public void OnGetAreaRsp(JsonData msg) {
            var callback = OnGetArea;
            OnGetArea = null;
            IDictionary dic = msg;
            if (dic.Contains("exit")) {
                if (Utils.GetJsonStr(msg["exit"]) == "0") {
                    JsonData data = msg["data"];
                    if (data.Count > 0) {
                        var parentCode = Utils.GetJsonStr(data[0]["parentCode"]);
                        var dicCode = new Dictionary<string, string>();
                        for (int i = 0; i < data.Count; i++) {
                            dicCode.Add(Utils.GetJsonStr(data[i]["areaCode"]), Utils.GetJsonStr(data[i]["areaName"]));
                        }
                        if(!DicAreaList.ContainsKey(parentCode))
                        DicAreaList.Add(parentCode, dicCode);
                        if (callback != null)
                            callback(true, dicCode);
                    }
                } else {
                    LogSystem.Error("get area list return failed.");
                }
            } else {
                LogSystem.Error("network require is failed.");
            }
        }

        public void OnAddOrUpdateModelSizeRsq(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnAddOrUpdateModelSize;
            OnAddOrUpdateModelSize = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null) {
                        callback(true, msg["data"]);
                    }
                } else {
                    if (callback != null) {
                        JsonData jn = new JsonData();
                        callback(false, jn);
                    }
                    LogSystem.Warn("AddOrUpdateModelSize return failed.");
                }
            } else {
                if (callback != null) {
                    JsonData json = new JsonData();
                    callback(false, json);
                }
                LogSystem.Warn("network require failed.");
            }
        }
        public void OnGetModelSizeListRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnGetModelSizeList;
            OnGetModelSizeList = null;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null) {
                        callback(true, msg["data"]);
                    }
                } else {
                    if (callback != null) {
                        JsonData jn = new JsonData();
                        callback(false, jn);
                    }
                    LogSystem.Warn("Get ModelSizeList return failed.");
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
        }public void OnGetUserSizeListRsp(JsonData msg) {
        }public void OnSaveUserSizeRsp(JsonData msg) {
        }public void OnDeleteUserSizeRsp(JsonData msg) {
        }public void OnQueryFabricListRsq(JsonData msg) {
        }public void OnGetFabricDetailRsq(JsonData msg) {
        }public void OnCountProductInfoPriceRsq(JsonData msg) {
        }
    }
}
