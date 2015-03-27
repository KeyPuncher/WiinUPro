using System;
using System.Collections.Generic;
using System.IO;
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
                        LoadPrefs();
                    }
                    else
                    {
                        _instance = new UserPrefs();
                        _instance.devicePrefs = new List<Property>();
                        _instance.defaultProfile = new Profile();
                        SavePrefs();
                    }
                }

                return _instance;
            }
        }

        public List<Property> devicePrefs;
        public Profile defaultProfile;

        public UserPrefs()
        { }

        public static bool LoadPrefs()
        {
            bool successful = false;
            XmlSerializer X = new XmlSerializer(typeof(UserPrefs));
            
            try
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\prefs.config"))
                {
                    using (FileStream stream = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + @"\prefs.config"))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        _instance = X.Deserialize(reader) as UserPrefs;
                        reader.Close();
                        stream.Close();
                    }

                    successful = true;
                }
            }
            catch (Exception e) 
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return successful;
        }

        public static void SavePrefs()
        {
            XmlSerializer X = new XmlSerializer(typeof(UserPrefs));
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\prefs.config";

            try
            {
                if (File.Exists(path))
                {
                    FileInfo prefs = new FileInfo(path);
                    using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        X.Serialize(writer, _instance);
                        writer.Close();
                        stream.Close();
                    }
                }
                else
                {
                    using (FileStream stream = File.Create(path))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        X.Serialize(writer, _instance);
                        writer.Close();
                        stream.Close();
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
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

            return null;
        }

        public void AddDevicePref(Property property)
        {
            foreach (var pref in devicePrefs)
            {
                if (pref.hid == property.hid)
                {
                    pref.name = property.name;
                    pref.autoConnect = property.autoConnect;
                    pref.profile = property.profile;
                    pref.connType = property.connType;
                    pref.autoNum = property.autoNum;
                    pref.rumbleIntensity = property.rumbleIntensity;
                    pref.useRumble = property.useRumble;

                    return;
                }
            }

            devicePrefs.Add(property);
        }
    }

    public class Property
    {
        public enum ProfHolderType
        {
            XInput = 0,
            DInput = 1
        }

        public string hid = "";
        public string name = "";
        public bool autoConnect = false;
        public bool useRumble = true;
        public int autoNum = 0;
        public int rumbleIntensity = 2;
        public ProfHolderType connType;
        public string profile = "";

        public Property()
        {
            hid = "";
            connType = ProfHolderType.XInput;
        }

        public Property(string ID)
        {
            hid = ID;
            connType = ProfHolderType.XInput;
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
