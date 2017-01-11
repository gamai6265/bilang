using UnityEngine;

namespace Cloth3D.Interfaces {
    interface IInput:IComponent {
        event MyAction OnBackEvent;
        void FilterNguiEvent(bool isFilter);
        void SwitchSwipe(bool tof, MyAction<string, float, float> callback);
        void SwitchLongPress(bool tof, MyAction callback);
        void SwitchTap(bool tof, MyAction<Vector2> callback);
        void SwitchDoubleTap(bool tof, MyAction callback);
        void SwitchTwist(bool tof, MyAction<float> callback);
        void SwitchPinch(bool tof, MyAction<float> callback);
        void SwitchDrag(bool tof, MyAction<Vector2, Vector2, Vector2> callback);
    }
}
