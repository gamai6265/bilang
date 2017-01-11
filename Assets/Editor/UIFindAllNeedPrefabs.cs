using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

public class UIFindAllNeedPrefabs :EditorWindow {
    private static UIFindAllNeedPrefabs window;

    private static int selectIndex = -1;
    private static string selectCameraName;
    private static string[] GameObjectName;
    private static string selectinformationOriginal = "选择相机状态：未选择";
    private static string SelectTipsOriginal="";
    private static string selection;
    private static GameObject SelectObj;
    private static string pathname;
    private static Vector2 mScroll = Vector2.zero;
    private static string[] SelectCameraName = new string [10];
    private static List<Camera> cameraList = new List<Camera>();
    private static Camera CameraSelect;
    private static List<string> selectNameList=new List<string>();
    //要查找的类型
    private static Dictionary<string, List<UILabel>> AllObjs = new Dictionary<string, List<UILabel>>();

    public static void Init() {      
        window = (UIFindAllNeedPrefabs)EditorWindow.GetWindow<UIFindAllNeedPrefabs>("查找",true);
        ClearAll();
        InitData(); 
        OnSearchForNeed();
    }

    private static void ClearAll() {
        AllObjs.Clear();
        cameraList.Clear();
        selectNameList.Clear();
        for (int i = 0; i < SelectCameraName.Length; i++) {
            SelectCameraName[i] = "";
        }
    }

    private static void InitData() {
        selection = null;
        CameraSelect = null;
        mScroll = Vector2.zero;
        selectCameraName = null;
        pathname = null;
        SelectObj = null;
        selectIndex = -1;
        SelectTipsOriginal = "";
        selectinformationOriginal = "选择相机状态：未选择";
    }

    void OnGUI() {
        try {        
            int startWidth = 20;
            int startHeight = 30;
            EditorGUIUtility.labelWidth = 100f;
            int _LineStartWidth = startWidth;
            int _LineStartHeight = startHeight;
            EditorGUI.LabelField(new Rect(_LineStartWidth,_LineStartHeight,500,20),"显示信息：",EditorStyles.boldLabel );
            _LineStartWidth += 40;
            _LineStartHeight += 20;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 500, 20), "选择相机：", EditorStyles.boldLabel); 
            _LineStartWidth += 40;
            _LineStartHeight += 30;
            selectIndex = EditorGUI.Popup(new Rect(_LineStartWidth, _LineStartHeight, 105, 15), selectIndex,SelectCameraName);

            _LineStartWidth += 110;
            if(GUI.Button(new Rect(_LineStartWidth,_LineStartHeight,80,18),"选择")) {
                if (selectIndex >= 0) {
                    selectCameraName = SelectCameraName[selectIndex];
                    SetSelectPrefabsOriginal();
                    selectinformationOriginal = "选择相机状态：已选择";
                    //相机选择
                    Debug.Log("选择的相机是：" + CameraSelect);
                } else {
                   Debug.Log("未选择相机");
                }
            }
            _LineStartWidth += 90;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 500, 20), SelectTipsOriginal, EditorStyles.boldLabel);
            _LineStartWidth +=-200;
            _LineStartHeight += 30;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 500, 20), selectinformationOriginal, EditorStyles.boldLabel);

            GUILayout.Space(150f);
           
            NGUIEditorTools.DrawHeader("Select");
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(3);
                GUILayout.BeginVertical();

                mScroll = GUILayout.BeginScrollView(mScroll);
                int index = 0;
                foreach (var objs in AllObjs) {
                    foreach (var obj in objs.Value) {
                        ++index;
                        GUILayout.Space(-1f);
                        bool isSelect;
                        if (selection == obj.name) {
                            isSelect = true;
                        } else {
                            isSelect = false;
                        }
                        GUI.backgroundColor = isSelect ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                        GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                        GUILayout.Label(index.ToString(), GUILayout.Width(24f));
                        if (GUILayout.Button(obj.name, "OL TextField", GUILayout.Height(20f))) {                        
                            selection = obj.name;
                            ShowSelectGameObjectForHierarchy();
                            Debug.Log("选择的对象是："+SelectObj);
                        }
                        //if (selectNameList.Contains(obj.name)) {
                        //    if (GUILayout.Button("选择", GUILayout.Width(60f))) {
                        //    }
                        //}                     
                        GUILayout.EndHorizontal();
                    }                   
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.Space(3f);
                GUILayout.EndHorizontal();
            }
        }
         catch (System.Exception ex) {
                 Debug.Log(ex.Message);
        }
    }

    private static void OnSearchForNeed() {
        var findCamera = FindObjectsOfType(typeof(Camera));
        foreach (Camera go in findCamera) {
           cameraList.Add(go);     
        }

        var findObjs = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in findObjs) {
            if (PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance) {
                UnityEngine.Object parentObject = EditorUtility.GetPrefabParent(go);
                string path = AssetDatabase.GetAssetPath(parentObject);
                if (path != null) {
                    GameObjectName = path.Split('/', '.');
                    pathname = GameObjectName[GameObjectName.Length - 2];
                }
                if (go.name == pathname) {
                    //获取子对象
                    UILabel[] labels = go.GetComponentsInChildren<UILabel>(true);
                    foreach (var label in labels) {
                        if (AllObjs.ContainsKey(path)) {
                            AllObjs[path].Add(label);
                        } else {
                            AllObjs.Add(path, new List<UILabel>());
                            AllObjs[path].Add(label);
                        }
                    }
                }
            }          
        }

        int i = 0;
        foreach (var objs in AllObjs) {
            foreach (var obj in objs.Value) {
                selectNameList.Add(obj.name);
                i++;
            }
        }
        i = 0;
        foreach (var camera in cameraList) {
                SelectCameraName[i] =camera.name;
                i++;
            
        }
    }

    private void SetSelectPrefabsOriginal() {
        foreach (var camera in cameraList) {
            if (camera.name == selectCameraName) {
                CameraSelect = camera;
            }
        }
        EditorGUIUtility.PingObject(CameraSelect);
        Selection.activeObject = CameraSelect;      
    }

    private void ShowSelectGameObjectForHierarchy() {
        foreach (var label in AllObjs) {
            foreach (var obj in label.Value) {
                if (selection == obj.name) {
                    SelectObj = obj.gameObject;
                }
            }
        }
        EditorGUIUtility.PingObject(SelectObj);
        Selection.activeGameObject = SelectObj;
    }
}
