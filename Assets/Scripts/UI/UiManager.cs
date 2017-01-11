using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cloth3D.Ui {
    /// <summary>
    ///     UI 界面管理类
    /// </summary>
    public class UiManager: UnitySingleton<UiManager> {
        // 层级分离depth
        private const int FixedWindowDepth = 500; // 界面固定window起始depth
        private const int NormalWindowDepth = 2; // Normal类型window起始depth
        private const int PopUpWindowDepth = 1000; // PopUp类型window起始depth
        protected Dictionary<WindowId, ViewPresenter> DicAllWindows = new Dictionary<WindowId, ViewPresenter>();
        protected Dictionary<WindowId, ViewPresenter> DicShownedWindows = new Dictionary<WindowId, ViewPresenter>();
        private Dictionary<WindowId, string> _winPrefabPath  = new Dictionary<WindowId, string>();

        // Atlas refrence
        public UIAtlas MaskAtlas;
        // Fixed Window节点
        private Transform _uiFixedWidowRoot;

        // NormalWindow节点
        private Transform _uiNormalWindowRoot;

        // PopUpWindow节点
        private Transform _uiPopUpWindowRoot;

        // 保存UI常用节点
        private Transform _uiRoot;

        public override void Awake() {
            base.Awake();
            _uiRoot = transform;

            DicAllWindows.Clear();
            DicShownedWindows.Clear();
            _winPrefabPath.Clear();

            if (_uiFixedWidowRoot == null) {
                _uiFixedWidowRoot = new GameObject("UIFixedWidowRoot").transform;
                NguiUtility.AddChildToTarget(_uiRoot, _uiFixedWidowRoot);
                NguiUtility.ChangeChildLayer(_uiFixedWidowRoot, _uiRoot.gameObject.layer);
            }
            if (_uiPopUpWindowRoot == null) {
                _uiPopUpWindowRoot = new GameObject("UIPopUpWindowRoot").transform;
                NguiUtility.AddChildToTarget(_uiRoot, _uiPopUpWindowRoot);
                NguiUtility.ChangeChildLayer(_uiPopUpWindowRoot, _uiRoot.gameObject.layer);
            }
            if (_uiNormalWindowRoot == null) {
                _uiNormalWindowRoot = new GameObject("UINormalWindowRoot").transform;
                NguiUtility.AddChildToTarget(_uiRoot, _uiNormalWindowRoot);
                NguiUtility.ChangeChildLayer(_uiNormalWindowRoot, _uiRoot.gameObject.layer);
            }
            LogSystem.Debug("UIManager is call awake.");
        }

        public void AddWinInfo(WindowId id, string path) {
            if (!_winPrefabPath.ContainsKey(id)) {
                _winPrefabPath.Add(id, path);
            } else {
                LogSystem.Error("erro, uimanager winPrefabs exist:{0}" , id);
            }
        }

        public void ShowWindow(WindowId id, Zorder refZorder=Zorder.TopMost, int zorderOffset=0, MyAction onComplete = null) {
            var win =_GetWindow(id);
            if (win == null) {
                LogSystem.Error("error, load window failed.");
                return;
            }
            //win 
            AdjustWindowZorder(win, refZorder, zorderOffset);
            AddColliderBgForWindow(win);
            _ShowWindow(win, id, false, onComplete);
        }

        public void ShowWindowImmediately(WindowId id, Zorder refZorder = Zorder.TopMost, int zorderOffset = 0, MyAction onComplete = null) {
            var win = _GetWindow(id);
            if (win == null) {
                LogSystem.Error("error, load window failed.");
                return;
            }
            //win 
            AdjustWindowZorder(win, refZorder, zorderOffset);
            AddColliderBgForWindow(win);
            _ShowWindow(win, id, true, onComplete);
        }
        private class SortPanel :IComparer<ViewPresenter> {
            public int Compare(ViewPresenter x, ViewPresenter y) {
                var xPanels = x.GetSortedPanels();
                var yPanels = y.GetSortedPanels();
                int left = xPanels.Count > 0 ? xPanels[0].depth : -1;
                int right = yPanels.Count > 0 ? yPanels[0].depth : -1;
                return left - right;
            }
        }
        public void AdjustWindowZorder(ViewPresenter win, Zorder refZorder, int zorderOffset, bool forceRenewPanels=false) {
            var windowType = win.WinData.WindowType;
            //step0 find out the same type view
            var lstWin = new List<ViewPresenter>();
            foreach (var item in DicAllWindows) {
                if (windowType == item.Value.WinData.WindowType) {
                    lstWin.Add(item.Value);
                }
            }
            if (forceRenewPanels) {
                win.GetSortedPanels(true);
            }
            lstWin.Sort(new SortPanel());

            //step1. sort view 
            int depth = NormalWindowDepth;
            switch (windowType) {
                case UiWindowType.Fixed:
                    depth = FixedWindowDepth;
                    break;
                case UiWindowType.PopUp:
                    depth = PopUpWindowDepth;
                    break;
            }
            int targetIndex = 0;
            if (refZorder == Zorder.TopMost) {
                targetIndex = lstWin.Count - 1 - zorderOffset;
                if (targetIndex < 0) targetIndex = 0;
            } else if (refZorder == Zorder.BottomMost) {
                targetIndex = zorderOffset;
                if (targetIndex > lstWin.Count-1) targetIndex = lstWin.Count-1;
            }
            //step2 update panel depth
            int otherIndex = 0;
            for (int i = 0; i < lstWin.Count; i++) {
                if (i == targetIndex) {
                    var itemPanel = win.GetSortedPanels();
                    for (int j = 0; j < itemPanel.Count; j++) {
                        itemPanel[j].depth = depth;
                        depth++;
                    }
                    otherIndex++;
                } else {
                    List<UIPanel> itemPanel=null;
                    if (lstWin[i] == win) {
                        otherIndex ++;
                    }
                    if (otherIndex >= lstWin.Count) {
                        otherIndex = lstWin.Count - 1;
                    }
                    itemPanel = lstWin[otherIndex].GetSortedPanels();
                    for (int j = 0; j < itemPanel.Count; j++) {
                        itemPanel[j].depth = depth;
                        depth++;
                    }
                    otherIndex++;
                }
            }
        }

        public bool AdjustZorderBaseAnother(ViewPresenter win, WindowId refWinId, int offset) {
            Assert.IsTrue(offset != 0, "offset must not be 0!");
            if (offset == 0)
                return false;
            var refWin = _GetWindow(refWinId);
            var windowType = win.WinData.WindowType;
            if (refWin == null || refWin.WinData.WindowType != windowType)
                return false;
            //step0 find out the same type view
            var lstWin = new List<ViewPresenter>();
            foreach (var item in DicAllWindows) {
                if (windowType == item.Value.WinData.WindowType) {
                    lstWin.Add(item.Value);
                }
            }
            lstWin.Sort(new SortPanel());

            //step1. sort view 
            int depth = NormalWindowDepth;
            switch (windowType) {
                case UiWindowType.Fixed:
                    depth = FixedWindowDepth;
                    break;
                case UiWindowType.PopUp:
                    depth = PopUpWindowDepth;
                    break;
            }
            int targetIndex = 0;
            int winIndex = 0;
            int refWinIndex = 0;
            for (int i = 0; i < lstWin.Count; i++) {
                if (lstWin[i] == win) {
                    winIndex = i;
                }else if (lstWin[i] == refWin) {
                    refWinIndex = i;
                }
            }
            if (winIndex < refWinIndex) {
                if (offset > 0)
                    targetIndex = refWinIndex - 1 + offset;
                else
                    targetIndex = refWinIndex + offset;
            } else {
                targetIndex = refWinIndex + offset;
            }
            if (targetIndex < 0) {
                targetIndex = 0;
            }else if (targetIndex >= lstWin.Count) {
                targetIndex = lstWin.Count - 1;
            }

            //step2 update panel depth
            int otherIndex = 0;
            for (int i = 0; i < lstWin.Count; i++) {
                if (i == targetIndex) {
                    var itemPanel = win.GetSortedPanels();
                    for (int j = 0; j < itemPanel.Count; j++) {
                        itemPanel[j].depth = depth;
                        depth++;
                    }
                    otherIndex++;
                } else {
                    List<UIPanel> itemPanel = null;
                    if (lstWin[i] == win) {
                        otherIndex++;
                    }
                    if (otherIndex >= lstWin.Count) {
                        otherIndex = lstWin.Count - 1;
                    }
                    itemPanel = lstWin[otherIndex].GetSortedPanels();
                    for (int j = 0; j < itemPanel.Count; j++) {
                        itemPanel[j].depth = depth;
                        depth++;
                    }
                    otherIndex++;
                }
            }
            return true;
        }


        protected void _ShowWindow(ViewPresenter win, WindowId id, bool immediately, MyAction onComplete = null) {
            DicShownedWindows[id] = win;
            if (immediately) {
                win.ShowWindowImmediately();
                if (null != onComplete) {
                    onComplete();
                }
            } else {
                win.ShowWindow(onComplete);
            }
        }

        public void ShowWindowDelay(float delayTime, WindowId id) {
            StartCoroutine(_ShowWindowDelay(delayTime, id));
        }

        private IEnumerator _ShowWindowDelay(float delayTime, WindowId id) {
            yield return new WaitForSeconds(delayTime);
            ShowWindow(id);
        }


        private ViewPresenter _GetWindow(WindowId id) {
            if (DicAllWindows.ContainsKey(id))
                return DicAllWindows[id];
            ViewPresenter win = null;
            if (_winPrefabPath.ContainsKey(id)) {
                var prefabPath = _winPrefabPath[id];
                var prefab = Resources.Load<GameObject>(prefabPath); //TODO 配合resourcemgr
                if (prefab != null) {
                    var uiObject = Instantiate(prefab);
                    NGUITools.SetActive(uiObject, false);
                    win = uiObject.GetComponent<ViewPresenter>();
                    var targetRoot = GetTargetRoot(win.WinData.WindowType);
                    NguiUtility.AddChildToTarget(targetRoot, win.gameObject.transform);
                    DicAllWindows[id] = win;
                }
                Resources.UnloadUnusedAssets();
                return win;
            }
            return null;
        }


        /// <summary>
        ///     窗口背景碰撞体处理
        /// </summary>
        private void AddColliderBgForWindow(ViewPresenter win) {
            var colliderMode = win.WinData.ColliderMode;
            if (colliderMode == UiWindowColliderMode.None)
                return;

            if (colliderMode == UiWindowColliderMode.Normal)
                NguiUtility.AddColliderBgToTarget(win.gameObject, "Mask02", MaskAtlas, true);
            if (colliderMode == UiWindowColliderMode.WithBg)
                NguiUtility.AddColliderBgToTarget(win.gameObject, "Mask02", MaskAtlas, false);
        }

        private Transform GetTargetRoot(UiWindowType type) {
            if (type == UiWindowType.Fixed)
                return _uiFixedWidowRoot;
            if (type == UiWindowType.Normal)
                return _uiNormalWindowRoot;
            if (type == UiWindowType.PopUp)
                return _uiPopUpWindowRoot;
            return _uiRoot;
        }

        public T GetWindow<T>(WindowId id) where T : ViewPresenter {
            var win = _GetWindow(id);
            if (win != null)
                return win.GetComponent<T>();
            return default(T);
        }


        /// <summary>
        ///     清空所有界面
        /// </summary>
        public virtual void ClearAllWindow() {
            if (DicAllWindows != null) {
                foreach (var window in DicAllWindows) {
                    var baseWindow = window.Value;
                    baseWindow.DestroyWindow();
                }
                DicAllWindows.Clear();
                DicShownedWindows.Clear();
            }
        }

        public void HideWindow(WindowId id, bool immediately = false, MyAction onCompleted = null) {
            if (DicShownedWindows.ContainsKey(id)) {
                var win = DicShownedWindows[id];
                DicShownedWindows.Remove(id);
                if (immediately) {
                    win.HideWindowImmediately();
                    if (null != onCompleted)
                        onCompleted();
                } else {
                    win.HideWindow(onCompleted);
                }
            }
        }

        //暂时只提供即时的隐藏
        public void HideAllShownWindow(bool includeFixed = true) {
            if (!includeFixed) {
                var removedKey = new List<WindowId>();
                foreach (var window in DicShownedWindows) {
                    if (window.Value.WinData.WindowType == UiWindowType.Fixed)
                        continue;
                    removedKey.Add(window.Key);
                    window.Value.HideWindowImmediately();
                }
                for (var i = 0; i < removedKey.Count; i++)
                    DicShownedWindows.Remove(removedKey[i]);
            } else {
                foreach (var window in DicShownedWindows)
                    window.Value.HideWindowImmediately();
                DicShownedWindows.Clear();
            }
        }

        private class SortPaireWinPanel : IComparer<KeyValuePair<WindowId,ViewPresenter>> {
            public int Compare(KeyValuePair<WindowId,ViewPresenter> x, KeyValuePair<WindowId,ViewPresenter> y) {
                var xPanels = x.Value.GetSortedPanels();
                var yPanels = y.Value.GetSortedPanels();
                int left = xPanels.Count > 0 ? xPanels[0].depth : -1;
                int right = yPanels.Count > 0 ? yPanels[0].depth : -1;
                return left - right;
            }
        }
        public List<KeyValuePair<WindowId, ViewPresenter>> GetSortedVisibleWindows() {
            List<KeyValuePair<WindowId, ViewPresenter>> lstShowedWins = new List<KeyValuePair<WindowId, ViewPresenter>>();
            foreach (var item in DicShownedWindows) {
                lstShowedWins.Add(new KeyValuePair<WindowId, ViewPresenter>(item.Key, item.Value));
            }
            lstShowedWins.Sort(new SortPaireWinPanel());
            return lstShowedWins;
        }
    }
}