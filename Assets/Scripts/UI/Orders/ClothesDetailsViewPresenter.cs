using System;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class ClothesDetailsViewPresenter : ViewPresenter {
        public UIButton BtnBack;

        public UIScrollView ClothesDetailsScrollView;
        public UIGrid ClothesDetailsGrid;
        public GameObject ClothesDetailsPartsPrefabs;
        public UILabel LblButtonsCode;

        public UILabel LblClothesName;
        public UILabel LblFabricCode;
        public UILabel LblQuantity;

        public UILabel LblTitle;
        public UILabel LblQuantityName;
        public UILabel LblFabric;
        public UILabel LblButton;

        public UITexture TexClothes1;
        public UITexture TexClothes2;

        public event MyAction ClickBack;

        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("cd_title");
            LblQuantityName.text = StrConfigProvider.Instance.GetStr("cd_quantity");
            LblFabric.text = StrConfigProvider.Instance.GetStr("cd_mainfabric");
            LblButton.text = StrConfigProvider.Instance.GetStr("cd_button");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblQuantityName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblFabric.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblButton.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.PopUp;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        public void SetLabels(string clothesName, string quantity, string fabric, string button) {
            LblClothesName.text = clothesName;
            LblQuantity.text = quantity;
            LblFabricCode.text = fabric;
            LblButtonsCode.text = button;
        }

        public void AddDetailsPartsPrefabs(string partsName, string fabric1, string fabric2, string remark,
            Texture2D texture2Ds) {
                
            var obj = NGUITools.AddChild(ClothesDetailsGrid.gameObject, ClothesDetailsPartsPrefabs);

            var cdpp = obj.GetComponent<ClothesDetailsPartsPrefabs>();
//            cdpp.LblPartName.text = partsName;
            cdpp.LblPartStyleName.text = partsName;
            cdpp.LblFabric1.text = fabric1;
            cdpp.LblFabric2.text = fabric2;
//            cdpp.LblRemark.text = remark;
            cdpp.TexClothesDetail.mainTexture = texture2Ds;

            RepositionGrid();
        }

        private void RepositionGrid() {
            ClothesDetailsGrid.Reposition();
            ClothesDetailsGrid.repositionNow = true;
            NGUITools.SetDirty(ClothesDetailsGrid);
             ClothesDetailsScrollView.ResetPosition();
        }

        public void AdjustSizeItemDepth() {
            var depth = ClothesDetailsScrollView.GetComponentInChildren<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(ClothesDetailsScrollView.gameObject, depth + 1);
            GetSortedPanels(true);
        }

        public void DeleteAll() {
            ClothesDetailsGrid.transform.DestroyChildren();
        }

        public void SetClothesTexture(int index, Texture2D texture2D) {
            if (index == 1) {
                TexClothes1.mainTexture = texture2D;
            }else {
                TexClothes2.mainTexture = texture2D;
            }
        }
    }
}