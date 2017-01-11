using System;
using UnityEngine;

namespace Cloth3D.Design {
    class HotPoint :MonoBehaviour {
        private bool _init;
        private Vector3 _forward;
        private Quaternion _direction;
        private bool _isVisible;
        private TweenScale _tweenScale;
        private static Vector3 _from;
        private static Vector3 _to;
        private Ray _ray;
        public GameObject MeshObj;

        public int PartId { get; private set; }
        public int ModelId;

        void Awake() {
            _init = false;
            //gameObject.SetActive(false);
            _isVisible = false;
            _tweenScale = MeshObj.GetComponent<TweenScale>();
            _from = _tweenScale.from;
            _to = _tweenScale.to;
            _ray = new Ray();
        }

        public void DrawDebug() {
            var dir = transform.rotation * Vector3.forward;
            Debug.DrawLine(transform.position, dir + transform.position);
            Debug.DrawLine(transform.position, transform.position+_forward, Color.green);
        }

        public void UpdateOrientation() {
            if (_init) { 
                //transform.rotation = Camera.main.transform.rotation * _direction;
                var camTrans = Camera.main.transform;
                bool preVisible = _isVisible;
                _ray.origin = transform.position;
                _ray.direction = camTrans.position - transform.position;
                var hit = Physics.Raycast(_ray, float.PositiveInfinity);

                var rot = Quaternion.FromToRotation(_forward, camTrans.forward);
                transform.rotation = rot * _direction;

                var angle = Vector3.Angle(transform.forward, _forward);
                if (!hit) {
                    if (Math.Abs(angle) <= 30) {
                        _isVisible = true;
                        
                    } else {
                        _isVisible = false;
                    }
                } else {
                    _isVisible = false;
                }

                if (preVisible != _isVisible) {
                    if (_isVisible) {
                        MeshObj.transform.localScale = _from;
                        TweenScale.Begin(MeshObj.gameObject, _tweenScale.duration, _to);
                    } else {
                        MeshObj.transform.localScale = _to;
                        TweenScale.Begin(MeshObj.gameObject, _tweenScale.duration, _from);
                    }
                }
            }
        }

        public void UpdateScale(float scale) {
            transform.localScale = new Vector3(scale, scale, 1);
        }

        public void Hide() {
            _isVisible = false;
            MeshObj.transform.localScale = Vector2.zero;
        }

        public void Init(int partId, Vector3 pos, Vector3 rot,int modelId) {
            PartId = partId;
            transform.localPosition = pos;
            transform.Rotate(rot);
            _forward = transform.forward;
            ModelId = modelId;
            _direction = new Quaternion {
                x = transform.localRotation.x,
                y = transform.localRotation.y,
                z = transform.localRotation.z,
                w = transform.localRotation.w
            };
            _init = true;
        }

        public Vector3 GetOrignalDir() {
            return _forward;
        }
    }
}
