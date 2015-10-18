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
        Dim Extended As Boolean = (VKey = VirtualKeyCode.VK_RMENU Or VKey = VirtualKeyCode.VK_RCONTROL Or VKey = VirtualKeyCode.VK_LEFT Or VKey = VirtualKeyCode.VK_RIGHT Or VKey = VirtualKeyCode.VK_UP Or VKey = VirtualKeyCode.VK_DOWN Or VKey = VirtualKeyCode.VK_HOME Or VKey = VirtualKeyCode.VK_DELETE Or VKey = VirtualKeyCode.VK_PRIOR Or VKey = VirtualKeyCode.VK_NEXT Or VKey = VirtualKeyCode.VK_END Or VKey = VirtualKeyCode.VK_INSERT Or VKey = VirtualKeyCode.VK_NUMLOCK Or VKey = VirtualKeyCode.VK_SNAPSHOT Or VKey = VirtualKeyCode.VK_DIVIDE)
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
    Public Shared Sub ShortcutKeys(ByVal kCode() As VirtualKeyCode, Optional ByVal Delay As Integer = 0)
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
    Public Shared Sub KeyPress(ByVal kCode As VirtualKeyCode, Optional ByVal Delay As Integer = 0)
        Dim SendKeys() As VirtualKeyCode = {kCode}
        Dim KeysPress As New KeyPressStruct(SendKeys, Delay)
        Dim t As New Thread(New ParameterizedThreadStart(AddressOf KeyPressThread))
        t.Start(KeysPress)
    End Sub
    Private Shared Sub KeyPressThread(ByVal KeysP As KeyPressStruct)
        For Each k As VirtualKeyCode In KeysP.Keys
            KeyDown(k)
        Next
        If KeysP.Delay > 0 Then Thread.Sleep(KeysP.Delay)
        For Each k As VirtualKeyCode In KeysP.Keys
            KeyUp(k)
        Next
    End Sub
    Private Structure KeyPressStruct
        Dim Keys() As VirtualKeyCode
        Dim Delay As Integer
        Public Sub New(ByVal KeysToPress() As VirtualKeyCode, Optional ByVal DelayTime As Integer = 0)
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
    Public Shared Sub ShortcutKeys(ByVal kCode() As VirtualKeyCode, Optional ByVal Delay As Integer = 0)
        Dim KeyPress As New KeyPressStruct(kCode, Delay)
        Dim t As New Thread(New ParameterizedThreadStart(AddressOf KeyPressThread))
        t.Start(KeyPress)
    End Sub
    ''' <summary>
    ''' Sends a key down signal.
    ''' </summary>
    ''' <param name="kCode">The virtual keycode to send.</param>
    ''' <remarks></remarks>
    Public Shared Sub KeyDown(ByVal kCode As VirtualKeyCode)
        keybd_event(kCode, 0, 0, 0)
    End Sub
    ''' <summary>
    ''' Sends a key up signal.
    ''' </summary>
    ''' <param name="kCode">The virtual keycode to send.</param>
    ''' <remarks></remarks>
    Public Shared Sub KeyUp(ByVal kCode As VirtualKeyCode)
        keybd_event(kCode, 0, KEYEVENTF_KEYUP, 0)
    End Sub
    ''' <summary>
    ''' Sends a key press signal (key down and up).
    ''' </summary>
    ''' <param name="kCode">The virtual key code to send.</param>
    ''' <param name="Delay">The delay to set between the key down and up commands.</param>
    ''' <remarks></remarks>
    Public Shared Sub KeyPress(ByVal kCode As VirtualKeyCode, Optional ByVal Delay As Integer = 0)
        Dim SendKeys() As VirtualKeyCode = {kCode}
        Dim KeyPress As New KeyPressStruct(SendKeys, Delay)
        Dim t As New Thread(New ParameterizedThreadStart(AddressOf KeyPressThread))
        t.Start(KeyPress)
    End Sub
    Private Shared Sub KeyPressThread(ByVal KeysP As KeyPressStruct)
        For Each k As VirtualKeyCode In KeysP.Keys
            KeyDown(k)
        Next
        If KeysP.Delay > 0 Then Thread.Sleep(KeysP.Delay)
        For Each k As VirtualKeyCode In KeysP.Keys
            KeyUp(k)
        Next
    End Sub
    Private Structure KeyPressStruct
        Dim Keys() As VirtualKeyCode
        Dim Delay As Integer
        Public Sub New(ByVal KeysToPress() As VirtualKeyCode, Optional ByVal DelayTime As Integer = 0)
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
        input.mkhi.mi.dx = X * (65535 / My.Computer.Screen.Bounds.Width)
        input.mkhi.mi.dy = Y * (65535 / My.Computer.Screen.Bounds.Height)
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

''' <summary>
''' Code values for keyboard keys.
''' More information at https://msdn.microsoft.com/en-us/library/dd375731(v=vs.85).aspx
''' </summary>
''' <remarks></remarks>
Public Enum VirtualKeyCode As UInteger
    '''<summary>Left mouse button</summary>
    VK_LBUTTON = &H1
    '''<summary>Right mouse button</summary>
    VK_RBUTTON = &H2
    '''<summary>Control-break processing</summary>
    VK_CANCEL = &H3
    '''<summary>Middle mouse button (three-button mouse)</summary>
    VK_MBUTTON = &H4
    '''<summary>X1 mouse button</summary>
    VK_XBUTTON1 = &H5
    '''<summary>X2 mouse button</summary>
    VK_XBUTTON2 = &H6
    '''<summary>BACKSPACE key</summary>
    VK_BACK = &H8
    '''<summary>TAB key</summary>
    VK_TAB = &H9
    '''<summary>CLEAR key</summary>
    VK_CLEAR = &HC
    '''<summary>ENTER key</summary>
    VK_RETURN = &HD
    '''<summary>SHIFT key</summary>
    VK_SHIFT = &H10
    '''<summary>CTRL key</summary>
    VK_CONTROL = &H11
    '''<summary>ALT key</summary>
    VK_MENU = &H12
    '''<summary>PAUSE key</summary>
    VK_PAUSE = &H13
    '''<summary>CAPS LOCK key</summary>
    VK_CAPITAL = &H14
    '''<summary>IME Kana mode</summary>
    VK_KANA = &H15
    '''<summary>IME Hanguel mode (maintained for compatibility; use VK_HANGUL)</summary>
    VK_HANGUEL = &H15
    '''<summary>IME Hangul mode</summary>
    VK_HANGUL = &H15
    '''<summary>IME Junja mode</summary>
    VK_JUNJA = &H17
    '''<summary>IME final mode</summary>
    VK_FINAL = &H18
    '''<summary>IME Hanja mode</summary>
    VK_HANJA = &H19
    '''<summary>IME Kanji mode</summary>
    VK_KANJI = &H19
    '''<summary>ESC key</summary>
    VK_ESCAPE = &H1B
    '''<summary>IME convert</summary>
    VK_CONVERT = &H1C
    '''<summary>IME nonconvert</summary>
    VK_NONCONVERT = &H1D
    '''<summary>IME accept</summary>
    VK_ACCEPT = &H1E
    '''<summary>IME mode change request</summary>
    VK_MODECHANGE = &H1F
    '''<summary>SPACEBAR</summary>
    VK_SPACE = &H20
    '''<summary>PAGE UP key</summary>
    VK_PRIOR = &H21
    '''<summary>PAGE DOWN key</summary>
    VK_NEXT = &H22
    '''<summary>END key</summary>
    VK_END = &H23
    '''<summary>HOME key</summary>
    VK_HOME = &H24
    '''<summary>LEFT ARROW key</summary>
    VK_LEFT = &H25
    '''<summary>UP ARROW key</summary>
    VK_UP = &H26
    '''<summary>RIGHT ARROW key</summary>
    VK_RIGHT = &H27
    '''<summary>DOWN ARROW key</summary>
    VK_DOWN = &H28
    '''<summary>SELECT key</summary>
    VK_SELECT = &H29
    '''<summary>PRINT key</summary>
    VK_PRINT = &H2A
    '''<summary>EXECUTE key</summary>
    VK_EXECUTE = &H2B
    '''<summary>PRINT SCREEN key</summary>
    VK_SNAPSHOT = &H2C
    '''<summary>INS key</summary>
    VK_INSERT = &H2D
    '''<summary>DEL key</summary>
    VK_DELETE = &H2E
    '''<summary>HELP key</summary>
    VK_HELP = &H2F
    '''<summary>0 key</summary>
    K_0 = &H30
    '''<summary>1 key</summary>
    K_1 = &H31
    '''<summary>2 key</summary>
    K_2 = &H32
    '''<summary>3 key</summary>
    K_3 = &H33
    '''<summary>4 key</summary>
    K_4 = &H34
    '''<summary>5 key</summary>
    K_5 = &H35
    '''<summary>6 key</summary>
    K_6 = &H36
    '''<summary>7 key</summary>
    K_7 = &H37
    '''<summary>8 key</summary>
    K_8 = &H38
    '''<summary>9 key</summary>
    K_9 = &H39
    '''<summary>A key</summary>
    K_A = &H41
    '''<summary>B key</summary>
    K_B = &H42
    '''<summary>C key</summary>
    K_C = &H43
    '''<summary>D key</summary>
    K_D = &H44
    '''<summary>E key</summary>
    K_E = &H45
    '''<summary>F key</summary>
    K_F = &H46
    '''<summary>G key</summary>
    K_G = &H47
    '''<summary>H key</summary>
    K_H = &H48
    '''<summary>I key</summary>
    K_I = &H49
    '''<summary>J key</summary>
    K_J = &H4A
    '''<summary>K key</summary>
    K_K = &H4B
    '''<summary>L key</summary>
    K_L = &H4C
    '''<summary>M key</summary>
    K_M = &H4D
    '''<summary>N key</summary>
    K_N = &H4E
    '''<summary>O key</summary>
    K_O = &H4F
    '''<summary>P key</summary>
    K_P = &H50
    '''<summary>Q key</summary>
    K_Q = &H51
    '''<summary>R key</summary>
    K_R = &H52
    '''<summary>S key</summary>
    K_S = &H53
    '''<summary>T key</summary>
    K_T = &H54
    '''<summary>U key</summary>
    K_U = &H55
    '''<summary>V key</summary>
    K_V = &H56
    '''<summary>W key</summary>
    K_W = &H57
    '''<summary>X key</summary>
    K_X = &H58
    '''<summary>Y key</summary>
    K_Y = &H59
    '''<summary>Z key</summary>
    K_Z = &H5A
    '''<summary>Left Windows key (Natural keyboard)</summary>
    VK_LWIN = &H5B
    '''<summary>Right Windows key (Natural keyboard)</summary>
    VK_RWIN = &H5C
    '''<summary>Applications key (Natural keyboard)</summary>
    VK_APPS = &H5D
    '''<summary>Computer Sleep key</summary>
    VK_SLEEP = &H5F
    '''<summary>Numeric keypad 0 key</summary>
    VK_NUMPAD0 = &H60
    '''<summary>Numeric keypad 1 key</summary>
    VK_NUMPAD1 = &H61
    '''<summary>Numeric keypad 2 key</summary>
    VK_NUMPAD2 = &H62
    '''<summary>Numeric keypad 3 key</summary>
    VK_NUMPAD3 = &H63
    '''<summary>Numeric keypad 4 key</summary>
    VK_NUMPAD4 = &H64
    '''<summary>Numeric keypad 5 key</summary>
    VK_NUMPAD5 = &H65
    '''<summary>Numeric keypad 6 key</summary>
    VK_NUMPAD6 = &H66
    '''<summary>Numeric keypad 7 key</summary>
    VK_NUMPAD7 = &H67
    '''<summary>Numeric keypad 8 key</summary>
    VK_NUMPAD8 = &H68
    '''<summary>Numeric keypad 9 key</summary>
    VK_NUMPAD9 = &H69
    '''<summary>Multiply key</summary>
    VK_MULTIPLY = &H6A
    '''<summary>Add key</summary>
    VK_ADD = &H6B
    '''<summary>Separator key</summary>
    VK_SEPARATOR = &H6C
    '''<summary>Subtract key</summary>
    VK_SUBTRACT = &H6D
    '''<summary>Decimal key</summary>
    VK_DECIMAL = &H6E
    '''<summary>Divide key</summary>
    VK_DIVIDE = &H6F
    '''<summary>F1 key</summary>
    VK_F1 = &H70
    '''<summary>F2 key</summary>
    VK_F2 = &H71
    '''<summary>F3 key</summary>
    VK_F3 = &H72
    '''<summary>F4 key</summary>
    VK_F4 = &H73
    '''<summary>F5 key</summary>
    VK_F5 = &H74
    '''<summary>F6 key</summary>
    VK_F6 = &H75
    '''<summary>F7 key</summary>
    VK_F7 = &H76
    '''<summary>F8 key</summary>
    VK_F8 = &H77
    '''<summary>F9 key</summary>
    VK_F9 = &H78
    '''<summary>F10 key</summary>
    VK_F10 = &H79
    '''<summary>F11 key</summary>
    VK_F11 = &H7A
    '''<summary>F12 key</summary>
    VK_F12 = &H7B
    '''<summary>F13 key</summary>
    VK_F13 = &H7C
    '''<summary>F14 key</summary>
    VK_F14 = &H7D
    '''<summary>F15 key</summary>
    VK_F15 = &H7E
    '''<summary>F16 key</summary>
    VK_F16 = &H7F
    '''<summary>F17 key</summary>
    VK_F17 = &H80
    '''<summary>F18 key</summary>
    VK_F18 = &H81
    '''<summary>F19 key</summary>
    VK_F19 = &H82
    '''<summary>F20 key</summary>
    VK_F20 = &H83
    '''<summary>F21 key</summary>
    VK_F21 = &H84
    '''<summary>F22 key</summary>
    VK_F22 = &H85
    '''<summary>F23 key</summary>
    VK_F23 = &H86
    '''<summary>F24 key</summary>
    VK_F24 = &H87
    '''<summary>NUM LOCK key</summary>
    VK_NUMLOCK = &H90
    '''<summary>SCROLL LOCK key</summary>
    VK_SCROLL = &H91
    '''<summary>Left SHIFT key</summary>
    VK_LSHIFT = &HA0
    '''<summary>Right SHIFT key</summary>
    VK_RSHIFT = &HA1
    '''<summary>Left CONTROL key</summary>
    VK_LCONTROL = &HA2
    '''<summary>Right CONTROL key</summary>
    VK_RCONTROL = &HA3
    '''<summary>Left MENU key</summary>
    VK_LMENU = &HA4
    '''<summary>Right MENU key</summary>
    VK_RMENU = &HA5
    '''<summary>Browser Back key</summary>
    VK_BROWSER_BACK = &HA6
    '''<summary>Browser Forward key</summary>
    VK_BROWSER_FORWARD = &HA7
    '''<summary>Browser Refresh key</summary>
    VK_BROWSER_REFRESH = &HA8
    '''<summary>Browser Stop key</summary>
    VK_BROWSER_STOP = &HA9
    '''<summary>Browser Search key</summary>
    VK_BROWSER_SEARCH = &HAA
    '''<summary>Browser Favorites key</summary>
    VK_BROWSER_FAVORITES = &HAB
    '''<summary>Browser Start and Home key</summary>
    VK_BROWSER_HOME = &HAC
    '''<summary>Volume Mute key</summary>
    VK_VOLUME_MUTE = &HAD
    '''<summary>Volume Down key</summary>
    VK_VOLUME_DOWN = &HAE
    '''<summary>Volume Up key</summary>
    VK_VOLUME_UP = &HAF
    '''<summary>Next Track key</summary>
    VK_MEDIA_NEXT_TRACK = &HB0
    '''<summary>Previous Track key</summary>
    VK_MEDIA_PREV_TRACK = &HB1
    '''<summary>Stop Media key</summary>
    VK_MEDIA_STOP = &HB2
    '''<summary>Play/Pause Media key</summary>
    VK_MEDIA_PLAY_PAUSE = &HB3
    '''<summary>Start Mail key</summary>
    VK_LAUNCH_MAIL = &HB4
    '''<summary>Select Media key</summary>
    VK_LAUNCH_MEDIA_SELECT = &HB5
    '''<summary>Start Application 1 key</summary>
    VK_LAUNCH_APP1 = &HB6
    '''<summary>Start Application 2 key</summary>
    VK_LAUNCH_APP2 = &HB7
    '''<summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key</summary>
    VK_OEM_1 = &HBA
    '''<summary>For any country/region, the '+' key</summary>
    VK_OEM_PLUS = &HBB
    '''<summary>For any country/region, the ',' key</summary>
    VK_OEM_COMMA = &HBC
    '''<summary>For any country/region, the '-' key</summary>
    VK_OEM_MINUS = &HBD
    '''<summary>For any country/region, the '.' key</summary>
    VK_OEM_PERIOD = &HBE
    '''<summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key</summary>
    VK_OEM_2 = &HBF
    '''<summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key</summary>
    VK_OEM_3 = &HC0
    '''<summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key</summary>
    VK_OEM_4 = &HDB
    '''<summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\\|' key</summary>
    VK_OEM_5 = &HDC
    '''<summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key</summary>
    VK_OEM_6 = &HDD
    '''<summary>Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key</summary>
    VK_OEM_7 = &HDE
    '''<summary>Used for miscellaneous characters; it can vary by keyboard.</summary>
    VK_OEM_8 = &HDF
    '''<summary>Either the angle bracket key or the backslash key on the RT 102-key keyboard</summary>
    VK_OEM_102 = &HE2
    '''<summary>IME PROCESS key</summary>
    VK_PROCESSKEY = &HE5
    '''<summary>Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP</summary>
    VK_PACKET = &HE7
    '''<summary>Attn key</summary>
    VK_ATTN = &HF6
    '''<summary>CrSel key</summary>
    VK_CRSEL = &HF7
    '''<summary>ExSel key</summary>
    VK_EXSEL = &HF8
    '''<summary>Erase EOF key</summary>
    VK_EREOF = &HF9
    '''<summary>Play key</summary>
    VK_PLAY = &HFA
    '''<summary>Zoom key</summary>
    VK_ZOOM = &HFB
    '''<summary>PA1 key</summary>
    VK_PA1 = &HFD
    '''<summary>Clear key</summary>
    VK_OEM_CLEAR = &HFE
End Enum