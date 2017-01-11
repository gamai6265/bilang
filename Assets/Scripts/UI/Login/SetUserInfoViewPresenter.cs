using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class SetUserInfoViewPresenter : ViewPresenter {
        public UIButton BtnAgreement;
        public UIButton BtnBack;
        public UIButton BtnRegister;
        public UIButton BtnSex;

        public UIButton BtnShow;
        public UIInput IpName;
        public UIInput IpPassWord;
        public UIInput IpPassWord2;
        public UIInput IpSex;
        public UIInput IpUser;

        public UILabel LblTitle;
        public UILabel LblUser;
        public UILabel LblPassword;
        public UILabel LblPasswordC;
        public UILabel LblName;
        public UILabel LblSex;
        public UILabel LblRegister;


        public UIGrid SexGrid;
        public GameObject PanelSetUserInfo;
        public GameObject SexButtonPrefabs;

        public event MyAction ClickBack;
        public event MyAction<string,string,string,string,string> ClickRegister;
        public event MyAction ClickShow;
        public event MyAction ClickSex;
        public event MyAction ClickAgreement;
        public event MyAction<string> ChooseSexClick;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }

        protected override void StartUnityMsg() {
            BtnsListener();
        }


        private void ChangeLanguage() {
            LblTitle.text = StrConfigProvider.Instance.GetStr("pr_title");
            LblUser.text = StrConfigProvider.Instance.GetStr("pr_username");
            LblPassword.text = StrConfigProvider.Instance.GetStr("pr_password");
            LblPasswordC.text = StrConfigProvider.Instance.GetStr("pr_passwordc");
            LblName.text = StrConfigProvider.Instance.GetStr("pr_name");
            LblSex.text = StrConfigProvider.Instance.GetStr("pr_sex");
            LblRegister.text = StrConfigProvider.Instance.GetStr("pr_register");
            BtnAgreement.GetComponent<UILabel>().text = StrConfigProvider.Instance.GetStr("pr_message");
            LblTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblUser.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPassword.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblPasswordC.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblSex.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblRegister.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            BtnAgreement.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        protected override void OnDestroyUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        private void BtnsListener() {
            BtnBack.onClick.Add(new EventDelegate(OnBtnBackClick));
            BtnRegister.onClick.Add(new EventDelegate(OnBtnRegisterClick));
            BtnShow.onClick.Add(new EventDelegate(OnBtnShowClick));
            BtnSex.onClick.Add(new EventDelegate(OnBtnSexClick));
            BtnAgreement.onClick.Add(new EventDelegate(OnBtnAgreementClick));
        }

        private void OnBtnAgreementClick() {
            if (null != ClickAgreement) {
                ClickAgreement();
            }
        }

        private void OnBtnSexClick() {
            if (null != ClickSex) {
                ClickSex();
            }
        }

        private void OnBtnShowClick() {
            if (null != ClickShow) {
                ClickShow();
            }
        }

        private void OnBtnRegisterClick() {
            if (null != ClickRegister) {
                ClickRegister(IpUser.value.Trim(),
                    IpPassWord.value.Trim(),
                    IpPassWord2.value.Trim(),
                    IpName.value.Trim(),
                    IpSex.value.Trim());
            }
        }

        private void OnBtnBackClick() {
            if (null != ClickBack) {
                ClickBack();
            }
        }

        public void SetPhoneText(string phone) {
            IpUser.value = phone;
        }

        public void SetSexText(string text) {
            IpSex.value = text;
        }

        private void ShowSexPanel(bool tof) {
            PanelSetUserInfo.SetActive(tof);
        }

        public void PassWordShow() {
            IpPassWord.inputType = IpPassWord.inputType == UIInput.InputType.Password
                ? UIInput.InputType.Standard
                : UIInput.InputType.Password;
            IpPassWord2.inputType = IpPassWord2.inputType == UIInput.InputType.Password
                ? UIInput.InputType.Standard
                : UIInput.InputType.Password;
            IpPassWord.UpdateLabel();
            IpPassWord2.UpdateLabel();
        }

        public void AddSexBtn(CodeSet codeSet) {
            ShowSexPanel(true);
            var data = codeSet.GetData();
            foreach (var item in data) {
                GameObject obj = NGUITools.AddChild(SexGrid.gameObject, SexButtonPrefabs);

                SexButtonPrefabs sp = obj.GetComponent<SexButtonPrefabs>();
                sp.NameLabel.text = sp.codeName = item.Value;
                sp.codeValue = item.Key;

                sp.ChooseClick += OnChooseClick;

                SexGrid.Reposition();
                SexGrid.repositionNow = true;
                NGUITools.SetDirty(SexGrid);
            }
            AdjustSizeItemDepth();
        }

        private void AdjustSizeItemDepth() {
            var depth = PanelSetUserInfo.GetComponentInChildren<UIPanel>().depth;
            NguiUtility.SetMinPanelDepth(PanelSetUserInfo.gameObject, depth + 1);
            GetSortedPanels(true);
        }

        private void OnChooseClick(string codeName,string codevalue) {
            if (null != ChooseSexClick) {
                ChooseSexClick(codeName);
            }
        }

        public void DeleteAll() {
            SexGrid.transform.DestroyChildren();
            ShowSexPanel(false);
        }
    }
}