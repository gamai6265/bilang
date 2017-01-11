using System.Collections.Generic;

namespace Cloth3D.Ui {
    public interface IController {
        bool Init();
        void Show(MyAction onComplete = null);
        void Update();
        bool OnBack(int zorder);
        void Hide(MyAction onComplete = null);
        List<KeyValuePair<WindowId, string>> GetWinLst();
    }
}
