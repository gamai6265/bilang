using Cloth3D.Comm;

namespace Cloth3D.Interfaces {
    public enum StageType {
        StageNone,
        StageEmpty,
        StageDesign
    }

    public delegate void StageEventDelegate(StageType stage);

    public delegate void LoadDataDelegate();

    public interface ILogicModule : IComponent {
        void StartLogic();
        void PauseLogic();
        void SwitchStage(StageType stage);
        StageType GetCurrentStage();
        StageEventDelegate OnBeforeStageEnter { get; set; }
        StageEventDelegate OnAfterStageEnter { get; set; }
        StageEventDelegate OnBeforeStageLeave { get; set; }
        LoadDataDelegate OnLoadDataDone { get; set; }
        ConfigFile Config { get; set; }
    }
}