using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if MouseMode
using WindowsInput;
#endif

namespace WiinUSoft.Holders
{
    public abstract class Holder
    {
        public System.Collections.Concurrent.ConcurrentDictionary<string, float> Values { get; protected set; }
        //public Dictionary<string, float> Values { get; protected set; }
        public Dictionary<string, string> Mappings { get; protected set; }
        public Dictionary<string, bool> Flags { get; protected set; }
        public float RumbleAmount { get; protected set; }
        
        
#if MouseMode
        public bool InMouseMode { get; protected set; }
        protected DateTime _mmLastTime = new DateTime(0);
        protected InputSimulator _inputSim;
#endif

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

        public void ClearAllValues()
        {
            Values.Clear();
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
        public abstract void AddMapping(NintrollerLib.ControllerType controller);

#if MouseMode
        protected void MouseModeCheck(bool pressed)
        {
            if (pressed && DateTime.Now.Subtract(_mmLastTime).TotalSeconds > 3)
            {
                _mmLastTime = DateTime.Now;
                InMouseMode = false;
            }
        }
        

        private DateTime lastTime = DateTime.Now;
        protected void ShowDesktop()
        {
            if (DateTime.Now.Subtract(lastTime).TotalSeconds > 2)
            {
                Type typeShell = Type.GetTypeFromProgID("Shell.Application");
                object objShell = Activator.CreateInstance(typeShell);
                typeShell.InvokeMember("MinimizeAll", System.Reflection.BindingFlags.InvokeMethod, null, objShell, null); // Call function MinimizeAll
            }
        }

        // used for basic mouse and keyboard actions (fixed mappings)
        protected void UpdateMouseMode()
        {
            SimulatedInput simInput = new SimulatedInput();

            foreach (KeyValuePair<string, float> input in Values)
            {
                switch (input.Key)
                {
                    #region Pro Controller Inputs
                    case Inputs.ProController.A     : simInput.leftMouseBtn  |= input.Value > 0f; break;
                    case Inputs.ProController.B     : simInput.rightMouseBtn |= input.Value > 0f; break;
                    case Inputs.ProController.X     : simInput.delKey        |= input.Value > 0f; break;
                    case Inputs.ProController.Y     : simInput.escKey        |= input.Value > 0f; break;

                    case Inputs.ProController.L     : simInput.altKey        |= input.Value > 0f; break;
                    case Inputs.ProController.R     : simInput.tabKey        |= input.Value > 0f; break;
                    case Inputs.ProController.ZL    : simInput.ctrlKey       |= input.Value > 0f; break;
                    case Inputs.ProController.ZR    : simInput.shiftKey      |= input.Value > 0f; break;

                    case Inputs.ProController.UP    : simInput.upKey         |= input.Value > 0f; break;
                    case Inputs.ProController.DOWN  : simInput.downKey       |= input.Value > 0f; break;
                    case Inputs.ProController.LEFT  : simInput.leftKey       |= input.Value > 0f; break;
                    case Inputs.ProController.RIGHT : simInput.rightKey      |= input.Value > 0f; break;

                    case Inputs.ProController.LUP   : simInput.moveMouseY    += 6 * input.Value; break;
                    case Inputs.ProController.LDOWN : simInput.moveMouseY    -= 6 * input.Value; break;
                    case Inputs.ProController.LLEFT : simInput.moveMouseX    -= 6 * input.Value; break;
                    case Inputs.ProController.LRIGHT: simInput.moveMouseX    += 6 * input.Value; break;
                    case Inputs.ProController.LS    : simInput.leftMouseBtn  |= input.Value > 0f; break;

                    case Inputs.ProController.RUP   : simInput.upKey         |= input.Value > 0.1f; break;
                    case Inputs.ProController.RDOWN : simInput.downKey       |= input.Value > 0.1f; break;
                    case Inputs.ProController.RLEFT : simInput.leftKey       |= input.Value > 0.1f; break;
                    case Inputs.ProController.RRIGHT: simInput.rightKey      |= input.Value > 0.1f; break;
                    case Inputs.ProController.RS    : simInput.rightMouseBtn |= input.Value > 0f; break;

                    case Inputs.ProController.START : simInput.enterKey      |= input.Value > 0f; break;
                    case Inputs.ProController.SELECT: simInput.desktop |= input.Value > 0f; break;
                    case Inputs.ProController.HOME: MouseModeCheck(input.Value > 0f); break; 
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
                    case Inputs.Wiimote.HOME  : MouseModeCheck(input.Value > 0f); break;
                    #endregion

                    #region Nunchuk Inputs
                    case Inputs.Nunchuk.C     : simInput.ctrlKey       |= input.Value > 0f; break;
                    case Inputs.Nunchuk.Z     : simInput.shiftKey      |= input.Value > 0f; break;

                    case Inputs.Nunchuk.UP    : simInput.moveMouseY    += 6 * input.Value; break;
                    case Inputs.Nunchuk.DOWN  : simInput.moveMouseY    -= 6 * input.Value; break;
                    case Inputs.Nunchuk.LEFT  : simInput.moveMouseX    -= 6 * input.Value; break;
                    case Inputs.Nunchuk.RIGHT : simInput.moveMouseX    += 6 * input.Value; break;
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

                    case Inputs.ClassicController.LUP   : simInput.moveMouseY    += 6 * input.Value; break;
                    case Inputs.ClassicController.LDOWN : simInput.moveMouseY    -= 6 * input.Value; break;
                    case Inputs.ClassicController.LLEFT : simInput.moveMouseX    -= 6 * input.Value; break;
                    case Inputs.ClassicController.LRIGHT: simInput.moveMouseX    += 6 * input.Value; break;

                    case Inputs.ClassicController.RUP   : simInput.upKey         |= input.Value > 0.1f; break;
                    case Inputs.ClassicController.RDOWN : simInput.downKey       |= input.Value > 0.1f; break;
                    case Inputs.ClassicController.RLEFT : simInput.leftKey       |= input.Value > 0.1f; break;
                    case Inputs.ClassicController.RRIGHT: simInput.rightKey      |= input.Value > 0.1f; break;

                    case Inputs.ClassicController.START : break;
                    case Inputs.ClassicController.SELECT: break;
                    case Inputs.ClassicController.HOME: MouseModeCheck(input.Value > 0f); break; // might need some sort of delay
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

                    case Inputs.ClassicControllerPro.LUP   : simInput.moveMouseY    += 6 * input.Value; break;
                    case Inputs.ClassicControllerPro.LDOWN : simInput.moveMouseY    -= 6 * input.Value; break;
                    case Inputs.ClassicControllerPro.LLEFT : simInput.moveMouseX    -= 6 * input.Value; break;
                    case Inputs.ClassicControllerPro.LRIGHT: simInput.moveMouseX    += 6 * input.Value; break;

                    case Inputs.ClassicControllerPro.RUP   : simInput.upKey         |= input.Value > 0.1f; break;
                    case Inputs.ClassicControllerPro.RDOWN : simInput.downKey       |= input.Value > 0.1f; break;
                    case Inputs.ClassicControllerPro.RLEFT : simInput.leftKey       |= input.Value > 0.1f; break;
                    case Inputs.ClassicControllerPro.RRIGHT: simInput.rightKey      |= input.Value > 0.1f; break;

                    case Inputs.ClassicControllerPro.START : break;
                    case Inputs.ClassicControllerPro.SELECT: break;
                    case Inputs.ClassicControllerPro.HOME: MouseModeCheck(input.Value > 0f); break; // might need some sort of delay
                    #endregion
                }
            }

            #region Apply input
            if (simInput.desktop)
                ShowDesktop();

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
            if (simInput.escKey && !_lastInput.escKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            else if (!simInput.escKey && _lastInput.escKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.ESCAPE);

            if (simInput.altKey && !_lastInput.altKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.MENU);
            else if (!simInput.altKey && _lastInput.altKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.MENU);

            if (simInput.tabKey && !_lastInput.tabKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.TAB);
            else if (!simInput.tabKey && _lastInput.tabKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.TAB);

            if (simInput.ctrlKey && !_lastInput.ctrlKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL);
            else if (!simInput.ctrlKey && _lastInput.ctrlKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.CONTROL);

            if (simInput.shiftKey && !_lastInput.shiftKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.SHIFT);
            else if (!simInput.shiftKey && _lastInput.shiftKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.SHIFT);

            if (simInput.enterKey && !_lastInput.enterKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.RETURN);
            else if (!simInput.enterKey && _lastInput.enterKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.RETURN);

            if (simInput.delKey && !_lastInput.delKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.DELETE);
            else if (!simInput.delKey && _lastInput.delKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.DELETE);

            if (simInput.upKey && !_lastInput.upKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.UP);
            else if (!simInput.upKey && _lastInput.upKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.UP);

            if (simInput.downKey && !_lastInput.downKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.DOWN);
            else if (!simInput.downKey && _lastInput.downKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.DOWN);

            if (simInput.leftKey && !_lastInput.leftKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.LEFT);
            else if (!simInput.leftKey && _lastInput.leftKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.LEFT);

            if (simInput.rightKey && !_lastInput.rightKey) _inputSim.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.RIGHT);
            else if (!simInput.rightKey && _lastInput.rightKey) _inputSim.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.RIGHT);
            #endregion

            _lastInput = simInput;
        }

        protected SimulatedInput _lastInput = new SimulatedInput();

        protected struct SimulatedInput
        {
            public bool leftMouseBtn;
            public bool rightMouseBtn;
            public bool escKey;
            public bool altKey;
            public bool tabKey;
            public bool ctrlKey;
            public bool shiftKey;
            public bool enterKey;
            public bool delKey;

            public bool upKey;
            public bool downKey;
            public bool leftKey;
            public bool rightKey;

            public float moveMouseX;
            public float moveMouseY;
            public float mouseAbsX;
            public float mouseAbsY;

            public bool desktop;
        }
#endif
    }
}
