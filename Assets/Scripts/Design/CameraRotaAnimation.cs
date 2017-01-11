using UnityEngine;

namespace Cloth3D.Design {
    public class CameraRotaAnimation {
        public AnimationCurve pitchCurve;
        public AnimationCurve distanceCurve;

        private class TransInfo {
            public Vector3 Position;
            public Quaternion Rotation;
        }

        private TransInfo _startPosition;
        private TransInfo _endPosition;
        public float _duration = 0.35f;
        private float _elapse;
        private bool _isPlaying;
        private MyAction _callback = null;

        public void PlayCameraAnimation(Vector3 startPosition, Quaternion startRotation, Vector3 endPosition,
            Quaternion endRotation, MyAction callback=null) {
            _startPosition = new TransInfo() {
                Position = startPosition,
                Rotation = startRotation
            };
            _endPosition = new TransInfo() {
                Position = endPosition,
                Rotation = endRotation
            };
            _elapse = 0;
            _isPlaying = true;

            pitchCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
            Keyframe[] ks = new Keyframe[2];
            ks[0] = new Keyframe(0, 0);
            ks[0].outTangent = 0;

            ks[1] = new Keyframe(1.0f, 1.0f);
            ks[1].inTangent = 90;
            distanceCurve = new AnimationCurve(ks);
            _callback = callback;
        }

        public void Update() {
            if (_isPlaying) {
                _elapse += Time.deltaTime; //TODO scaletime
                float zoom;
                if (_elapse < _duration) {
                    zoom = _elapse / _duration;
                } else {
                    _isPlaying = false;
                    zoom = 1;
                }
                float t = pitchCurve.Evaluate(zoom);
                Quaternion targetRot = Quaternion.Slerp(_startPosition.Rotation, _endPosition.Rotation, t);
                Camera.main.transform.rotation = targetRot;
                var targetPos = _startPosition.Position + (_endPosition.Position - _startPosition.Position) * t;
                Camera.main.transform.position = targetPos;
            }else if (_callback != null) {
                _callback();
                _callback = null;
            }
        }

        public bool IsPlaying() {
            return _isPlaying;
        }
    }
}
