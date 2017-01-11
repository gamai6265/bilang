using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloth3D;

public class ExportDesignModel {
    private static  int _selectedInstanceId;
    [MenuItem("GameObject/Cloth3D/ExportDesignModel", false, 0)]
    private static void Export() {
        GameObject selectedGameObject = EditorUtility.InstanceIDToObject(_selectedInstanceId) as GameObject;
        Stack<Transform> stackObjs = new Stack<Transform>();
        StringBuilder path = new StringBuilder();
        if (selectedGameObject!=null)
            PreTraverseObj(selectedGameObject.transform, (trans) => {
                if (trans.parent != null && (stackObjs.Count>0 && trans.parent == stackObjs.First())) {
                    stackObjs.Push(trans);
                } else {
                    while (stackObjs.Count>0 && stackObjs.First()!=trans.parent) {
                        stackObjs.Pop();
                    }
                    stackObjs.Push(trans);
                }
                StringBuilder temPath = new StringBuilder();
                foreach (var item in stackObjs.Reverse()) {
                    temPath.Append("/");
                    temPath.Append(item.name);
                }
                //Debug.Log(temPath.ToString());
                path.Append(temPath.ToString());
                path.Append("\n");
            });
        Debug.Log(path.ToString());
    }

    private static void PreTraverseObj(Transform go, MyAction<Transform> doAction) {
        if (doAction != null)
            doAction(go);
        for (int k = 0; k < go.childCount; k++) {
            Transform chil = go.GetChild(k);
            PreTraverseObj(chil, doAction);
        }
    }


    [InitializeOnLoadMethod]
    static void StartInitializeOnLoadMethod() {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGui;
    }

    static void OnHierarchyGui(int instanceId, Rect selectionRect) {
        if (Event.current != null && selectionRect.Contains(Event.current.mousePosition)
            && Event.current.button == 1 && Event.current.type <= EventType.mouseUp) {
            GameObject selectedGameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (selectedGameObject) {
                Vector2 mousePosition = Event.current.mousePosition;
                _selectedInstanceId = instanceId;
                //EditorUtility.DisplayPopupMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), "GameObject/Cloth3D", null);
                //EditorUtility.DisplayCustomMenu();
                //Event.current.Use();
            }
        }
    }

}