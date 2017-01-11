using UnityEngine;

namespace Cloth3D.Ui {
    public class InstructionViewPresenter : ViewPresenter {
        public UISprite BGSprite;
        public UILabel LblText;

        protected override void AwakeUnityMsg() {
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
        }

        public void SetInstruction(string str) {
            LblText.text = str;
        }

        public void SetSecelcSpritePosition(Vector3 position) {
            BGSprite.gameObject.transform.position = position;
        }
    }
}