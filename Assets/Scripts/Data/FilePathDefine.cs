using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloth3D.Data {
    public class FilePathDefine {
        //app版本文件
        public const string ClientVersionFile = "ClientVersion.txt";
        public const string ServerVersionFile = "ServerVersion.txt";
        //资源文件
        public const string ResVersionFile = "ResVersion.txt";
        public const string ResVersionZip = "ResVersion.ab";
        //资源缓存文件
        public const string ResCacheFile = "ResCache.txt";
        public const string ResCacheZip = "ResCache.ab";
        public const string s_ResBuildInPath = "ResFile/";
        public const string ResCachePath = "ResFile/";

        public const string s_ResVersionClientFile = "ClientResVersion.txt";
        public const string s_ResVersionClientFormat = "{0}	{1}	{2}";
        public const string s_ResVersionClientHeader = "Name	MD5	IsBuildIn";

        //表格类配置文件
        public const string ResSheetList = "SheetList.txt";
        public const string ResSheetZip = "SheetList.ab";
        public const string ResSheetCacheDir = "DataFile/";

        public const string IniConfig= "Client/Config.ini";
        public const string GlobalConfig = "Client/Globals.txt";
        public const string ModelsConfig = "Client/Models.txt";
        public const string StrDictionary = "Client/StrDictionary.txt";
        public const string ModelsTypeConfig = "Client/ModelsType.txt";
    }
}
