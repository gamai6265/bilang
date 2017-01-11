using System.Collections.Generic;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class OrdersViewPresenter : ViewPresenter {
        private string _ordersId;
        public UILabel LblTitle;
        public UILabel LblPayment;
        public UILabel LblReceiving;
        public UILabel LblComment;
        public UILabel LblAll;
        
        public UIButton BtnAll;
        public UIButton BtnBack;
        public UIButton BtnComment;
        public UIButton BtnPayment;
        public UIButton BtnReceiving;

        public UIGrid OrdersGrid;
        public GameObject OrdersPrefabs;

        public UIScrollView OrdersScrollView;

        public event MyAction SvDragFinished;

        public event MyAction ClickBack;
        public event MyAction ClickPayment;
        public event MyAction ClickReceiving;
        public event MyAction ClickComment;
        public event MyAction ClickAll;
        public event MyAction<string> ClickSure;
        public event MyAction ClickRemind;

        public event MyAction<string, string, string, string> ClickPay;
        public event MyAction<string> ClickReceipt;
        public event MyAction<List<string>, List<Texture2D>, List<string>> ClickAppraise;
        public event MyAction<OrderInfo, List<Texture2D>> ClickChooce;

        protected override void AwakeUnityMsg() {
            OrdersScrollView.onDragFinished += OnSelectionScrollViewDragFinished;
            ChangeLanguage();
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("o_title");
            LblPayment.text = StrConfigProvider.Instance.GetStr("o_payment");
            LblReceiving.text = StrConfigProvider.Instance.GetStr("o_receiving");
            LblComment.text = StrConfigProvider.Instance.GetStr("o_comment");
            LblAll.text = StrConfigProvider.Instance.GetStr("o_all");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPayment.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblReceiving.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblComment.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblAll.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnPayment.onClick.Add(new EventDelegate(OnBtnPaymentClick));
            BtnReceiving.onClick.Add(new EventDelegate(OnBtnReceivingClick));
            BtnComment.onClick.Add(new EventDelegate(OnBtnCommentClick));
            BtnAll.onClick.Add(new EventDelegate(OnBtnAllClick));
        }
        
        private void OnSelectionScrollViewDragFinished() {
            var constraint = OrdersScrollView.panel.CalculateConstrainOffset(OrdersScrollView.bounds.min,
                OrdersScrollView.bounds.min);
            if (constraint.y < 1f) {
                if (null != SvDragFinished) {
                    SvDragFinished();
                }
            }
        }

        private void OnBtnAllClick() {
            if (null != ClickAll) {
                ClickAll();
            }
            ChangeChoose(3);
        }

        private void OnBtnCommentClick() {
            if (null != ClickComment) {
                ClickComment();
            }
            ChangeChoose(2);
        }

        private void OnBtnReceivingClick() {
            if (null != ClickReceiving) {
                ClickReceiving();
            }
            ChangeChoose(1);
        }

        private void OnBtnPaymentClick() {
            if (null != ClickPayment) {
                ClickPayment();
            }
            ChangeChoose(0);
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
    
        public void AddPrefabs(OrderInfo orderInfo, string state, List<Texture2D> texture2D, string stateCode, List<string> lstGoodId) {
            var obj = NGUITools.AddChild(OrdersGrid.gameObject, OrdersPrefabs);

            var op = obj.GetComponent<OrdersPrefabs>();
            obj.name = op.OrderId = Utils.GetJsonStr(orderInfo.OrderId); ;
            op.OrderInfoItem = orderInfo;
            op.LblOrderCode.text = Utils.GetJsonStr(orderInfo.OrderCode);
            op.LblOrderName.text = Utils.GetJsonStr(orderInfo.OrderMemo); 
            op.LblNumber.text = Utils.GetJsonStr(orderInfo.ClothNumber);
            op.LblPrice.text = Utils.GetJsonStr(orderInfo.OrderAmount);
            op.LblState.text = state;
            op.LstGoodId = lstGoodId;
            for (int i = 0; i < texture2D.Count; i++) {
                //TODO 暂时只有三件
                if(i>2)
                    break;
                op.Textures[i].mainTexture = texture2D[i];
            }
            op.SetBtns(stateCode);

            op.ClickCancel += OnClickCancel;
            op.ClickPay += OnClickPay;
            op.ClickAppraise += OnClickAppraise;
            op.ClickReceipt += OnClickReceipt;
            op.ClickChooce += OnClickChooce;
            op.ClickRemind += OnClickRemind;

            RepositionGrid();
        }

        private void OnClickRemind() {
            if (null != ClickRemind)
                ClickRemind();
        }


        private void OnClickChooce(OrderInfo _orderInfo, List<Texture2D> texture2Ds) {
            if (null != ClickChooce) {
                ClickChooce(_orderInfo, texture2Ds);
            }
        }

        private void OnClickReceipt(string ordersId) {
            _ordersId = ordersId;
            if (null != ClickReceipt) {
                ClickReceipt(_ordersId);
            }
        }

        private void OnClickAppraise(List<string> lstStrs, List<Texture2D> texTure2D, List<string> lstGoodId) {
           _ordersId = lstStrs[0];
            if (null != ClickAppraise) {
                ClickAppraise(lstStrs, texTure2D, lstGoodId);
            }
        }

        private void OnClickPay(string orderCode, string ordersId, string price, string subject) {
            _ordersId = ordersId;
            if (null != ClickPay) {
                ClickPay(orderCode, _ordersId, price, subject);
            }
        }

        private void OnClickCancel(string ordersId) {
            _ordersId = ordersId;
            if (null != ClickSure) {
                ClickSure(_ordersId);
            }
        }

        private void RepositionGrid() {           
            OrdersGrid.Reposition();
            OrdersGrid.repositionNow = true;
            NGUITools.SetDirty(OrdersGrid);
            //SpringPanel();
        }

        public void SpringPanel(int temp = -1) {
            var springPanel = OrdersScrollView.GetComponent<SpringPanel>();
            var y = (temp == -1) ? OrdersScrollView.transform.localPosition.y - 1 : temp;
            springPanel.target = new Vector3(0, y, 0);
            springPanel.enabled = true;
        }

        public void AdjustSizeItemDepth() {
            var depth = OrdersScrollView.GetComponentInChildren<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(OrdersScrollView.gameObject, depth + 1);
            GetSortedPanels(true);
        }

        public void DeleteAllPrefabs() {
            OrdersGrid.transform.DestroyChildren();
        }

        public void DeleteItem(string ordId) {
            var children = OrdersGrid.transform.GetComponentsInChildren<OrdersPrefabs>(true);
            for (int i = 0; i < children.Length; i++) {
                if (children[i].OrderId == ordId) {
                    GameObject.Destroy(children[i].gameObject);
                    break;
                }
            }
            RepositionGrid();
            SpringPanel();
        }

        private void ChangeChoose(int index) {
            GameObject[] objs = {
                BtnPayment.gameObject, BtnReceiving.gameObject, BtnComment.gameObject,
                BtnAll.gameObject
            };
            for (var i = 0; i < objs.Length; i++) {
                if (i == index) {
                    var sprite = objs[i].transform.Find("SpBtnBG").gameObject.GetComponentInChildren<UISprite>();
                    sprite.spriteName = "button2";
                    var label = objs[i].GetComponentInChildren<UILabel>();
                    label.color = Color.black;
                } else {
                    var sprite = objs[i].transform.Find("SpBtnBG").gameObject.GetComponentInChildren<UISprite>();
                    sprite.spriteName = "Bg1";
                    var label = objs[i].GetComponentInChildren<UILabel>();
                    label.color = Color.white;
                }
            }
        }
    }
}