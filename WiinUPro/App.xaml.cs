using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

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
            catch
            {
                output = default(T);
                return false;
            }

            return true;
        }
    }
}
