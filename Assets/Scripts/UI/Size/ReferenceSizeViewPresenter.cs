using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Cloth3D;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class ReferenceSizeViewPresenter : ViewPresenter {
        public UIButton BtnBack;

        public UIGrid BtnGrid;
        public GameObject BtnSize;

        public UILabel LblNeck;
        public UILabel LblBust;
        public UILabel LblCuff;
        public UILabel LblSleeveWidth;
        public UILabel LblWaistline;
        public UILabel LblClothesLength;
        public UILabel LblShoulder;
        public UILabel LblSleeveLength;

        public UILabel LblTitle;
        public UILabel LblSize;
        public UILabel LblNeckName;
        public UILabel LblBustName;
        public UILabel LblCuffName;
        public UILabel LblSleeveWidthName;
        public UILabel LblWaistLineName;
        public UILabel LblClothesLengthName;
        public UILabel LblShoulderName;
        public UILabel LbSleeveLengthName;
        public UILabel LblChoose;


        public UIButton BtnChoose;

        public event MyAction ClickBack;
        public event MyAction<string> ClickSize;
        public event MyAction<ClothSize> ClickChoose;

        private List<ReferenceSizeBtnPrefabs> _referenceSizeBtnPrefabses;
        private string _SizeName;

        protected override void AwakeUnityMsg() {
            _referenceSizeBtnPrefabses = new List<ReferenceSizeBtnPrefabs>();
            ChangeLanguage();
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("mm_r_title");
            LblSize.text = StrConfigProvider.Instance.GetStr("mm_r_size");
            LblNeckName.text = StrConfigProvider.Instance.GetStr("mm_r_neck");
            LblBustName.text = StrConfigProvider.Instance.GetStr("mm_r_bust");
            LblWaistLineName.text = StrConfigProvider.Instance.GetStr("mm_r_waistline");
            LblClothesLengthName.text = StrConfigProvider.Instance.GetStr("mm_r_clotheslength");
            LbSleeveLengthName.text = StrConfigProvider.Instance.GetStr("mm_r_sleevelength");
            LblShoulderName.text = StrConfigProvider.Instance.GetStr("mm_r_shoulder");
            LblCuffName.text = StrConfigProvider.Instance.GetStr("mm_r_cuff");
            LblSleeveWidthName.text = StrConfigProvider.Instance.GetStr("mm_r_sleevewidth");
            LblChoose.text = StrConfigProvider.Instance.GetStr("mm_r_choose");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSize.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblNeckName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblBustName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblWaistLineName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblClothesLengthName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LbSleeveLengthName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblShoulderName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCuffName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSleeveWidthName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblChoose.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnChoose.onClick.Add(new EventDelegate(OnBtnChooseClick));
        }

        public void AddSizeItem(string sizeName) {
            var obj = NGUITools.AddChild(BtnGrid.gameObject, BtnSize);
            
            var item = obj.GetComponent<ReferenceSizeBtnPrefabs>();
            item.LabelSize.text = sizeName;
            item.ChooseSize += OnSizeClick;

            _referenceSizeBtnPrefabses.Add(item);

            BtnGrid.Reposition();
            BtnGrid.repositionNow = true;
            NGUITools.SetDirty(BtnGrid);
        }


        private void OnBtnChooseClick() {
            if (null != ClickChoose) {
                var clothSize = new ClothSize();
                clothSize.SizeName = _SizeName;
                clothSize.SizeLingW = LblNeck.text;
                clothSize.SizeXiongW = LblBust.text;
                clothSize.SizeXiuKLong = LblCuff.text;
                clothSize.SizeXiuF = LblSleeveWidth.text;
                clothSize.SizeYaoW = LblWaistline.text;
                clothSize.SizeYiC = LblClothesLength.text;
                clothSize.SizeJianK = LblShoulder.text;
                clothSize.SizeXiuCLong = LblSleeveLength.text;
                ClickChoose(clothSize);
            }
        }

        private void OnSizeClick(string sizeName) {
            SetSelectSprite(sizeName);
            if (null != ClickSize) {
                ClickSize(sizeName);
                _SizeName = sizeName;
            }
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
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }



        public void SelectedItem(ClothSize clothSize) {
            LblNeck.text = clothSize.SizeLingW;
            LblBust.text = clothSize.SizeXiongW;
            LblCuff.text = clothSize.SizeXiuKLong;
            LblSleeveWidth.text = clothSize.SizeXiuF;
            LblWaistline.text = clothSize.SizeYaoW;
            LblClothesLength.text = clothSize.SizeYiC;
            LblShoulder.text = clothSize.SizeJianK;
            LblSleeveLength.text = clothSize.SizeXiuCLong;
            SetSelectSprite(clothSize.SizeName);
        }

        public void SetSelectSprite(string spName) {
            for (int i = 0; i < _referenceSizeBtnPrefabses.Count; i++) {
                if (_referenceSizeBtnPrefabses[i].LabelSize.text==spName) {
                    _referenceSizeBtnPrefabses[i].SpriteSelect.spriteName = "Select";
                } else {
                    _referenceSizeBtnPrefabses[i].SpriteSelect.spriteName = "All";
                }
            }
        }

        public void DeleteAllPrefab() {
           BtnGrid.transform.DestroyChildren();
        }
    }
}