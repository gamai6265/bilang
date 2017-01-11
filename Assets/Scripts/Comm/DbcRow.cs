using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cloth3D {
    public class DbcRow {
        private List<string> _data;
        private Dbc _dbc;


        public DbcRow(Dbc dbc, List<string> data) {
            _data = data;
            _dbc = dbc;
        }

        public bool HasFields {
            get { return _data != null && _data.Count > 0; }
        }

        ~DbcRow() {
            _dbc = null;
            _data.Clear();
            _data = null;
        }

        public string GetFieldByName(string name) {
            if (!HasFields) {
                LogSystem.Error("GetFieldByName DbcRow has no vaild data.");
                return null;
            }
            var index = _dbc.GetHeaderIndexByName(name);
            if (index >= 0 && index < _data.Count) {
                return _data[index];
            }
            return null;
        }

        public string GetFiledByIndex(int index) {
            if (!HasFields) {
                LogSystem.Error("DbcRow has no vaild data.");
                return null;
            }
            if (index < 0 || index >= _data.Count) {
                LogSystem.Error("GetFiledByIndex index:{0} is out of range.", index);
                return null;
            }
            return _data[index];
        }

        public List<string> GetFieldsByPrefix(string filedPrefix) {
            var list = new List<string>();
            if (!HasFields) {
                LogSystem.Error("GetFieldsByPrefix DbcRow has no vaild data.");
                return list;
            }
            if (string.IsNullOrEmpty(filedPrefix)) {
                LogSystem.Error("GetFieldsByPrefix filePrefix is null or empty.");
                return list;
            }
            LogSystem.Debug("GetFieldsByPrefix. content start:");
            foreach (var name in _dbc.Header) {
                if (!name.StartsWith(filedPrefix))
                    continue;
                var index = _dbc.GetHeaderIndexByName(name);
                if (index >= 0 && index < _data.Count) {
                    list.Add(_data[index]);
                    LogSystem.Debug(_data[index]);
                }
            }
            LogSystem.Debug("GetFieldsByPrefix. content end.");
            return list;
        }

        public Dictionary<string, string> GetAllFields() {
            var dic = new Dictionary<string, string>();
            if (!HasFields) {
                LogSystem.Error("GetAllFields. DbcRow has no vaild data");
                return dic;
            }
            LogSystem.Debug("GetAllFields content start:");
            foreach (var name in _dbc.Header) {
                var index = _dbc.GetHeaderIndexByName(name);
                if (index >= 0 && index < _data.Count) {
                    dic.Add(name, _data[index]);
                    LogSystem.Debug(_data[index]);
                }
            }
            LogSystem.Debug("GetAllFields content end.");
            return dic;
        }
    }

    public class Dbc {
        private static readonly string[] FieldSeperator = {"\t"};
        private readonly List<DbcRow> _dataBuf = new List<DbcRow>();
        private readonly Dictionary<string, DbcRow> _hashData = new Dictionary<string, DbcRow>();
        private string _fileName = "";
        private long _fileSize;
        private List<string> _header = new List<string>();

        public Dbc() {
            ColNum = 0;
            RowNum = 0;
        }

        public int RowNum { get; private set; }

        public int ColNum { get; private set; }

        public List<string> Header {
            get { return _header; }
        }

        public string FileName {
            get { return _fileName; }
        }

        public string GetHeaderNameByIndex(int index) {
            if (_header == null || _header.Count == 0) {
                LogSystem.Error("GetHeaderNameByIndex _header is null or empty.");
                return null;
            }
            if (index < 0 || index >= _header.Count) {
                LogSystem.Error("GetHeaderNameByIndex index:{0} is out of range.", index);
                return null;
            }
            return _header[index];
        }

        public int GetHeaderIndexByName(string name) {
            var ret = -1;
            if (string.IsNullOrEmpty(name)) {
                LogSystem.Warn("GetHeaderIndexByName name is empty or null");
                return -1;
            }
            if (_header != null) {
                for (var index = 0; index < _header.Count; index++) {
                    if (name == _header[index]) {
                        ret = index;
                        break;
                    }
                }
            }
            return ret;
        }

        public DbcRow GetRowByIndex(int index) {
            DbcRow ret = null;
            if (index < 0 || index >= RowNum) {
                LogSystem.Error("GetRowByIndex index:{0} is out of range.", index);
                return null;
            }
            if (_dataBuf != null && index < _dataBuf.Count) {
                ret = _dataBuf[index];
            }
            return ret;
        }

        public string GetField(int row, int col) {
            if (row < 0 || row >= RowNum) {
                LogSystem.Error("GetField row index:{0} is out of range.", row);
                return null;
            }
            if (col < 0 || col >= ColNum) {
                LogSystem.Error("GetField col index:{0} is out of range.", col);
                return null;
            }
            var dbcRow = GetRowByIndex(row);
            if (dbcRow != null) {
                return dbcRow.GetFiledByIndex(col);
            }
            return null;
        }

        public bool Load(string filePath) {
            bool ret;
            if (filePath == "" || !FileProxy.Exists(filePath)) {
                LogSystem.Error("DBC.Load file is not exist! filePath={0}", filePath);
                return false;
            }
            Stream stream = null;
            StreamReader streamReader = null;
            try {
                stream = FileProxy.ReadFileAsMemoryStream(filePath);
                if (stream == null) {
                    LogSystem.Error("Dbc file is empty:{0}", filePath);
                    return false;
                }
                _fileName = filePath;
                stream.Seek(0, SeekOrigin.Begin);
                _fileSize = stream.Length;
                if (_fileSize <= 0 || _fileSize >= int.MaxValue) {
                    LogSystem.Error("Dbc filesize erro: {0}{1}", filePath, _fileSize);
                    return false;
                }
                var encoding = Encoding.UTF8;
                streamReader = new StreamReader(stream, encoding);
                ret = LoadFromStream(streamReader);
            } catch (Exception e) {
                var err = "Exception:" + e.Message + "\n" + e.StackTrace + "\n";
                LogSystem.Error(err);
                return false;
            } finally {
                if (streamReader != null) {
                    streamReader.Close();
                }
                if (stream != null) {
                    stream.Close();
                }
            }
            return ret;
        }

        public bool LoadFromStream(StreamReader reader) {
            //read first line as title.
            var strLine = reader.ReadLine();
            if (strLine == null)
                return false;

            var rowElements = Utils.SplitAsList(strLine, FieldSeperator);
            if (rowElements == null || rowElements.Count == 0)
                return false;
            _header = rowElements;

            // begin parse the data row.
            var nRecordsNum = 0;
            var nFieldsNum = rowElements.Count;
            do {
                //read one line
                strLine = reader.ReadLine();
                //if read the end. end the loop.
                if (strLine == null) break;

                //comment start with '#', skip this line.
                if (strLine.StartsWith("#")) continue;

                rowElements = Utils.SplitAsList(strLine, FieldSeperator);

                //if rowElement num is 0, skip this line.
                if (rowElements.Count == 0) continue;
                if (rowElements.Count != nFieldsNum) {
                    //append space element.
                    if (rowElements.Count < nFieldsNum) {
                        var nSubNum = nFieldsNum - rowElements.Count;
                        for (var i = 0; i < nSubNum; i++) {
                            rowElements.Add("");
                        }
                    }
                }

                //the first field of element is the indentify must not be empty.
                if (string.IsNullOrEmpty(rowElements[0])) continue;

                var dbcRow = new DbcRow(this, rowElements);

                _dataBuf.Add(dbcRow);

                nRecordsNum++;
            } while (true);

            ColNum = nFieldsNum;
            RowNum = nRecordsNum;

            CreateIndex();

            return true;
        }

        private void CreateIndex() {
            foreach (var row in _dataBuf) {
                if (row.HasFields) {
                    var key = row.GetFiledByIndex(0);
                    if (!_hashData.ContainsKey(key)) {
                        _hashData.Add(key, row);
                    } else {
                        var err = string.Format("Dbc.CreateIndex FileName:{0} SameKey:{1}", _fileName, key);
                        LogSystem.Warn(err);
                    }
                }
            }
        }
    }
}