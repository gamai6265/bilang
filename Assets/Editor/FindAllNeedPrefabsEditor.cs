using UnityEngine;
using System.Collections;
using UnityEditor;


public class FindAllNeedPrefabsEditor {
    [MenuItem("FindAll/打开窗口", false, 11)]
    static void OpenFindAllNeedPrefabs() {
            UIFindAllNeedPrefabs.Init();
    }
}
