using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Cloth3D.Comm;
using LitJson;
using UnityEngine;

namespace Cloth3D {
    public class Utils {
        public static List<string> SplitAsList(string orgStr, string[] split) {
            var splitStr = orgStr.Split(split, StringSplitOptions.None);
            var list = new List<string>();
            foreach (var str in splitStr) {
                list.Add(str);
            }
            return list;
        }

        public static List<T> SplitAsNumericList<T>(string vec, string[] split) {
            var list = new List<T>();
            var strPos = vec;
            var resut = strPos.Split(split, StringSplitOptions.None);

            try {
                if (resut.Length > 0 && resut[0] != "") {
                    for (var index = 0; index < resut.Length; index++) {
                        list.Add((T) Convert.ChangeType(resut[index], typeof (T)));
                    }
                }
            } catch (Exception ex) {
                var info = string.Format("ConvertNumericList vec:{0} ex:{1} stacktrace:{2}",
                    vec, ex.Message, ex.StackTrace);
                LogSystem.Error(info);
                list.Clear();
            }

            return list;
        }

        public static void LogCallStack(LogType logType) {
            var trace = new StackTrace();
            var logStr = string.Format("LogCallStack:\n{0}\n", trace);
            switch (logType) {
                case LogType.Debug:
                    LogSystem.Debug(logStr);
                    break;
                case LogType.Info:
                    LogSystem.Info(logStr);
                    break;
                case LogType.Warn:
                    LogSystem.Warn(logStr);
                    break;
                case LogType.Error:
                    LogSystem.Error(logStr);
                    break;
                case LogType.Fatal:
                    LogSystem.Fatal(logStr);
                    break;
            }
        }

        public static string GetJsonStr(JsonData json) {
            if (json == null) {
                return string.Empty;
            }
            return json.ToString();
        }
        
        public static string GetMd5(string input) {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }


        public class AsyncInfo {
            public bool IsDone = false;
            public bool IsError = false;
            public Coroutine CurCoroutine = null;
            public System.Object Target = null;
        }
        public static IEnumerator LoadCacheImag(AsyncInfo asynInfo, DownCacheImageInfo downInfo) {
            string filePath = Application.persistentDataPath + "/" + downInfo.PathPrefix + downInfo.ImageName;
            if (File.Exists(filePath)) {
                FileStream fs = new FileStream(filePath, FileMode.Open);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                fs.Dispose();
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(buffer);
                asynInfo.IsDone = true;
                downInfo.Texture = texture;
            } else {
                WWW www = new WWW(downInfo.Url + downInfo.ImageName);
                yield return www;
                Texture2D newTexture = null;
                if (www.error == null) {
                    if (www.isDone) {
                        newTexture = www.texture;
                    }
                    else {
                        asynInfo.IsError = true;
                    }
                }
                else {
                    www = new WWW(Constants.DefaultImage);
                    yield return www;
                    newTexture = www.texture;
                }
                www.Dispose();
                var pngData = newTexture.EncodeToJPG();
                File.WriteAllBytes(filePath, pngData);
                asynInfo.IsDone = true;
                downInfo.Texture = newTexture;
            }
        }
        //
        public class DownCacheImageInfo {
            public string ImageName = string.Empty;
            public string PathPrefix = string.Empty;
            public string Url = string.Empty;
            public Texture2D Texture = null;
            public object AppendInfo = null;
        }
        public static IEnumerator DownLoadCacheImags(AsyncInfo info, IEnumerable<DownCacheImageInfo> lstDownload, MyAction<DownCacheImageInfo> itemCallback = null) {
            if (lstDownload.Any()) {
                List<AsyncInfo> lstTask = new List<AsyncInfo>();
                List<AsyncInfo> lstDoneTask = new List<AsyncInfo>();
                var iter = lstDownload.GetEnumerator();
                while (true) {
                    bool hasNext = true;
                    while (lstTask.Count < 4 && (hasNext=iter.MoveNext())) { //TODO 线程
                        var downInfo = iter.Current;
                        AsyncInfo asyncInfo = new AsyncInfo();
                        asyncInfo.Target = downInfo;
                        asyncInfo.CurCoroutine = CoroutineMgr.Instance.StartCoroutine(LoadCacheImag(asyncInfo, downInfo));
                        lstTask.Add(asyncInfo);
                    }
                    lstDoneTask.Clear();
                    foreach (var taskItem in lstTask) {
                        if (taskItem.IsError) {
                            info.IsError = true;
                            yield break;
                        }
                        if (taskItem.IsDone) {
                            lstDoneTask.Add(taskItem);
                            var downInfo = (DownCacheImageInfo)taskItem.Target;
                            if (downInfo != null && itemCallback != null) {
                                itemCallback(downInfo);
                            }
                        }
                    }
                    foreach (var doneItem in lstDoneTask) {
                        lstTask.Remove(doneItem);
                    }
                    if (lstTask.Count == 0 && !hasNext) {
                        break;
                    }
                    yield return 1;
                }
            }
            info.IsDone = true;
        }
    }
}