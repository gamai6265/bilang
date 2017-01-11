using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cloth3D.Comm;
using Cloth3D.Data;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Logic {
    public class LogicModule : ILogicModule {
        private static string _stateReady = "StateReady";
        private static string _stateLoadData = "StateLoadData";
        private static string _statePlay = "StatePlay";
        private static string _stateUpdate = "StateUpdate";
        //private readonly MyThread _logicThread = new MyThread();
        private Fsm _fsm;
        private long _lastLogicTickTime;
        private long _lastLogTime;
        private bool _isStart;
        private bool _isPause;


        private ConfigFile _config;

        public LoadDataDelegate OnLoadDataDone { get; set; }

        public StageEventDelegate OnBeforeStageEnter { get; set; }
        public StageEventDelegate OnAfterStageEnter { get; set; }
        public StageEventDelegate OnBeforeStageLeave { get; set; }

        public string Name { get; set; }

        public ConfigFile Config {
            get {
                return _config;
            }
            set { _config = value; }
        }

        public void Tick() {
            if (_isStart) {
                if (!_isPause) {
                    _LogicTick();
                }
            }
        }

        public bool Init() {
            //_logicThread.OnTickEvent += _LogicTick;
            _SetupFileProxy();
            CodeSetProvider.Instance.Init();
            _InitLogicState();
            return true;
        }

        public void StartLogic() {
            //_logicThread.Start();
            _isStart = true;
        }

        public void PauseLogic() {
            //_logicThread.Pause = true;
            _isPause = true;
        }

        public void ResumeLogic() {
            //_logicThread.Pause = false;
            _isPause = false;
        }

        public void StopLogic() {
            //_logicThread.Stop();
            _isStart = false;
        }

        public void SwitchStage(StageType stage) {
            if (_fsm.CurState.Name == _statePlay) {
                var statePlay = _fsm.CurState as StatePlay;
                if (statePlay != null)
                    statePlay.SwitchStage(stage);
            }
        }

        public StageType GetCurrentStage() {
            var ret = StageType.StageNone;
            if (_fsm.CurState.Name == _statePlay) {
                var statePlay = _fsm.CurState as StatePlay;
                if (statePlay != null) {
                    ret = statePlay.GetCurrentStage();
                } else {
                   LogSystem.Debug("logic.GetCurrentStage state is not in StatePlay");
                }
            }
            return ret;
        }

        private void _SetupFileProxy() {
            string key = "防君子不防小人";
            byte[] xor = Encoding.UTF8.GetBytes(key);

            FileProxy.RegisterReadFileHandler((string filePath) => {
                byte[] buffer = null;
                try {
                    buffer = File.ReadAllBytes(filePath);
#if !DEBUG
                    //if (filePath.EndsWith(".txt")) {
                    //Helper.Xor(buffer, xor);
//                    }
#endif
                } catch (Exception e) {
                  //LogSystem.Error("Exception:{0}\n{1}", e.Message, e.StackTrace);
                    return null;
                }
                return buffer;
            });
        }
        private void _InitLogicState() {
            _fsm = new Fsm("FsmLogic");
            /* // add event show cur state changed info
            _fsm.CurStateChangedEvent +=
                (CurStateChangedEventHandler)
                    (sender => {
                        //Console.WriteLine("state changed, last : {0}, cur : {1}", fsm.LastState, fsm.CurState);
                    });
            // add event show params changed info
            _fsm.ParamsChangedEvent += (ParamsChangedEventHandler) ((sender, paramChangedArgs) => {
                //Console.WriteLine(string.Format(
                //    "param:{1}, src:{2}, to: {3}",
                //    fsm, paramChangedArgs.CurValue.Name, paramChangedArgs.LastValue.Value, paramChangedArgs.CurValue.Value));
            });*/

            //_fsm.Blackboard.AddData("LogicModule", this);

            // add some states
            _fsm.AddState(new StateLoadData(_stateLoadData));
            _fsm.AddState(new StatePlay(_statePlay));
            _fsm.AddState(new StateUpdate(_stateUpdate));
            _fsm.AddState(new StateReady(_stateReady));
            //_fsm.AnyState = new StateReady("StateReady");
            _fsm.CurState = _fsm.GetState(_stateReady);

            //start state
            _fsm.AddParam(new ParamBoolean("startUpdate") {Value = false});
            Fsm.AddTransitionTo(_fsm, _stateReady, _stateUpdate, new List<Condition>() {new Condition("startUpdate", true, ConditionType.Equals)});
            //update state
            _fsm.AddParam(new ParamBoolean("doneUpdate") {Value = false});
            Fsm.AddTransitionTo(_fsm, _stateUpdate, _stateLoadData, new List<Condition>() {new Condition("doneUpdate", true, ConditionType.Equals)});
            // loading  state
            _fsm.AddParam(new ParamBoolean("doneLoadData") {Value = false});
            Fsm.AddTransitionTo(_fsm, _stateLoadData, _statePlay, new List<Condition>() {new Condition("doneLoadData", true, ConditionType.Equals)});
            // normal state
            _fsm.AddParam(new ParamBoolean("startLoading") {Value = false});
            Fsm.AddTransitionTo(_fsm, _statePlay, _stateLoadData, new List<Condition>() {new Condition("startLoading", true, ConditionType.Equals)});
        }

        private void _LogicTick() {
            try {
                TimeUtility.SampleClientTick();

                var curTime = TimeUtility.GetLocalMilliseconds();
                if (_lastLogTime + 10000 < curTime) {
                    _lastLogTime = curTime;
                    //_logicThread.DebugPoolCount(msg => {
                        //GfxModule.GfxLog("LogicActionQueue {0}", msg);
                    //});
                }
                //limit frame rate with 10
                curTime = TimeUtility.GetLocalMilliseconds();
                if (_lastLogicTickTime + 40 <= curTime) {
                    _lastLogicTickTime = curTime;
                } else {
                    return;
                }
                if (null != _fsm) {
                    _fsm.Update();
                }
            } catch (Exception ex) {
               LogSystem.Error("GameLogic.Tick throw Exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
    }
}