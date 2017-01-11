using System.Collections.Generic;

namespace  Cloth3D.Ui {
     class LoadingTipsCtr:IController {
         private LoadingTipsViewPresenter _view;
         public bool Init() {
             return true;
         }
         public void Show(MyAction onComplete=null) {
             if (_view == null) {
                 _view = UiManager.Instance.GetWindow<LoadingTipsViewPresenter>(WindowId.LoadingTips);
             }
             UiManager.Instance.ShowWindow(WindowId.LoadingTips);
         }

         public void Update() {

         }

         public bool OnBack(int zorder) {
             //TODO 加载时是否可以返回
             return true;
         }

         public void Hide(MyAction onComplete=null) {
         }

         public List<KeyValuePair<WindowId, string>> GetWinLst() {
             return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.LoadingTips, "ui/LoadingTips")
            };
         }

    }
}
