﻿ /**********************************************************************************************/ /**
 * @namespace   
 *
 * @brief   .数据接口
 * @author  daniezhang
 * @data 2016/4/27
 **************************************************************************************************/

namespace Cloth3D {
    public interface IData {
        /**********************************************************************************************/ /**
         * @fn  bool CollectDataFromDbc(DbcRow row);
         *
         * @brief   DBC数据接口
         *
         * @param   row The row.
         *
         * @return  true if it succeeds, false if it fails.
         **************************************************************************************************/

        bool CollectDataFromDbc(DbcRow row);

        /**********************************************************************************************/ /**
         * @fn  int GetId();
         *
         * @brief   Gets the identifier.
         *
         * @return  The identifier.
         **************************************************************************************************/

        int GetId();
    }
}