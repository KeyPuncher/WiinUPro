using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsInput;

namespace WiinUSoft.Holders
{
    public abstract class Holder
    {
        public System.Collections.Concurrent.ConcurrentDictionary<string, float> Values { get; protected set; }
        //public Dictionary<string, float> Values { get; protected set; }
        public Dictionary<string, string> Mappings { get; protected set; }
        public Dictionary<string, bool> Flags { get; protected set; }
        public bool InMouseMode { get; protected set; }

        protected int _modeChangeCount = 0;
        protected InputSimulator _inputSim;

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
            Console.WriteLine("Mouse Mode");
            SimulatedInput simInput = new SimulatedInput();

            foreach (KeyValuePair<string, float> input in Values)
            {
                switch (input.Key)
                {
                    #region Pro Controller Inputs
                    case Inputs.ProController.A     : simInput.leftMouseBtn  |= input.Value > 0f; break;
                    case Inputs.ProController.B     : simInput.rightMouseBtn |= input.Value > 0f; break;
                    case Inputs.ProController.X     : break;
                    case Inputs.ProController.Y     : break;

                    case Inputs.ProController.L     : simInput.altKey        |= input.Value > 0f; break;
                    case Inputs.ProController.R     : simInput.tabKey        |= input.Value > 0f; break;
                    case Inputs.ProController.ZL    : simInput.ctrlKey       |= input.Value > 0f; break;
                    case Inputs.ProController.ZR    : simInput.shiftKey      |= input.Value > 0f; break;

                    case Inputs.ProController.UP    : simInput.upKey         |= input.Value > 0f; break;
                    case Inputs.ProController.DOWN  : simInput.downKey       |= input.Value > 0f; break;
                    case Inputs.ProController.LEFT  : simInput.leftKey       |= input.Value > 0f; break;
                    case Inputs.ProController.RIGHT : simInput.rightKey      |= input.Value > 0f; break;

                    case Inputs.ProController.LUP   : simInput.moveMouseY    += input.Value; break;
                    case Inputs.ProController.LDOWN : simInput.moveMouseY    -= input.Value; break;
                    case Inputs.ProController.LLEFT : simInput.moveMouseX    -= input.Value; break;
                    case Inputs.ProController.LRIGHT: simInput.moveMouseX    += input.Value; break;
                    case Inputs.ProController.LS    : break;

                    case Inputs.ProController.RUP   : simInput.upKey         |= input.Value > 0f; break;
                    case Inputs.ProController.RDOWN : simInput.downKey       |= input.Value > 0f; break;
                    case Inputs.ProController.RLEFT : simInput.leftKey       |= input.Value > 0f; break;
                    case Inputs.ProController.RRIGHT: simInput.rightKey      |= input.Value > 0f; break;
                    case Inputs.ProController.RS    : break;

                    case Inputs.ProController.START : break;
                    case Inputs.ProController.SELECT: break;
                    case Inputs.ProController.HOME  : 
                        if (_modeChangeCount == 0)
                        {
                            _modeChangeCount = 100;
                            InMouseMode = !(input.Value > 0);
                        }
                        break; // might need some sort of delay
                    #endregion

                    #region Wiimote Inputs
                    case Inputs.Wiimote.A     : simInput.leftMouseBtn  |= input.Value > 0f; break;
                    case Inputs.Wiimote.B     : simInput.rightMouseBtn |= input.Value > 0f; break;
                    case Inputs.Wiimote.ONE   : simInput.altKey        |= input.Value > 0f; break;
                    case Inputs.Wiimote.TWO   : simInput.tabKey        |= input.Value > 0f; break;

                    case Inputs.Wiimote.UP    : simInput.upKey         |= input.Value > 0f; break;
                    case Inputs.Wiimote.DOWN  : simInput.downKey       |= input.Value > 0f; break;
                    case Inputs.Wiimote.LEFT  : simInput.leftKey       |= input.Value > 0f; break;
                    case Inputs.Wiimote.RIGHT : simInput.rightKey      |= input.Value > 0f; break;

                    case Inputs.Wiimote.PLUS  : simInput.ctrlKey       |= input.Value > 0f; break;
                    case Inputs.Wiimote.MINUS : simInput.shiftKey      |= input.Value > 0f; break;
                    case Inputs.Wiimote.HOME: InMouseMode = !(input.Value > 0); break;
                    #endregion

                    #region Nunchuk Inputs
                    case Inputs.Nunchuk.C     : simInput.ctrlKey       |= input.Value > 0f; break;
                    case Inputs.Nunchuk.Z     : simInput.shiftKey      |= input.Value > 0f; break;

                    case Inputs.Nunchuk.UP    : simInput.moveMouseY    += input.Value; break;
                    case Inputs.Nunchuk.DOWN  : simInput.moveMouseY    -= input.Value; break;
                    case Inputs.Nunchuk.LEFT  : simInput.moveMouseX    -= input.Value; break;
                    case Inputs.Nunchuk.RIGHT : simInput.moveMouseX    += input.Value; break;
                    #endregion

                    #region Classic Controller Inputs
                    case Inputs.ClassicController.A     : simInput.leftMouseBtn  |= input.Value > 0f; break;
                    case Inputs.ClassicController.B     : simInput.rightMouseBtn |= input.Value > 0f; break;
                    case Inputs.ClassicController.X     : break;
                    case Inputs.ClassicController.Y     : break;

                    case Inputs.ClassicController.L     : simInput.altKey        |= input.Value > 0f; break;
                    case Inputs.ClassicController.R     : simInput.tabKey        |= input.Value > 0f; break;
                    case Inputs.ClassicController.ZL    : simInput.ctrlKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.ZR    : simInput.shiftKey      |= input.Value > 0f; break;

                    case Inputs.ClassicController.UP    : simInput.upKey         |= input.Value > 0f; break;
                    case Inputs.ClassicController.DOWN  : simInput.downKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.LEFT  : simInput.leftKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.RIGHT : simInput.rightKey      |= input.Value > 0f; break;

                    case Inputs.ClassicController.LUP   : simInput.moveMouseY    += input.Value; break;
                    case Inputs.ClassicController.LDOWN : simInput.moveMouseY    -= input.Value; break;
                    case Inputs.ClassicController.LLEFT : simInput.moveMouseX    -= input.Value; break;
                    case Inputs.ClassicController.LRIGHT: simInput.moveMouseX    += input.Value; break;

                    case Inputs.ClassicController.RUP   : simInput.upKey         |= input.Value > 0f; break;
                    case Inputs.ClassicController.RDOWN : simInput.downKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.RLEFT : simInput.leftKey       |= input.Value > 0f; break;
                    case Inputs.ClassicController.RRIGHT: simInput.rightKey      |= input.Value > 0f; break;

                    case Inputs.ClassicController.START : break;
                    case Inputs.ClassicController.SELECT: break;
                    case Inputs.ClassicController.HOME: InMouseMode = !(input.Value > 0); break; // might need some sort of delay
                    #endregion

                    #region Classic Controller Pro Inputs
                    case Inputs.ClassicControllerPro.A     : simInput.leftMouseBtn  |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.B     : simInput.rightMouseBtn |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.X     : break;
                    case Inputs.ClassicControllerPro.Y     : break;

                    case Inputs.ClassicControllerPro.L     : simInput.altKey        |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.R     : simInput.tabKey        |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.ZL    : simInput.ctrlKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.ZR    : simInput.shiftKey      |= input.Value > 0f; break;

                    case Inputs.ClassicControllerPro.UP    : simInput.upKey         |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.DOWN  : simInput.downKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.LEFT  : simInput.leftKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.RIGHT : simInput.rightKey      |= input.Value > 0f; break;

                    case Inputs.ClassicControllerPro.LUP   : simInput.moveMouseY    += input.Value; break;
                    case Inputs.ClassicControllerPro.LDOWN : simInput.moveMouseY    -= input.Value; break;
                    case Inputs.ClassicControllerPro.LLEFT : simInput.moveMouseX    -= input.Value; break;
                    case Inputs.ClassicControllerPro.LRIGHT: simInput.moveMouseX    += input.Value; break;

                    case Inputs.ClassicControllerPro.RUP   : simInput.upKey         |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.RDOWN : simInput.downKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.RLEFT : simInput.leftKey       |= input.Value > 0f; break;
                    case Inputs.ClassicControllerPro.RRIGHT: simInput.rightKey      |= input.Value > 0f; break;

                    case Inputs.ClassicControllerPro.START : break;
                    case Inputs.ClassicControllerPro.SELECT: break;
                    case Inputs.ClassicControllerPro.HOME: InMouseMode = !(input.Value > 0); break; // might need some sort of delay
                    #endregion
                }
            }

            // TODO: Apply the movements
            // Mouse
            _inputSim.Mouse.MoveMouseBy((int)simInput.moveMouseX, (int)simInput.moveMouseY * -1);

            if (simInput.leftMouseBtn && !_lastInput.leftMouseBtn)
                _inputSim.Mouse.LeftButtonDown();
            else if (!simInput.leftMouseBtn && _lastInput.leftMouseBtn)
                _inputSim.Mouse.LeftButtonUp();

            if (simInput.rightMouseBtn && !_lastInput.rightMouseBtn)
                _inputSim.Mouse.RightButtonDown();
            else if (!simInput.rightMouseBtn && _lastInput.rightMouseBtn)
                _inputSim.Mouse.RightButtonUp();

            // Keyboard


            _lastInput = simInput;
        }

        protected SimulatedInput _lastInput = new SimulatedInput();

        protected struct SimulatedInput
        {
            public bool leftMouseBtn ;
            public bool rightMouseBtn;
            public bool escKey       ;
            public bool altKey       ;
            public bool tabKey       ;
            public bool ctrlKey      ;
            public bool shiftKey     ;
            public bool enterKey     ;

            public bool upKey        ;
            public bool downKey      ;
            public bool leftKey      ;
            public bool rightKey     ;

            public float moveMouseX  ;
            public float moveMouseY  ;
            public float mouseAbsX   ;
            public float mouseAbsY   ;
        }
    }
}
