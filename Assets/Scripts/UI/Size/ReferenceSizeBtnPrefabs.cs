using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloth3D.Ui {
    class ReferenceSizeBtnPrefabs:ViewPresenter {
        public UILabel LabelSize;
        public UISprite SpriteSelect;
        public UIButton BtnChoose;
        public event MyAction<string> ChooseSize;
        public override void InitWindowData() {
        }

        protected override void AwakeUnityMsg() {
            BtnChoose.onClick.Add(new EventDelegate(OnChoose));
        }

        private void OnChoose() {
            if (ChooseSize != null)
                ChooseSize(LabelSize.text.Trim());
        }
    }
}
