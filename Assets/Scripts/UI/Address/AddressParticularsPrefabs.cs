using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class AddressParticularsPrefabs : ViewPresenter {
        public UIButton BtnChoose;
        public UIButton BtnCompile;
        public UIButton BtnDefault;
        public UIButton BtnDelete;
        public UILabel LblAddress;
        public string NewAddress;

        public UILabel LblName;
        public UILabel LblPhone;
        public UISprite SpDefault;

        public UILabel LblDefault;
        public UILabel LblCompile;
        public UILabel LblDelete;

        [HideInInspector]
        public string Id;
        [HideInInspector]
        public bool isDefault;
        public event MyAction<string> ClickEdit;
        public event MyAction<bool,string> ClickDelete;
        public event MyAction<string> ClickChoose;
        public event MyAction<string, bool> ClickDefault;

        protected override void AwakeUnityMsg() {
            BtnsListener();
            ChangeLanguage();
        }

        protected override void StartUnityMsg() {
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }

        private void ChangeLanguage() {
            LblDefault.text = StrConfigProvider.Instance.GetStr("addr_default");
            LblCompile.text = StrConfigProvider.Instance.GetStr("addr_compile");
            LblDelete.text = StrConfigProvider.Instance.GetStr("addr_delete");
            LblDefault.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblCompile.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LblDelete.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void BtnsListener() {
            BtnCompile.onClick.Add(new EventDelegate(OnBtnCompileClick));
            BtnDelete.onClick.Add(new EventDelegate(OnBtnDeleteClick));
            BtnChoose.onClick.Add(new EventDelegate(OnBtnChooseClick));
            BtnDefault.onClick.Add(new EventDelegate(OnBtnDefaultClick));
        }

        private void OnBtnDefaultClick() {
            if (null != ClickDefault) {
                ClickDefault(Id, isDefault);
            }
        }

        private void OnBtnChooseClick() {
            if (null != ClickChoose) {
                ClickChoose(Id);
            }
        }

        private void OnBtnDeleteClick() {
            if (null != ClickDelete) {
                ClickDelete(isDefault,Id);
            }
        }

        private void OnBtnCompileClick() {
            if (null != ClickEdit) {
                ClickEdit(Id);
            }
        }

        public void SetDefaultState(bool isDefaultAddress) {
            if (isDefaultAddress) {
                var defaultStr = "[234497FF]" + "[默认]" + "[-]";
                LblAddress.text = defaultStr + LblAddress.text;
                SpDefault.spriteName = "Select";
            } else {
                LblAddress.text = NewAddress;
                SpDefault.spriteName = "All";
            }
            isDefault = isDefaultAddress;
        }
    }
}