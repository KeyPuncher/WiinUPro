using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    _instance = new UserPrefs();
                }

                return _instance;
            }
        }

        public Dictionary<string, Property> devicePrefs;
        public Profile defaultProfile;

        UserPrefs()
        {
            // TODO: Load saved preferences
            XmlSerializer x = new XmlSerializer(typeof(UserPrefs));
            FileInfo prefs = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\prefs.config");
            
            if (prefs.Exists)
            {
                
            }
            else
            {
                devicePrefs = new Dictionary<string, Property>();
                defaultProfile = new Profile();
                var stream = prefs.Create();
                TextWriter writer = new StreamWriter(stream);
                x.Serialize(writer, Instance);
                writer.Close();
                stream.Close();
            }
        }
    }

    class Property
    {
        public string name = "";
        public bool autoConnect = false;
        public string profile = "";
    }

    public class Profile
    {
        public Dictionary<NintrollerLib.ControllerType, KeyValuePair<string, string>> controllerMaps;
    }

}
