using System;
using UnityEngine;

namespace Cloth3D.Ui {
    public class PartItemPrefab : ViewPresenter {
        public UITexture PartItemPrefabs;
        public int PartId;//部件编号
        public UISprite SelectSprite;
        public UISprite TextureBG;
        public TweenPosition TweenAnimation;
        public UILabel LblInstruction;
        public string Instruction = string.Empty;
        private Vector2 _from;
        private Vector2 _to;
        private bool _isFold;
        public string QueryType = string.Empty;

        public bool IsFold {
            get {
                return _isFold;
            }

            private set {
                _isFold = value;
            }
        }

        public bool IsSelected {
            get { return SelectSprite.gameObject.activeSelf; }
        }

        //private event MyAction _callBack;

        public override void InitWindowData() {
        }

        protected override void AwakeUnityMsg() {
            _from = TweenAnimation.from;
            _to = TweenAnimation.to;
            IsFold = true;
        }
        
        public void UnFold(bool isImmediate, EventDelegate.Callback callback = null) {
            UiManager.Instance.ShowWindow(WindowId.Masks);
            IsFold = false;
            Vector2 from = new Vector2(_from.x, transform.localPosition.y);
            Vector2 to = new Vector2( _to.x, transform.localPosition.y);
            TextureBG.gameObject.transform.localPosition = new Vector3(0, TextureBG.transform.localPosition.y, TextureBG.transform.localPosition.x);
            if (isImmediate) {
                transform.localPosition= to;
                if (callback != null)
                    callback();
            } else {
                transform.localPosition = from;
                TweenPosition.Begin(gameObject, TweenAnimation.duration, to);
                
                if (callback != null)
                    EventDelegate.Set(TweenAnimation.onFinished, callback);
            }
        }

        public void Fold(bool isImmediate, EventDelegate.Callback callback = null) {
            IsFold = true;
            Vector2 from = new Vector2(_from.x, transform.localPosition.y);
            Vector2 to = new Vector2(_to.x, transform.localPosition.y);
            TextureBG.gameObject.transform.localPosition = new Vector3(65, TextureBG.transform.localPosition.y, TextureBG.transform.localPosition.x);
            if (isImmediate) {
                transform.localPosition = from;
                if (callback != null)
                    callback();
            } else {
                transform.localPosition = to;
                TweenPosition.Begin(gameObject, TweenAnimation.duration, from);
                
                if (callback != null)
                    EventDelegate.Set(TweenAnimation.onFinished, callback);
            }
        }

        public void SetSelected(bool selected) {
            if (selected) {
                LblInstruction.text = Instruction;
                SelectSprite.gameObject.SetActive(true);
            }
            else {
                SelectSprite.gameObject.SetActive(false);
            }
        }
    }
}