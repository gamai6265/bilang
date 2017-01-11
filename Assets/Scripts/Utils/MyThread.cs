using System;
using System.Threading;

namespace Cloth3D.Comm {
    public delegate void MyThreadEventDelegate();

    public class MyThread {
        private int _actionNumPerTick = 100;

        private ActionQueue _actionQueue;
        private bool _bStop = true;

        private int _tickSleepTime = 10;
        public MyThreadEventDelegate OnQuitEvent;


        public MyThreadEventDelegate OnStartEvent;
        public MyThreadEventDelegate OnTickEvent;
        public MyThreadEventDelegate OnPauseTickEvent;

        public MyThread() {
            Pause = false;
            Thread = null;
            InitThread(new ActionQueue());
        }

        public MyThread(int tickSleepTime) {
            Pause = false;
            Thread = null;
            _tickSleepTime = tickSleepTime;
            InitThread(new ActionQueue());
        }

        public MyThread(int tickSleepTime, int actionNumPerTick) {
            Pause = false;
            Thread = null;
            _tickSleepTime = tickSleepTime;
            _actionNumPerTick = actionNumPerTick;
            InitThread(new ActionQueue());
        }

        public MyThread(ActionQueue asyncActionQueue) {
            Pause = false;
            Thread = null;
            InitThread(asyncActionQueue);
        }

        public MyThread(int tickSleepTime, ActionQueue asyncActionQueue) {
            Pause = false;
            Thread = null;
            _tickSleepTime = tickSleepTime;
            InitThread(asyncActionQueue);
        }

        public MyThread(int tickSleepTime, int actionNumPerTick, ActionQueue asyncActionQueue) {
            Pause = false;
            Thread = null;
            _tickSleepTime = tickSleepTime;
            _actionNumPerTick = actionNumPerTick;
            InitThread(asyncActionQueue);
        }

        public int TickSleepTime {
            get { return _tickSleepTime; }
            set { _tickSleepTime = value; }
        }

        public int ActionNumPerTick {
            get { return _actionNumPerTick; }
            set { _actionNumPerTick = value; }
        }

        public int CurActionNum {
            get { return _actionQueue.CurActionNum; }
        }

        public Thread Thread { get; private set; }

        public bool Pause { get; set; }

        public IActionQueue GetActionQueue() {
            return _actionQueue;
        }

        public void DebugPoolCount(MyAction<string> output) {
            _actionQueue.DebugPoolCount(output);
        }

        public void Start() {
            _bStop = false;
            Thread.Start();
        }

        public void Stop() {
            _bStop = true;
            Thread.Join();
        }

        protected virtual void OnStart() {
        }

        protected virtual void OnTick() {
        }

        protected virtual void OnQuit() {
        }

        private void InitThread(ActionQueue actionQueue) {
            Thread = new Thread(Loop);
            _actionQueue = actionQueue;
        }

        private void Loop() {
            try {
                if (OnStartEvent != null) 
                    OnStartEvent();
                else 
                    OnStart();
                
                while (!_bStop) {
                    if (Pause) {
                        if (null != OnPauseTickEvent) 
                            OnPauseTickEvent();
                        Thread.Sleep(_tickSleepTime);
                        continue;
                    }
                    try {
                        if (OnTickEvent != null) 
                            OnTickEvent();
                        else 
                            OnTick();
                        _actionQueue.HandleActions(_actionNumPerTick);
                    } catch (Exception ex) {
                        LogSystem.Error("MyThread.Tick throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
                    }
                    Thread.Sleep(_tickSleepTime);
                }
                if (OnQuitEvent != null)
                    OnQuitEvent();
                else 
                    OnQuit();
                
            } catch (Exception ex) {
                LogSystem.Error("MyThread.Loop throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
    }
}