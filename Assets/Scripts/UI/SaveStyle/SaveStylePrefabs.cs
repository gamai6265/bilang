using UnityEngine;
using System.Collections;

namespace Cloth3D.Ui {
    public class SaveStylePrefabs : ViewPresenter {
        public UILabel LblText;

        public string codeName;
        public string codeValue;
        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.Normal;
        }
    }
}