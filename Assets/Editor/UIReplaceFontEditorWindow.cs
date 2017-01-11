using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class FontInfo {

}

public class UIReplaceFontEditorWindow : EditorWindow {
    private static UIReplaceFontEditorWindow window;

    private static Dictionary<string, Font> allFonts = new Dictionary<string, Font>();  // key: fontName, value : Font 
    private static Dictionary<string, HashSet<int>> allFontsSize = new Dictionary<string, HashSet<int>>(); // key: fontName, value :Hash<int>, Hash<int> 保存所有的字号（去重） 
    private static Dictionary<string, Dictionary<int, HashSet<string>>> allFontNameSizePrefabName = new Dictionary<string, Dictionary<int, HashSet<string>>>();  // key : fontName, value: < key:fontSize, value: prefabNames >

    private static bool isReplace = false;
    private static UnityEngine.Object[] selectObjs;
    private static List<GameObject> allNeedReplacePrefab = new List<GameObject>();
    private static Dictionary<string,UILabel[]> allLabel=new Dictionary<string, UILabel[]>();
    private static string[] allFontName = new string[8];  // 预先分配
    private static string writefileinfo = "获取prefabs内容信息状态：未获取 ";
    // prefab文字信息保存目录  
    private static string prefabFontSizeInfoSaveDir = "Assets/Resources/Fonts";
    private int ID = -1;
    // 待替换字体 
    public Font previousFont = null;
    private string previourFontSelectInfo = "";
    // 新字体 
    public Font newFont = null;
    private string newFontSelectInfo = "";
    // 字体对应文件 
    public string fontSizeFilePath = "fontsize";
    private string fontSizeFilePathSelectInfo = "";
    // 保存字号对应文件 
    private ArrayList fontSizeArray = new ArrayList();
    // 新prefab保存目录  
    public string newPrefabSaveDir = "Assets/Resources/Fonts";

    private string objpath_test = "";

    public static void Init() {
        // Get existing open window or if none, make a new one:
        window = (UIReplaceFontEditorWindow)EditorWindow.GetWindow<UIReplaceFontEditorWindow>("替换字体窗口", true);

        StaticInitData();
    }

    static void StaticInitData() {
        allFonts.Clear();
        foreach (var item in allFontsSize) {
            item.Value.Clear();
        }
        allFontsSize.Clear();
        allNeedReplacePrefab.Clear();
        for (int i = 0; i < allFontName.Length; i++) {
            allFontName[i] = "";
        }
        writefileinfo = "获取prefabs内容信息状态：未获取";
        prefabFontSizeInfoSaveDir = "Assets/Resources/Fonts";
        isReplace = false;
       allFontNameSizePrefabName.Clear();
       allLabel.Clear();
    }

    void InitData() {
        ID = -1;
        // 待替换字体 
        previousFont = null;
        previourFontSelectInfo = "";
        // 新字体 
        newFont = null;
        newFontSelectInfo = "";
        // 字体对应文件 
        fontSizeFilePath = "fontsize";
        fontSizeFilePathSelectInfo = "";
        // 保存字号对应文件 
        fontSizeArray.Clear();
        // 新prefab保存目录  
        newPrefabSaveDir = "Assets/Resources/Fonts";

        objpath_test = "";
    }

    static void Savefile(Font replace, Font matching,string fontsize) {     
        writefileinfo = WriteFontSizeInfoToFile(prefabFontSizeInfoSaveDir,replace,matching,fontsize);                 
    }

    static void FontInit() {
            isReplace = true;
            // 查找选中的prefab内的所有字体，以及字体所设置的所有字号  
            selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            foreach (UnityEngine.Object selectObj in selectObjs) {
                GameObject obj = null;
                try {
                    obj = (GameObject) selectObj;
                } catch {
                    continue;
                }

                if (obj == null || selectObj == null) {
                    Debug.LogWarning("ERROR:Obj Is Null !!!");
                    continue;
                }
                string objPath = AssetDatabase.GetAssetPath(selectObj);

                if (objPath.Length < 1 || objPath.EndsWith(".prefab") == false) {
                    Debug.LogWarning("ERROR:Folder=" + objPath);
                } else {
                    string prePath = (objPath).Substring(7, objPath.Length - 7).Replace("\\", "/").ToLower();

                    allNeedReplacePrefab.Add(obj);
                    Debug.Log("Selected Folder=" + objPath);

                    // 直接修改prefab  
                    UILabel[] labels = obj.GetComponentsInChildren<UILabel>(true);
                    if (!allLabel.ContainsKey(objPath)) {
                        allLabel.Add(objPath,labels);
                    }
                    
                    foreach (UILabel label in labels) {
                        // 保存字体 以及字号 

                        if (label.trueTypeFont == null) {
                            Debug.Log("font is null" + prePath);
                        } else {
                            if (!allFonts.ContainsKey(label.trueTypeFont.name)) {
                                allFonts.Add(label.trueTypeFont.name, label.trueTypeFont);
                            }
                            if (allFontsSize.ContainsKey(label.trueTypeFont.name)) {
                                allFontsSize[label.trueTypeFont.name].Add(label.fontSize);
                            } else {
                                allFontsSize.Add(label.trueTypeFont.name, new HashSet<int>());
                                allFontsSize[label.trueTypeFont.name].Add(label.fontSize);
                            }

                            if (allFontNameSizePrefabName.ContainsKey(label.trueTypeFont.name)) {
                                if (allFontNameSizePrefabName[label.trueTypeFont.name].ContainsKey(label.fontSize)) {
                                    allFontNameSizePrefabName[label.trueTypeFont.name][label.fontSize].Add(objPath);
                                } else {
                                    allFontNameSizePrefabName[label.trueTypeFont.name].Add(label.fontSize,
                                        new HashSet<string>());
                                    allFontNameSizePrefabName[label.trueTypeFont.name][label.fontSize].Add(objPath);
                                }
                            } else {
                                allFontNameSizePrefabName.Add(label.trueTypeFont.name,
                                    new Dictionary<int, HashSet<string>>());
                                allFontNameSizePrefabName[label.trueTypeFont.name].Add(label.fontSize,
                                    new HashSet<string>());
                                allFontNameSizePrefabName[label.trueTypeFont.name][label.fontSize].Add(objPath);
                            }
                        }
                    }
                    UIPopupList[] poplists = obj.GetComponentsInChildren<UIPopupList>(true);
                    foreach (UIPopupList popListItem in poplists) {
                        // 保存字体 以及字号 
                        if (popListItem.trueTypeFont == null) {
                            Debug.Log("font is null" + prePath);
                        } else {
                            if (!allFonts.ContainsKey(popListItem.trueTypeFont.name)) {
                                allFonts.Add(popListItem.trueTypeFont.name, popListItem.trueTypeFont);
                            }
                            if (allFontsSize.ContainsKey(popListItem.trueTypeFont.name)) {
                                allFontsSize[popListItem.trueTypeFont.name].Add(popListItem.fontSize);
                            } else {
                                allFontsSize.Add(popListItem.trueTypeFont.name, new HashSet<int>());
                                allFontsSize[popListItem.trueTypeFont.name].Add(popListItem.fontSize);
                            }
                        }
                    }
                }
            }
            int i = 0;
            foreach (KeyValuePair<string, Font> kv in allFonts) {
                allFontName[i] = kv.Key;
                i++;
            }
            writefileinfo = "获取prefabs内容信息状态：获取成功";
    }

    static string WriteFontSizeInfoToFile(string path, Font replace, Font matching,string size) {

        FileStream fs = new FileStream(prefabFontSizeInfoSaveDir + "/fontsizeinfo.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs,Encoding.UTF8);
        List<int> fontSizeList = new List<int>();
        sw.WriteLine();
        sw.WriteLine("修改前字体信息如下，格式为：【 字体名称：所有字号 】 ");
        // 输出信息  
        foreach (var item in allFontsSize) {
            sw.Write("   "+item.Key + ":");
            fontSizeList.Clear();

            foreach (var fontsize in item.Value) {
                fontSizeList.Add(fontsize);
                //sw.Write(fontsize + " ");
            }
            fontSizeList.Sort();

            foreach (var fontsizeInList in fontSizeList) {
                sw.Write(fontsizeInList + ",");
            }
            sw.WriteLine();
        }
        //foreach (KeyValuePair<string, Dictionary<int, HashSet<string>>> allItemInfo in allFontNameSizePrefabName) {
        //    sw.WriteLine();
        //    sw.Write("字体名称(fontName) ：" + allItemInfo.Key + "," + "所有字号个数(allFontSizeCount)：" + allItemInfo.Value.Count);
        //    sw.WriteLine();

        //    List<KeyValuePair<int, HashSet<string>>> lst = new List<KeyValuePair<int, HashSet<string>>>(allItemInfo.Value);
        //    lst.Sort(delegate(KeyValuePair<int, HashSet<string>> s1, KeyValuePair<int, HashSet<string>> s2) {
        //        return s1.Key.CompareTo(s2.Key);
        //    });
        sw.WriteLine();
             sw.WriteLine("   " + "需替换字体(原来)：" + matching.name);
            sw.WriteLine("   "+"新字体(修改)：" + replace.name + "    "+"字体大小(修改)：" + size+"    "+"修改时间：" + DateTime.Now);
            foreach (var lablepath in allLabel) {
                sw.WriteLine();
                sw.WriteLine("  "+"--路径：" + lablepath.Key);
                foreach (var child in lablepath.Value) {
                    if (child.trueTypeFont == matching) {
                        sw.WriteLine("      "+"名称:" + child.name + "      "+"字体(原来)：" + child.trueTypeFont.name +"      "+"字体(修改)：" + replace.name +
                                    "      "+"size(原来):" + child.fontSize +"      "+"size(修改):" + size);
                    } else {
                        sw.WriteLine("      "+"名称:" + child.name +"      "+"字体(原来)：" + child.trueTypeFont.name+"      " +"与所选的需要替换字体不匹配不需要修改");
                    }

                }
            }
      //  }
        sw.Flush();
        sw.Close();
        fs.Close();
        return "获取prefabs内容信息状态：获取成功";
    }

    /// <summary> 保存.</summary>
    void OnSelectNewFont(UnityEngine.Object obj) {
        newFont = obj as Font;
        //NGUISettings.ambigiousFont = obj;
        Repaint();
    }

    void OnSelectPreviousFont(UnityEngine.Object obj) {
        previousFont = obj as Font;
        //NGUISettings.ambigiousFont = obj;
        Repaint();
    }
    void OnSelectAtlas(UnityEngine.Object obj) {
        NGUISettings.atlas = obj as UIAtlas;
        Repaint();
    }
    /// <summary> 刷新窗口. </summary>
    void OnSelectionChange() { Repaint(); }

    public static bool IsSetNullFont;

    /// <summary>UI绘制区域.</summary>
    void OnGUI() {
        try {
            int startWidth = 10;
            int startHeight = 16;
            EditorGUIUtility.labelWidth = 100f;


            int _LineStartWidth = startWidth;
            int _LineStartHeight = startHeight;

            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 500, 30), "1、选择Prefabs内字体名称、字号信息文件保存目录（文件名默认为fontsizeinfo.txt）", EditorStyles.boldLabel);

            _LineStartWidth = startWidth + 40;
            _LineStartHeight += 30;
            newPrefabSaveDir = EditorGUI.TextArea(new Rect(_LineStartWidth, _LineStartHeight, 300, 20), prefabFontSizeInfoSaveDir, EditorStyles.textField);
            _LineStartWidth += 310;
            if (GUI.Button(new Rect(_LineStartWidth, _LineStartHeight, 80, 18), "选择目录")) {
                prefabFontSizeInfoSaveDir = EditorUtility.SaveFolderPanel("Save newPrefab to directory", "./", "");
            }
            _LineStartWidth = startWidth;
            _LineStartHeight += 30;

            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 500, 30), "2、点击按钮获取所选prefabs内的字体信息", EditorStyles.boldLabel);

            _LineStartWidth = startWidth + 40;
            _LineStartHeight += 30;

            if (GUI.Button(new Rect(_LineStartWidth, _LineStartHeight, 300, 40), "获取所选Prefab内字体信息")) {
               FontInit();
            }
            _LineStartWidth = startWidth + 40;
            _LineStartHeight += 40;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 500, 30), writefileinfo, EditorStyles.boldLabel);

            _LineStartWidth = startWidth;
            _LineStartHeight += 40;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 150, 30), "3、选择需要替换的字体", EditorStyles.boldLabel);

            _LineStartWidth = startWidth + 40;
            _LineStartHeight += 30;

            ID = EditorGUI.Popup(new Rect(_LineStartWidth, _LineStartHeight, 105, 15), ID, allFontName);
            if (ID >= 0) {
                previousFont = allFonts[allFontName[ID]];
            }

            _LineStartWidth += 120;
            //EditorGUI.TextArea(new Rect(_LineStartWidth, _LineStartHeight, 300, 20), prefabFontSizeInfoSaveDir, EditorStyles.textField);
            EditorGUI.ObjectField(new Rect(_LineStartWidth, _LineStartHeight, 150, 20), previousFont, typeof(Font), false);
            _LineStartWidth += 150;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 150, 30), previourFontSelectInfo, EditorStyles.boldLabel);
            /***
            _LineStartWidth = startWidth + 40;
            _LineStartHeight += 40;

            GUILayout.BeginHorizontal();

            if (GUI.Button(new Rect(_LineStartWidth, _LineStartHeight, 80, 20), "Font       ▽"))
            {
                ComponentSelector.Show<Font>(OnSelectPreviousFont);
            }
            _LineStartWidth += 90;
            previousFont = EditorGUI.ObjectField(new Rect(_LineStartWidth, _LineStartHeight, 200, 20), previousFont, typeof(Font), false) as Font;
            _LineStartWidth += 210;
            if (previousFont != null && GUI.Button(new Rect(_LineStartWidth, _LineStartHeight, 20, 20), "X"))
            {
                previousFont = null;
            }
            GUILayout.EndHorizontal();
            **/
            _LineStartWidth = startWidth;
            _LineStartHeight += 30;
            EditorGUI.LabelField(new Rect(startWidth, _LineStartHeight, 300, 30), "4、选择新字体(如不选择新字体，则Prefab内字体将置空)", EditorStyles.boldLabel);

            _LineStartWidth = startWidth;
            _LineStartHeight += 20;
            GUILayout.BeginHorizontal();
            _LineStartWidth = startWidth + 40;
            if (GUI.Button(new Rect(_LineStartWidth, _LineStartHeight, 80, 20), "Font       ▽")) {
                ComponentSelector.Show<Font>(OnSelectNewFont);
            }
            _LineStartWidth += 90;
            newFont = EditorGUI.ObjectField(new Rect(_LineStartWidth, _LineStartHeight, 200, 20), newFont, typeof(Font), false) as Font;
            _LineStartWidth += 210;
            if (newFont != null && GUI.Button(new Rect(_LineStartWidth, _LineStartHeight, 20, 20), "X")) {
                newFont = null;
            }
            _LineStartWidth += 30;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 150, 30), newFontSelectInfo, EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            _LineStartWidth = startWidth;
            _LineStartHeight += 30;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 300, 30), "5、输入字号（不输入字号，则字号不变）", EditorStyles.boldLabel);

            _LineStartWidth = startWidth + 40;
            _LineStartHeight += 20;
            //Rect textRect = new Rect(_LineStartWidth, _LineStartHeight, 300, 150);
            fontSizeFilePath = EditorGUI.TextArea(new Rect(_LineStartWidth, _LineStartHeight, 300, 20), fontSizeFilePath, EditorStyles.textField);
            _LineStartWidth += 310;

            _LineStartWidth = startWidth + 40;
            _LineStartHeight += 20;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 150, 30), fontSizeFilePathSelectInfo, EditorStyles.boldLabel);

            _LineStartWidth = startWidth + 40;
            _LineStartHeight += 20;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 150, 20), "示例：", EditorStyles.boldLabel);
            _LineStartWidth += 30;
            EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 150, 20), "12", EditorStyles.boldLabel);
            _LineStartWidth = startWidth + 70;
            _LineStartHeight += 20;
            //EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 150, 20), "2，25，40", EditorStyles.boldLabel);

            //_LineStartWidth = startWidth + 70;
            //_LineStartHeight += 20;
            //EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 150, 20), "3，18，28", EditorStyles.boldLabel);

            //_LineStartWidth = startWidth;
            //_LineStartHeight += 30;

            //EditorGUI.LabelField(new Rect(_LineStartWidth, _LineStartHeight, 150, 150), "选择新Prefab保存目录", EditorStyles.boldLabel);

            //_LineStartWidth = startWidth;
            //_LineStartHeight += 20;
            //newPrefabSaveDir = EditorGUI.TextArea(new Rect(_LineStartWidth, _LineStartHeight, 300, 20), newPrefabSaveDir, EditorStyles.textField);
            //_LineStartWidth += 310;
            //if (GUI.Button(new Rect(_LineStartWidth, _LineStartHeight, 80, 18), "选择目录"))
            //{
            //    newPrefabSaveDir = EditorUtility.SaveFolderPanel("Save newPrefab to directory", "./", "");
            //}

            _LineStartWidth = startWidth;
            _LineStartHeight += 30;
            EditorGUI.LabelField(new Rect(startWidth, _LineStartHeight, 150, 30), "6、点击按钮替换文字信息", EditorStyles.boldLabel);

            _LineStartWidth = startWidth + 40;
            _LineStartHeight += 30;

            if (GUI.Button(new Rect(_LineStartWidth, _LineStartHeight, 300, 36), "开始替换")) {
                ReplaceFontAndFontSize();
            }
        } catch (System.Exception ex) {
            Debug.LogError(ex.Message + objpath_test);
            previousFont = null;
            newFont = null;
            fontSizeFilePath = "fontsize";

        }
    }

    private void ReplaceFontAndFontSize() {

        if (isReplace == false) {          
            Debug.LogError("未获取所选prefabs内的字体信息");
        } else {
            if (previousFont == null) {
                previourFontSelectInfo = "未选择需要替换的字体，请选择...";
                Debug.LogError(previourFontSelectInfo);
                return;
            }
            if (newFont == null) {
                newFontSelectInfo = "未选择新字体，字体将置空";
                Debug.LogWarning(newFontSelectInfo);
            }
           
            if (fontSizeFilePath.Equals("") || fontSizeFilePath.Equals("fontsize")) {
                fontSizeFilePathSelectInfo = "你没有选择对应字号文件，将不替换字号 ";
                Debug.LogWarning(fontSizeFilePathSelectInfo);
                Savefile(newFont, previousFont, "未修改");
                CorrectionPublicFont(newFont, previousFont, false);

            } else {
            //读取文件内容 
                //StreamReader sr = new StreamReader(prefabFontSizeInfoSaveDir + @"\fontsizeinfo.txt", Encoding.Default);
                //string strLine = null;
                //fontSizeArray.Clear();
                //while ((strLine = sr.ReadLine()) != null) {
                //    if (!string.IsNullOrEmpty(strLine)) {
                //        string[] newArray = strLine.Split(',');
                //        foreach (var item in newArray) {
                //            Debug.Log(item);
                //        }
                //        //   fontSizeArray.Add(newArray);
                //    }
                //}
                //sr.Close();
                //// 具体替换 
                Savefile(newFont, previousFont, fontSizeFilePath);
                CorrectionPublicFont(newFont, previousFont, true);
               
            }
            StaticInitData();
            InitData();
        }
    }

    private void CorrectionPublicFont(Font replace, Font matching, bool isReplaceFontSize) {

        foreach (GameObject obj in allNeedReplacePrefab) {
            // 直接修改prefab  
            UILabel[] labels = obj.GetComponentsInChildren<UILabel>(true);
            int newFontSize = 0;
            foreach (UILabel label in labels) {
                if (label.trueTypeFont == matching) {
                    label.trueTypeFont = replace;
                    if (isReplaceFontSize) {
                        newFontSize = GetNewFontSizeByOldSize(int.Parse(fontSizeFilePath));
                    } else {
                        newFontSize = GetNewFontSizeByOldSize(label.fontSize);
                    }
                    label.fontSize = newFontSize;             
                }
            }
            UIPopupList[] poplists = obj.GetComponentsInChildren<UIPopupList>(true);
            foreach (UIPopupList popListItem in poplists) {
                if (popListItem.trueTypeFont == matching) {
                    //    NGUISettings.ambigiousFont = replace;
                    popListItem.trueTypeFont = replace;
                    newFontSize = GetNewFontSizeByOldSize(popListItem.fontSize);
                    popListItem.fontSize = newFontSize;
                }
            }
            EditorUtility.SetDirty(obj);
            /**** 创建新prefab 失败  ******/
            /****
            GameObject clone = GameObject.Instantiate(obj) as GameObject;
            UILabel[] labels = clone.GetComponentsInChildren<UILabel>(true);
            int newFontSize = 0;
            foreach (UILabel label in labels)
            {
                if (label.trueTypeFont == matching)
                {
                    label.trueTypeFont = replace;
                    newFontSize = GetNewFontSizeByOldSize(label.fontSize);
                    label.fontSize = newFontSize;
                }
            }
            UIPopupList[] poplists = clone.GetComponentsInChildren<UIPopupList>(true);
            foreach (UIPopupList popListItem in poplists)
            {
                if (popListItem.trueTypeFont == matching)
                {
                    //    NGUISettings.ambigiousFont = replace;
                    popListItem.trueTypeFont = replace;
                    newFontSize = GetNewFontSizeByOldSize(popListItem.fontSize);
                    popListItem.fontSize = newFontSize;
                }
            }
            SaveDealFinishPrefab(clone, path);
            GameObject.DestroyImmediate(clone);
             ******/
            Debug.Log("Connect Font Success=" + obj.name);

        }
        EditorApplication.SaveAssets();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private int GetNewFontSizeByOldSize(int fontSize) {
        int newFontSize = fontSize;
        if (fontSizeArray.Count > 0) {
            int nRow = fontSizeArray.Count;
            int nCol = ((string[])fontSizeArray[0]).Length;
            for (int i = 0; i < nRow; i++) {
                string[] data = (string[])fontSizeArray[i];
                for (int j = 0; j < nCol; j++) {
                    if (fontSize == int.Parse(data[1])) {
                        newFontSize = int.Parse(data[2]);
                        return newFontSize;
                    }
                }

            }
        }
        return newFontSize;
    }
    //private void SaveDealFinishPrefab(GameObject go, string path)
    //{
    //    if (File.Exists(path) == true)
    //    {
    //        UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
    //        PrefabUtility.ReplacePrefab(go, prefab);
    //    }
    //    else
    //    {
    //        PrefabUtility.CreatePrefab(path, go);
    //    }
    //}
}
