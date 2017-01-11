using System;
using System.Diagnostics;
using Cloth3D.Interfaces;
using UnityEngine;

namespace Cloth3D.Input {
    public class InputComponent : UnitySingleton<InputComponent>, IInput {
        public string Name { get; set; }

        public event MyAction OnBackEvent;
        private event MyAction< string, float, float> OnSwipeEvent;
        private event MyAction OnLongPressEvent;
        private event MyAction OnDoubleTapEvent;
        private event MyAction<Vector2> OnTapEvent;
        private event MyAction< float> OnTwistEvent;
        private event MyAction< float> OnPinchEvent;
        private event MyAction<Vector2,Vector2,Vector2> OnDragEvent;

        private bool _rotating = false;
        private bool _pinching = false;
        private FingerGestures _fingerGestures;
        private TapRecognizer _tapRecognizer;
        private TapRecognizer _doubleTapRecognizer;
        private int _dragFingerIndex = -1;

        bool Rotating {
            get { return _rotating; }
            set {
                if (_rotating != value) {
                    _rotating = value;
                }
            }
        }

        bool Pinching {
            get { return _pinching; }
            set {
                if (_pinching != value) {
                    _pinching = value;
                }
            }
        }

        public void Tick() {
            if (Application.platform == RuntimePlatform.Android && (UnityEngine.Input.GetKeyDown(KeyCode.Escape))) {
                if (OnBackEvent != null) {
                    OnBackEvent();
                }
            }
        }

        public bool Init() {
            _fingerGestures = GetComponent<FingerGestures>();
            var taps = GetComponentsInChildren<TapRecognizer>(true);
            for (int i = 0; i < taps.Length; i++) {
                var tap = taps[i];
                if (tap.EventMessageName == "OnDoubleTap")
                    _doubleTapRecognizer = tap;
                else if (tap.EventMessageName == "OnTap")
                    _tapRecognizer = tap;
            }
            return true;
        }

        private void FingerGesturesIsDisEnable() {
            if (OnSwipeEvent == null && OnDoubleTapEvent == null && OnLongPressEvent == null && OnTwistEvent == null &&
                OnPinchEvent == null && OnDragEvent==null && OnTapEvent==null) {
                _fingerGestures.enabled = false;
            } else if (!_fingerGestures.enabled) {
                _fingerGestures.enabled = true;
            }
        }

        public void FilterNguiEvent(bool isFilter) {
            if (isFilter) {
                FingerGestures.GlobalTouchFilter = ShouldProcessTouch;
            } else {
                FingerGestures.GlobalTouchFilter = null;
            }
        }

        public void SwitchSwipe(bool tof, MyAction< string, float, float> callback) {
            if (callback != null) {
                if (tof) {
                    OnSwipeEvent += callback;
                    GetComponent<SwipeRecognizer>().enabled = true;
                } else {
                    OnSwipeEvent -= callback;
                    if (OnSwipeEvent == null) {
                        GetComponent<SwipeRecognizer>().enabled = false;
                    }
                }
                FingerGesturesIsDisEnable();
            }
        }

        public void SwitchLongPress(bool tof, MyAction callback) {
            if (callback != null) {
                if (tof) {
                    OnLongPressEvent += callback;
                    GetComponent<LongPressRecognizer>().enabled = true;
                } else {
                    OnLongPressEvent -= callback;
                    if (OnLongPressEvent==null) {
                        GetComponent<LongPressRecognizer>().enabled = false;
                    }
                }
                FingerGesturesIsDisEnable();
            }
        }

        public void SwitchTap(bool tof, MyAction<Vector2> callback) {
            if (callback != null) {
                if (tof) {
                    OnTapEvent += callback;
                    _tapRecognizer.enabled = true;
                } else {
                    OnTapEvent -= callback;
                    if (OnTapEvent == null)
                        _tapRecognizer.enabled = false;
                }
                FingerGesturesIsDisEnable();
            }
        }

        public void SwitchDoubleTap(bool tof, MyAction callback) {
            if (callback != null) {
                if (tof) {
                    OnDoubleTapEvent += callback;
                    _doubleTapRecognizer.enabled = true;
                } else {
                    OnDoubleTapEvent -= callback;
                    if (OnDoubleTapEvent == null)
                        _doubleTapRecognizer.enabled = false;
                }
                FingerGesturesIsDisEnable();
            }
        }

        public void SwitchTwist(bool tof, MyAction< float> callback) {
            if (callback != null) {
                if (tof) {
                    OnTwistEvent += callback;
                    GetComponent<TwistRecognizer>().enabled = true;
                } else {
                    OnTwistEvent -= callback;
                    if (OnTwistEvent == null)
                        GetComponent<TwistRecognizer>().enabled = false;
                }
                FingerGesturesIsDisEnable();
            }
        }

        public void SwitchPinch(bool tof, MyAction< float> callback) {
            if (callback != null) {
                if (tof) {
                    OnPinchEvent += callback;
                    GetComponent<PinchRecognizer>().enabled = true;
                } else {
                    OnPinchEvent -= callback;
                    if (OnPinchEvent == null)
                        GetComponent<PinchRecognizer>().enabled = false;

                }
                FingerGesturesIsDisEnable();
            }
        }

        public void SwitchDrag(bool tof, MyAction<Vector2, Vector2, Vector2> callback) {
            if (callback != null) {
                if (tof) {
                    OnDragEvent += callback;
                    GetComponent<DragRecognizer>().enabled = true;
                } else {
                    OnDragEvent -= callback;
                    if(OnDragEvent==null)
                        GetComponent<DragRecognizer>().enabled = false;
                }
                FingerGesturesIsDisEnable();
            }
        }

        bool ShouldProcessTouch(int fingerIndex, Vector2 position) {
            Ray ray = UICamera.mainCamera.ScreenPointToRay(position);
            bool hasTouchUi = Physics.Raycast(ray, float.PositiveInfinity, 1 << LayerMask.NameToLayer(Constants.UI));
            return !hasTouchUi;
        }
        private void OnSwipe(SwipeGesture gesture) {
            if (OnSwipeEvent != null) {
                OnSwipeEvent(gesture.Direction.ToString(), gesture.Velocity, gesture.Move.magnitude);
            }
        }

        private void OnLongPress(LongPressGesture gesture) {
            if (OnLongPressEvent != null)
                OnLongPressEvent();
        }

        private void OnTap(TapGesture gesture) {
            if (OnTapEvent != null)
                OnTapEvent(gesture.Position);
        }
        private void OnDoubleTap(TapGesture gesture) {
            if (OnDoubleTapEvent != null)
                OnDoubleTapEvent();
        }

        //旋转
        private void OnTwist(TwistGesture gesture) {
            if (gesture.Phase == ContinuousGesturePhase.Started) {
                Rotating = true;
            } else if (gesture.Phase == ContinuousGesturePhase.Updated) {
                if (Rotating) {
                    if (OnTwistEvent != null)
                        OnTwistEvent(gesture.DeltaRotation);
                }
            } else {
                if (Rotating) {
                    Rotating = false;
                }
            }
        }

        //缩放
        private void OnPinch(PinchGesture gesture) {
            if (gesture.Phase == ContinuousGesturePhase.Started) {
                Pinching = true;
            } else if (gesture.Phase == ContinuousGesturePhase.Updated) {
                if (Pinching) {
                    if (OnPinchEvent != null) {
                        OnPinchEvent(gesture.Delta.Centimeters());
                    }
                }
            } else {
                if (Pinching) {
                    Pinching = false;
                }
            }
        }
        
        void OnDrag(DragGesture gesture) {
            FingerGestures.Finger finger = gesture.Fingers[0];
            if (gesture.Phase == ContinuousGesturePhase.Started) {
                _dragFingerIndex = finger.Index;
            } else if (finger.Index == _dragFingerIndex){
                if (gesture.Phase == ContinuousGesturePhase.Updated) {
                    if (OnDragEvent != null)
                        OnDragEvent(gesture.StartPosition, gesture.Position, gesture.DeltaMove);
                } else {
                    _dragFingerIndex = -1;
                }
            }
        }
    }
}