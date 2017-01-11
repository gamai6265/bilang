using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class ShopCarCtr : IController{
        private INetworkMgr _network;
        private IUserCenter _userCenter;
        private IOrderCenter _orderCenter;
        private UIGrid _shopCarGrid;
        private ShopCarViewPresenter _view;

        private int _index;
        private readonly int showCount = 5; //每次加载个数

        public bool Init() {
            _network = ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (null == _network) {
                LogSystem.Error("network is null.");
                return false;
            }

            _userCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComUserCenter) as IUserCenter;
            if (_userCenter == null) {
                LogSystem.Error("user center is null.");
                return false;
            }
            _orderCenter = ComponentMgr.Instance.FindComponent(ComponentNames.ComOrderCenter) as IOrderCenter;
            if (_orderCenter == null) {
                LogSystem.Error("order center is null.");
                return false;
            }
            EventCenter.Instance.AddListener<WindowId>(EventId.BtnsEvent, BtnsEvent);
            return true;
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<ShopCarViewPresenter>(WindowId.ShopCar);
                _shopCarGrid = _view.gameObject.GetComponentInChildren<UIGrid>();

                _view.SvDragFinished += OnSvDragFinished;

                _view.ClickBack += OnBackClick;
                _view.ClickAll += OnAllClick;
                _view.ClickSubmit += OnSubmitClick;
                _view.ClickSure += OnSureClick;

                _view.ClickAdd += OnClickAdd;
                _view.ClickSubtract += OnClickSubtract;
            }
            _view.SetDefaultChooseAll();
            UiManager.Instance.ShowWindow(WindowId.ShopCar);
            var btns = (HomeBtnsCtr) UiControllerMgr.Instance.GetController(WindowId.HomeBtns);
            btns.ShowBtns(WindowId.ShopCar, 1);
            UiControllerMgr.Instance.ShowLodingTips(true);
            _orderCenter.GetShopCarList(ShopCarList);
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            Hide();
            Constants.IsShopCar = false;
            Constants.HomeShow = true;
            Constants.HomeBtnsShow = true;
            UiControllerMgr.Instance.GetController(WindowId.HomeBtns).Show();
            UiControllerMgr.Instance.GetController(WindowId.Home).Show();
            return true;
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.ShopCar);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.ShopCar, "ui/ShopCar"),
            };
        }

        private void OnSvDragFinished() {
//            _index++;
//            GetShopCarList();
        }

        private void OnClickSubtract(string shopCarItemId) {
            for (var i = 0; i < _shopCarGrid.transform.childCount; i++) {
                var obj = _shopCarGrid.transform.GetChild(i).gameObject;
                if (obj.name == shopCarItemId) {
                    var scpp = obj.GetComponent<ShopCarPrefabViewPresenter>();
                    int tem;
                    int.TryParse(scpp.LblQuantity.text, out tem);
                    var quantity = tem;
                    float tem2;
                    float.TryParse(scpp.LblPrice.text, out tem2);
                    var unitprice = tem2 / tem;
                    if (quantity != 1)
                        quantity--;
                    scpp.LblQuantity.text = quantity.ToString();
                    scpp.LblPrice.text = (unitprice*quantity).ToString(CultureInfo.InvariantCulture);
                    if (scpp.IsChoose)
                        _view.SetQuantityAndPrice();
                    break;
                }
            }
        }

        private void OnClickAdd(string shopCarItemId) {
            for (var i = 0; i < _shopCarGrid.transform.childCount; i++) {
                var obj = _shopCarGrid.transform.GetChild(i).gameObject;
                if (obj.name == shopCarItemId) {
                    var scpp = obj.GetComponent<ShopCarPrefabViewPresenter>();
                    int tem;
                    int.TryParse(scpp.LblQuantity.text, out tem);
                    var quantity = tem;
                    float tem2;
                    float.TryParse(scpp.LblPrice.text, out tem2);
                    var unitprice = tem2 / tem;
                    quantity++;
                    scpp.LblQuantity.text = quantity.ToString();
                    scpp.LblPrice.text = (unitprice*quantity).ToString(CultureInfo.InvariantCulture);
                    if (scpp.IsChoose)
                        _view.SetQuantityAndPrice();
                    break;
                }
            }
        }

        private void OnSubmitClick() {
            List<ShopItems> shopitems=new List<ShopItems>();
            var tempQuantity = 0;
            float price = 0;
            for (var i = 0; i < _shopCarGrid.transform.childCount; i++) {
                var obj = _shopCarGrid.transform.GetChild(i).gameObject;
                var scpp = obj.GetComponent<ShopCarPrefabViewPresenter>();
                if (!scpp.IsChoose) continue;
                tempQuantity++;
                float tem;
                float.TryParse(scpp.LblPrice.text,out tem);
                price += tem;
                var shopItem=new ShopItems();
                shopItem.ModelId = scpp.modelID;
                shopItem.Id = scpp.Id;
                shopItem.GoodsName = scpp.LblName.text;
                shopItem.Num = scpp.LblQuantity.text;
                shopItem.GoodPic = (Texture2D)scpp.TexShopCar.mainTexture;
                shopitems.Add(shopItem);
            }
            if (tempQuantity == 0) {
                UiControllerMgr.Instance.ShowTips(StrConfigProvider.Instance.GetStr("tip_chooseclothes"));
                return;
            }
            Hide();
            UiControllerMgr.Instance.GetController(WindowId.OrderConfirm).Show();
            EventCenter.Instance.Broadcast(EventId.SetOrderConfirmPrice, price.ToString(CultureInfo.InvariantCulture));
            EventCenter.Instance.Broadcast(EventId.GetShopCarData, shopitems);

        }
        private void BtnsEvent(WindowId id) {
            if (WindowId.ShopCar == id) {
                Show();
            } else {
                Hide();
            }
        }
        private void OnSureClick(string shopCarItemId) {
            UiControllerMgr.Instance.ShowMessageBox(
                "",
                StrConfigProvider.Instance.GetStr("mb_deleteclothes"),
                StrConfigProvider.Instance.GetStr("mb_cancle"),
                () => {
                },
                StrConfigProvider.Instance.GetStr("mb_confirm"),
                () => {
                    _orderCenter.DeleteShorCarItem(shopCarItemId, DeleteItem);
                }
            );

        }

        private void OnAllClick() {
            _view.ChooseAll();
        }

        private void OnBackClick() {
            OnBack(-1);
        }


        private void ShopCarList(bool result, List<ShopCarItem> lstItems) {
            if (result) {
                _view.DeleteAll();
                _view.SetQuantityAndPrice();
                CoroutineMgr.Instance.StartCoroutine(IntShoppingcar(lstItems));
            }
        }


        private IEnumerator IntShoppingcar(List<ShopCarItem> lstShopCarInfo) {
            for (var i = 0; i < lstShopCarInfo.Count; i++) {
                var item = lstShopCarInfo[i];
                var www = new WWW(Constants.ImageURL + item.GoodsPic);
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
                _view.AddPrefabs(item, texture);
                _view.AdjustSizeItemDepth();

            }
            UiControllerMgr.Instance.ShowLodingTips(false);
        }

        private void DeleteItem(bool result, string  id) {
            if (result) {
                _view.DeletePrefabs(id);
            }
        }
    }
}