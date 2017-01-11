using System;
using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class CollectPrefabs : ViewPresenter {
        public UIButton BtnDelete;
        public UILabel LblDate;
        public UILabel LblName;
        public UILabel LblPrice;
        public UILabel LblCollect;
        public string TempletCode;

        public UITexture TexCollect;

        public event MyAction<string> ClickDelete;

        protected override void AwakeUnityMsg() {
            BtnDelete.onClick.Add(new EventDelegate(OnBtnDeleteClick));
            LblCollect.text = StrConfigProvider.Instance.GetStr("c_collect");
            LblCollect.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnBtnDeleteClick() {
            if (null != ClickDelete) {
                ClickDelete(TempletCode);
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