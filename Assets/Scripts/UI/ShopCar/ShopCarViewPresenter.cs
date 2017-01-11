using System.Globalization;
using Cloth3D.Data;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class ShopCarViewPresenter : ViewPresenter {
        private string _itenId;

        public UILabel LblTitle;
        public UILabel LblAll;
        public UILabel Lbltotal;
        public UILabel LblCurrency;
        public UILabel LblSubmit;
        public UILabel Lblquantity;

        public UIButton BtnAll;
        public UIButton BtnBack;
        public UIButton BtnSubmit;

        private bool _isAll;
        public UILabel LblPrice;

        public UILabel LblQuantity;
        public UIGrid ShopCarGrid;
        public GameObject ShopCarPrefabs;

        public UIScrollView ShopCarScrollView;

        public event MyAction ClickBack;
        public event MyAction ClickAll;
        public event MyAction ClickSubmit;
        public event MyAction<string> ClickSure;
        public event MyAction SvDragFinished;

        public event MyAction<string> ClickAdd;
        public event MyAction<string> ClickSubtract;

        protected override void AwakeUnityMsg() {
            ShopCarScrollView.onDragFinished += OnSelectionScrollViewDragFinished;
            ChangeLanguage();
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("sc_title");
            LblAll.text = StrConfigProvider.Instance.GetStr("sc_all");
            Lbltotal.text = StrConfigProvider.Instance.GetStr("sc_total");
            LblCurrency.text = StrConfigProvider.Instance.GetStr("sc_currency");
            LblSubmit.text = StrConfigProvider.Instance.GetStr("sc_submit");
            Lblquantity.text = StrConfigProvider.Instance.GetStr("sc_quantity");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblAll.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            Lbltotal.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCurrency.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSubmit.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            Lblquantity.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnAll.onClick.Add(new EventDelegate(OnBtnAllClick));
            BtnSubmit.onClick.Add(new EventDelegate(OnBtnSubmitClick));
        }

        private void OnBtnSubmitClick() {
            if (null != ClickSubmit) {
                ClickSubmit();
            }
        }

        private void OnBtnAllClick() {
            if (null == ClickAll) return;
            _isAll = !_isAll;
            ClickAll();
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        protected override void OnDestroyUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        private void OnSelectionScrollViewDragFinished() {
            var constraint = ShopCarScrollView.panel.CalculateConstrainOffset(ShopCarScrollView.bounds.min,
                ShopCarScrollView.bounds.min);
            if (constraint.y < 1f) {
                if (null != SvDragFinished) {
                    SvDragFinished();
                }
            }
        }


        private void RepositionGrid() {
            ShopCarGrid.Reposition();
            ShopCarGrid.repositionNow = true;
            NGUITools.SetDirty(ShopCarGrid);
            var springPanel = ShopCarScrollView.GetComponent<SpringPanel>();
            var y = ShopCarScrollView.transform.localPosition.y - 1;
            springPanel.target = new Vector3(0, y, 0);
            springPanel.enabled = true;
        }

        public void AddPrefabs(ShopCarItem info, Texture2D texture2D) {
            var obj = NGUITools.AddChild(ShopCarGrid.gameObject, ShopCarPrefabs);

            var scpp = obj.GetComponent<ShopCarPrefabViewPresenter>();
            obj.name = scpp.Id = info.Id;
            scpp.modelID = info.ModelId;
            scpp.Code = info.GoodsId;
            scpp.TexShopCar.mainTexture = texture2D;
            scpp.LblName.text = info.GoodsName;
            scpp.LblQuantity.text = info.GoodsNum.ToString();
            scpp.LblPrice.text = info.GoodsPrice;

            scpp.ClickDelete += OnPrefabsDeleteClick;
            scpp.ClickChoose += OnClickChoose;
            scpp.ClickAdd += OnClickAdd;
            scpp.ClickSubtract += OnClickSubtract;

            RepositionGrid();
        }

        public void AdjustSizeItemDepth() {
            var depth = ShopCarScrollView.GetComponentInChildren<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(ShopCarScrollView.gameObject, depth + 1);
            GetSortedPanels(true);
        }

        private void OnClickSubtract(string shopCarItemId) {//减少
            _itenId = shopCarItemId;
            if (null != ClickSubtract) {
                ClickSubtract(shopCarItemId);
            }
        }

        private void OnClickAdd(string shopCarItemId) {//添加
            _itenId = shopCarItemId;
            if (null != ClickAdd) {
                ClickAdd(_itenId);
            }
        }
        
        private void OnClickChoose(string shopCarItemId) {//点击选中
            for (var i = 0; i < ShopCarGrid.transform.childCount; i++) {
                var obj = ShopCarGrid.transform.GetChild(i).gameObject;
                if (obj.name == shopCarItemId) {
                    var scpp = obj.GetComponent<ShopCarPrefabViewPresenter>();
                    if (_isAll) {
                        _isAll = !_isAll; //取消全选状态
                        var sprite2 = BtnAll.GetComponentInChildren<UISprite>();
                        sprite2.spriteName = "All";
                    }
                    scpp.IsChoose = !scpp.IsChoose;
                    var sprite = obj.transform.Find("BtnChoose").gameObject.GetComponentInChildren<UISprite>();
                    sprite.spriteName = scpp.IsChoose ? "Select" : "All";
                    break;
                }
            }
            SetQuantityAndPrice();
        }

        private void OnPrefabsDeleteClick(string shoCarItemId) {
            if (null != ClickSure) {
                ClickSure(shoCarItemId);
            }
        }

        public void DeletePrefabs(string objname) {
            for (var i = 0; i < ShopCarGrid.transform.childCount; i++) {
                var obj = ShopCarGrid.transform.GetChild(i).gameObject;
                if (obj.name == objname) {
                    Destroy(obj);
                    break;
                }
            }
            RepositionGrid();
            SetQuantityAndPrice();
        }

        public void DeleteAll() {
            ShopCarGrid.transform.DestroyChildren();
        }

        public void ChooseAll() {//全选
            for (var i = 0; i < ShopCarGrid.transform.childCount; i++) {
                var obj = ShopCarGrid.transform.GetChild(i).gameObject;
                var scpp = obj.GetComponent<ShopCarPrefabViewPresenter>();
                scpp.IsChoose = _isAll;
                var sprite = obj.transform.Find("BtnChoose").gameObject.GetComponentInChildren<UISprite>();
                sprite.spriteName = _isAll ? "Select" : "All";
            }
            var sprite2 = BtnAll.GetComponentInChildren<UISprite>();
            sprite2.spriteName = _isAll ? "Select" : "All";
            SetQuantityAndPrice();
        }

        public void SetQuantityAndPrice() {//设置价钱和数量
            var quantity = 0;
            float price = 0;

            for (var i = 0; i < ShopCarGrid.transform.childCount; i++) {
                var obj = ShopCarGrid.transform.GetChild(i).gameObject;
                var scpp = obj.GetComponent<ShopCarPrefabViewPresenter>();
                if (scpp.IsChoose) {
                    int tem;
                    int.TryParse(scpp.LblQuantity.text,out tem);
                    quantity += tem;
                    float tem2;
                    float.TryParse(scpp.LblPrice.text,out tem2);
                    price += tem2;
                }
            }
            LblQuantity.text = quantity.ToString();
            LblPrice.text = price.ToString(CultureInfo.InvariantCulture);
        }

        public void SetDefaultChooseAll() {
            _isAll = false;
            BtnAll.GetComponentInChildren<UISprite>().spriteName="All";
        }
    }
}