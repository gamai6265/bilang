using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using cn.sharesdk.unity3d;
using Cloth3D.Comm;
using Cloth3D.Design;
using Cloth3D.Gfx;
using Cloth3D.Interfaces;
using Cloth3D.Logic;
using Cloth3D.Login;
using Cloth3D.Network;
using Cloth3D.Resource;
using Cloth3D.Ui;
using Cloth3D.Update;
using Cloth3D.User;
using Cloth3D.Order;
using Cloth3D.Input;
using HttpRequest;
using LitJson;
using UnityEngine;

namespace Cloth3D {
    internal class Main : UnitySingleton<Main> {
        private float m_LastUpdateShowTime = 0f;  //上一次更新帧率的时间;  

        private float m_UpdateShowDeltaTime = 0.01f;//更新帧率的时间间隔;  

        private int m_FrameUpdate = 0;//帧数;  

        private float m_FPS = 0;

        private int _fps;
        
        // Use this for initialization
        internal void Start() {
            LogSystem.OnLogOutput = (logtype, msg) => {
                if (Application.platform == RuntimePlatform.WindowsEditor) {
                    switch (logtype) {
                        case LogType.Debug:
                        case LogType.Info:
                            Debug.Log(logtype + msg);
                            break;
                        case LogType.Warn:
                            Debug.LogWarning(logtype + msg);
                            break;
                        case LogType.Error:
                        case LogType.Fatal:
                            Debug.LogError(logtype + msg);
                            break;
                    }
                }
                var http = new RequestHttpWebRequest();
                var reqInfo = new RequestInfo("http://log.bi-lang.com/log/unity3d.php") {
                    Headers = new WebHeaderCollection(),
                };
                string str = "msg=" + GlobalVariables.deviceUniqueIdentifier + msg;
                byte[] data = Encoding.UTF8.GetBytes(str);
                reqInfo.PostData = data;

                http.GetResponseAsync(reqInfo, null);
            };

            m_LastUpdateShowTime = Time.realtimeSinceStartup;
            _SetupGlobalVariables();
            _SetupQuality();
            var ret = _CreateComponets();
            if (ret) {
                var initOk = ComponentMgr.Instance.Init();
                if (initOk) {
                    var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as LogicModule;
                    if (logic != null) {
                        logic.StartLogic();
                    }
                } else {
                    LogSystem.Error("Componets init failed.");
                }
            } else {
                LogSystem.Error("create components failed.");
            }
            ShareSDK.Instance.Init();

            var loginMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogin) as ILoginMgr;
            if (loginMgr != null) {
                if (PlayerPrefs.GetString(Constants.UserName, string.Empty) != string.Empty ||
                    PlayerPrefs.GetString(Constants.PassWord, string.Empty) != string.Empty)
                    loginMgr.Login(PlayerPrefs.GetString(Constants.UserName, string.Empty),
                        PlayerPrefs.GetString(Constants.PassWord, string.Empty));
            }
        }

        private void _SetupGlobalVariables() {
            if (Application.isEditor)
                GlobalVariables.IsEditor = true;
            else
                GlobalVariables.IsEditor = false;
            GlobalVariables.PersistentDataPath = Application.persistentDataPath;
            GlobalVariables.BuildinDataPath = Application.streamingAssetsPath;
            GlobalVariables.IsPublish = false;
            GlobalVariables.StartRoutineObjectName = gameObject.name;
            GlobalVariables.deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        }

        private void _SetupQuality() {
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 0;
            //QualitySettings.SetQualityLevel(3);
            Application.runInBackground = true;
//            HardWareQuality.Clear();
//            HardWareQuality.ComputeHardwarePerformance();
            //HardWareQuality.SetResolution();
//            HardWareQuality.SetQualityAll(); //TODO danie 这里有问题
        }

        internal void FixedUpdate() {
        }

        private void Update() {
            ComponentMgr.Instance.Tick();
            return;
            try {
                ComponentMgr.Instance.Tick();
            } catch (Exception e) {
                LogSystem.Error(e.Message);
            }
            // ShowFps();
            //   LogSystem.Debug("{0}fps:{1}",Application.targetFrameRate,m_FPS);
        }

        private void ShowFps() {
            m_FrameUpdate++;
            if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime) {
                m_FPS = m_FrameUpdate/(Time.realtimeSinceStartup - m_LastUpdateShowTime);
                m_FrameUpdate = 0;
                m_LastUpdateShowTime = Time.realtimeSinceStartup;
            }
        }

        internal void OnApplicationPause(bool isPause) {
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as LogicModule;
            if (logic != null) {
                if (isPause) {
                    _fps = Application.targetFrameRate;
                    Application.targetFrameRate = 1;
                    LogSystem.Debug("targetFrameRate:{0}Fps1:{1}", Application.targetFrameRate, m_FPS);
                    logic.PauseLogic();
                } else {
                    Application.targetFrameRate = _fps;
                    LogSystem.Debug("targetFrameRate:{0}Fps2:{1}", Application.targetFrameRate, m_FPS);
                    logic.ResumeLogic();
                }
            }
        }

        private bool _CreateComponets() {
            //TODO factory instead.
            var componentMgr = ComponentMgr.Instance;
            //gfx module
            var gfx = new GfxModule {Name = ComponentNames.ComGfx};
            var ret = componentMgr.AddComponent(gfx);
            //logic module.
            var logic = new LogicModule {Name = ComponentNames.ComLogic};
            ret &= componentMgr.AddComponent(logic);
            //resource module
            var resource = new ResourceMgr {Name = ComponentNames.ComResource};
            ret &= componentMgr.AddComponent(resource);
            //update module
            var update = new UpdateMgr {Name = ComponentNames.ComUpdate};
            ret &= componentMgr.AddComponent(update);
            //network module
            var network = new NetworkMgr {Name = ComponentNames.ComNetwork};
            network.RegistDispatcher(new HttpJsonServer(network));
            ret &= componentMgr.AddComponent(network);

            // businiess module
            //ui controller.
            var uiController = UiControllerMgr.Instance;
            uiController.Name = ComponentNames.ComUiController;
            ret &= componentMgr.AddComponent(uiController);
            //login module
            var login = new LoginMgr() {Name = ComponentNames.ComLogin};
            ret &= componentMgr.AddComponent(login);
            //user center
            var user = new UserCenter() {Name = ComponentNames.ComUserCenter};
            ret &= componentMgr.AddComponent(user);
            //design world
            var design = new DesignWorld() {Name = ComponentNames.ComDesignWorld};
            ret &= componentMgr.AddComponent(design);
            //order center
            var order = new OrderCenter() {Name = ComponentNames.ComOrderCenter};
            ret &= componentMgr.AddComponent(order);
            //input module
            //var input = new InputComponent() {Name = ComponentNames.ComInput};
            var input = InputComponent.Instance;
            input.Name = ComponentNames.ComInput;
            ret &= componentMgr.AddComponent(input);
            if (!ret) {
                LogSystem.Error("_CreateComponets failed.");
            }
            return ret;
        }

        internal void OnApplicationQuit() {
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as LogicModule;
            if (logic != null)
                logic.StopLogic();
        }

        private void SdkCall(string jsonStr) {
            var json = JsonMapper.ToObject(jsonStr);
            SDKContorl.Instance.SdkCallBack(json);
        }

        private void _Callback(string data) {
            ShareSDK.Instance._Callback(data);
        }
    }
}