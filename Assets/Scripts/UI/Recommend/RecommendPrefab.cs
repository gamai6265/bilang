using LitJson;

namespace Cloth3D.Ui {
    public class RecommendPrefab : ViewPresenter {
        public UITexture TexRecommend;
        public UILabel LblName;
        public UILabel LblPrice;

        public string atid;
        public JsonData ProductJson;

        public override void InitWindowData() {
        }
    }
}