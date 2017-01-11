using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class DesignCtr : IController{
        private ILoginMgr _loginMgr;
        private DesignViewPresenter _view;
        private IDesignWorld _designWorld;
        private ILogicModule _logicModule;
        private IOrderCenter _orderCenter;
        private IUserCenter _userCenter;
        private bool _hasFileterEvent = false;
        private string _price = string.Empty;
        private bool _isBuyNow = false;
        private Texture2D _shirtTexture2D = null;

        public bool Init() {
            _loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (null == _loginMgr) {
                LogSystem.Error("login mgr is null.");
                return false;
            }
            _designWorld = ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (null == _designWorld) {
                LogSystem.Error("design world is null");
                return false;
            }
            _designWorld.OnChangeDesignStateEvent += OnChangeDesignState;
            _logicModule = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (_logicModule == null) {
                LogSystem.Error("error. logic module is null");
                return false;
            }
            _orderCenter=ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter)as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("error ordercenter is null");
                return false;
            }
            _userCenter=ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter)as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("error usercenter is null");
                return false;
            }
            EventCenter.Instance.AddListener(EventId.LeaveStageDesign, () => Hide());
            EventCenter.Instance.AddListener<string>(EventId.SetDesignTitle, SetDesignTitle);
            _hasFileterEvent = false;
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<DesignViewPresenter>(WindowId.Design);
                _view.ClickBack += OnBackClick;
                _view.ClickNext += OnNextClick;
                _view.ClickSave += OnSaveClick;
                _view.ClickShopCar += OnShopCarClick;
                _view.ClickShare += OnShareClick;
                _view.ClickBuyNow += OnBuyNow;
            }
            if (!_hasFileterEvent) {
                _designWorld.OnCountPrice += SetPrice;
                _hasFileterEvent = true;
            }
            UiManager.Instance.ShowWindow(WindowId.Design);
        }

        private void SetPrice(bool result, string display, string price) {
            if (result) {
                _price = price;
                _view.SetPrice(display, price);
                _view.SetShirtName(_designWorld.GetShirtName());
            }

        }

        private void SetDesignTitle(string str) {
            _view.SetDesignTitle(str);
        }

        private void OnChangeDesignState(DesignState state) {
            switch (state) {
                case DesignState.StatePreview:
                    Show();
                    _view.SetDesignTitle(StrConfigProvider.Instance.GetStr("dp_titlepreview"));//TODO
                    _view.ChangeBtnToDesign(false);
                    break;
                case DesignState.StateDesign:
                    Show();
                    _view.ChangeBtnToDesign(true);
                    _view.SetDesignTitle(StrConfigProvider.Instance.GetStr("dp_titledesign"));
                    break;
                case DesignState.StateStyle:
                    Show();
                    _view.ChangeBtnToDesign(true);
                    break;
            }
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            return false;
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Design, "ui/Design"),
            };
        }

        public void Hide(MyAction onComplete=null) {
            if (_view != null) {
                UiManager.Instance.HideWindow(WindowId.Design);
                _view.SetPrice(string.Empty, string.Empty);
                _designWorld.OnCountPrice -= SetPrice;
                _hasFileterEvent = false;
            }
        }

        private void OnDestroy() {
        }
        
        private void SendProductToShopCar(MyAction<bool, JsonData> callback) {
            List<PartInfo> partInfos = _designWorld.GetPartInfo();
            JsonData productJson = new JsonData();
            productJson["modelId"] = _designWorld.GetModelId().ToString();
            productJson["goodsNumber"] = "1";
            productJson["goodsPrice"] = _price;
            productJson["goodsName"] = _designWorld.GetShirtName();
            var parts = productJson["parts"] = new JsonData();
            for (int i = 0; i < partInfos.Count; i++) {
                var partItem = new JsonData();
                partItem["partId"] = partInfos[i].PartId.ToString();
                partItem["mateId1"] = partInfos[i].Fabric1;
                partItem["mateId2"] = partInfos[i].Fabric2;
                partItem["mateRotate1"] = partInfos[i].Fabric1Rotation;
                partItem["mateRotate2"] = partInfos[i].Fabric2Rotation;
                partItem["weight"] = partInfos[i].PartWeight;
//                                Debug.Log(partInfos[i].PartId + "/" + partInfos[i].Partname+"/"+ partInfos[i].Fabric1+"/"+ partInfos[i].Fabric2);
                parts.Add(partItem);
            }

            IList<KeyValuePair<string, byte[]>> files = new List<KeyValuePair<string, byte[]>>();
            byte[] bytes = null;
            byte[] bytes2 = null;
            _designWorld.GetScreenShot((texture2D, texture2D2) => {
                _shirtTexture2D = texture2D;
                bytes = texture2D.EncodeToJPG();
                bytes2 = texture2D2.EncodeToJPG();
            });
            files.Add(new KeyValuePair<string, byte[]>("images", bytes));
            files.Add(new KeyValuePair<string, byte[]>("images", bytes2));
//                        Debug.Log(productJson.ToJson());
//            if (_isBuyNow) {
//                _orderCenter.OneKeyBuyProduct(productJson.ToJson(), files, callback);
//            } else {
                _orderCenter.SaveShopCar(productJson.ToJson(), files, string.Empty, callback);
//            }
        }
        
        private void OnShopCarClick() {
            _isBuyNow = false;
            ProductToShopCar();
        }

        private void ProductToShopCar() {
            _designWorld.StraightenModel();
            if (_loginMgr.IsLogined()) {
                CoroutineMgr.Instance.StartCoroutine(SaveShopCar(0.2f));
            } else {
                MyAction<bool> logicSuccAction = null;
                logicSuccAction = (isSucc) => {
                    EventCenter.Instance.RemoveListener<bool>(EventId.LoginDone, logicSuccAction);
                    if (isSucc) {
                        UiControllerMgr.Instance.ShowLodingTips(true);
                        SendProductToShopCar(SaveShopCarCallBack);
                    }
                };
                EventCenter.Instance.AddListener<bool>(EventId.LoginDone, logicSuccAction);
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
            }
        }


        private IEnumerator SaveShopCar(float waitTime) {//TODO LOW
            yield return new WaitForSeconds(waitTime);
            UiControllerMgr.Instance.ShowLodingTips(true);
            SendProductToShopCar(SaveShopCarCallBack);
        }

        private void SaveShopCarCallBack(bool result, JsonData jsonData) {
            UiControllerMgr.Instance.ShowLodingTips(false);
            if (result) {
                if (!_isBuyNow) {
                    UiControllerMgr.Instance.ShowMessageBox(
                        "",
                        "添加购物车成功,马上前往购物车结算",
                        StrConfigProvider.Instance.GetStr("mb_cancle"),
                        () => {
                        },
                        StrConfigProvider.Instance.GetStr("mb_confirm"),
                        () => {
                            Constants.HomeShow = false;
                            _logicModule.SwitchStage(StageType.StageEmpty);
                            UiControllerMgr.Instance.GetController(WindowId.ShopCar).Show(); //TODO too low
                        }
                        );
                }
                else {
                    Constants.HomeShow = false;
                    Constants.HomeBtnsShow = false;
                    _logicModule.SwitchStage(StageType.StageEmpty);
                    List<ShopItems> shopitems = new List<ShopItems>();
                    var shopItem = new ShopItems();
                    shopItem.ModelId = _designWorld.GetModelId().ToString();
                    shopItem.Id = Utils.GetJsonStr(jsonData["atid"]);//Utils.GetJsonStr(jsonData["goodsId"]);
                    shopItem.GoodsName = Utils.GetJsonStr(jsonData["goodsName"]);//_designWorld.GetShirtName();
                    shopItem.Num = Utils.GetJsonStr(jsonData["goodsNumber"]);//"1";
                    shopItem.GoodPic = _shirtTexture2D;
                    shopitems.Add(shopItem);
                    _isBuyNow = false;
                    _shirtTexture2D = null;

                    UiControllerMgr.Instance.GetController(WindowId.OrderConfirm).Show();
                    EventCenter.Instance.Broadcast(EventId.SetOrderConfirmPrice, _price);
                    EventCenter.Instance.Broadcast(EventId.GetShopCarData, shopitems);
                    Hide();
                }
            }
        }
        
        private void OnSaveClick() {
            _designWorld.StraightenModel();
            if (_loginMgr.IsLogined()) {
                UiControllerMgr.Instance.GetController(WindowId.SaveStyle).Show();
                EventCenter.Instance.Broadcast<string>(EventId.SaveStylePrice, _price);
            } else {
                MyAction<bool> logicSuccAction = null;
                logicSuccAction = (isSucc) => {
                    EventCenter.Instance.RemoveListener<bool>(EventId.LoginDone, logicSuccAction);
                    if (isSucc) {
                        UiControllerMgr.Instance.GetController(WindowId.SaveStyle).Show();
                        EventCenter.Instance.Broadcast<string>(EventId.SaveStylePrice, _price);
                    }
                };
                EventCenter.Instance.AddListener<bool>(EventId.LoginDone, logicSuccAction);
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
            }
        }
        
        private void OnNextClick() {
            _designWorld.DesignModel();
        }

        private void OnBackClick() {
            UiControllerMgr.Instance.ShowMessageBox(
                "",
                "退出设计",
               StrConfigProvider.Instance.GetStr("mb_cancle"),
                () => {
                },
                StrConfigProvider.Instance.GetStr("mb_confirm"),
                () => {
                    UiManager.Instance.HideAllShownWindow();
                    _logicModule.SwitchStage(StageType.StageEmpty);
                }
            );
        }

        private void OnShareClick() {
            if (_loginMgr.IsLogined()) {
                _userCenter.ShareUrl();
            } else {
                MyAction<bool> logicSuccAction = null;
                logicSuccAction = (isSucc) => {
                    EventCenter.Instance.RemoveListener<bool>(EventId.LoginDone, logicSuccAction);
                    if (isSucc) {
                        _userCenter.ShareUrl();
                    }
                };
                EventCenter.Instance.AddListener<bool>(EventId.LoginDone, logicSuccAction);
                UiControllerMgr.Instance.GetController(WindowId.Login).Show();
            }
        }

        private void OnBuyNow() {
            _isBuyNow = true;
            ProductToShopCar();
        }
    }
}