using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace WiinUPro
{
    [JsonObject]
    public class AppPrefs
    {
        private const string PREFS_FILE_NAME = "prefs.config";
        private const string PREFS_DEVICE_EXT = ".pref";

        private static AppPrefs _instance;
        public static AppPrefs Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Config\" + PREFS_FILE_NAME))
                    {
                        DataPath = AppDomain.CurrentDomain.BaseDirectory + @"\Config\";
                        Load();
                    }
                    else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WiinUPro\" + PREFS_FILE_NAME))
                    {
                        DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WiinUPro\";
                        BaseDir = @"\Settings\";
                        Load();
                    }
                    else
                    {
                        _instance = new AppPrefs();
                        DataPath = AppDomain.CurrentDomain.BaseDirectory + @"\Config\";

                        if (!Save())
                        {
                            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WiinUPro\";
                            Save();
                        }
                    }
                }

                return _instance;
            }
        }

        public static string DataPath { get; protected set; }
        public static string BaseDir { get; protected set; }

        public HashSet<string> KnownDevices { get; set; } = new HashSet<string>();

        [JsonIgnore]
        public List<DevicePrefs> devicePreferences = new List<DevicePrefs>();
        public bool startMinimized;
        public bool useExclusiveMode;
        public bool useToshibaMode;
        public int autoAddXInputDevices;

        public static string ToFileName(string text)
        {
            StringBuilder fileName = new StringBuilder(text);
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName.ToString();
        }
        
        private static bool Load()
        {
            bool success = false;

            try
            {
                if (File.Exists(DataPath + PREFS_FILE_NAME))
                {
                    using (StreamReader stream = File.OpenText(DataPath + PREFS_FILE_NAME))
                    {
                        JsonSerializer jsonSerializer = new JsonSerializer();
                        _instance = (AppPrefs)jsonSerializer.Deserialize(stream, typeof(AppPrefs));
                        stream.Close();
                    }

                    success = true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return success;
        }

        private static bool Save()
        {
            bool success = false;

            try
            {
                if (!Directory.Exists(DataPath))
                {
                    Directory.CreateDirectory(DataPath);
                }

                File.WriteAllText(DataPath + PREFS_FILE_NAME, JsonConvert.SerializeObject(_instance, Formatting.Indented));
                success = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return success;
        }

        public DevicePrefs GetDevicePreferences(string deviceId)
        {
            DevicePrefs prefs = devicePreferences.Find((d) => d.deviceId == deviceId);

            if (prefs != null)
            {
                return prefs;
            }

            if (KnownDevices.Contains(deviceId))
            {
                try
                {
                    string filePath = DataPath + ToFileName(deviceId) + PREFS_DEVICE_EXT;
                    if (File.Exists(filePath))
                    {
                        using (StreamReader stream = File.OpenText(filePath))
                        {
                            JsonSerializer jsonSerializer = new JsonSerializer();
                            prefs = (DevicePrefs)jsonSerializer.Deserialize(stream, typeof(DevicePrefs));
                            stream.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            if (prefs == null)
            {
                prefs = new DevicePrefs
                {
                    deviceId = deviceId
                };

                KnownDevices.Add(deviceId);
                SaveDevicePrefs(prefs);
                Save();
            }

            devicePreferences.Add(prefs);

            return prefs;
        }

        public bool SaveDevicePrefs(DevicePrefs devicePrefs)
        {
            bool success = false;
            
            try
            {
                File.WriteAllText(DataPath + ToFileName(devicePrefs.deviceId) + PREFS_DEVICE_EXT, JsonConvert.SerializeObject(devicePrefs, Formatting.Indented));
                success = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return success;
        }
    }
}
