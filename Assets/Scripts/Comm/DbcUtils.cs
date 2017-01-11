using System;
using System.Collections.Generic;

namespace Cloth3D {
    internal class DbcUtils {
        public static string[] FieldStringSeperator = {"|"};
        public static string[] FieldNumSeperator = {",", "|"};
        /**
         * @brief 从Xml节点中读取字符串
         *
         * @param row xml节点
         * @param fieldName 节点名字
         * @param defualtVal 默认值
         *
         * @return 
         */

        public static string ExtractString(DbcRow row, string fieldName, string defualtVal) {
            var result = defualtVal;

            if (row == null || !row.HasFields || string.IsNullOrEmpty(row.GetFieldByName(fieldName))) {
                return result;
            }

            var fieldText = row.GetFieldByName(fieldName);
            if (!string.IsNullOrEmpty(fieldText)) {
                result = fieldText;
            }

            return result;
        }

        /**
         * @brief 从Xml节点中读取字符串数组
         *
         * @param row xml节点
         * @param fieldName 节点名字
         * @param defualtVal 默认值
         *
         * @return 
         */

        public static List<string> ExtractStringList(DbcRow row, string filedName, string defualtVal) {
            var result = new List<string>();

            if (row == null || !row.HasFields) {
                return result;
            }

            var nodeText = row.GetFieldByName(filedName);
            if (!string.IsNullOrEmpty(nodeText)) {
                result = Utils.SplitAsList(nodeText, FieldStringSeperator);
            }

            return result;
        }

        /**
         * @brief 从Xml节点中读取布尔值
         *
         * @param row xml节点
         * @param fieldName 节点名字
         * @param defualtVal 默认值

         * @return 
         */

        public static bool ExtractBool(DbcRow row, string fieldName, bool defualtVal) {
            var result = defualtVal;

            if (row == null || !row.HasFields || string.IsNullOrEmpty(row.GetFieldByName(fieldName))) {
                return result;
            }

            var nodeText = row.GetFieldByName(fieldName);
            if (!string.IsNullOrEmpty(nodeText)) {
                if (nodeText.Trim().ToLower() == "true" || nodeText.Trim().ToLower() == "1") {
                    result = true;
                }

                if (nodeText.Trim().ToLower() == "false" || nodeText.Trim().ToLower() == "0") {
                    result = false;
                }
            }
            return result;
        }

        /**
         * @brief 从Xml节点中读取数值类型，使用时，必须在函数中指明数值类型
         *          如: int id = ExtractNumeric<int>(xmlNode, "Id", -1, true);
         *
         * @param row xml节点
         * @param fieldName 节点名字
         * @param defualtVal 默认值
         *
         * @return 
         */

        public static T ExtractNumeric<T>(DbcRow row, string fieldName, T defualtVal) {
            var result = defualtVal;

            if (row == null || !row.HasFields || string.IsNullOrEmpty(row.GetFieldByName(fieldName))) {
                return result;
            }

            var nodeText = row.GetFieldByName(fieldName);
            if (!string.IsNullOrEmpty(nodeText)) {
                try {
                    result = (T) Convert.ChangeType(nodeText, typeof (T));
                } catch (Exception ex) {
                    var info = string.Format("ExtractNumeric Error  fieldName:{0} ex:{1} stacktrace:{2}",
                        fieldName, ex.Message, ex.StackTrace);
                    LogSystem.Error(info);
                }
            }

            return result;
        }

        /**
         * @brief 从Xml节点中读取数值类型，使用时，必须在函数中指明数值类型
         *          如: int id = ExtractNumeric<int>(xmlNode, "Id", -1, true);
         *
         * @param row xml节点
         * @param fieldName 节点名字
         * @param defualtVal 默认值
         *
         * @return 
         */

        public static List<T> ExtractNumericList<T>(DbcRow row, string fieldName, T defualtVal) {
            var result = new List<T>();

            if (row == null || !row.HasFields || string.IsNullOrEmpty(row.GetFieldByName(fieldName))) {
                return result;
            }

            var nodeText = row.GetFieldByName(fieldName);
            if (!string.IsNullOrEmpty(nodeText)) {
                result = Utils.SplitAsNumericList<T>(nodeText, FieldNumSeperator);
            }

            return result;
        }

        /**
         * @brief 从Xml节点中抽取所有以prefix为前缀的节点
         *
         * @param row xml节点
         * @param prefix 前缀字符串
         *
         * @return 
         */

        public static List<string> ExtractNodeByPrefix(DbcRow row, string prefix) {
            if (row == null || !row.HasFields) {
                return null;
            }

            return row.GetFieldsByPrefix(prefix);
        }
    }
}