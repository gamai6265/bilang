using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Interfaces;
using LitJson;

namespace Cloth3D.Ui {
    public class ClothesDetailsCtr : IController {
        private ClothesDetailsViewPresenter _view;
        private IOrderCenter _orderCenter;
        private IDesignWorld _designWorld;
        private string _goodId = string.Empty;
        private string _quantity = string.Empty;

        public bool Init() {
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            _designWorld = ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (_designWorld == null) {
                LogSystem.Error("design world is null.");
                return false;
            }
            EventCenter.Instance.AddListener<string, string, string, string>(EventId.GetClothesDetailsData,
                GetClothesDetailsData);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<ClothesDetailsViewPresenter>(WindowId.ClothesDetails);
                _view.ClickBack += OnBackClick;
            }
            UiManager.Instance.ShowWindow(WindowId.ClothesDetails);     
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

        public void Hide(MyAction onComplete = null) {
            UiManager.Instance.HideWindow(WindowId.ClothesDetails);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.ClothesDetails, "ui/ClothesDetails"),
            };
        }


        void OnBackClick() {
            OnBack(-1);
        }

        void OnDestroy() {
        }


        private void GetClothesDetailsData(string orderId, string goodId, string channel, string quantity) {
            if (channel == "Order") {
                //获取订单详情
                _goodId = goodId;
                _orderCenter.GetOrderDetail(orderId, OrdersDetails);
//                _orderCenter.GetGoodsInfo(goodId, OrdersDetails);
            }
            else if (channel == "ShoppingCart") {
                //获取购物车详情
                _quantity = quantity;
                _orderCenter.GetShopCarDetail(goodId, GetShopCarDetail);
            }
        }

        private void OrdersDetails(bool result, JsonData json) {
            if (result) {
                CoroutineMgr.Instance.StartCoroutine(Orderdetails(json));
            }
        }

        private IEnumerator Orderdetails(JsonData json) {
            _view.DeleteAll();
            for (var i = 0; i < json["orderGoods"].Count; i++) {
                if (_goodId == Utils.GetJsonStr(json["orderGoods"][i]["goodsId"])) {                  
                    var goods = json["orderGoods"][i];
                    var clothesName = Utils.GetJsonStr(goods["goodsName"]);
                    var quantity = Utils.GetJsonStr(goods["goodsNumber"]);
                    var fabricCode = "fabricCode";
                    var buttonsCode = "buttonsCode";
                    _view.SetLabels(clothesName, quantity, fabricCode, buttonsCode);
                    for (int j = 1; j < 3; j++) {
//                        Debug.Log(Constants.ImageURL + Utils.GetJsonStr(goods["goodsId"]) + "-" + j + ".jpg");
                        WWW www = new WWW(Constants.ImageURL + Utils.GetJsonStr(goods["goodsId"]) + "-" + j + ".jpg");
                        yield return www;
                        Texture2D clothesTexture = null;
                        if (www.error == null) {
                            if (www.isDone) {
                                clothesTexture = www.texture;
                            }
                        }
                        else {
                            www = new WWW(Constants.DefaultImage);
                            yield return www;
                            clothesTexture = www.texture;
                        }
                        www.Dispose();
                        _view.SetClothesTexture(j, clothesTexture);
                    }
                    for (int j = 0; j < goods["parts"].Count; j++) {
                        var partsItem = goods["parts"][j];
                        var part = _designWorld.GetModelPart(int.Parse(Utils.GetJsonStr(partsItem["modelId"])),
                            int.Parse(Utils.GetJsonStr(partsItem["partId"])));                        
                        if (part != null && !string.IsNullOrEmpty(part.SampleImage)) {
                            WWW www = new WWW(Constants.YangShiTuPian + part.SampleImage);
                            yield return www;
                            Texture2D partTexture = null;
                            if (www.error == null) {
                                if (www.isDone) {
                                    partTexture = www.texture;
                                }
                            }
                            else {
                                www = new WWW(Constants.DefaultImage);
                                yield return www;
                                partTexture = www.texture;
                            }
                            www.Dispose();
                            var partName = part.DisplayName;
                            var remark = part.Instruction;
                            var fabric1 = Utils.GetJsonStr(partsItem["mateId1"]);
                            var fabric2 = Utils.GetJsonStr(partsItem["mateId2"]);
                            _view.AddDetailsPartsPrefabs(partName, fabric1, fabric2, remark, partTexture);
                            _view.AdjustSizeItemDepth();
                        }                      
                    }
                }
            }
        }

        private void GetShopCarDetail(bool result, JsonData jsonData) {
            if (result) {
                CoroutineMgr.Instance.StartCoroutine(InitShopCarDetail(jsonData));
            }
        }

        private IEnumerator InitShopCarDetail(JsonData json) {
            _view.DeleteAll();
            var clothesName = Utils.GetJsonStr(json["goodsName"]);
            var fabricCode = "fabricCode";
            var buttonsCode = "buttonsCode";
            _view.SetLabels(clothesName, _quantity, fabricCode, buttonsCode);

            for (int j = 1; j < 3; j++) {
                WWW www = new WWW(Constants.ImageURL + Utils.GetJsonStr(json["goodsId"]) + "-" + j + ".jpg");
                yield return www;
                Texture2D clothesTexture = null;
                if (www.error == null) {
                    if (www.isDone) {
                        clothesTexture = www.texture;
                    }
                }
                else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    clothesTexture = www.texture;
                }
                www.Dispose();
                _view.SetClothesTexture(j, clothesTexture);
            }

            for (var i = 0; i < json["goods"]["parts"].Count; i++) {
                var goods = json["goods"]["parts"][i];
                var part =_designWorld.GetModelPart(int.Parse(Utils.GetJsonStr(goods["modelId"])),
                        int.Parse(Utils.GetJsonStr(goods["partId"])));
                if (part != null&&!string.IsNullOrEmpty(part.SampleImage)) {
                    WWW www = new WWW(Constants.YangShiTuPian + part.SampleImage);
                    yield return www;
                    Texture2D partTexture = null;
                    if (www.error == null) {
                        if (www.isDone) {
                            partTexture = www.texture;
                        }
                    }
                    else {
                        www = new WWW(Constants.DefaultImage);
                        yield return www;
                        partTexture = www.texture;
                    }
                    www.Dispose();
                    var partName = Utils.GetJsonStr(part.DisplayName);
                    var remark = Utils.GetJsonStr(goods["modelId"]);
                    var fabric1 = Utils.GetJsonStr(goods["mateId1"]);
                    var fabric2 = Utils.GetJsonStr(goods["mateId2"]);
                    _view.AddDetailsPartsPrefabs(partName, fabric1, fabric2, remark, partTexture);
                    _view.AdjustSizeItemDepth();
                }
            }
        }
    }
}