using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cloth3D.Ui{
    /// <summary>
    /// 窗口基类
    /// </summary>
    public abstract class ViewPresenter : MonoBehaviour {
        public WindowData WinData;
        private List<UIPanel> _uiPanels;
        public List<UIPanel> GetSortedPanels(bool force=false) {
            if (_uiPanels == null) {
                _uiPanels = NguiUtility.GetPanelSorted(gameObject, true);
            } else if (force) {
                _uiPanels = NguiUtility.GetPanelSorted(gameObject, true);
            }
            return _uiPanels;
        }

        protected void Awake() {
            //ViewRoot = ViewRoot ?? GetComponent<ViewPresenter>();
            gameObject.SetActive(true);
            WinData = new WindowData();
            InitWindowData();
            AwakeUnityMsg();
        }

        void Start() {
            StartUnityMsg();
        }

        void OnDestroy() {
            OnDestroyUnityMsg();
        }

        void OnValidate() {
            AutoPopulateDeclaredWidgets();

            OnValidateUnityMsg();
        }

        /// <summary>
        /// 在Awake中调用，初始化界面(给界面元素赋值操作)
        /// </summary>
        protected virtual void AwakeUnityMsg() {
        }
        protected virtual void StartUnityMsg() {
        }
        protected virtual void OnDestroyUnityMsg() {
        }
        protected virtual void OnValidateUnityMsg() {
        }

        [ContextMenu("Auto populate widgets")]
        public void AutoPopulateDeclaredWidgetsContextMenu() {
            AutoPopulateDeclaredWidgets();
        }
        /// <summary>
        ///     This matches the widgets found in the prefab this MB is attached to
        ///     with the properties defined in this file, so we can keep a reference
        ///     to them.
        ///     We use the following rules to do the matching:
        ///     GameObject's name == Property Name
        ///     GameObjects widget component type == Property type
        /// </summary>
        void AutoPopulateDeclaredWidgets() {
            foreach (var nguiWidget in GetDeclaredUiWidgets()) {
                var childTransform = GetChildRecursive(transform, nguiWidget.Name);
                if (childTransform == null) {
                    continue;
                }

                if (nguiWidget.GetValue(this) == null) {
                    nguiWidget.SetValue(this, childTransform.GetComponent(nguiWidget.FieldType));
                }
            }
        }
        /// <summary>
        ///    Finds all properties that derive from UIWidgets or UIWidgetContainers
        ///    in this object
        /// </summary>
        /// <returns>The declared user interface widgets.</returns>
        IEnumerable<FieldInfo> GetDeclaredUiWidgets() {
            return this.GetType().GetFields(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).Where(
                m => typeof(UIWidget).IsAssignableFrom(m.FieldType)
                || typeof(UIWidgetContainer).IsAssignableFrom(m.FieldType) || typeof(UIInput).IsAssignableFrom(m.FieldType) || typeof(UIScrollView).IsAssignableFrom(m.FieldType));
        }

        public static Transform GetChildRecursive(Transform trans, string name) {
            Component[] transforms = trans.GetComponentsInChildren(typeof(Transform), true);

            foreach (var component in transforms) {
                var atrans = (Transform) component;
                if (atrans.name == name) {
                    return atrans;
                }
            }
            return null;
        }

        /// <summary>
        /// 重置窗口
        /// </summary>
        public virtual void ResetWindow() {
        }

        /// <summary>
        /// 初始化窗口数据
        /// </summary>
        public abstract void InitWindowData();

        public virtual void ShowWindow(MyAction onComplete = null) {
            ShowWindowImmediately();
            if (onComplete != null)
                onComplete();
        }

        public virtual void HideWindow(MyAction action = null) {
            HideWindowImmediately();
            if (action != null)
                action();
        }

        public void HideWindowImmediately() {
            //SetActive(gameObject);
            gameObject.SetActive(false);
        }

        public void ShowWindowImmediately() {
            //NGUITools.SetActive(gameObject, true);
            gameObject.SetActive(true);
        }

        public virtual void DestroyWindow() {
            GameObject.Destroy(this.gameObject);
        }

        public bool IsVisible() {
            if (gameObject.activeSelf)
                return true;
            return false;
        }
    }
}
