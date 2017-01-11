using LitJson;
using UnityEngine;

/// <summary>
///     纽扣按钮
/// </summary>
public class ButtonItem : MonoBehaviour {
    public string buttonCode; //钮扣编号
    public string buttonColor; //钮扣颜色代码
    public string buttonImage; //钮扣图片名称
    public string buttonPrice; //钮扣价格
    public string colorValue; //颜色值
    public UILabel lbl_name;
    public UITexture tex;

    private void OnClick() {
        //ModelView.Instance.ChangeButtonFabric(BeJson());
        //ImputedPrice.Instance.ImputedShirtPrice();
    }

    private JsonData BeJson() {
        var data = new JsonData();
        data["buttonCode"] = buttonCode;
        data["buttonColor"] = buttonColor;
        data["buttonImage"] = buttonImage;
        data["buttonPrice"] = buttonPrice;
        data["colorValue"] = colorValue;
        return data;
    }
}