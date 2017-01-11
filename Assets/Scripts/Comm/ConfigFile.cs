using System;
using System.IO;
using System.Collections;

namespace Cloth3D.Comm{
    public class ConfigFile {
        private readonly Hashtable _hashKeyPairs = new Hashtable();
        private string _filePath;

        private struct SectionPair {
            public string Section;
            public string Key;
        }

        public ConfigFile(string filePath) {
            TextReader iniFile = null;
            string currentRoot = null;
            _filePath = filePath;
            if (File.Exists(filePath)) {
                try {
                    iniFile = new StreamReader(filePath);
                    var strLine = iniFile.ReadLine();
                    while (strLine != null) {
                        strLine = strLine.Trim();
                        if (strLine != "" && !strLine.StartsWith("#")) {
                            if (strLine.StartsWith("[") && strLine.EndsWith("]")) {
                                currentRoot = strLine.Substring(1, strLine.Length - 2);
                            } else {
                                var keyPair = strLine.Split(new[] {'='}, 2);

                                SectionPair sectionPair;
                                string value = null;
                                if (currentRoot == null)
                                    currentRoot = "ROOT";
                                sectionPair.Section = currentRoot.ToUpper();
                                sectionPair.Key = keyPair[0].ToUpper();
                                if (keyPair.Length > 1)
                                    value = keyPair[1];
                                _hashKeyPairs.Add(sectionPair, value);
                            }
                        }
                        strLine = iniFile.ReadLine();
                    }
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    if (iniFile != null)
                        iniFile.Close();
                }
            } else
                throw new FileNotFoundException("Unable to locate " + filePath);
        }

        public ConfigFile(string filePath, byte[] buffer) {
            TextReader iniFile = null;
            MemoryStream ms = null;
            string currentRoot = null;
            _filePath = filePath;
            if (buffer != null && buffer.Length > 0) {
                try {
                    ms = new MemoryStream(buffer);
                    ms.Seek(0, SeekOrigin.Begin);
                    iniFile = new StreamReader(ms);
                    var strLine = iniFile.ReadLine();
                    while (strLine != null) {
                        strLine = strLine.Trim();
                        if (strLine != "" && !strLine.StartsWith("#")) {
                            if (strLine.StartsWith("[") && strLine.EndsWith("]")) {
                                currentRoot = strLine.Substring(1, strLine.Length - 2);
                            } else {
                                var keyPair = strLine.Split(new[] {'='}, 2);

                                SectionPair sectionPair;
                                string value = null;
                                if (currentRoot == null)
                                    currentRoot = "ROOT";
                                sectionPair.Section = currentRoot.ToUpper();
                                sectionPair.Key = keyPair[0].ToUpper();
                                if (keyPair.Length > 1)
                                    value = keyPair[1];
                                _hashKeyPairs.Add(sectionPair, value);
                            }
                        }
                        strLine = iniFile.ReadLine();
                    }
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    if (iniFile != null) {
                        iniFile.Close();
                    }
                    if (ms != null) {
                        ms.Close();
                    }
                    if (iniFile != null) {
                        iniFile.Close();
                    }
                }
            } else
                throw new FileNotFoundException("Unable to locate " + filePath);
        }

        public string GetSetting(string sectionName, string settingName) {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper();
            sectionPair.Key = settingName.ToUpper();

            if (_hashKeyPairs.Contains(sectionPair)) {
                return (string) _hashKeyPairs[sectionPair];
            }
            return string.Empty;
        }

        public string[] EnumSection(string sectionName) {
            ArrayList tmpArray = new ArrayList();

            foreach (SectionPair pair in _hashKeyPairs.Keys) {
                if (pair.Section == sectionName.ToUpper())
                    tmpArray.Add(pair.Key);
            }

            return (string[]) tmpArray.ToArray(typeof(string));
        }

        public void AddSetting(string sectionName, string settingName, string settingValue) {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper();
            sectionPair.Key = settingName.ToUpper();

            if (_hashKeyPairs.ContainsKey(sectionPair))
                _hashKeyPairs.Remove(sectionPair);

            _hashKeyPairs.Add(sectionPair, settingValue);
        }

        public void AddSetting(string sectionName, string settingName) {
            AddSetting(sectionName, settingName, null);
        }

        public void DeleteSetting(string sectionName, string settingName) {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper();
            sectionPair.Key = settingName.ToUpper();

            if (_hashKeyPairs.ContainsKey(sectionPair))
                _hashKeyPairs.Remove(sectionPair);
        }

        public void SaveSettings(string newFilePath) {
            ArrayList sections = new ArrayList();
            string tmpValue;
            var strToSave = "";

            foreach (SectionPair sectionPair in _hashKeyPairs.Keys) {
                if (!sections.Contains(sectionPair.Section))
                    sections.Add(sectionPair.Section);
            }
            foreach (var section in sections) {
                strToSave += ("[" + section + "]\r\n");
                foreach (SectionPair sectionPair in _hashKeyPairs.Keys) {
                    if (sectionPair.Section == section) {
                        tmpValue = (string) _hashKeyPairs[sectionPair];
                        if (tmpValue != null)
                            tmpValue = "=" + tmpValue;
                        strToSave += (sectionPair.Key + tmpValue + "\r\n");
                    }
                }
                strToSave += "\r\n";
            }
            try {
                TextWriter tw = new StreamWriter(newFilePath);
                tw.Write(strToSave);
                tw.Close();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public void SaveSettings() {
            SaveSettings(_filePath);
        }
    }
}