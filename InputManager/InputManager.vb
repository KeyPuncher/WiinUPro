Imports System.Windows.Forms
Imports System.Runtime
Imports System.Runtime.InteropServices
Imports System.Threading
    ''' <summary>
    ''' Provide methods to send keyboard input that also works in DirectX games.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Keyboard
#Region "API Declaring"
#Region "SendInput"
        Private Declare Function SendInput Lib "user32.dll" (ByVal cInputs As Integer, ByRef pInputs As INPUT, ByVal cbSize As Integer) As Integer
        Private Structure INPUT
            Dim dwType As Integer
            Dim mkhi As MOUSEKEYBDHARDWAREINPUT
        End Structure

        Private Structure KEYBDINPUT
            Public wVk As Short
            Public wScan As Short
            Public dwFlags As Integer
            Public time As Integer
            Public dwExtraInfo As IntPtr
        End Structure

        Private Structure HARDWAREINPUT
            Public uMsg As Integer
            Public wParamL As Short
            Public wParamH As Short
        End Structure

        <StructLayout(LayoutKind.Explicit)> _
    Private Structure MOUSEKEYBDHARDWAREINPUT
            <FieldOffset(0)> Public mi As MOUSEINPUT
            <FieldOffset(0)> Public ki As KEYBDINPUT
            <FieldOffset(0)> Public hi As HARDWAREINPUT
        End Structure

        Private Structure MOUSEINPUT
            Public dx As Integer
            Public dy As Integer
            Public mouseData As Integer
            Public dwFlags As Integer
            Public time As Integer
            Public dwExtraInfo As IntPtr
        End Structure

        Const INPUT_MOUSE As UInt32 = 0
        Const INPUT_KEYBOARD As Integer = 1
        Const INPUT_HARDWARE As Integer = 2
        Const KEYEVENTF_EXTENDEDKEY As UInt32 = &H1
        Const KEYEVENTF_KEYUP As UInt32 = &H2
        Const KEYEVENTF_UNICODE As UInt32 = &H4
        Const KEYEVENTF_SCANCODE As UInt32 = &H8
        Const XBUTTON1 As UInt32 = &H1
        Const XBUTTON2 As UInt32 = &H2
        Const MOUSEEVENTF_MOVE As UInt32 = &H1
        Const MOUSEEVENTF_LEFTDOWN As UInt32 = &H2
        Const MOUSEEVENTF_LEFTUP As UInt32 = &H4
        Const MOUSEEVENTF_RIGHTDOWN As UInt32 = &H8
        Const MOUSEEVENTF_RIGHTUP As UInt32 = &H10
        Const MOUSEEVENTF_MIDDLEDOWN As UInt32 = &H20
        Const MOUSEEVENTF_MIDDLEUP As UInt32 = &H40
        Const MOUSEEVENTF_XDOWN As UInt32 = &H80
        Const MOUSEEVENTF_XUP As UInt32 = &H100
        Const MOUSEEVENTF_WHEEL As UInt32 = &H800
        Const MOUSEEVENTF_VIRTUALDESK As UInt32 = &H4000
        Const MOUSEEVENTF_ABSOLUTE As UInt32 = &H8000
#End Region
        Private Declare Auto Function MapVirtualKey Lib "user32.dll" (ByVal uCode As UInt32, ByVal uMapType As MapVirtualKeyMapTypes) As UInt32
        Private Declare Auto Function MapVirtualKeyEx Lib "user32.dll" (ByVal uCode As UInt32, ByVal uMapType As MapVirtualKeyMapTypes, ByVal dwhkl As IntPtr) As UInt32
        Private Declare Auto Function GetKeyboardLayout Lib "user32.dll" (ByVal idThread As UInteger) As IntPtr
        ''' <summary>The set of valid MapTypes used in MapVirtualKey
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum MapVirtualKeyMapTypes As UInt32
            ''' <summary>uCode is a virtual-key code and is translated into a scan code.
            ''' If it is a virtual-key code that does not distinguish between left- and
            ''' right-hand keys, the left-hand scan code is returned.
            ''' If there is no translation, the function returns 0.
            ''' </summary>
            ''' <remarks></remarks>
            MAPVK_VK_TO_VSC = &H0

            ''' <summary>uCode is a scan code and is translated into a virtual-key code that
            ''' does not distinguish between left- and right-hand keys. If there is no
            ''' translation, the function returns 0.
            ''' </summary>
            ''' <remarks></remarks>
            MAPVK_VSC_TO_VK = &H1

            ''' <summary>uCode is a virtual-key code and is translated into an unshifted
            ''' character value in the low-order word of the return value. Dead keys (diacritics)
            ''' are indicated by setting the top bit of the return value. If there is no
            ''' translation, the function returns 0.
            ''' </summary>
            ''' <remarks></remarks>
            MAPVK_VK_TO_CHAR = &H2

            ''' <summary>Windows NT/2000/XP: uCode is a scan code and is translated into a
            ''' virtual-key code that distinguishes between left- and right-hand keys. If
            ''' there is no translation, the function returns 0.
            ''' </summary>
            ''' <remarks></remarks>
            MAPVK_VSC_TO_VK_EX = &H3

            ''' <summary>Not currently documented
            ''' </summary>
            ''' <remarks></remarks>
            MAPVK_VK_TO_VSC_EX = &H4
        End Enum
#End Region
        Private Shared Function GetScanKey(ByVal VKey As UInteger) As ScanKey
            Dim ScanCode As UInteger = MapVirtualKey(VKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC)
            Dim Extended As Boolean = (VKey = Keys.RMenu Or VKey = Keys.RControlKey Or VKey = Keys.Left Or VKey = Keys.Right Or VKey = Keys.Up Or VKey = Keys.Down Or VKey = Keys.Home Or VKey = Keys.Delete Or VKey = Keys.PageUp Or VKey = Keys.PageDown Or VKey = Keys.End Or VKey = Keys.Insert Or VKey = Keys.NumLock Or VKey = Keys.PrintScreen Or VKey = Keys.Divide)
            Return New ScanKey(ScanCode, Extended)
        End Function
        Private Structure ScanKey
            Dim ScanCode As UInteger
            Dim Extended As Boolean
            Public Sub New(ByVal sCode As UInteger, Optional ByVal ex As Boolean = False)
                ScanCode = sCode
                Extended = ex
            End Sub
        End Structure
        ''' <summary>
        ''' Sends shortcut keys (key down and up) signals.
        ''' </summary>
        ''' <param name="kCode">The array of keys to send as a shortcut.</param>
        ''' <param name="Delay">The delay in milliseconds between the key down and up events.</param>
        ''' <remarks></remarks>
        Public Shared Sub ShortcutKeys(ByVal kCode() As Keys, Optional ByVal Delay As Integer = 0)
            Dim KeysPress As New KeyPressStruct(kCode, Delay)
            Dim t As New Thread(New ParameterizedThreadStart(AddressOf KeyPressThread))
            t.Start(KeysPress)
        End Sub
        ''' <summary>
        ''' Sends a key down signal.
        ''' </summary>
        ''' <param name="kCode">The virtual keycode to send.</param>
        ''' <remarks></remarks>
    Public Shared Sub KeyDown(ByVal kCode As UInteger)
        Dim sKey As ScanKey = GetScanKey(kCode)
        Dim input As New INPUT()
        input.dwType = INPUT_KEYBOARD
        input.mkhi.ki = New KEYBDINPUT()
        input.mkhi.ki.wScan = sKey.ScanCode
        input.mkhi.ki.dwExtraInfo = IntPtr.Zero
        input.mkhi.ki.dwFlags = KEYEVENTF_SCANCODE Or IIf(sKey.Extended, KEYEVENTF_EXTENDEDKEY, Nothing)
        Dim cbSize As Integer = Marshal.SizeOf(GetType(INPUT))
        SendInput(1, input, cbSize)
    End Sub
        ''' <summary>
        ''' Sends a key up signal.
        ''' </summary>
        ''' <param name="kCode">The virtual keycode to send.</param>
        ''' <remarks></remarks>
    Public Shared Sub KeyUp(ByVal kCode As UInteger)
        Dim sKey As ScanKey = GetScanKey(kCode)
        Dim input As New INPUT()
        input.dwType = INPUT_KEYBOARD
        input.mkhi.ki = New KEYBDINPUT()
        input.mkhi.ki.wScan = sKey.ScanCode
        input.mkhi.ki.dwExtraInfo = IntPtr.Zero
        input.mkhi.ki.dwFlags = KEYEVENTF_SCANCODE Or KEYEVENTF_KEYUP Or IIf(sKey.Extended, KEYEVENTF_EXTENDEDKEY, Nothing)
        Dim cbSize As Integer = Marshal.SizeOf(GetType(INPUT))
        SendInput(1, input, cbSize)
    End Sub
        ''' <summary>
        ''' Sends a key press signal (key down and up).
        ''' </summary>
        ''' <param name="kCode">The virtual keycode to send.</param>
        ''' <param name="Delay">The delay to set between the key down and up commands.</param>
        ''' <remarks></remarks>
        Public Shared Sub KeyPress(ByVal kCode As Keys, Optional ByVal Delay As Integer = 0)
            Dim SendKeys() As Keys = {kCode}
            Dim KeysPress As New KeyPressStruct(SendKeys, Delay)
            Dim t As New Thread(New ParameterizedThreadStart(AddressOf KeyPressThread))
            t.Start(KeysPress)
        End Sub
        Private Shared Sub KeyPressThread(ByVal KeysP As KeyPressStruct)
            For Each k As Keys In KeysP.Keys
                KeyDown(k)
            Next
            If KeysP.Delay > 0 Then Thread.Sleep(KeysP.Delay)
            For Each k As Keys In KeysP.Keys
                KeyUp(k)
            Next
        End Sub
        Private Structure KeyPressStruct
            Dim Keys() As Keys
            Dim Delay As Integer
            Public Sub New(ByVal KeysToPress() As Keys, Optional ByVal DelayTime As Integer = 0)
                Keys = KeysToPress
                Delay = DelayTime
            End Sub
        End Structure
    End Class
    ''' <summary>
    ''' Provides methods to send keyboard input. The keys are being sent virtually and cannot be used with DirectX.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class VirtualKeyboard
#Region "API Declaring"
        <DllImport("user32.dll", CallingConvention:=CallingConvention.StdCall, _
               CharSet:=CharSet.Unicode, EntryPoint:="keybd_event", _
               ExactSpelling:=True, SetLastError:=True)> _
    Public Shared Function keybd_event(ByVal bVk As Int32, ByVal bScan As Int32, _
                                  ByVal dwFlags As Int32, ByVal dwExtraInfo As Int32) As Boolean
        End Function
        Const KEYEVENTF_EXTENDEDKEY = &H1
        Const KEYEVENTF_KEYUP = &H2
#End Region
        ''' <summary>
        ''' Sends shortcut keys (key down and up) signals.
        ''' </summary>
        ''' <param name="kCode">The array of keys to send as a shortcut.</param>
        ''' <param name="Delay">The delay in milliseconds between the key down and up events.</param>
        ''' <remarks></remarks>
        Public Shared Sub ShortcutKeys(ByVal kCode() As Keys, Optional ByVal Delay As Integer = 0)
            Dim KeyPress As New KeyPressStruct(kCode, Delay)
            Dim t As New Thread(New ParameterizedThreadStart(AddressOf KeyPressThread))
            t.Start(KeyPress)
        End Sub
        ''' <summary>
        ''' Sends a key down signal.
        ''' </summary>
        ''' <param name="kCode">The virtual keycode to send.</param>
        ''' <remarks></remarks>
        Public Shared Sub KeyDown(ByVal kCode As Keys)
            keybd_event(kCode, 0, 0, 0)
        End Sub
        ''' <summary>
        ''' Sends a key up signal.
        ''' </summary>
        ''' <param name="kCode">The virtual keycode to send.</param>
        ''' <remarks></remarks>
        Public Shared Sub KeyUp(ByVal kCode As Keys)
            keybd_event(kCode, 0, KEYEVENTF_KEYUP, 0)
        End Sub
        ''' <summary>
        ''' Sends a key press signal (key down and up).
        ''' </summary>
        ''' <param name="kCode">The virtual key code to send.</param>
        ''' <param name="Delay">The delay to set between the key down and up commands.</param>
        ''' <remarks></remarks>
        Public Shared Sub KeyPress(ByVal kCode As Keys, Optional ByVal Delay As Integer = 0)
            Dim SendKeys() As Keys = {kCode}
            Dim KeyPress As New KeyPressStruct(SendKeys, Delay)
            Dim t As New Thread(New ParameterizedThreadStart(AddressOf KeyPressThread))
            t.Start(KeyPress)
        End Sub
        Private Shared Sub KeyPressThread(ByVal KeysP As KeyPressStruct)
            For Each k As Keys In KeysP.Keys
                KeyDown(k)
            Next
            If KeysP.Delay > 0 Then Thread.Sleep(KeysP.Delay)
            For Each k As Keys In KeysP.Keys
                KeyUp(k)
            Next
        End Sub
        Private Structure KeyPressStruct
            Dim Keys() As Keys
            Dim Delay As Integer
            Public Sub New(ByVal KeysToPress() As Keys, Optional ByVal DelayTime As Integer = 0)
                Keys = KeysToPress
                Delay = DelayTime
            End Sub
        End Structure
    End Class
    ''' <summary>
    ''' Provides methods to send mouse input that also works in DirectX games.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Mouse
#Region "APIs"
        Private Declare Auto Function GetSystemMetrics Lib "user32.dll" (ByVal smIndex As Integer) As Integer
        Private Const SM_SWAPBUTTON As Integer = 23
#Region "SendInput"
        Private Declare Function SendInput Lib "user32.dll" (ByVal cInputs As Integer, ByRef pInputs As INPUT, ByVal cbSize As Integer) As Integer
        Private Structure INPUT
            Dim dwType As Integer
            Dim mkhi As MOUSEKEYBDHARDWAREINPUT
        End Structure

        Private Structure KEYBDINPUT
            Public wVk As Short
            Public wScan As Short
            Public dwFlags As Integer
            Public time As Integer
            Public dwExtraInfo As IntPtr
        End Structure

        Private Structure HARDWAREINPUT
            Public uMsg As Integer
            Public wParamL As Short
            Public wParamH As Short
        End Structure

        <StructLayout(LayoutKind.Explicit)> _
    Private Structure MOUSEKEYBDHARDWAREINPUT
            <FieldOffset(0)> Public mi As MOUSEINPUT
            <FieldOffset(0)> Public ki As KEYBDINPUT
            <FieldOffset(0)> Public hi As HARDWAREINPUT
        End Structure

        Private Structure MOUSEINPUT
            Public dx As Integer
            Public dy As Integer
            Public mouseData As Integer
            Public dwFlags As Integer
            Public time As Integer
            Public dwExtraInfo As IntPtr
        End Structure

        Const INPUT_MOUSE As UInt32 = 0
        Const INPUT_KEYBOARD As Integer = 1
        Const INPUT_HARDWARE As Integer = 2
        Const KEYEVENTF_EXTENDEDKEY As UInt32 = &H1
        Const KEYEVENTF_KEYUP As UInt32 = &H2
        Const KEYEVENTF_UNICODE As UInt32 = &H4
        Const KEYEVENTF_SCANCODE As UInt32 = &H8
        Const XBUTTON1 As UInt32 = &H1
        Const XBUTTON2 As UInt32 = &H2
        Const MOUSEEVENTF_MOVE As UInt32 = &H1
        Const MOUSEEVENTF_LEFTDOWN As UInt32 = &H2
        Const MOUSEEVENTF_LEFTUP As UInt32 = &H4
        Const MOUSEEVENTF_RIGHTDOWN As UInt32 = &H8
        Const MOUSEEVENTF_RIGHTUP As UInt32 = &H10
        Const MOUSEEVENTF_MIDDLEDOWN As UInt32 = &H20
        Const MOUSEEVENTF_MIDDLEUP As UInt32 = &H40
        Const MOUSEEVENTF_XDOWN As UInt32 = &H80
        Const MOUSEEVENTF_XUP As UInt32 = &H100
        Const MOUSEEVENTF_WHEEL As UInt32 = &H800
        Const MOUSEEVENTF_VIRTUALDESK As UInt32 = &H4000
        Const MOUSEEVENTF_ABSOLUTE As UInt32 = &H8000
#End Region
#End Region
        Public Enum MouseButtons
            LeftDown = &H2
            LeftUp = &H4
            RightDown = &H8
            RightUp = &H10
            MiddleDown = &H20
            MiddleUp = &H40
            Absolute = &H8000
            Wheel = &H800
        End Enum
        ''' <summary>
        ''' Returns true if mouse buttons are swapped
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property IsLeftHanded() As Boolean
            Get
                Try
                    Return (GetSystemMetrics(SM_SWAPBUTTON) = 1)
                Catch ex As Exception
                    Return False
                End Try
            End Get
        End Property
        ''' <summary>
        ''' Sends a mouse button signal. To send a scroll use the Scroll method.
        ''' </summary>
        ''' <param name="mButton">The button to send.</param>
        ''' <remarks></remarks>
        Public Shared Sub SendButton(ByVal mButton As MouseButtons)
            Dim input As New INPUT()
            input.dwType = INPUT_MOUSE
            input.mkhi.mi = New MOUSEINPUT()
            input.mkhi.mi.dwExtraInfo = IntPtr.Zero
            input.mkhi.mi.dwFlags = mButton
            input.mkhi.mi.dx = 0
            input.mkhi.mi.dy = 0
            Dim cbSize As Integer = Marshal.SizeOf(GetType(INPUT))
            SendInput(1, input, cbSize)
        End Sub
        ''' <summary>
        ''' Sends a mouse press signal (down and up).
        ''' </summary>
        ''' <param name="mKey">The key to press.</param>
        ''' <param name="Delay">The delay to set between the events.</param>
        ''' <remarks></remarks>
        Public Shared Sub PressButton(ByVal mKey As MouseKeys, Optional ByVal Delay As Integer = 0)
            ButtonDown(mKey)
            If Delay > 0 Then System.Threading.Thread.Sleep(Delay)
            ButtonUp(mKey)
        End Sub
        ''' <summary>
        ''' Send a mouse button down signal.
        ''' </summary>
        ''' <param name="mKey">The mouse key to send as mouse button down.</param>
        ''' <remarks></remarks>
        Public Shared Sub ButtonDown(ByVal mKey As MouseKeys)
            Select Case mKey
                Case MouseKeys.Left
                    SendButton(MouseButtons.LeftDown)
                Case MouseKeys.Right
                    SendButton(MouseButtons.RightDown)
                Case MouseKeys.Middle
                    SendButton(MouseButtons.MiddleDown)
            End Select
        End Sub
        ''' <summary>
        ''' Send a mouse button up signal.
        ''' </summary>
        ''' <param name="mKey">The mouse key to send as mouse button up.</param>
        ''' <remarks></remarks>
        Public Shared Sub ButtonUp(ByVal mKey As MouseKeys)
            Select Case mKey
                Case MouseKeys.Left
                    SendButton(MouseButtons.LeftUp)
                Case MouseKeys.Right
                    SendButton(MouseButtons.RightUp)
                Case MouseKeys.Middle
                    SendButton(MouseButtons.MiddleUp)
            End Select
        End Sub
        ''' <summary>
        ''' Moves the mouse to a certain location on the screen.
        ''' </summary>
        ''' <param name="X">The x location to move the mouse.</param>
        ''' <param name="Y">The y location to move the mouse</param>
        ''' <remarks></remarks>
        Public Shared Sub Move(ByVal X As Integer, ByVal Y As Integer)
            Dim input As New INPUT()
            input.dwType = INPUT_MOUSE
            input.mkhi.mi = New MOUSEINPUT()
            input.mkhi.mi.dwExtraInfo = IntPtr.Zero
            input.mkhi.mi.dwFlags = MOUSEEVENTF_ABSOLUTE + MOUSEEVENTF_MOVE
            input.mkhi.mi.dx = X * (65535 / Screen.PrimaryScreen.Bounds.Width)
            input.mkhi.mi.dy = Y * (65535 / Screen.PrimaryScreen.Bounds.Height)
            Dim cbSize As Integer = Marshal.SizeOf(GetType(INPUT))
            SendInput(1, input, cbSize)
        End Sub
        ''' <summary>
        ''' Moves the mouse to a location relative to the current one.
        ''' </summary>
        ''' <param name="X">The amount of pixels to move the mouse on the x axis.</param>
        ''' <param name="Y">The amount of pixels to move the mouse on the y axis.</param>
        ''' <remarks></remarks>
        Public Shared Sub MoveRelative(ByVal X As Integer, ByVal Y As Integer)
            Dim input As New INPUT()
            input.dwType = INPUT_MOUSE
            input.mkhi.mi = New MOUSEINPUT()
            input.mkhi.mi.dwExtraInfo = IntPtr.Zero
            input.mkhi.mi.dwFlags = MOUSEEVENTF_MOVE
            input.mkhi.mi.dx = X
            input.mkhi.mi.dy = Y
            Dim cbSize As Integer = Marshal.SizeOf(GetType(INPUT))
            SendInput(1, input, cbSize)
        End Sub
        Public Enum MouseKeys
            Left = -1
            Right = -2
            Middle = -3
        End Enum
        Public Enum ScrollDirection
            Up = 120
            Down = -120
        End Enum
        ''' <summary>
        ''' Sends a scroll signal with a specific direction to scroll.
        ''' </summary>
        ''' <param name="Direction">The direction to scroll.</param>
        ''' <remarks></remarks>
        Public Shared Sub Scroll(ByVal Direction As ScrollDirection)
            Dim input As New INPUT()
            input.dwType = INPUT_MOUSE
            input.mkhi.mi = New MOUSEINPUT()
            input.mkhi.mi.dwExtraInfo = IntPtr.Zero
            input.mkhi.mi.dwFlags = MouseButtons.Wheel
            input.mkhi.mi.mouseData = Direction
            input.mkhi.mi.dx = 0
            input.mkhi.mi.dy = 0
            Dim cbSize As Integer = Marshal.SizeOf(GetType(INPUT))
            SendInput(1, input, cbSize)
        End Sub
    End Class
    ''' <summary>
''' Creates a low level keyboard hook.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class KeyboardHook
#Region "Constants and API's Declaring"
        ''This is Constants
        Private Const HC_ACTION As Integer = 0
        Private Const WH_KEYBOARD_LL As Integer = 13
        Private Const WM_KEYDOWN = &H100
        Private Const WM_KEYUP = &H101
        Private Const WM_SYSKEYDOWN = &H104
        Private Const WM_SYSKEYUP = &H105
        Private Const WM_KEYLAST = &H108
        ''Key press events
        Public Structure KBDLLHOOKSTRUCT
            Public vkCode As Integer
            Public scancode As Integer
            Public flags As Integer
            Public time As Integer
            Public dwExtraInfo As Integer
        End Structure
        ''API Functions
        Private Declare Function SetWindowsHookEx Lib "user32" _
        Alias "SetWindowsHookExA" _
        (ByVal idHook As Integer, _
        ByVal lpfn As KeyboardProcDelegate, _
        ByVal hmod As Integer, _
        ByVal dwThreadId As Integer) As Integer

        Private Declare Function CallNextHookEx Lib "user32" _
    (ByVal hHook As Integer, _
    ByVal nCode As Integer, _
    ByVal wParam As Integer, _
    ByVal lParam As KBDLLHOOKSTRUCT) As Integer

        Private Declare Function UnhookWindowsHookEx Lib "user32" _
    (ByVal hHook As Integer) As Integer

        ''Our Keyboard Delegate
        Private Delegate Function KeyboardProcDelegate _
        (ByVal nCode As Integer, _
        ByVal wParam As Integer, _
        ByRef lParam As KBDLLHOOKSTRUCT) As Integer

        ''The KeyPress events
        Public Shared Event KeyDown(ByVal vkCode As Integer)
        Public Shared Event KeyUp(ByVal vkCode As Integer)

        ''The identifyer for our KeyHook
        Private Shared KeyHook As Integer
        ''KeyHookDelegate
        Private Shared KeyHookDelegate As KeyboardProcDelegate
#End Region
        Private Shared Function KeyboardProc(ByVal nCode As Integer, ByVal wParam As Integer, ByRef lParam As KBDLLHOOKSTRUCT) As Integer
            'If it's a keyboard state event...
            If (nCode = HC_ACTION) Then
                Select Case wParam
                    Case WM_KEYDOWN, WM_SYSKEYDOWN 'If it's a keydown event...
                        RaiseEvent KeyDown(lParam.vkCode)
                    Case WM_KEYUP, WM_SYSKEYUP 'If it's a keyup event... 
                        RaiseEvent KeyUp(lParam.vkCode)
                End Select
            End If
            ''Next
            Return CallNextHookEx(KeyHook, nCode, wParam, lParam)
        End Function
        ''' <summary>
        ''' Installs low level keyboard hook. This hook raises events every time a keyboard event occured.
        ''' </summary>
        ''' <remarks></remarks>
    Public Shared Sub InstallHook()
        KeyHookDelegate = New KeyboardProcDelegate(AddressOf KeyboardProc)
        KeyHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyHookDelegate, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly.GetModules()(0)).ToInt32, 0)
    End Sub
        ''' <summary>
        ''' Uninstalls the low level keyboard hook. Hooks events are stopped from being raised.
        ''' </summary>
        ''' <remarks></remarks>
    Public Shared Sub UninstallHook()
        UnhookWindowsHookEx(KeyHook)
    End Sub

        Protected Overrides Sub Finalize()
            ''On close it UnHooks the Hook
            UnhookWindowsHookEx(KeyHook)
            MyBase.Finalize()
        End Sub
    End Class
    ''' <summary>
''' Creates a low level mouse hook.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MouseHook
#Region "Constants and API's Declaring"
        Public Enum MouseEvents
            LeftDown = &H201
            LeftUp = &H202
            LeftDoubleClick = &H203
            RightDown = &H204
            RightUp = &H205
            RightDoubleClick = &H206
            MiddleDown = &H207
            MiddleUp = &H208
            MiddleDoubleClick = &H209
            MouseScroll = &H20A
        End Enum
        Public Enum MouseWheelEvents
            ScrollUp = 7864320
            ScrollDown = -7864320
        End Enum


        ''Constants
        Private Const HC_ACTION As Integer = 0
        Private Const WH_MOUSE_LL As Integer = 14
        Private Const WM_MOUSEMOVE As Integer = &H200
        Private Const WM_LBUTTONDOWN As Integer = &H201
        Private Const WM_LBUTTONUP As Integer = &H202
        Private Const WM_LBUTTONDBLCLK As Integer = &H203
        Private Const WM_RBUTTONDOWN As Integer = &H204
        Private Const WM_RBUTTONUP As Integer = &H205
        Private Const WM_RBUTTONDBLCLK As Integer = &H206
        Private Const WM_MBUTTONDOWN As Integer = &H207
        Private Const WM_MBUTTONUP As Integer = &H208
        Private Const WM_MBUTTONDBLCLK As Integer = &H209
        Private Const WM_MOUSEWHEEL As Integer = &H20A

        ''Mouse Structures
        Public Structure POINT
            Public x As Integer
            Public y As Integer
        End Structure

        Public Structure MSLLHOOKSTRUCT
            Public pt As POINT
            Public mouseData As Integer
            Private flags As Integer
            Private time As Integer
            Private dwExtraInfo As Integer
        End Structure

        ''API Functions
        Private Declare Function SetWindowsHookEx Lib "user32" Alias "SetWindowsHookExA" (ByVal idHook As Integer, ByVal lpfn As MouseProcDelegate, ByVal hmod As Integer, ByVal dwThreadId As Integer) As Integer
        Private Declare Function CallNextHookEx Lib "user32" (ByVal hHook As Integer, ByVal nCode As Integer, ByVal wParam As Integer, ByVal lParam As MSLLHOOKSTRUCT) As Integer
        Private Declare Function UnhookWindowsHookEx Lib "user32" (ByVal hHook As Integer) As Integer

        ''Our Mouse Delegate
        Private Delegate Function MouseProcDelegate(ByVal nCode As Integer, ByVal wParam As Integer, ByRef lParam As MSLLHOOKSTRUCT) As Integer
        ''The Mouse events
        Public Shared Event MouseMove(ByVal ptLocat As POINT)
        Public Shared Event MouseEvent(ByVal mEvent As MouseEvents)
        Public Shared Event WheelEvent(ByVal wEvent As MouseWheelEvents)

        ''The identifyer for our MouseHook
        Private Shared MouseHook As Integer
        ''MouseHookDelegate
        Private Shared MouseHookDelegate As MouseProcDelegate
#End Region
        ''' <summary>
        ''' Installs low level mouse hook. This hook raises events every time a mouse event occured.
        ''' </summary>
        ''' <remarks></remarks>
    Public Shared Sub InstallHook()
        MouseHookDelegate = New MouseProcDelegate(AddressOf MouseProc)
        MouseHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookDelegate, Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly.GetModules()(0)).ToInt32, 0)
    End Sub
        ''' <summary>
        ''' Uninstalls the low level mouse hook. Hooks events are stopped from being raised.
        ''' </summary>
        ''' <remarks></remarks>
    Public Shared Sub UninstallHook()
        UnhookWindowsHookEx(MouseHook)
    End Sub

        Private Shared Function MouseProc(ByVal nCode As Integer, ByVal wParam As Integer, ByRef lParam As MSLLHOOKSTRUCT) As Integer
            ''If it is a Mouse event
            If (nCode = HC_ACTION) Then
                If wParam = WM_MOUSEMOVE Then
                    'If the mouse is moving...
                    RaiseEvent MouseMove(lParam.pt)
                ElseIf wParam = WM_LBUTTONDOWN Or wParam = WM_LBUTTONUP Or wParam = WM_LBUTTONDBLCLK Or wParam = WM_RBUTTONDOWN Or wParam = WM_RBUTTONUP Or wParam = WM_RBUTTONDBLCLK Or wParam = WM_MBUTTONDOWN Or wParam = WM_MBUTTONUP Or wParam = WM_MBUTTONDBLCLK Then
                    'If a mouse button event happend...
                    RaiseEvent MouseEvent(wParam)
                ElseIf wParam = WM_MOUSEWHEEL Then
                    'If the wheel moved...
                    RaiseEvent WheelEvent(lParam.mouseData)
                End If
            End If
            'Calls the next hook chain
            Return CallNextHookEx(MouseHook, nCode, wParam, lParam)
        End Function

        Protected Overrides Sub Finalize()
            UnhookWindowsHookEx(MouseHook)
            MyBase.Finalize()
        End Sub
    End Class
