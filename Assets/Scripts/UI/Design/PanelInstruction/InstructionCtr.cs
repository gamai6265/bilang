using System;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class InstructionCtr : IController {
        private InstructionViewPresenter _view;
        private INetworkMgr _network;

        public bool Init() {
            _network=ComponentMgr.Instance.FindComponent(ComponentNames.ComNetwork) as INetworkMgr;
            if (_network == null) {
                LogSystem.Error("_network is null ...");
                return false;
            }
            EventCenter.Instance.AddListener<Vector3>(EventId.ShowInstruction, ShowInstruction);
            return true;
           
        }

        public void Show(MyAction onComplete = null) {
            if (_view == null) {
                _view = UiManager.Instance.GetWindow<InstructionViewPresenter>(WindowId.PanelInstruction);
            }
            UiManager.Instance.ShowWindow(WindowId.PanelInstruction);

        } 

        public void Update() {
        }

        public bool OnBack(int zorder) {
            return false;
        }

        public void Hide(MyAction onComplete = null) {
         //   UiManager.Instance.HideWindow(WindowId.Parts);
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.PanelInstruction, "ui/ShowInstruction"),
            };
        }

        private  void ShowInstruction(Vector3 spritePosition) {
              _view.SetSecelcSpritePosition(spritePosition);
        }


    }
}
