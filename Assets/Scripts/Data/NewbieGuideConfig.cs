namespace Cloth3D.Data {
    public class NewbieGuideConfig : IData {
        public int Id;
        public string ChildName = "";
        public int ChildNumber;
        public float[] LocalPosition = {0.0f, 0.0f, 0.0f};
        public string MyPath = "";
        public string ParentPath = "";
        public int PreviousGuideId;
        public float[] RotateThree = {0.0f, 0.0f, 0.0f};
        public float[] Scale = {1.0f, 1.0f, 1.0f};
        public int Type;
        public bool Visible;

        public bool CollectDataFromDbc(DbcRow node) {
            Id = DbcUtils.ExtractNumeric(node, "Id", 0);
            ParentPath = DbcUtils.ExtractString(node, "ParentPath", "");
            MyPath = DbcUtils.ExtractString(node, "MyPath", "");
            var list = DbcUtils.ExtractNumericList<float>(node, "Rotate", 0);
            var num = list.Count;
            if (num > 0) RotateThree[0] = list[0];
            if (num > 1) RotateThree[1] = list[1];
            if (num > 2) RotateThree[2] = list[2];

            Visible = DbcUtils.ExtractNumeric(node, "Visible", false);
            Type = DbcUtils.ExtractNumeric(node, "Type", 0);
            PreviousGuideId = DbcUtils.ExtractNumeric(node, "PreviousGuideId", 0);

            list = DbcUtils.ExtractNumericList<float>(node, "LocalPosition", 0);
            num = list.Count;
            if (num > 0) LocalPosition[0] = list[0];
            if (num > 1) LocalPosition[1] = list[1];
            if (num > 2) LocalPosition[2] = list[2];

            ChildNumber = DbcUtils.ExtractNumeric(node, "ChildNumber", 0);
            ChildName = DbcUtils.ExtractString(node, "ChildName", "");

            list = DbcUtils.ExtractNumericList<float>(node, "Scale", 0);
            num = list.Count;
            if (num > 0) Scale[0] = list[0];
            if (num > 1) Scale[1] = list[1];
            if (num > 2) Scale[2] = list[2];
            return true;
        }


        public int GetId() {
            return Id;
        }
    }

    public class NewbieGuideProvider : SingletonDataMgr<NewbieGuideConfig> {
    }
}