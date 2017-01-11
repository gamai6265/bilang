using System;
using System.Collections;
using System.Collections.Generic;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Ui {
    public class UiControllerMgr : Singleton<UiControllerMgr>, IUiControllerMgr {
        private Dictionary<WindowId, IController> _dicWin2Ctrls = new Dictionary<WindowId, IController>();
        private Dictionary<IController, IController> _dicCtrls = new Dictionary<IController, IController>();
        public string Name { get; set; }
        public void Tick() {
            foreach (var item in _dicCtrls) {
                item.Value.Update();
            }
        }

        public bool Init() {
            //TODO danie  监听 更新，加载事件
            //TODO danie UiManager.Instance.Init();
            var updateMgr = ComponentMgr.Instance.FindComponent(ComponentNames.ComUpdate) as IUpdateMgr;
            if (updateMgr == null) {
                return false;
            }
            updateMgr.OnUpdateBegin = OnUpdateBegin;
            var logic = ComponentMgr.Instance.FindComponent(ComponentNames.ComLogic) as ILogicModule;
            if (logic == null) {
                return false;
            }
            
            logic.OnLoadDataDone = OnLoadDataDone;
            logic.OnBeforeStageEnter = OnBeforeStageEnter;
            logic.OnAfterStageEnter = OnAfterStageEnter;
            logic.OnBeforeStageLeave = OnBeforeStageLeave;

            //            CoroutineMgr.Instance.StartCoroutine(VersionCheck(logic));

            SetupWinInfo();
            bool isCtrInitOk = true;
            foreach (var item in _dicCtrls) {
                isCtrInitOk &= item.Value.Init();
            }
            var input = ComponentMgr.Instance.FindComponent(ComponentNames.ComInput) as IInput;
            if (input == null) {
                return false;
            }
            input.OnBackEvent+=OnBackEvent;
            return isCtrInitOk;
        }


        private IEnumerator VersionCheck(ILogicModule logic) {
//            var www = new WWW(logic.Config.GetSetting("Server", "versionUrl"));
            var www = new WWW("file:///E:/test.txt");
            yield return www;
            if (www.error != null) {
                LogSystem.Error("VersionCheck Error");
            }
            else {
                if (www.isDone) {
                    var versionData = www.text.Split(',');
                    var serverVersion = versionData[0].Split('=')[1];//TODO
                    var androidUrl = versionData[1].Split('=')[1];
                    var iosUrl = versionData[2].Split('=')[1];
                    var persistDir = GlobalVariables.PersistentDataPath + "/" + FilePathDefine.ResSheetCacheDir;
                    logic.Config = new ConfigFile(persistDir + FilePathDefine.IniConfig);
                    var localVersion = logic.Config.GetSetting("Server", "version");
                    if (localVersion != serverVersion) {
                        ShowMessageBox(
                            "",
                            "请更新到最新版本",
                            "取消",
                            () => {
                            },
                            "确认",
                            () => {
                                #if UNITY_ANDROID
                                    Application.OpenURL(androidUrl);
                                #endif

                                #if UNITY_IPHONE
                                    Application.OpenURL(iosUrl);
                                #endif
                            }
                            );
                        Application.Quit();
                    }
                }
            }
            www.Dispose();
        }

        //暂时一个控制类对应一个视口类，如果有需要再变更
        public IController GetController(WindowId id) {
            if (_dicWin2Ctrls.ContainsKey(id)) {
                return _dicWin2Ctrls[id];
            }
            return null;
        }
        public void ShowTips(string msg) {
            var msgWindow = UiManager.Instance.GetWindow<TipsViewPresenter>(WindowId.Tips);
            if (msgWindow != null) {
                msgWindow.SetMsg(msg);
                GetController(WindowId.Tips).Show();
            }
        }

        private void OnBackEvent() {
            //step1. loop those visible windows by zorder
            //step2 if window return ture ,then broken the loop, else continue to process.
            var lstWins = UiManager.Instance.GetSortedVisibleWindows();
            for (int i = 0; i < lstWins.Count; i++) {
                var item = lstWins[lstWins.Count - 1 - i];
                var ret = GetController(item.Key).OnBack(i);
                if(ret)
                    break;
            }
        }

        public void ShowLodingTips(bool msg) {
            var msgWindow = UiManager.Instance.GetWindow<LoadingTipsViewPresenter>(WindowId.LoadingTips);
            if (msgWindow != null) {
                if (msg) {
                    GetController(WindowId.LoadingTips).Show();
                } else {
                    UiManager.Instance.HideWindow(WindowId.LoadingTips);
                }
            }
        }

        public void ShowMessageBox(string title, string msg, string leftStr, MyAction leftCallBack, string rightStr, MyAction rightCallBack) {
            var msgCtr = (MessageBoxCtr)GetController(WindowId.MessageBox);
            if(msgCtr!=null)
                msgCtr.ShowMessageBox(title, msg, leftStr, leftCallBack, rightStr, rightCallBack);
        }
        private void SetupWinInfo() {
            List<IController> lstCtrls = new List<IController> {
                new AddressCtr(),
                new AppointmentCtr(),
                new AgreementCtr(),
                new BrandStoryCtr(),
                new ButtonCtr(),
                new ClothesDetailsCtr(),
                new CollectCtr(),
                new DesignCtr(),
                new EditeAddressCtr(),
                new FabricCtr(),
                new ForgetPassWordCtr(),
                new HomeCtr(),
                new HomeBtnsCtr(),
                new HistorySizeCtr(),
                new EditorSizeCtr(),
                new LoginCtr(),
                new MeasurementCtr(),
                new MeasuringSizeCtr(),
                new MessageBoxCtr(),
                new MyWalletWebViewCtr(),
                new MySizeCtr(),
                new OrderConfirmCtr(),
                new OrderEvaluateCtr(),
                new OrdersCtr(),
                new OrdersDetailsCtr(),
                //new PartPointsCtr(),
                new PartCtr(),
                new InstructionCtr(),
                new PaymentCtr(),
                new PayResultCtr(),
                new PersonalCenterCtr(),
                new RecommendCtr(),
                new ReferenceSizeCtr(),
                new RegistrVerificationCtr(),
                new ShopCarCtr(),
                new SetUserInfoCtr(),
                new SaveStyleCtr(),
                new TrendyCtr(),
                new TrendDetailsCtr(),
                new TipsCtr(),
                new LoadingTipsCtr(),
                new UserInfoCtr(),
                new WebViewCtr(),
                new ChooseSizeTypeCtr(),
                new ChooseModelsCtr(),
                new CouponCtr(),
                new DesignersRecommendCtr(),
                new DesignersRecommendPreviewCtr(),
                new DesignersRecommendSelectCtr(),
            };

            //            { WindowId.WebView,"WebView"},
            //            { WindowId.PanelInstruction,"PanelInstruction" },

            //regsite window info  & controlers info
            var uiManager = UiManager.Instance;
            for (int i = 0; i < lstCtrls.Count; i++) {
                var lstItem = lstCtrls[i].GetWinLst();
                if (null != lstItem) {
                    for (int j = 0; j < lstItem.Count; j++) {
                        uiManager.AddWinInfo(lstItem[j].Key, lstItem[j].Value);
                        if (!_dicWin2Ctrls.ContainsKey(lstItem[j].Key)) {
                            _dicWin2Ctrls.Add(lstItem[j].Key, lstCtrls[i]);
                            if(!_dicCtrls.ContainsKey(lstCtrls[i]))
                                _dicCtrls.Add(lstCtrls[i], lstCtrls[i]);
                        } else {
                            LogSystem.Error("Error UiControllerMgr Dictionary has the same key.");
                        }
                    }
                }
            }
        }

        private void OnUpdateBegin() {
            // TODO danie 
            return;
            LogSystem.Debug(" show extract ui.");
        }

        private void OnLoadDataDone() {
            //api version here
            //UiManager.Instance.LoadSceneWindows((int)StageType.StageGeneral);
            //UiManager.Instance.LoadSceneWindows((int)StageType.StageEmpty);
            //UiManager.Instance.LoadSceneWindows((int)StageType.StageDesign);
        }

        private void OnAfterStageEnter(StageType stage) {
            switch (stage) {
                case StageType.StageEmpty:
                    UICamera.mainCamera.clearFlags = CameraClearFlags.SolidColor;
                    UICamera.mainCamera.backgroundColor = Color.white;
                    EventCenter.Instance.Broadcast(EventId.EnterStageEmpty);
                    break;
                case StageType.StageDesign:
                    UICamera.mainCamera.clearFlags = CameraClearFlags.Depth;
                    EventCenter.Instance.Broadcast(EventId.EnterStageDesign);
                    break;
            }
        }

        private void OnBeforeStageEnter(StageType stage) {
            
        }
        private void OnBeforeStageLeave(StageType stage) {
            switch (stage) {
                case StageType.StageEmpty:
                    EventCenter.Instance.Broadcast(EventId.LeaveStageEmpty);
                    break;
                case StageType.StageDesign:
                    EventCenter.Instance.Broadcast(EventId.LeaveStageDesign);
                    break;
            }
        }
    }
}