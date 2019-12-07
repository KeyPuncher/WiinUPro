using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Windows;

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
        public bool suppressConnectionLost;
        public int autoAddXInputDevices;

        public static string ToFileName(string text)
        {
            StringBuilder fileName = new StringBuilder(text);

            // Strip out unnecessary bits
            fileName.Replace("\\\\?\\hid#{00001124-0000-1000-8000-00805f9b34fb}_", "");
            fileName.Replace("#{4d1e55b2-f16f-11cf-88cb-001111000030}", "");

            // Replace invalid characters
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            
            return fileName.ToString();
        }
        
        private static bool Load()
        {
            bool success = false;
            
            if (File.Exists(DataPath + PREFS_FILE_NAME))
            {
                success = App.LoadFromFile<AppPrefs>(DataPath + PREFS_FILE_NAME, out _instance);
            }

            return success;
        }

        public static bool Save()
        {
            bool success = false;
            
            if (!Directory.Exists(DataPath))
            {
                try
                {
                    Directory.CreateDirectory(DataPath);
                }
                catch
                {
                    return false;
                }
            }
                
            success = App.SaveToFile<AppPrefs>(DataPath + PREFS_FILE_NAME, _instance);

            return success;
        }

        public DevicePrefs GetDevicePreferences(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId)) return null;

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

        public bool GetAutoStartSet()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            return key.GetValue("WiinUPro") != null || File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "WiinUPro.lnk"));
        }

        public void SetAutoStart(bool value)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            string startupDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            
            try
            {

                if (value)
                {
                    if (key.GetValue("WiinUPro") == null)
                    {
                        key.SetValue("WiinUPro", (new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase)).LocalPath);
                    }
                }
                else
                {
                    key.DeleteValue("WiinUPro", false);
                }
            }
            catch
            {
                if (value)
                {
                    if (!File.Exists(Path.Combine(startupDir, "WiinUPro.lnk")))
                    {
                        IShellLink link = (IShellLink)new ShellLink();

                        link.SetDescription("WiinUPro");
                        link.SetPath(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath);

                        IPersistFile file = (IPersistFile)link;
                        file.Save(Path.Combine(startupDir, "WiinUPro.lnk"), false);
                    }
                }
            }

            if (!value && File.Exists(Path.Combine(startupDir, "WiinUPro.lnk")))
            {
                File.Delete(Path.Combine(startupDir, "WiinUPro.lnk"));
            }
        }

        public void PromptToSaveCalibration(string deviceId, string targetCalibration, string fileName)
        {
            var prefs = GetDevicePreferences(deviceId);
            if (prefs != null && !string.IsNullOrEmpty(fileName) && 
               (!prefs.calibrationFiles.ContainsKey(targetCalibration) || prefs.calibrationFiles[targetCalibration] != fileName))
            {
                var prompt = MessageBox.Show("Set calibration as default?", "Set as Default", MessageBoxButton.YesNo);
                if (prompt == MessageBoxResult.Yes)
                {
                    if (prefs.calibrationFiles.ContainsKey(targetCalibration))
                    {
                        prefs.calibrationFiles[targetCalibration] = fileName;
                    }
                    else
                    {
                        prefs.calibrationFiles.Add(targetCalibration, fileName);
                    }

                    SaveDevicePrefs(prefs);
                }
            }
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }
    }
}
