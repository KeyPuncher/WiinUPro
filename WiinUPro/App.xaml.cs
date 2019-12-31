using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal const string PROFILE_FILTER = "WiinUPro Profile|*.wup";
        internal const string JOY_CAL_FILTER = "Joystick Calibration|*.joy";
        internal const string TRIG_CAL_FILTER = "Trigger Calibration|*.trg";
        internal const string AXIS_CAL_FILTER = "Axis Calibration|*.axs";
        internal const string IR_CAL_FILTER = "IR Calibration|*.irc";

        internal const string CAL_NUN_JOYSTICK = "nJoy";
        internal const string CAL_CC_LJOYSTICK = "ccJoyL";
        internal const string CAL_CC_RJOYSTICK = "ccJoyR";
        internal const string CAL_CCP_LJOYSTICK = "ccpJoyL";
        internal const string CAL_CCP_RJOYSTICK = "ccpJoyR";
        internal const string CAL_PRO_LJOYSTICK = "proJoyL";
        internal const string CAL_PRO_RJOYSTICK = "proJoyR";
        internal const string CAL_SWP_LJOYSTICK = "swpJoyL";
        internal const string CAL_SWP_RJOYSTICK = "swpJoyR";
        internal const string CAL_GUT_JOYSTICK = "gutJoy";
        internal const string CAL_GUT_WHAMMY = "gutWT";
        internal const string CAL_CC_LTRIGGER = "ccLT";
        internal const string CAL_CC_RTRIGGER = "ccRT";
        internal const string CAL_GCN_LTRIGGER = "gcnLT";
        internal const string CAL_GCN_RTRIGGER = "gcnRT";
        internal const string CAL_WII_IR = "IR";

        public static bool SaveToFile<T>(string file, T data)
        {
            try
            {
                File.WriteAllText(file, JsonConvert.SerializeObject(data, Formatting.Indented));
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool LoadFromFile<T>(string file, out T output)
        {
            try
            {
                using (StreamReader stream = File.OpenText(file))
                {
                    JsonSerializer jsonSerializer = new JsonSerializer();
                    output = (T)jsonSerializer.Deserialize(stream, typeof(T));
                    stream.Close();
                }
            }
            catch (JsonReaderException)
            {
                try
                {
                    // Might be the old XML format
                    XmlSerializer serializer = new XmlSerializer(typeof(AssignmentProfile));

                    using (FileStream stream = File.OpenRead(file))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        output = (T)serializer.Deserialize(reader);
                        reader.Close();
                        stream.Close();
                    }
                }
                catch
                {
                    output = default(T);
                    return false;
                }
            }
            catch (Exception ex)
            {
                output = default(T);
                return false;
            }

            return true;
        }
    }
}
