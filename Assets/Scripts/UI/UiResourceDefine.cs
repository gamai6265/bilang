using System;

namespace Cloth3D.Ui{
    public enum WindowId {
        Address,
        Appointment,
        BrandStory,
        ClothesDetails,
        Collect,
        EditeAddress,
        Home,
        HomeBtns,
        HistorySize,
        Measurement,
        MeasuringSize,
        EditorSize,
        MessageBox,
        MySize,
        OrderConfirm,
        OrderEvaluate,
        Orders,
        OrdersDetails,
        Payment,
        PayResult,
        PersonalCenter,
        Recommend,
        ReferenceSize,
        ShopCar,
        Trendy,
        TrendyDetails,
        UserInfo,
        WebView,
        MyWalletWebView,
        Design,
        Button,
        Fabric,
        PanelInstruction,
        Masks,
        //PartPoints,
        Parts,
        SaveStyle,
        Tips,
        LoadingTips,
        Login,
        Agreement,
        ForgetPassWord,
        RegistrVerification,
        SetUserInfo,
        ChooseSizeType,
        ChooseModels,
        Coupon,
        DesignersRecommend,
        DesignersRecommendPreview,
        DesignersRecommendSelect,
    }
    [Flags]
    public enum UiWindowType {
        Normal=1<<0,    // 可推出界面(UIMainMenu,UIRank等)
        Fixed=1<<1,     // 固定窗口(UITopBar等)
        PopUp=1<<2,     // 模式窗口
    }

    public enum UiWindowColliderMode {
        None,      // 显示该界面不包含碰撞背景
        Normal,    // 碰撞透明背景
        WithBg,    // 碰撞非透明背景
    }
    public enum Zorder {
        TopMost,
        BottomMost
    }
    public class WindowData {
        public UiWindowColliderMode ColliderMode = UiWindowColliderMode.None;

        // 是否是导航起始窗口(到该界面需要重置导航信息)
        public UiWindowType WindowType = UiWindowType.Normal;
    }
}
