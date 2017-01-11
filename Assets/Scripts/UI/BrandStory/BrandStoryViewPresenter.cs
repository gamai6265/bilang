using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class BrandStoryViewPresenter : ViewPresenter {
        public UIButton BtnBack;
        public UILabel LblTitle;

        public event MyAction ClickBack;

        protected override void AwakeUnityMsg() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            LblTitle.text = StrConfigProvider.Instance.GetStr("b_title");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
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
    }
}