using System;
using LitJson;

namespace Cloth3D.Ui {
    public class ShopCarPrefabViewPresenter : ViewPresenter {
        public UIButton BtnAdd;
        public UIButton BtnChoose;
        public UIButton BtnDelete;
        public UIButton BtnSubtract;
        public string Code;
        public string Id;
        public string modelID;
        public JsonData JsonData;
        public bool IsChoose = false;

        public UILabel LblName;
        public UILabel LblPrice;
        public UILabel LblQuantity;
        public UITexture TexShopCar;

        public event MyAction<string> ClickAdd;
        public event MyAction<string> ClickSubtract;
        public event MyAction<string> ClickChoose;
        public event MyAction<string> ClickDelete;

        protected override void AwakeUnityMsg() {
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        private void BtnsListener() {
            BtnAdd.onClick.Add(new EventDelegate(OnBtnAddClick));
            BtnSubtract.onClick.Add(new EventDelegate(OnBtnSubtractClick));
            BtnChoose.onClick.Add(new EventDelegate(OnBtnChooseClick));
            BtnDelete.onClick.Add(new EventDelegate(OnBtnDeleteClick));
        }

        private void OnBtnAddClick() {
            if (null != ClickAdd) {
                ClickAdd(Id);

            }
        }

        private void OnBtnSubtractClick() {
            if (null != ClickSubtract) {
                ClickSubtract(Id);
            }
        }

        private void OnBtnChooseClick() {
            if (null != ClickChoose) {
                ClickChoose(Id);
            }
        }

        private void OnBtnDeleteClick() {
            if (null != ClickDelete) {
                ClickDelete(Id);
            }
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }
    }
}