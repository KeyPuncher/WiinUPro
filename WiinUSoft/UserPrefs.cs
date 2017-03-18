using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Xml.Serialization;

namespace WiinUSoft
{
    public class UserPrefs
    {
        private static UserPrefs _instance;
        public static UserPrefs Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\prefs.config"))
                    {
                        DataPath = AppDomain.CurrentDomain.BaseDirectory + @"\prefs.config";
                        LoadPrefs();
                    }
                    else if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WiinUSoft_prefs.config"))
                    {
                        DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WiinUSoft_prefs.config";
                        LoadPrefs();
                    }
                    else
                    {
                        _instance = new UserPrefs();
                        _instance.devicePrefs = new List<Property>();
                        _instance.defaultProfile = new Profile();
                        DataPath = AppDomain.CurrentDomain.BaseDirectory + @"\prefs.config";
                        
                        if (!SavePrefs())
                        {
                            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\WiinUSoft_prefs.config";
                            SavePrefs();
                        }
                    }
                }

                return _instance;
            }
        }

        public static string DataPath { get; protected set; }
        public static bool AutoStart
        {
            get { return Instance.autoStartup; }
            set
            {
                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                    if (value)
                    {
                        if (key.GetValue("WiinUSoft") == null)
                        {
                            key.SetValue("WiinUSoft", (new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase)).LocalPath);
                        }
                    }
                    else
                    {
                        key.DeleteValue("WiinUSoft", false);
                    }
                }
                catch
                {
                    string dir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                    if (value)
                    {
                        if (!File.Exists(Path.Combine(dir, "WiinUSoft.lnk")))
                        {
                            MainWindow.Instance.CreateShortcut(dir);
                        }
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(dir, "WiinUSoft.lnk")))
                        {
                            File.Delete(Path.Combine(dir, "WiinUSoft.lnk"));
                        }
                    }
                }

                Instance.autoStartup = value;
            }
        }

        public List<Property> devicePrefs;
        public Profile defaultProfile;
        public Property defaultProperty;
        public bool autoStartup;
        public bool startMinimized;
        public bool greedyMode;
        public bool autoRefresh = true;

        public UserPrefs()
        { }

        public static bool LoadPrefs()
        {
            bool successful = false;
            XmlSerializer X = new XmlSerializer(typeof(UserPrefs));
            
            try
            {
                if (File.Exists(DataPath))
                {
                    using (FileStream stream = File.OpenRead(DataPath))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        _instance = X.Deserialize(reader) as UserPrefs;
                        reader.Close();
                        stream.Close();
                    }

                    successful = true;

                    if (_instance != null && _instance.devicePrefs != null)
                        _instance.defaultProperty = _instance.devicePrefs.Find((p) => p.hid.ToLower().Equals("all"));
                }
            }
            catch (Exception e) 
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return successful;
        }

        public static bool SavePrefs()
        {
            bool successful = false;
            XmlSerializer X = new XmlSerializer(typeof(UserPrefs));

            try
            {
                if (File.Exists(DataPath))
                {
                    FileInfo prefs = new FileInfo(DataPath);
                    using (FileStream stream = File.Open(DataPath, FileMode.Create, FileAccess.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        X.Serialize(writer, _instance);
                        writer.Close();
                        stream.Close();
                    }
                }
                else
                {
                    using (FileStream stream = File.Create(DataPath))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        X.Serialize(writer, _instance);
                        writer.Close();
                        stream.Close();
                    }
                }

                successful = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return successful;
        }

        public Property GetDevicePref(string hid)
        {
            foreach (var pref in devicePrefs)
            {
                if (pref.hid == hid)
                {
                    return pref;
                }
            }

            return defaultProperty;
        }

        public void AddDevicePref(Property property)
        {
            foreach (var pref in devicePrefs)
            {
                if (pref.hid == property.hid)
                {
                    pref.name            = property.name;
                    pref.autoConnect     = property.autoConnect;
                    pref.profile         = property.profile;
                    pref.connType        = property.connType;
                    pref.autoNum         = property.autoNum;
                    pref.rumbleIntensity = property.rumbleIntensity;
                    pref.useRumble       = property.useRumble;
                    pref.calPref         = property.calPref;

                    return;
                }
            }

            devicePrefs.Add(property);
        }

        public void UpdateDeviceIcon(string path, string icon)
        {
            var prop = devicePrefs.FindIndex((p) => p.hid == path);

            if (prop >= 0)
            {
                devicePrefs[prop].lastIcon = icon;
                SavePrefs();
            }
        }

        public string GetDeviceIcon(string path)
        {
            var prop = devicePrefs.FindIndex((p) => p.hid == path);

            if (prop >= 0)
            {
                return devicePrefs[prop].lastIcon;
            }

            return "";
        }
    }

    public class Property
    {
        public enum ProfHolderType
        {
            XInput = 0,
            DInput = 1
        }

        public enum CalibrationPreference
        {
            Raw     = -2,
            Minimal = -1,
            Defalut = 0,
            More    = 1,
            Extra   = 2,
            Custom  = 3
        }

        public string hid = "";
        public string name = "";
        public string lastIcon = "";
        public bool autoConnect = false;
        public bool useRumble = true;
        public int autoNum = 0;
        public int rumbleIntensity = 2;
        public ProfHolderType connType;
        public string profile = "";
        public CalibrationPreference calPref;
        public string calString = ""; // not the best solution for saving the custom config but makes it easy

        public Property()
        {
            hid = "";
            connType = ProfHolderType.XInput;
            calPref = CalibrationPreference.Defalut;
        }

        public Property(string ID)
        {
            hid = ID;
            connType = ProfHolderType.XInput;
            calPref = CalibrationPreference.Defalut;
        }

        public Property(Property copy)
        {
            hid = copy.hid;
            name = copy.name;
            autoConnect = copy.autoConnect;
            autoNum = copy.autoNum;
            useRumble = copy.useRumble;
            rumbleIntensity = copy.rumbleIntensity;
            connType = copy.connType;
            profile = copy.profile;
            calPref = copy.calPref;
            calString = copy.calString;
        }
    }

    public class Profile
    {
        public enum HolderType
        {
            XInput = 0,
            DInput = 1
        }

        public NintrollerLib.ControllerType profileType;
        public HolderType connType;
        public List<string> controllerMapKeys;
        public List<string> controllerMapValues;

        public Profile()
        {
            profileType = NintrollerLib.ControllerType.Wiimote;
            controllerMapKeys = new List<string>();
            controllerMapValues = new List<string>();
            connType = HolderType.XInput;
        }

        public Profile(NintrollerLib.ControllerType type)
        {
            profileType = type;
            controllerMapKeys = new List<string>();
            controllerMapValues = new List<string>();
            connType = HolderType.XInput;
        }
    }

}
