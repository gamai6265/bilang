using System;
using System.Collections.Generic;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Ui {
    public class EditorSizePrefabs : ViewPresenter {
        public UIGrid SizeGrid;
        public UIButton BtnChoose;
        public UIButton BtnDelete;
        public UIButton BtnEditor;

        public string Atid = string.Empty;
        public List<string> Ids;
        public List<SizeInfo> LstSizeInfo;

        public event MyAction<string, List<SizeInfo>> ClickChoose;
        public event MyAction<string> ClickDelete;
        public event MyAction<string, List<SizeInfo>> ClickEditor;

        protected override void AwakeUnityMsg() {
            ChangeLanguage();
        }

        public void AddListener() {
            BtnChoose.onClick.Add(new EventDelegate(OnBtnChooseClick));
            BtnDelete.onClick.Add(new EventDelegate(OnBtnDeleteClick));
            BtnEditor.onClick.Add(new EventDelegate(OnBtnEditorClick));
        }


        private void ChangeLanguage() {
        }

        private void OnBtnChooseClick() {
            if (null != ClickChoose) 
                ClickChoose(Atid, LstSizeInfo);
        }

        private void OnBtnEditorClick() {
            if (null != ClickEditor) 
                ClickEditor(Atid, LstSizeInfo);
        }

        private void OnBtnDeleteClick() {
            if (null != ClickDelete) {
                ClickDelete(Atid);
            }
        }

        protected override void OnDestroyUnityMsg() {
            HideWindow();
        }

        public override void InitWindowData() {
        }
    }
}
