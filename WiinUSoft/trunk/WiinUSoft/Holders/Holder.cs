using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiinUSoft.Holders
{
    public abstract class Holder
    {
        public System.Collections.Concurrent.ConcurrentDictionary<string, float> Values { get; protected set; }
        //public Dictionary<string, float> Values { get; protected set; }
        public Dictionary<string, string> Mappings { get; protected set; }
        public Dictionary<string, bool> Flags { get; protected set; }
        public bool InMouseMode { get; protected set; }

        public void SetValue(string name, bool value)
        {
            SetValue(name, value ? 1.0f : 0.0f);
        }

        public void SetValue(string name, float value)
        {
            if (Mappings.ContainsKey(name))
            {
                if (Values.ContainsKey(name))
                {
                    Values[name] = Math.Abs(value);
                }
                else if (!Values.ContainsKey(name))
                {
                    //Values.Add(name, Math.Abs(value));
                    Values.TryAdd(name, Math.Abs(value));
                }
            }
        }

        public void SetMapping(string name, string mapping)
        {
            if (Mappings.ContainsKey(name))
            {
                Mappings[name] = mapping;
            }
            else
            {
                Mappings.Add(name, mapping);
            }
        }

        public void ClearMapping(string name)
        {
            if (Mappings.ContainsKey(name))
            {
                Mappings.Remove(name);
            }
        }

        public void ClearAllMappings()
        {
            Mappings.Clear();
        }

        public bool GetFlag(string name)
        {
            if (Flags.ContainsKey(name))
            {
                return Flags[name];
            }
            else
            {
                return false;
            }
        }

        public abstract void Update();
        public abstract void Close();
        public abstract void AddMapping(NintrollerLib.New.ControllerType controller);

        // used for basic mouse and keyboard actions (fixed mappings)
        protected void UpdateMouseMode()
        {
            bool leftMouseBtn = false;
            bool rightMouseBtn = false;
            bool escKey = false;
            bool altKey = false;
            bool tabKey = false;
            bool ctrlKey = false;
            bool shiftKey = false;
            bool enterKey = false;

            bool upKey = false;
            bool downKey = false;
            bool leftKey = false;
            bool rightKey = false;

            float moveMouseX = 0f;
            float moveMouseY = 0f;
            float mouseAbsX = 0f;
            float mouseAbsY = 0f;

            foreach (KeyValuePair<string, float> input in Values)
            {
                switch (input.Key)
                {
                    #region Pro Controller Inputs
                    case Inputs.ProController.A     : leftMouseBtn  |= input.Value > 0f; break;
                    case Inputs.ProController.B     : rightMouseBtn |= input.Value > 0f; break;
                    case Inputs.ProController.X     : break;
                    case Inputs.ProController.Y     : break;

                    case Inputs.ProController.L     : altKey        |= input.Value > 0f; break;
                    case Inputs.ProController.R     : tabKey        |= input.Value > 0f; break;
                    case Inputs.ProController.ZL    : ctrlKey       |= input.Value > 0f; break;
                    case Inputs.ProController.ZR    : shiftKey      |= input.Value > 0f; break;

                    case Inputs.ProController.UP    : upKey         |= input.Value > 0f; break;
                    case Inputs.ProController.DOWN  : downKey       |= input.Value > 0f; break;
                    case Inputs.ProController.LEFT  : leftKey       |= input.Value > 0f; break;
                    case Inputs.ProController.RIGHT : rightKey      |= input.Value > 0f; break;

                    case Inputs.ProController.LUP   : moveMouseY    += input.Value; break;
                    case Inputs.ProController.LDOWN : moveMouseY    -= input.Value; break;
                    case Inputs.ProController.LLEFT : moveMouseX    -= input.Value; break;
                    case Inputs.ProController.LRIGHT: moveMouseX    += input.Value; break;
                    case Inputs.ProController.LS    : break;

                    case Inputs.ProController.RUP   : upKey         |= input.Value > 0f; break;
                    case Inputs.ProController.RDOWN : downKey       |= input.Value > 0f; break;
                    case Inputs.ProController.RLEFT : leftKey       |= input.Value > 0f; break;
                    case Inputs.ProController.RRIGHT: rightKey      |= input.Value > 0f; break;
                    case Inputs.ProController.RS    : break;

                    case Inputs.ProController.START : break;
                    case Inputs.ProController.SELECT: break;
                    case Inputs.ProController.HOME  : InMouseMode   = false; break; // might need some sort of delay
                    #endregion

                    #region Wiimote Inputs
                    case Inputs.Wiimote.A     : leftMouseBtn  |= input.Value > 0f; break;
                    case Inputs.Wiimote.B     : rightMouseBtn |= input.Value > 0f; break;
                    case Inputs.Wiimote.ONE   : altKey        |= input.Value > 0f; break;
                    case Inputs.Wiimote.TWO   : tabKey        |= input.Value > 0f; break;

                    case Inputs.Wiimote.UP    : upKey         |= input.Value > 0f; break;
                    case Inputs.Wiimote.DOWN  : downKey       |= input.Value > 0f; break;
                    case Inputs.Wiimote.LEFT  : leftKey       |= input.Value > 0f; break;
                    case Inputs.Wiimote.RIGHT : rightKey      |= input.Value > 0f; break;

                    case Inputs.Wiimote.PLUS  : ctrlKey       |= input.Value > 0f; break;
                    case Inputs.Wiimote.MINUS : shiftKey      |= input.Value > 0f; break;
                    case Inputs.Wiimote.HOME  : InMouseMode = false; break;
                    #endregion

                    #region Nunchuk Inputs
                    case Inputs.Nunchuk.C     : ctrlKey       |= input.Value > 0f; break;
                    case Inputs.Nunchuk.Z     : shiftKey      |= input.Value > 0f; break;

                    case Inputs.Nunchuk.UP    : moveMouseY    += input.Value; break;
                    case Inputs.Nunchuk.DOWN  : moveMouseY    -= input.Value; break;
                    case Inputs.Nunchuk.LEFT  : moveMouseX    -= input.Value; break;
                    case Inputs.Nunchuk.RIGHT : moveMouseX    += input.Value; break;
                    #endregion

                    #region Classic Controller Inputs
                    case Inputs.ClassicController.A     : leftMouseBtn  |= input.Value > 0f; break;
                    case Inputs.ClassicController.B     : rightMouseBtn |= input.Value > 0f; break;
                    case Inputs.ClassicController.X     : break;
                    case Inputs.ClassicController.Y     : break;

                    case Inputs.ClassicController.L     : altKey        |= input.Value > 0f; break;
                    case Inputs.ClassicController.R     : tabKey        |= input.Value > 0f; break;
                    case Inputs.ClassicController.ZL    : ctrlKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.ZR    : shiftKey      |= input.Value > 0f; break;

                    case Inputs.ClassicController.UP    : upKey         |= input.Value > 0f; break;
                    case Inputs.ClassicController.DOWN  : downKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.LEFT  : leftKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.RIGHT : rightKey      |= input.Value > 0f; break;

                    case Inputs.ClassicController.LUP   : moveMouseY    += input.Value; break;
                    case Inputs.ClassicController.LDOWN : moveMouseY    -= input.Value; break;
                    case Inputs.ClassicController.LLEFT : moveMouseX    -= input.Value; break;
                    case Inputs.ClassicController.LRIGHT: moveMouseX    += input.Value; break;

                    case Inputs.ClassicController.RUP   : upKey         |= input.Value > 0f; break;
                    case Inputs.ClassicController.RDOWN : downKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.RLEFT : leftKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.RRIGHT: rightKey      |= input.Value > 0f; break;

                    case Inputs.ClassicController.START : break;
                    case Inputs.ClassicController.SELECT: break;
                    case Inputs.ClassicController.HOME  : InMouseMode = false; break; // might need some sort of delay
                    #endregion

                    #region Classic Controller Pro Inputs
                    case Inputs.ClassicControllerPro.A     : leftMouseBtn  |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.B     : rightMouseBtn |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.X     : break;
                    case Inputs.ClassicControllerPro.Y     : break;

                    case Inputs.ClassicControllerPro.L     : altKey        |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.R     : tabKey        |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.ZL    : ctrlKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.ZR    : shiftKey      |= input.Value > 0f; break;

                    case Inputs.ClassicControllerPro.UP    : upKey         |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.DOWN  : downKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.LEFT  : leftKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.RIGHT : rightKey      |= input.Value > 0f; break;

                    case Inputs.ClassicControllerPro.LUP   : moveMouseY    += input.Value; break;
                    case Inputs.ClassicControllerPro.LDOWN : moveMouseY    -= input.Value; break;
                    case Inputs.ClassicControllerPro.LLEFT : moveMouseX    -= input.Value; break;
                    case Inputs.ClassicControllerPro.LRIGHT: moveMouseX    += input.Value; break;

                    case Inputs.ClassicControllerPro.RUP   : upKey         |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.RDOWN : downKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.RLEFT : leftKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.RRIGHT: rightKey      |= input.Value > 0f; break;

                    case Inputs.ClassicControllerPro.START : break;
                    case Inputs.ClassicControllerPro.SELECT: break;
                    case Inputs.ClassicControllerPro.HOME  : InMouseMode = false; break; // might need some sort of delay
                    #endregion
                }
            }

            // TODO: Apply the movements
        }
    }
}
