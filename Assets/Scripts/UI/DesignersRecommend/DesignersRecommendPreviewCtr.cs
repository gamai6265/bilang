using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class DesignersRecommendPreviewCtr : IController {
        private DesignersRecommendPreviewViewPresenter _view;
        private ILoginMgr _loginMgr;
        private IOrderCenter _orderCenter;

        private bool IsBuyNow = false;
        private Texture2D _mainPic = null;
        private JsonData _Productjson = null;
        private List<string> _lisBannerPic = new List<string>();
        private List<string> _lisDetailPic = new List<string>();

        public bool Init() {
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (null == _loginMgr) {
                LogSystem.Error("login mgr is null.");
                return false;
            }
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            EventCenter.Instance.AddListener<JsonData>(EventId.DesignersRecommendPreviewData, GetData);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<DesignersRecommendPreviewViewPresenter>(WindowId.DesignersRecommendPreview);
                _view.ClickClose += OnCloseClick;
                _view.ClickSaveShopCar += OnSaveShopCarClick;
                _view.ClickBuyNow += OnBuyNowClick;
                _view.ClickGoShopCar += OnGoShopCarClick;
            }
            UiManager.Instance.ShowWindow(WindowId.DesignersRecommendPreview);
        }

        private void OpenWeb(string url) {
            //          UniWebDemo.Instance.OpenWeb(url, WebType.DesignersRecommend);
        }

        private void GetData(JsonData data) {
            _Productjson = null;
            _Productjson = data;
            UiControllerMgr.Instance.ShowLodingTips(true);
            _view.SetProductLabel(data);
            GetPicList(data);
        }

        private void GetPicList(JsonData data) {
            _view.ReSetBanner();
            _view.ReSetPreview();
            _lisBannerPic.Clear();
            _lisDetailPic.Clear();
            var str = string.Empty;
            for (var i = 1; i < 7; i++) {
                str = "bannerPic" + i;
                if(Utils.GetJsonStr(data[str])!=string.Empty)
                    _lisBannerPic.Add(Utils.GetJsonStr(data[str]));

                str = "detailPic" + i;
                if (Utils.GetJsonStr(data[str]) != string.Empty)
                    _lisDetailPic.Add(Utils.GetJsonStr(data[str]));
            }
            DownLoadBannerPic(_lisBannerPic);
            DownLoadDetailPic(_lisDetailPic);
            CoroutineMgr.Instance.StartCoroutine(DownLoadMainPic(Utils.GetJsonStr(data["mainPic"])));
        }

        private void DownLoadBannerPic(List<string> lstBanner) {
            if (lstBanner.Count > 0) {
                CoroutineMgr.Instance.StartCoroutine(_DownLoadBannerPic(lstBanner[0]));
            }
        }

        IEnumerator _DownLoadBannerPic(string picPath) {
            Texture2D texture2D = null;
            var www = new WWW(Constants.KuanShiCanKao + picPath);
            yield return www;
            if (www.error == null) {
                texture2D = www.texture;
            }else {
                www = new WWW(Constants.DefaultImage);
                yield return www;
                texture2D = www.texture;
            }
            www.Dispose();
            _view.InitBannerPic(texture2D);
            if (_lisBannerPic.Contains(picPath))
                _lisBannerPic.Remove(picPath);
            DownLoadBannerPic(_lisBannerPic);
        }

        private void DownLoadDetailPic(List<string> lstDetail) {
            if (lstDetail.Count > 0) {
                CoroutineMgr.Instance.StartCoroutine(_DownLoadDetailPic(lstDetail[0]));
            }else {
                UiControllerMgr.Instance.ShowLodingTips(false);
            }
        }

        IEnumerator _DownLoadDetailPic(string picPath) {
            Texture2D texture2D = null;
            var www = new WWW(Constants.KuanShiCanKao + picPath);
            yield return www;
            if (www.error == null) {
                texture2D = www.texture;
            } else {
                www = new WWW(Constants.DefaultImage);
                yield return www;
                texture2D = www.texture;
            }
            www.Dispose();
            _view.InitDetailPic(texture2D);
            if (_lisDetailPic.Contains(picPath))
                _lisDetailPic.Remove(picPath);
            DownLoadDetailPic(_lisDetailPic);
        }

        IEnumerator DownLoadMainPic(string picPath) {
            _mainPic = null;
            var www = new WWW(Constants.KuanShiCanKao + picPath);
            yield return www;
            if (www.error == null) {
                _mainPic = www.texture;
            } else {
                www = new WWW(Constants.DefaultImage);
                yield return www;
                _mainPic = www.texture;
            }
            www.Dispose();
        }

        private void OnBuyNowClick() {
            IList<KeyValuePair<string, byte[]>> files = new List<KeyValuePair<string, byte[]>>();
            if (_loginMgr.IsLogined()) {
                IsBuyNow = true;
                _orderCenter.SaveShopCar(GetGoodsJson(), files, GetShirtDesignerCode(), SaveShopCarCallBack);
            } else {
                MyAction<bool> logicSuccAction = null;
                logicSuccAction = (isSucc) => {
                    EventCenter.Instance.RemoveListener<bool>(EventId.LoginDone, logicSuccAction);
                    if (isSucc) {
                        IsBuyNow = true;
                        _orderCenter.SaveShopCar(GetGoodsJson(), files, GetShirtDesignerCode(), SaveShopCarCallBack);
                    }
                };
                EventCenter.Instance.AddListener<bool>(EventId.LoginDone, logicSuccAction);
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
            }
        }

        private void OnSaveShopCarClick() {
            IList<KeyValuePair<string, byte[]>> files = new List<KeyValuePair<string, byte[]>>();
            if (_loginMgr.IsLogined()) {
                IsBuyNow = false;
                _orderCenter.SaveShopCar(GetGoodsJson(), files, GetShirtDesignerCode(), SaveShopCarCallBack);
            } else {
                MyAction<bool> logicSuccAction = null;
                logicSuccAction = (isSucc) => {
                    EventCenter.Instance.RemoveListener<bool>(EventId.LoginDone, logicSuccAction);
                    if (isSucc) {
                        IsBuyNow = false;
                        _orderCenter.SaveShopCar(GetGoodsJson(), files, GetShirtDesignerCode(), SaveShopCarCallBack);
                    }
                };
                EventCenter.Instance.AddListener<bool>(EventId.LoginDone, logicSuccAction);
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
            }
        }
        
        private void OnGoShopCarClick() {
            if (_loginMgr.IsLogined()) {
//                Constants.IsShopCar = true;
                UiControllerMgr.Instance.GetController(WindowId.ShopCar).Show();
                Hide();
            } else {
                MyAction<bool> logicSuccAction = null;
                logicSuccAction = (isSucc) => {
                    EventCenter.Instance.RemoveListener<bool>(EventId.LoginDone, logicSuccAction);
                    if (isSucc) {
//                        Constants.IsShopCar = true;
                        UiControllerMgr.Instance.GetController(WindowId.ShopCar).Show();
                        Hide();
                    } 
                };
                EventCenter.Instance.AddListener<bool>(EventId.LoginDone, logicSuccAction);
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
            }
        }

        private string GetGoodsJson() {
            var jdata=new JsonData();
            jdata["goodsNumber"] = "1";
            jdata["modelId"] = "1";
            jdata["goodsPrice"] = Utils.GetJsonStr(_Productjson["salesPrice"]);
            jdata["goodsName"] = Utils.GetJsonStr(_Productjson["name"]);
            return jdata.ToJson();
        }

        private string GetShirtDesignerCode() {
            var str = Utils.GetJsonStr(_Productjson["productId"]);
            return str;
        }

        private void SaveShopCarCallBack(bool result, JsonData msg) {
            if (result) {
                if (!IsBuyNow)
                    UiControllerMgr.Instance.ShowTips("添加购物车成功");
                else {
                    List<ShopItems> shopitems = new List<ShopItems>();
                    var shopItem = new ShopItems();
                    shopItem.ModelId = "1";
                    shopItem.Id = Utils.GetJsonStr(msg["atid"]);
                    shopItem.GoodsName = Utils.GetJsonStr(msg["goodsName"]);
                    shopItem.Num = Utils.GetJsonStr(msg["goodsNumber"]);
                    shopItem.GoodPic = _mainPic;
                    shopitems.Add(shopItem);

                    UiControllerMgr.Instance.GetController(WindowId.OrderConfirm).Show();
                    EventCenter.Instance.Broadcast(EventId.SetOrderConfirmPrice, Utils.GetJsonStr(msg["goodsPrice"]));
                    EventCenter.Instance.Broadcast(EventId.GetShopCarData, shopitems);
                    Hide();
                }
            }
        }

        private void OnCloseClick() {
            OnBack(-1);
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
//            UniWebDemo.Instance.CloseWeb();
            Hide();
            return true;
        }

        public void Hide(MyAction onComplete = null) {
            IsBuyNow = false;
            UiManager.Instance.HideWindow(WindowId.DesignersRecommendPreview);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.DesignersRecommendPreview, "ui/DesignersRecommendPreview"),
            };
        }
    }
}