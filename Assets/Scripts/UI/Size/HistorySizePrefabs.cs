using System;
using System.Collections.Generic;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class HistorySizePrefabs : ViewPresenter {
        public UIButton BtnChoose;
        public UIButton BtnSave;

        public UILabel LblDate;
        public UILabel LblChoose;
        public UILabel LblSave;

        public List<SizeInfo> LstSizeInfo;

        public UIScrollView SizeScrollView;
        public UIGrid SizeGrid;

        public event MyAction<List<SizeInfo>> ClickChoose;
        public event MyAction<List<SizeInfo>> ClickSave;
        protected override void AwakeUnityMsg() {
            BtnChoose.onClick.Add(new EventDelegate(OnBtnChooseClick));
            BtnSave.onClick.Add(new EventDelegate(OnBtnSaveClick));
          //  ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblChoose.text = StrConfigProvider.Instance.GetStr("mm_hy_choosesize");
            LblChoose.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnBtnChooseClick() {
            if (null != ClickChoose) {
                ClickChoose(LstSizeInfo);
            }
        }

        private void OnBtnSaveClick() {
            if (null != ClickSave)
                ClickSave(LstSizeInfo);
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
        }
    }
}