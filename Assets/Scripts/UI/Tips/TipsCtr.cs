using System.Collections.Generic;

namespace Cloth3D.Ui{
    class TipsCtr:IController {
        public bool Init() {
            return true;
        }
        public void Show(MyAction onComplete=null) {
            UiManager.Instance.ShowWindow(WindowId.Tips);
        }

        public void Update() {
            
        }

        public bool OnBack(int zorder) {
            return false;
        }

        public void Hide(MyAction onComplete=null) {
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Tips, "ui/Tips")
            };
        }
    }
}
