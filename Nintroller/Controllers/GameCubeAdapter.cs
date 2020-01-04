using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintrollerLib
{
    public struct GameCubeAdapter : INintrollerState
    {
        public bool port1Connected;
        public bool port2Connected;
        public bool port3Connected;
        public bool port4Connected;

        public GameCubeController port1;
        public GameCubeController port2;
        public GameCubeController port3;
        public GameCubeController port4;

        public GameCubeAdapter(bool doPrefix)
        {
            this = new GameCubeAdapter();

            if (doPrefix)
            {
                port1 = new GameCubeController(INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX);
                port2 = new GameCubeController(INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX);
                port3 = new GameCubeController(INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX);
                port4 = new GameCubeController(INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX);
            }
        }

        public bool CalibrationEmpty
        {
            get
            {
                return port1.CalibrationEmpty && port2.CalibrationEmpty && port3.CalibrationEmpty && port4.CalibrationEmpty;
            }
        }
        
        public void Update(byte[] data)
        {
            // 0x10 is a wired controller
            // 0x22 is a wave bird
            port1Connected = data[ 1] >= 0x10;
            port2Connected = data[10] >= 0x10;
            port3Connected = data[19] >= 0x10;
            port4Connected = data[28] >= 0x10;

            if (port1Connected)
            {
                byte[] p1 = new byte[9];
                Array.Copy(data, 1, p1, 0, 9);
                port1.Update(p1);
            }
            
            if (port2Connected)
            {
                byte[] p2 = new byte[9];
                Array.Copy(data, 10, p2, 0, 9);
                port2.Update(p2);
            }
            
            if (port3Connected)
            {
                byte[] p3 = new byte[9];
                Array.Copy(data, 19, p3, 0, 9);
                port3.Update(p3);
            }

            if (port4Connected)
            {
                byte[] p4 = new byte[9];
                Array.Copy(data, 28, p4, 0, 9);
                port4.Update(p4);
            }
        }

        public float GetValue(string input)
        {
            switch (input)
            {
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_CONNECTED: return port1Connected ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_CONNECTED: return port2Connected ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_CONNECTED: return port3Connected ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_CONNECTED: return port4Connected ? 1 : 0;

                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.A:          return port1.A ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.B:          return port1.B ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.X:          return port1.X ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.Y:          return port1.Y ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.UP:         return port1.Up ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.DOWN:       return port1.Down ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LEFT:       return port1.Left ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RIGHT:      return port1.Right ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.L:          return port1.L.value > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.R:          return port1.R.value > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LT:         return port1.L.value;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RT:         return port1.R.value;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LFULL:      return port1.L.full ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RFULL:      return port1.R.full ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_X:      return port1.joystick.X;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_Y:      return port1.joystick.Y;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_X:        return port1.cStick.X;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_Y:        return port1.cStick.Y;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_UP:     return port1.joystick.Y > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_DOWN:   return port1.joystick.Y < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_LEFT:   return port1.joystick.X < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_RIGHT:  return port1.joystick.X > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_UP:       return port1.cStick.Y > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_DOWN:     return port1.cStick.Y < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_LEFT:     return port1.cStick.X < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_RIGHT:    return port1.cStick.X > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_1_PREFIX + INPUT_NAMES.GCN_CONTROLLER.START:      return port1.Start ? 1 : 0;

                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.A:          return port2.A ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.B:          return port2.B ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.X:          return port2.X ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.Y:          return port2.Y ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.UP:         return port2.Up ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.DOWN:       return port2.Down ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LEFT:       return port2.Left ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RIGHT:      return port2.Right ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.L:          return port2.L.value > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.R:          return port2.R.value > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LT:         return port2.L.value;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RT:         return port2.R.value;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LFULL:      return port2.L.full ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RFULL:      return port2.R.full ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_X:      return port2.joystick.X;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_Y:      return port2.joystick.Y;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_X:        return port2.cStick.X;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_Y:        return port2.cStick.Y;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_UP:     return port2.joystick.Y > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_DOWN:   return port2.joystick.Y < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_LEFT:   return port2.joystick.X < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_RIGHT:  return port2.joystick.X > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_UP:       return port2.cStick.Y > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_DOWN:     return port2.cStick.Y < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_LEFT:     return port2.cStick.X < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_RIGHT:    return port2.cStick.X > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_2_PREFIX + INPUT_NAMES.GCN_CONTROLLER.START:      return port2.Start ? 1 : 0;

                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.A:          return port3.A ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.B:          return port3.B ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.X:          return port3.X ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.Y:          return port3.Y ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.UP:         return port3.Up ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.DOWN:       return port3.Down ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LEFT:       return port3.Left ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RIGHT:      return port3.Right ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.L:          return port3.L.value > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.R:          return port3.R.value > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LT:         return port3.L.value;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RT:         return port3.R.value;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LFULL:      return port3.L.full ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RFULL:      return port3.R.full ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_X:      return port3.joystick.X;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_Y:      return port3.joystick.Y;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_X:        return port3.cStick.X;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_Y:        return port3.cStick.Y;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_UP:     return port3.joystick.Y > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_DOWN:   return port3.joystick.Y < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_LEFT:   return port3.joystick.X < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_RIGHT:  return port3.joystick.X > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_UP:       return port3.cStick.Y > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_DOWN:     return port3.cStick.Y < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_LEFT:     return port3.cStick.X < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_RIGHT:    return port3.cStick.X > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_3_PREFIX + INPUT_NAMES.GCN_CONTROLLER.START:      return port3.Start ? 1 : 0;

                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.A:          return port4.A ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.B:          return port4.B ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.X:          return port4.X ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.Y:          return port4.Y ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.UP:         return port4.Up ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.DOWN:       return port4.Down ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LEFT:       return port4.Left ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RIGHT:      return port4.Right ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.L:          return port4.L.value > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.R:          return port4.R.value > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LT:         return port4.L.value;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RT:         return port4.R.value;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.LFULL:      return port4.L.full ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.RFULL:      return port4.R.full ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_X:      return port4.joystick.X;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_Y:      return port4.joystick.Y;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_X:        return port4.cStick.X;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_Y:        return port4.cStick.Y;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_UP:     return port4.joystick.Y > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_DOWN:   return port4.joystick.Y < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_LEFT:   return port4.joystick.X < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.JOY_RIGHT:  return port4.joystick.X > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_UP:       return port4.cStick.Y > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_DOWN:     return port4.cStick.Y < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_LEFT:     return port4.cStick.X < 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.C_RIGHT:    return port4.cStick.X > 0 ? 1 : 0;
                case INPUT_NAMES.GCN_ADAPTER.PORT_4_PREFIX + INPUT_NAMES.GCN_CONTROLLER.START:      return port4.Start ? 1 : 0;
            }

            return 0;
        }
        
        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    port1.SetCalibration(Calibrations.Defaults.GameCubeControllerDefault);
                    port2.SetCalibration(Calibrations.Defaults.GameCubeControllerDefault);
                    port3.SetCalibration(Calibrations.Defaults.GameCubeControllerDefault);
                    port4.SetCalibration(Calibrations.Defaults.GameCubeControllerDefault);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    port1.SetCalibration(Calibrations.Moderate.GameCubeControllerModest);
                    port2.SetCalibration(Calibrations.Moderate.GameCubeControllerModest);
                    port3.SetCalibration(Calibrations.Moderate.GameCubeControllerModest);
                    port4.SetCalibration(Calibrations.Moderate.GameCubeControllerModest);
                    break;

                case Calibrations.CalibrationPreset.Extra:
                    port1.SetCalibration(Calibrations.Extras.GameCubeControllerExtra);
                    port2.SetCalibration(Calibrations.Extras.GameCubeControllerExtra);
                    port3.SetCalibration(Calibrations.Extras.GameCubeControllerExtra);
                    port4.SetCalibration(Calibrations.Extras.GameCubeControllerExtra);
                    break;

                case Calibrations.CalibrationPreset.Minimum:
                    port1.SetCalibration(Calibrations.Minimum.GameCubeControllerMinimal);
                    port2.SetCalibration(Calibrations.Minimum.GameCubeControllerMinimal);
                    port3.SetCalibration(Calibrations.Minimum.GameCubeControllerMinimal);
                    port4.SetCalibration(Calibrations.Minimum.GameCubeControllerMinimal);
                    break;

                case Calibrations.CalibrationPreset.None:
                    port1.SetCalibration(Calibrations.None.GameCubeControllerRaw);
                    port2.SetCalibration(Calibrations.None.GameCubeControllerRaw);
                    port3.SetCalibration(Calibrations.None.GameCubeControllerRaw);
                    port4.SetCalibration(Calibrations.None.GameCubeControllerRaw);
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            if (from.CalibrationEmpty)
            {
                // don't apply empty calibrations
                return;
            }

            if (from.GetType() == typeof(GameCubeAdapter))
            {
                port1.SetCalibration(((GameCubeAdapter)from).port1);
                port2.SetCalibration(((GameCubeAdapter)from).port2);
                port3.SetCalibration(((GameCubeAdapter)from).port3);
                port4.SetCalibration(((GameCubeAdapter)from).port4);
            }
        }

        public string GetCalibrationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-gcn");
            sb.Append("+"); sb.Append(port1.GetCalibrationString());
            sb.Append("+"); sb.Append(port2.GetCalibrationString());
            sb.Append("+"); sb.Append(port3.GetCalibrationString());
            sb.Append("+"); sb.Append(port4.GetCalibrationString());
            return sb.ToString();
        }

        public void SetCalibration(string calibrationString)
        {
            if (calibrationString.Count(c => c == '0') > 5)
            {
                // don't set empty calibrations
                return;
            }

            // Index 0 should be "gcn"
            string[] ports = calibrationString.Split(new char[] { '+' });

            if (ports.Length > 1)
            {
                port1.SetCalibration(ports[1]);
            }

            if (ports.Length > 2)
            {
                port2.SetCalibration(ports[2]);
            }

            if (ports.Length > 3)
            {
                port3.SetCalibration(ports[3]);
            }

            if (ports.Length > 4)
            {
                port4.SetCalibration(ports[4]);
            }
        }

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GCN_ADAPTER.PORT_1_CONNECTED, port1Connected ? 1 : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GCN_ADAPTER.PORT_2_CONNECTED, port2Connected ? 1 : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GCN_ADAPTER.PORT_3_CONNECTED, port3Connected ? 1 : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GCN_ADAPTER.PORT_4_CONNECTED, port4Connected ? 1 : 0);

            foreach (var p1 in port1) yield return p1;
            foreach (var p2 in port2) yield return p2;
            foreach (var p3 in port3) yield return p3;
            foreach (var p4 in port4) yield return p4;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct GameCubeController : IEnumerable<KeyValuePair<string, float>>
    {
        public Joystick joystick, cStick;
        public Trigger L, R;
        public bool A, B, X, Y;
        public bool Up, Down, Left, Right;
        public bool Z, Start;

        private string _prefix;

        public GameCubeController(string prefix)
        {
            this = new GameCubeController();
            _prefix = prefix;
        }

        public bool CalibrationEmpty
        {
            get
            {
                if (joystick.maxX == 0 && joystick.maxY == 0 && cStick.maxX == 0 && cStick.maxY == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Update(byte[] data)
        {
            // Buttons
            A = (data[1] & 0x01) != 0;
            B = (data[1] & 0x02) != 0;
            X = (data[1] & 0x04) != 0;
            Y = (data[1] & 0x08) != 0;
            Z = (data[2] & 0x02) != 0;
            Start = (data[2] & 0x01) != 0;

            // D-Pad
            Up = (data[1] & 0x80) != 0;
            Down = (data[1] & 0x40) != 0;
            Left = (data[1] & 0x10) != 0;
            Right = (data[1] & 0x20) != 0;

            // Joysticks
            joystick.rawX = data[3];
            joystick.rawY = data[4];
            cStick.rawX = data[5];
            cStick.rawY = data[6];

            // Triggers
            L.rawValue = data[7];
            R.rawValue = data[8];
            L.full = (data[2] & 0x08) != 0;
            R.full = (data[2] & 0x04) != 0;

            // Normalize
            joystick.Normalize();
            cStick.Normalize();
            L.Normalize();
            R.Normalize();
        }

        public void SetCalibration(GameCubeController from)
        {
            if (from.CalibrationEmpty)
            {
                // don't apply empty calibrations
                return;
            }

            joystick.Calibrate(from.joystick);
            cStick.Calibrate(from.cStick);
            L.Calibrate(from.L);
            R.Calibrate(from.R);
        }

        public void SetCalibration(string calibrationString)
        {
            if (calibrationString.Count(c => c == '0') > 5)
            {
                // don't set empty calibrations
                return;
            }

            string[] components = calibrationString.Split(new char[] { '-' });

            foreach (string component in components)
            {
                if (component.StartsWith("joy"))
                {
                    string[] joyLConfig = component.Split(new char[] { '|' });

                    for (int j = 1; j < joyLConfig.Length; j++)
                    {
                        int value = 0;
                        if (int.TryParse(joyLConfig[j], out value))
                        {
                            switch (j)
                            {
                                case 1: joystick.centerX = value; break;
                                case 2: joystick.minX = value; break;
                                case 3: joystick.maxX = value; break;
                                case 4: joystick.deadX = value; break;
                                case 5: joystick.centerY = value; break;
                                case 6: joystick.minY = value; break;
                                case 7: joystick.maxY = value; break;
                                case 8: joystick.deadY = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("cStk"))
                {
                    string[] joyRConfig = component.Split(new char[] { '|' });

                    for (int c = 1; c < joyRConfig.Length; c++)
                    {
                        int value = 0;
                        if (int.TryParse(joyRConfig[c], out value))
                        {
                            switch (c)
                            {
                                case 1: cStick.centerX = value; break;
                                case 2: cStick.minX = value; break;
                                case 3: cStick.maxX = value; break;
                                case 4: cStick.deadX = value; break;
                                case 5: cStick.centerY = value; break;
                                case 6: cStick.minY = value; break;
                                case 7: cStick.maxY = value; break;
                                case 8: cStick.deadY = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("tl"))
                {
                    string[] triggerLConfig = component.Split(new char[] { '|' });

                    for (int tl = 1; tl < triggerLConfig.Length; tl++)
                    {
                        int value = 0;
                        if (int.TryParse(triggerLConfig[tl], out value))
                        {
                            switch (tl)
                            {
                                case 1: L.min = value; break;
                                case 2: L.max = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("tr"))
                {
                    string[] triggerRConfig = component.Split(new char[] { '|' });

                    for (int tr = 1; tr < triggerRConfig.Length; tr++)
                    {
                        int value = 0;
                        if (int.TryParse(triggerRConfig[tr], out value))
                        {
                            switch (tr)
                            {
                                case 1: R.min = value; break;
                                case 2: R.max = value; break;
                                default: break;
                            }
                        }
                    }
                }
            }
        }

        public string GetCalibrationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(":joy");
            sb.Append("|"); sb.Append(joystick.centerX);
            sb.Append("|"); sb.Append(joystick.minX);
            sb.Append("|"); sb.Append(joystick.maxX);
            sb.Append("|"); sb.Append(joystick.deadX);
            sb.Append("|"); sb.Append(joystick.centerY);
            sb.Append("|"); sb.Append(joystick.minY);
            sb.Append("|"); sb.Append(joystick.maxY);
            sb.Append("|"); sb.Append(joystick.deadY);
            sb.Append(":cStk");
            sb.Append("|"); sb.Append(cStick.centerX);
            sb.Append("|"); sb.Append(cStick.minX);
            sb.Append("|"); sb.Append(cStick.maxX);
            sb.Append("|"); sb.Append(cStick.deadX);
            sb.Append("|"); sb.Append(cStick.centerY);
            sb.Append("|"); sb.Append(cStick.minY);
            sb.Append("|"); sb.Append(cStick.maxY);
            sb.Append("|"); sb.Append(cStick.deadY);
            sb.Append(":tl");
            sb.Append("|"); sb.Append(L.min);
            sb.Append("|"); sb.Append(L.max);
            sb.Append(":tr");
            sb.Append("|"); sb.Append(R.min);
            sb.Append("|"); sb.Append(R.max);

            return sb.ToString();
        }

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            string prefix = string.IsNullOrEmpty(_prefix) ? "" : _prefix;

            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.A, A ? 1 : 0);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.B, B ? 1 : 0);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.X, X ? 1 : 0);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.Y, Y ? 1 : 0);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.Z, Z ? 1 : 0);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.START, Start ? 1 : 0);

            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.UP, Up ? 1 : 0);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.DOWN, Down ? 1 : 0);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.LEFT, Left ? 1 : 0);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.RIGHT, Right ? 1 : 0);

            L.Normalize();
            R.Normalize();
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.L, L.value > 0 ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.R, R.value > 0 ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.LFULL, L.full ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.RFULL, R.full ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.LT, L.value);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.RT, R.value);

            joystick.Normalize();
            cStick.Normalize();
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.JOY_X, joystick.X);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.JOY_Y, joystick.Y);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.C_X, cStick.X);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.C_Y, cStick.Y);

            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.JOY_UP, joystick.Y > 0f ? joystick.Y : 0.0f);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.JOY_DOWN, joystick.Y > 0f ? 0.0f : -joystick.Y);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.JOY_LEFT, joystick.X > 0f ? 0.0f : -joystick.X);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.JOY_RIGHT, joystick.X > 0f ? joystick.X : 0.0f);

            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.C_UP, cStick.Y > 0f ? cStick.Y : 0.0f);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.C_DOWN, cStick.Y > 0f ? 0.0f : -cStick.Y);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.C_LEFT, cStick.X > 0f ? 0.0f : -cStick.X);
            yield return new KeyValuePair<string, float>(prefix + INPUT_NAMES.GCN_CONTROLLER.C_RIGHT, cStick.X > 0f ? cStick.X : 0.0f);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
