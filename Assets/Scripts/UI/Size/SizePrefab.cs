using UnityEngine;
using System.Collections;

namespace Cloth3D.Ui {

    public class SizePrefab : ViewPresenter {
        public UILabel LblName;
        public UILabel LblValue;

        public override void InitWindowData() {
           
        }

        public void ChangeLangele() {
            LblName.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }
    }
}
