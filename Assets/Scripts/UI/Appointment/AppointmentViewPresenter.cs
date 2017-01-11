using Cloth3D.Data;
using Cloth3D.Interfaces;

namespace Cloth3D.Ui {
    public class AppointmentViewPresenter : ViewPresenter {
        public UIButton BtnCall;
        public UIButton BtnClose;

        public UILabel LblTitle;
        public UILabel LblPhone;
        public UILabel LblTip;

        public event MyAction ClickCall;
        public event MyAction ClickClose;

        protected override void AwakeUnityMsg() {
            BtnCall.onClick.Add(new EventDelegate(OnBtnCallClick));
            BtnClose.onClick.Add(new EventDelegate(OnBtnCloseClick));
            ChangeLanguage();
        }

        private void OnBtnCallClick() {
            if (null != ClickCall) {
                ClickCall();
            }
        }

        private void OnBtnCloseClick() {
            if (null != ClickClose) {
                ClickClose();
            }
        }

        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("a_title");
            LblTip.text = StrConfigProvider.Instance.GetStr("a_tip");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblTip.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (null != logic) {
                if (null != logic.OnBeforeStageEnter) {
                    logic.OnBeforeStageEnter(StageType.StageNone);
                }
                LblPhone.text = logic.Config.GetSetting("Misc", "phone");
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