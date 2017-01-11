using System;
using Cloth3D.Interfaces;

namespace Cloth3D.Update {
    class UpdateMgr : IUpdateMgr{
        public string Name { get; set; }

        public void Tick() {
            //throw new NotImplementedException();
        }

        public bool Init() {
            //throw new NotImplementedException();
            return true;
        }
        /*
         * TODO 
         * 1.更新失败事件
         * 2.更新完成事件
         * 3. 中断更新  
         * 4. 更新进度事件  
         * 5. 设置更新提示  
         * 6. 开始更新    
         * 7. 清空？不需要！自己去监听事件
         * */
        public UpdateBeginDelegate OnUpdateBegin { get; set; }
    }
}