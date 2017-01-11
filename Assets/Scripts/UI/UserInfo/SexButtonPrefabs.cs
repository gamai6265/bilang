using UnityEngine;
using System.Collections;

namespace  Cloth3D.Ui {
     class SexButtonPrefabs : ViewPresenter {

        public string codeValue; //代码值(1=男,2=女,9=保密)
        public string codeName; //代码名称

        public UILabel NameLabel;
        public UIButton SexButton;

         public event MyAction<string,string> ChooseClick;

        public override void InitWindowData() {
        }

        protected override void AwakeUnityMsg() {
            SexButton.onClick.Add(new EventDelegate(OnChoose));
        }

        private void OnChoose() {
            if (ChooseClick != null) {
                ChooseClick(codeName,codeValue);
            }
        }
    }
}
