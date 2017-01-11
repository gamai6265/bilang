using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Design {
    public partial class DesignWorld {
        public void GetFabricDetail(SerialModelInfo serialModelInfo, MyAction<bool> callback) {
            if (callback != null) {
                getFabricDetailCallBack += callback;
                List<string> lstFabric = new List<string>();
                var parInfo = serialModelInfo.LstPartInfo;
                for (var i = 0; i < parInfo.Count; i++) {
                    if (parInfo[i].Fabric1 != string.Empty && !lstFabric.Contains(parInfo[i].Fabric1)) {
                        lstFabric.Add(parInfo[i].Fabric1);
                    }
                    if (parInfo[i].Fabric2 != string.Empty && !lstFabric.Contains(parInfo[i].Fabric2)) {
                        lstFabric.Add(parInfo[i].Fabric2);
                    }
                }
                _networkMgr.QuerySender<IDesignClothesSender>().SendGetFabricDetailReq(lstFabric);
            }
        }
        public void GetFabricStyleList(MyAction<bool, List<KeyValuePair<string, string>>> callback) {
            if (_dicFabricStyle.Count <= 0) {
                OnFabricStyle += callback;
                CodeSetProvider.Instance.QueryCodeset("FABRIC_FIGURE", OnGetFabricStyle);
            } else {
                if (callback != null)
                    callback(true, _GetFabricStyleList());
            }
        }

        private IEnumerator LoadGlobalCacheImag(PartInfo partInfo, ModelView _dicDesignModel, List<string> lstMaterials) {
            if (partInfo.Fabric1 != string.Empty || partInfo.Fabric1 != "" || partInfo.Fabric1 != null) {
                string filePath = Application.persistentDataPath + "/" + WWW.EscapeURL(partInfo.Fabric1 + ".JPG");
                if (File.Exists(filePath)) {
                    FileStream fs = new FileStream(filePath, FileMode.Open);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, (int) fs.Length);
                    fs.Close();
                    fs.Dispose();
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(buffer);
                    yield return texture;
                    _dicDesignModel.ChangeGlobal(texture, lstMaterials);
                    if (!_dicOrignFabricTexture.ContainsKey(WWW.EscapeURL(partInfo.Fabric1))) {
                        var dcii = new Utils.DownCacheImageInfo();
                        dcii.Texture = texture;
                        _dicOrignFabricTexture.Add(WWW.EscapeURL(partInfo.Fabric1), dcii);
                    }
                }
                else {
                    WWW www = new WWW(Constants.NiuKouTuPian + WWW.EscapeURL(partInfo.Fabric1 + ".JPG"));
                    yield return www;
                    Texture2D texture = null;
                    if (www.error == null) {
                        if (www.isDone) {
                            texture = www.texture;
                        }
                    }
                    else {
                        www = new WWW(Constants.DefaultImage);
                        yield return www;
                        texture = www.texture;
                    }
                    www.Dispose();
                    _dicDesignModel.ChangeGlobal(texture, lstMaterials);
                    if (!_dicOrignFabricTexture.ContainsKey(WWW.EscapeURL(partInfo.Fabric1))) {
                        var dcii = new Utils.DownCacheImageInfo();
                        dcii.Texture = texture;
                        _dicOrignFabricTexture.Add(WWW.EscapeURL(partInfo.Fabric1), dcii);
                    }
                    var pngData = texture.EncodeToJPG();
                    File.WriteAllBytes(filePath, pngData);
                }
            }
            else {
                yield return null;
            }
        }

        public void GetFabricInfo(string fabricStyleCode, int page, string queryType, MyAction<bool, List<FabricInfo>> callback) {
            if (_dicFabricStyle.ContainsKey(fabricStyleCode)) {
                var fabricStyle = _dicFabricStyle[fabricStyleCode];
                var sendPage = page;
                var needSend = true;
                if (fabricStyle.IsPageValid) {
                    if (page <= fabricStyle.PageNum) {
                        if (fabricStyle.DicFabricPageData.ContainsKey(page)) {
                            needSend = false;
                            if (callback != null)
                                callback(true, fabricStyle.DicFabricPageData[page]);
                        }
                    } else {
                        needSend = false;
                        if (callback != null)
                            callback(false, null);
                    }
                } else {
                    sendPage = 1;
                }
                if (needSend) {
                    OnFabricList += callback;
                    _networkMgr.QuerySender<IDesignClothesSender>()
                        .SendQueryFabricListReq(new FabricQuery() {
                            QueryType = queryType,
                            PageNo = sendPage.ToString(),
                            FabricFigure = fabricStyleCode,
                            Brand = "24",
                            MateType = "2",
                            MateName = "0"

                        });
                }
            }
            else if (fabricStyleCode == "Global") {
                var fabricFigure = string.Empty;
                var mateType = string.Empty;
                var mateName = "0";
                var brand = string.Empty;
                switch (queryType) {
                    case "0":
                        mateType = "2";//面料
                        fabricFigure = (_orderCenter.FabricFigureCode == "-1") ? "01" : _orderCenter.FabricFigureCode;//TODO LOW B
                        brand = "24";
                        break;
                    case "1":
                        mateType = "1";//纽扣
                        var fabricStyle = _dicFabricStyle["13"];
                        if (fabricStyle.IsPageValid) {
                            if (page > fabricStyle.PageNum) {
                                return;
                            }
                        }
                        break;
//                    case "2":
//                        return;
//                        衣缝线
//                        break;
                }

                OnFabricList += callback;
                _networkMgr.QuerySender<IDesignClothesSender>()
                    .SendQueryFabricListReq(new FabricQuery() {
                        QueryType = queryType,
                        PageNo = page.ToString(),
                        FabricFigure = fabricFigure,
                        MateType = mateType,
                        MateName = mateName,
                        Brand = brand,
                    });
            }
            else {
                if (callback != null)
                    callback(false, null);
            }
        }

        public void OnGetPricePropertysRsp(JsonData msg) {
            IDictionary json = msg;
            if (json.Contains("exit")) {
                if (Utils.GetJsonStr(msg["exit"]) == "0") {
                    //Constants.propertysData = msg["data"];
                } else {
                    LogSystem.Error("network requeset is failed.");
                }
            } else {
                LogSystem.Error("network requeset is failed.");
            }
        }

        public void OnGetCodesetRsp(JsonData msg) {
        }

        public void OnGetWishListRsp(JsonData msg) {
            //TODO achieve ShopCarList
            IDictionary json = msg;
            if (json.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    JsonData goods = msg["data"];
                } else {
                    LogSystem.Error("network requeset is failed.");
                }
            } else {
                LogSystem.Error("network requeset is failed.returns=0");

            }
        }

        public void OnDeleteShirtTempletRsp(JsonData msg) {
        }

        public void OnGetCollectDetailRsp(JsonData msg) {
        }

        public void OnSaveWishiListRsp(JsonData msg) {
        }

        public void OnGetModelSizeListRsp(JsonData msg) {
            var callback = OnModelSize;
            OnModelSize = null;
            IDictionary dic = msg;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    if (callback != null) {
                        callback(true, msg["data"]["properties"]);
                    }
                } else {
                    var json = new JsonData();
                    if (callback != null) {
                        callback(false, json);
                    }
                    LogSystem.Error("network requeset is failed.");
                }
            } else {
                LogSystem.Error("network requeset is failed.returns=0");
            }
        }

        public void OnGetUserSizeListRsp(JsonData msg) {
            var callback = OnUserSize;
            OnUserSize = null;
            IDictionary dic = msg;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    if (callback != null) {
                        callback(true, msg["data"]);
                    }
                } else {
                    var json = new JsonData();
                    if (callback != null) {
                        callback(false, json);
                    }
                    LogSystem.Error("network requeset is failed.");
                }
            } else {
                LogSystem.Error("network requeset is failed.returns=0");
            }
        }

        public void OnSaveUserSizeRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnSaveModelSize;
            OnSaveModelSize = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    if (callback != null) {
                        callback(true, msg["data"]);
                    }
                } else {
                    if (callback != null) {
                        var json = new JsonData();
                        callback(false, json);
                    }
                    LogSystem.Error("network requeset is failed.");
                }
            } else {
                LogSystem.Error("network requeset is failed.returns=0");
            }
        }

        public void OnDeleteUserSizeRsp(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnDeleteModelSize;
            OnDeleteModelSize = null;
            if (dic.Contains("successful")) {
                if (msg["successful"].IsBoolean) {
                    if (callback != null) {
                        callback(true, msg["data"]);
                    }
                } else {
                    if (callback != null) {
                        var json = new JsonData();
                        callback(false, json);
                    }
                    LogSystem.Error("network requeset is failed.");
                }
            } else {
                LogSystem.Error("network requeset is failed.returns=0");
            }
        }

        public void OnQueryFabricListRsq(JsonData msg) {
            var callback = OnFabricList;
            OnFabricList = null;
            IDictionary json = msg;
            if (json.Contains("resultCode") && (int) msg["resultCode"] == 1) {
                if (int.Parse(Utils.GetJsonStr(msg["data"]["recordCount"])) > 0) {
                    var fabricData = msg["data"]["fabricList"];
                    if (fabricData.Count > 0) {
                        var fabricStyleCode = Utils.GetJsonStr(fabricData[0]["fabricFigure"]);
                        if (_dicFabricStyle.ContainsKey(fabricStyleCode)) {
                            var fabricStyle = _dicFabricStyle[fabricStyleCode];
                            fabricStyle.PageNum = int.Parse(Utils.GetJsonStr(msg["data"]["pageCount"])); //TODO
                            fabricStyle.IsPageValid = true;
                            var page = int.Parse(Utils.GetJsonStr(msg["data"]["pageNo"])); //TODO
                            List<FabricInfo> lstFabrics = null;
                            if (fabricStyle.DicFabricPageData.ContainsKey(page)) {
                                lstFabrics = fabricStyle.DicFabricPageData[page];
                            }
                            else {
                                lstFabrics = new List<FabricInfo>();
                                for (var i = 0; i < fabricData.Count; i++) {
                                    var fabricInfo = new FabricInfo();
                                    fabricInfo.FabricCode = Utils.GetJsonStr(fabricData[i]["mateCode"]);
                                    fabricInfo.FabricImage = WWW.EscapeURL(Utils.GetJsonStr(fabricData[i]["mateImage"]));
                                    fabricInfo.Currency = Utils.GetJsonStr(fabricData[i]["currency"]);
                                    fabricInfo.FabricPrice = Utils.GetJsonStr(fabricData[i]["price"]);
                                    fabricInfo.FabricColor = Utils.GetJsonStr(fabricData[i]["fabricColor"]);
                                    fabricInfo.FabricFigure = Utils.GetJsonStr(fabricData[i]["fabricFigure"]);
                                    fabricInfo.FabricTexture = Utils.GetJsonStr(fabricData[i]["fabricTexture"]);
                                    fabricInfo.FabricPly = Utils.GetJsonStr(fabricData[i]["fabricPly"]);
                                    fabricInfo.FabricOrigin = Utils.GetJsonStr(fabricData[i]["fabricOrigin"]);
                                    fabricInfo.Brand = Utils.GetJsonStr(fabricData[i]["brand"]);
                                    fabricInfo.YarnCount = Utils.GetJsonStr(fabricData[i]["yarnCount"]);
                                    fabricInfo.FabricColorName = Utils.GetJsonStr(fabricData[i]["fabricColorName"]);
                                    fabricInfo.FabricTextureName = Utils.GetJsonStr(fabricData[i]["fabricTextureName"]);
                                    fabricInfo.FabricFigureName = Utils.GetJsonStr(fabricData[i]["fabricFigureName"]);
                                    fabricInfo.ActualWidth =
                                        float.Parse(Utils.GetJsonStr(fabricData[i]["takenActualWidth"]));
                                    fabricInfo.SuppType = Utils.GetJsonStr(fabricData[i]["mateSuppType"]);
                                    if (fabricInfo.ActualWidth > 0)
                                        fabricInfo.ActualWidth = 105/(fabricInfo.ActualWidth*1.31f); //TODO danie
                                    lstFabrics.Add(fabricInfo);
                                    if (!_dicAllFabric.ContainsKey(fabricInfo.FabricCode))
                                        _dicAllFabric.Add(fabricInfo.FabricCode, fabricInfo);
                                }
                                fabricStyle.DicFabricPageData.Add(page, lstFabrics);
                            }
                            if (callback != null)
                                callback(true, lstFabrics);
                        }
                    }
                }
            }
            else {
                LogSystem.Error("network requeset is failed.");
                if (callback != null)
                    callback(false, null);
            }
        }

        public void OnGetFabricDetailRsq(JsonData msg) {
            IDictionary dic = msg;
            var callback = getFabricDetailCallBack;
            getFabricDetailCallBack = null;
            if (dic.Contains("resultCode")) {
                if ((int)msg["resultCode"] == 1 )  {
                    if((int)msg["data"]["recordCount"] > 0) {
                        var fabricDetail = msg["data"]["fabricDetail"];
                        for (var i = 0; i < fabricDetail.Count; i++) {
                            var fabricDetailItem = fabricDetail[i];
                            var fabricCode = Utils.GetJsonStr(fabricDetailItem["mateCode"]);
                            if (!_dicAllFabric.ContainsKey(fabricCode)) {
                                var fabricInfo = new FabricInfo();
                                fabricInfo.FabricCode = fabricCode;
                                fabricInfo.FabricImage = WWW.EscapeURL(Utils.GetJsonStr(fabricDetailItem["mateImage"]));
                                fabricInfo.Currency = Utils.GetJsonStr(fabricDetailItem["currency"]);
                                fabricInfo.FabricPrice = Utils.GetJsonStr(fabricDetailItem["price"]);
                                fabricInfo.FabricColor = Utils.GetJsonStr(fabricDetailItem["fabricColor"]);
                                fabricInfo.FabricFigure = Utils.GetJsonStr(fabricDetailItem["fabricFigure"]);
                                fabricInfo.FabricTexture = Utils.GetJsonStr(fabricDetailItem["fabricTexture"]);
                                fabricInfo.FabricPly = Utils.GetJsonStr(fabricDetailItem["fabricPly"]);
                                fabricInfo.FabricOrigin = Utils.GetJsonStr(fabricDetailItem["fabricOrigin"]);
                                fabricInfo.Brand = Utils.GetJsonStr(fabricDetailItem["brand"]);
                                fabricInfo.YarnCount = Utils.GetJsonStr(fabricDetailItem["yarnCount"]);
                                fabricInfo.FabricColorName = Utils.GetJsonStr(fabricDetailItem["fabricColorName"]);
                                fabricInfo.FabricTextureName = Utils.GetJsonStr(fabricDetailItem["fabricTextureName"]);
                                fabricInfo.FabricFigureName = Utils.GetJsonStr(fabricDetailItem["fabricFigureName"]);
                                fabricInfo.ActualWidth = float.Parse(Utils.GetJsonStr(fabricDetailItem["takenActualWidth"]));
                                if (fabricInfo.ActualWidth > 0)
                                    fabricInfo.ActualWidth = 105 / (fabricInfo.ActualWidth * 1.31f);
                                fabricInfo.SuppType = Utils.GetJsonStr(fabricDetailItem["mateSuppType"]);

                                _dicAllFabric.Add(fabricCode, fabricInfo);
                            }
                        }
                    }
                    if (callback != null)
                        callback(true);
                } else {
                    LogSystem.Error("req return failed");
                    if (callback != null)
                        callback(false);
                }
            } else {
                LogSystem.Error("req return returns");
                if (callback != null)
                    callback(false);
            }
        }

        private void OnGetFabricStyle(CodeSet codeset) {
            var data = codeset.GetData();
            foreach (var item in data) {
                if (!_dicFabricStyle.ContainsKey(item.Key)) {
                    var fabricStyle = new FabricStyle {
                        StyleName = item.Value,
                        StyleCode = item.Key
                    };
                    _dicFabricStyle.Add(item.Key, fabricStyle);
                }
            }

            for (var i = 1; i < GlobalProvider.Instance.GetDataCount()+1; i++) {//TODO 临时做法
                var globalConfig = GlobalProvider.Instance.GetDataById(i);
                var style = new FabricStyle {
                    StyleName = globalConfig.Id.ToString(),
                    StyleCode = globalConfig.FabricStyle
                };
                if (!_dicGlobalStyle.ContainsKey(style.StyleCode)) {
                    _dicFabricStyle.Add(style.StyleCode, style);
                    _dicGlobalStyle.Add(style.StyleCode, style.StyleName);
                }
            }


            if (OnFabricStyle != null) {
                var callback = OnFabricStyle;
                OnFabricStyle = null;
                callback(true, _GetFabricStyleList());
            }
        }

        public void OnCountProductInfoPriceRsq(JsonData msg) {
            IDictionary dic = msg;
            var callback = OnCountPrice;
            if (dic.Contains("successful")) {
                if ((bool)msg["successful"]) {
                    if (callback != null) {
                        callback(true, Utils.GetJsonStr(msg["data"]["display"]), Utils.GetJsonStr(msg["data"]["price"]));
                    }
                } else {
                    if (callback != null) {
                        callback(false, string.Empty, string.Empty);
                    }
                    LogSystem.Warn("Get CountProductInfo return failed.");
                }
            } else {
                if (callback != null) {
                    callback(false, string.Empty, string.Empty);
                }
                LogSystem.Warn("network require failed.");
            }
        }
    }
}