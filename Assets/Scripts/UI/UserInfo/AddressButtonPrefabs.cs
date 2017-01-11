using UnityEngine;

namespace Cloth3D.Ui {
    public class AddressButtonPrefabs : ViewPresenter{
        public UILabel LabelCodeName;
        [HideInInspector]
        public string CodeValue;

        [HideInInspector]
        public string CodeName {
            get {
                return LabelCodeName.text;
            }
            set {
                LabelCodeName.text = value;
            } 
        }

        public MyAction<string, string> ClickChoose;

        protected override void AwakeUnityMsg() {
            GetComponent<UIButton>().onClick.Add(new EventDelegate(OnClick));
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void OnClick() {
            if (ClickChoose != null)
                ClickChoose(CodeValue, CodeName);
        }
    }
}
