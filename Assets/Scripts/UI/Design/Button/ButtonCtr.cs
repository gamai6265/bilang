using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using LitJson;
using UnityEngine;

namespace Cloth3D.Ui {
    public class ButtonCtr : IController{
        private ButtonViewPresenter _view;
        private Dictionary<string, JsonData> _dicButtons = new Dictionary<string, JsonData>();
        private int _page = 1;
        private int _pageCount;
        private IDesignWorld _designWorld;

        public bool Init() {
            _designWorld = ComponentMgr.Instance.FindComponent(ComponentNames.ComDesignWorld) as IDesignWorld;
            if (null == _designWorld) {
                LogSystem.Error("design world is null");
                return false;
            }
            return true;
        }

        public void Show(MyAction onComplete=null) {
            if (_view==null) {
                _view = UiManager.Instance.GetWindow<ButtonViewPresenter>(WindowId.Button);
                _view.ClickYes += OnClickYes;
                _view.ClickButtonItem += OnClickButtonItem;
                _view.SvDragFinished += OnSvDragFinished;
            }
            UiManager.Instance.ShowWindow(WindowId.Button);
            GetButtonList(1);
        }

        public void Update() {
        }

        public bool OnBack(int zorder) {
            return false;
        }

        public List<KeyValuePair<WindowId, string>> GetWinLst() {
            return new List<KeyValuePair<WindowId, string>>() {
                new KeyValuePair<WindowId, string>(WindowId.Button, "ui/Button"),
            };
        }

        public void Hide(MyAction onComplete=null) {
            UiManager.Instance.HideWindow(WindowId.Button);
        }

        private void OnSvDragFinished() {
            if (_page < _pageCount) {
                _page++;
                GetButtonList(_page);
            }
        }

        private void OnClickButtonItem(string buttonCode,Texture2D texture2D) {
            //_designWorld.ChangeButton(buttonCode, texture2D);
        }

        private void OnClickYes() {
            UiManager.Instance.HideWindow(WindowId.Button);
        }


        private void GetButtonList(int index) {
            UiControllerMgr.Instance.ShowLodingTips(true);
            //_designWorld.GetButtonInfo(index, ButtonListCallBack);
        }

//        private IEnumerator IntButtonItem(List<ButtonInfo> lstButtonInfos) {
//            for (var i = 0; i < lstButtonInfos.Count; i++) {
//                var buttonInfo = lstButtonInfos[i];
//                string filePath = Application.persistentDataPath + "/" + buttonInfo.ButtonImage;
//                if (Constants.ImagesDic.ContainsKey(buttonInfo.ButtonCode)) {
//                    _view.AddPrefabs(buttonInfo.ButtonCode, Constants.ImagesDic[buttonInfo.ButtonCode]);
//                } else {
//                    if (File.Exists(filePath)) {
//                        FileStream fs = new FileStream(filePath, FileMode.Open);
//                        byte[] buffer = new byte[fs.Length];
//                        fs.Read(buffer, 0, int.Parse(fs.Length.ToString())); 
//                        fs.Close();
//                        fs.Dispose();
//                        Texture2D texture = new Texture2D(2, 2);
//                        texture.LoadImage(buffer);
//                        _view.AddPrefabs(buttonInfo.ButtonCode, texture);
//                        Constants.ImagesDic.Add(buttonInfo.ButtonCode, texture);
//                    } else {
//                        WWW www = new WWW(Constants.NiuKouTuPian + WWW.EscapeURL(buttonInfo.ButtonImage));
//                        yield return www;
//                        if (www.isDone) {
//                            Texture2D newTexture = www.texture;
//                            _view.AddPrefabs(buttonInfo.ButtonCode, newTexture);
//                            var pngData = newTexture.EncodeToJPG();
//                            File.WriteAllBytes(filePath, pngData);
//                            Constants.ImagesDic.Add(buttonInfo.ButtonCode, newTexture);
//                        }
//                        www.Dispose();
//                    }
//                }
//            }
//            UiControllerMgr.Instance.ShowLodingTips(false);
//        }

//        private void ButtonListCallBack(bool result, string pageCount, List<ButtonInfo> lstButtonInfos) {
//            _pageCount = int.Parse(pageCount);
//            if (result)
//                CoroutineMgr.Instance.StartCoroutine(IntButtonItem(lstButtonInfos));
//        }
    }
}