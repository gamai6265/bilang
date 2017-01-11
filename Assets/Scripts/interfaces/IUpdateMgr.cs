namespace Cloth3D.Interfaces {
    public delegate void UpdateBeginDelegate();

    internal interface IUpdateMgr : IComponent {
        UpdateBeginDelegate OnUpdateBegin { get; set; }
    }
}