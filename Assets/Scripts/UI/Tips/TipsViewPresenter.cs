using System;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class TipsViewPresenter : ViewPresenter, IWindowAnimation {
        public UILabel LblTips;

        public TweenScale BackGroud;
        private TweenAlpha _tweeAlpha;

        protected override void AwakeUnityMsg() {
            _tweeAlpha = BackGroud.gameObject.GetComponent<TweenAlpha>();
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.PopUp;
        }

        public void SetMsg(string msg) {
            LblTips.text = msg;
            LblTips.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        public override void ShowWindow(MyAction onComplete = null) {
            ResetAnimation();
            base.ShowWindow(onComplete);
            EnterAnimation(()=> {
                UiManager.Instance.HideWindow(WindowId.Tips);
            });
        }

        public override void HideWindow(MyAction onComplete = null) {
            QuitAnimation(()=>{
                base.HideWindow(onComplete);
            });
        }

        public void EnterAnimation(EventDelegate.Callback onComplete) {
            BackGroud.PlayForward();
            if(onComplete!=null)
                EventDelegate.Set(BackGroud.onFinished, onComplete);
        }

        public void QuitAnimation(EventDelegate.Callback onComplete) {
            _tweeAlpha.PlayForward();
            if(onComplete!=null)
                EventDelegate.Set(_tweeAlpha.onFinished, onComplete);
        }

        public void ResetAnimation() {
            BackGroud.ResetToBeginning(); //TODO 
            _tweeAlpha.ResetToBeginning(); //TODO 
        }
    }
}