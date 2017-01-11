using UnityEngine;

namespace Cloth3D.Design {
    internal class CameraMgr : Singleton<CameraMgr> {
        
        public static void RotateAroundAxies(Vector3 center, float angle, int direction) {
            var trans = Camera.main.transform;
            var dir = trans.position - center;
            Vector3 axies;
            //var rotationx = trans.localEulerAngles.x;
            if (direction == 0) {
                axies = Vector3.up;
            } else {
                axies = trans.right;
            }
            var quaterRot = Quaternion.AngleAxis(angle, axies);
            var targerVec = quaterRot*dir;
            trans.position = center + targerVec;
            var upDir = quaterRot*trans.up;
            trans.LookAt(center, upDir);
        }
    }
}