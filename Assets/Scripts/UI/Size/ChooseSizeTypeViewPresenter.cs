using Cloth3D.Data;

namespace Cloth3D.Ui {
    public class ChooseSizeTypeViewPresenter : ViewPresenter {
        public UILabel LblTitle;
        public UILabel LblMySize;
        public UILabel LblAppointment;

        public UIButton BtnAppointment;
        public UIButton BtnMySize;
        public UIButton BtnBack;

        public event MyAction ClickAppointment;
        public event MyAction ClickMySize;
        public event MyAction ClickBack;

        protected override void AwakeUnityMsg() {
            BtnAppointment.onClick.Add(new EventDelegate(OnBtnAppointment));
            BtnMySize.onClick.Add(new EventDelegate(OnBtnBtnMySize));
            BtnBack.onClick.Add(new EventDelegate(OnBtnBack));
            ChangeLanguage();
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("cst_title");
            LblMySize.text = StrConfigProvider.Instance.GetStr("cst_mysize");
            LblAppointment.text = StrConfigProvider.Instance.GetStr("cst_appointment");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblMySize.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblAppointment.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        protected override void OnDestroyUnityMsg() {
        }
        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        private void OnBtnBack() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        private void OnBtnBtnMySize() {
            if (null != ClickMySize) {
                ClickMySize();
            }
        }

        private void OnBtnAppointment() {
            if (null != ClickAppointment) {
                ClickAppointment();
            }
        }
    }
}