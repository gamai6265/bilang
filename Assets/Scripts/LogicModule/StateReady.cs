using System.Collections;
using System.IO;
using Cloth3D.Comm;
using Cloth3D.Data;
using UnityEngine;

namespace Cloth3D.Logic {
    internal class StateReady : State {
        private bool _isExtractOk;
        public StateReady(string name) : base(name) {
        }

        protected override void Init(Fsm fsm) {
            //throw new NotImplementedException();
        }

        public override void EnterState(Fsm fsm) {
            _isExtractOk = false;
            LogSystem.Debug("enter StateReady state.");
            //TODO danie 提取标志
            //if (File.Exists(Path.Combine(GlobalVariables.PersistentDataPath, FilePathDefine.ResSheetCacheDir+FilePathDefine.UiConfig))) {
            //    _isExtractOk = true;
            //} else {
            /*var gfx = ComponentMgr.Instance.FindComponent(ComponentNames.ComGfx) as GfxModule;
            if (null != gfx) {
                LogSystem.Debug("QueueAction");
                gfx.Dispatcher.QueueAction(() => {
                    CoroutineMgr.Instance.StartCoroutine(_ExtractSheetList());
                }
            );
            }*/
            //}
            CoroutineMgr.Instance.StartCoroutine(_ExtractSheetList());
        }

        private IEnumerator _ExtractSheetList() {
            LogSystem.Debug("_ExtractSheetList");
            string buildinDir = GlobalVariables.BuildinDataPath+"/";
            string persistDir = GlobalVariables.PersistentDataPath+"/" + FilePathDefine.ResSheetCacheDir;
            LogSystem.Debug(buildinDir);
            LogSystem.Debug(persistDir);

            if (!buildinDir.Contains("://"))
                buildinDir = "file://" + buildinDir; //TODO danie 查看手机平台
            string sheetListPath = buildinDir  + FilePathDefine.ResSheetList;
            WWW listData = new WWW(sheetListPath);
            yield return listData;
            string listTxt = listData.text;
            if (null != listTxt) {
                using (StringReader sr = new StringReader(listTxt)) {
                    string path;
                    do {
                        path = sr.ReadLine();
                        if (null != path) {
                            path = path.Trim();
                            string url = buildinDir + path;
                            // LogSystem.Debug("extract " + url);
                            string filePath = Path.Combine(persistDir, path);
                            string dir = Path.GetDirectoryName(filePath);
                            if (dir != null && !Directory.Exists(dir))
                                Directory.CreateDirectory(dir);
                            WWW temp = new WWW(url);
                            yield return temp;
                            if (null != temp.bytes) {
                                File.WriteAllBytes(filePath, temp.bytes);
                            } else {
                                LogSystem.Warn(" can't load{0}",path);
                            }
                            temp.Dispose();
                        }
                    } while (null != path);
                    sr.Close();
                }
            } else {
                LogSystem.Warn("Can't load SheetList");
            }
            _isExtractOk = true;
        }

        public override void LeaveState(Fsm fsm) {
            fsm.SetParamValue("startUpdate", false);
        }

        public override void Excute(Fsm fsm) {
            if (_isExtractOk) {
                fsm.SetParamValue("startUpdate", true);
                _isExtractOk = false;
            }
        }
    }
}