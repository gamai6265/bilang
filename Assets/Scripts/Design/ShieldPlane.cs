using System;
using UnityEngine;

namespace Cloth3D.Design {
    public class ShieldPlane : MonoBehaviour {//TODO danie 
        private Quaternion _direction;
        void Start() {
            _direction = new Quaternion {
                x = transform.localRotation.x,
                y = transform.localRotation.y,
                z = transform.localRotation.z,
                w = transform.localRotation.w
            };
        }

        void Update() {
            transform.rotation = Camera.main.transform.rotation * _direction;
        }
    }
}
