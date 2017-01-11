using System;
using System.IO;

namespace Cloth3D {
    public delegate byte[] ReadFileDelegate(string filePath);

    internal class FileProxy {
        private static ReadFileDelegate _handlerReadFile;

        public static MemoryStream ReadFileAsMemoryStream(string filePath) {
            try {
                var buffer = ReadFileAsArray(filePath);
                if (buffer == null) {
                    LogSystem.Warn("FileProxy ReadFileAsMemoryStream failed:{0}", filePath);
                    return null;
                }
                return new MemoryStream(buffer);
            } catch (Exception e) {
                LogSystem.Error("Exception:{0}\n", e.Message);
                Utils.LogCallStack(LogType.Error);
                return null;
            }
        }

        public static byte[] ReadFileAsArray(string filePath) {
            byte[] buffer = null;
            try {
                if (_handlerReadFile != null) {
                    buffer = _handlerReadFile(filePath);
                } else {
                    LogSystem.Warn("FileProxy  _handlerReadFile have not register:{0}", filePath);
                }
            } catch (Exception e) {
                LogSystem.Error("Exeption:{0}\n", e.Message);
                Utils.LogCallStack(LogType.Error);
                return null;
            }
            return buffer;
        }

        public static bool Exists(string filePath) {
            return File.Exists(filePath);
        }
        public static void RegisterReadFileHandler(ReadFileDelegate handler) {
            _handlerReadFile = handler;
        }
    }
}