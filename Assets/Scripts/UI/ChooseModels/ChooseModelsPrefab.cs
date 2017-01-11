using System.Collections.Generic;

namespace Cloth3D.Ui{
    public class ChooseModelsPrefab : ViewPresenter{
        public UILabel Lbl;
        public UITexture Tex;
        public int Id;
        public List<int> LstInclude = new List<int>();

        public override void InitWindowData(){
            WinData.WindowType = UiWindowType.Normal;
        }
    }
}