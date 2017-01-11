using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

//
public class ReplaceFontEditor {
    [MenuItem("ChangeFont/打开替换字体窗口", false, 10)]
    [MenuItem("Assets/custom/打开替换字体窗口", false, 0)]
    static void OpenReplaceFontEditor() {
        UIReplaceFontEditorWindow.Init();
    }
}