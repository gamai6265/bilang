using UnityEngine;
using System;
using Cloth3D.Data;
using Cloth3D.Ui;

namespace Cloth3D.Ui {
    public class MessageBoxPresenter : ViewPresenter {
        public UILabel LabelTitle;
        public UILabel LabelPrompt;
        public UIButton BtnRight;
        public UIButton BtnLeft;
        public event MyAction ClickLeft;
        public event MyAction ClickRight;
       protected override void AwakeUnityMsg() {
            BtnLeft.onClick.Add(new EventDelegate(OnLeftClick));
            BtnRight.onClick.Add(new EventDelegate(OnRightClick));
        }

        public override void InitWindowData() {
            WinData.WindowType = UiWindowType.PopUp;
            WinData.ColliderMode = UiWindowColliderMode.Normal;
        }

        public void SetMsg(string title, string msg, string leftStr, string rightStr) {
            LabelTitle.text = title;
            LabelPrompt.text = msg;
            BtnRight.GetComponent<UILabel>().text = rightStr;
            BtnLeft.GetComponent<UILabel>().text = leftStr;
            LabelTitle.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            LabelPrompt.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            BtnRight.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
            BtnLeft.gameObject.GetComponent<UIWidget>().MakePixelPerfect();
        }

        private void OnRightClick() {
            if (ClickRight != null)
                ClickRight();
        }

        private void OnLeftClick() {
            if (ClickLeft != null)
                ClickLeft();
        }

        /* public override void ResetWindow() {
             base.ResetWindow();

             // 隐藏所有按钮
             NGUITools.SetActive(_btnCenter, false);
             NGUITools.SetActive(_btnLeft, false);
             NGUITools.SetActive(_btnRight, false);
         }


         public override void ShowWindow() {
             NGUITools.SetActive(gameObject, true);
             IsLock = true;
             EnterAnimation(delegate {
                 IsLock = false;
             });
         }

         public override void HideWindow(Action onComplete = null) {
             IsLock = true;
             QuitAnimation(delegate {
                 IsLock = false;
                 NGUITools.SetActive(gameObject, false);
                 if (onComplete != null)
                     onComplete();
             });
         }

         public void EnterAnimation(EventDelegate.Callback onComplete) {
             if (_twScale == null)
                 _twScale = _objAnimation.GetComponent<TweenScale>();
             _twScale.PlayForward();
             EventDelegate.Set(_twScale.onFinished, onComplete);
         }

         public void QuitAnimation(EventDelegate.Callback onComplete) {
             if (_twScale == null)
                 _twScale = _objAnimation.GetComponent<TweenScale>();
             _twScale.PlayReverse();
             EventDelegate.Set(_twScale.onFinished, onComplete);
         }

         public void ResetAnimation() {
             if (_twScale != null)
                 _twScale.ResetToBeginning();
         }*/
    }
}

