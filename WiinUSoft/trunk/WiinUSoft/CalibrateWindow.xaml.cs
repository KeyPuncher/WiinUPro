﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using NintrollerLib.New;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for CalibrateWindow.xaml
    /// </summary>
    public partial class CalibrateWindow : Window
    {
        enum CalibrationStep
        {
            Done,
            ChangeController,

            Wiimote_acc_x_center,   // first step for wiimote
            Wiimote_acc_x_range,
            Wiimote_acc_y_center,
            Wiimote_acc_y_range,
            Wiimote_acc_z_center,
            Wiimote_acc_z_range,

            Nunchuk_acc_x_center,   // first step for nunchuk
            Nunchuk_acc_x_range,
            Nunchuk_acc_y_center,
            Nunchuk_acc_y_range,
            Nunchuk_acc_z_center,
            Nunchuk_acc_z_range,
            Nunchuk_acc_done, 
            Nunchuk_joy_center,
            Nunchuk_joy_range,
            Nunchuk_joy_deadzone,

            Classic_joy_center,     // first step for classic controller
            Classic_joy_range,      // also triggers
            Classic_joy_deadzone,

            ClassicPro_joy_center,  // first step for classic controller pro
            ClassicPro_joy_range,
            ClassicPro_joy_deadzone,

            Pro_joy_center,         // first step for pro controller
            Pro_joy_range,
            Pro_joy_deadzone
        }

        private Nintroller _device;
        private bool _changingType = false;
        private List<ControllerType> _calibratedTypes = new List<ControllerType>();
        private CalibrationStep _step = CalibrationStep.ChangeController;

        private CalibrateWindow()
        {
            InitializeComponent();
        }

        public CalibrateWindow(Nintroller device) :this()
        {
            SelectStep(device.Type);
           
            _device = device;
            _device.StateUpdate += _device_StateUpdate;
            _device.ExtensionChange += _device_ExtensionChange;
        }

        private void SelectStep(ControllerType deviceType)
        {
            if (_calibratedTypes.Contains(deviceType))
            {
                return;
            }

            _changingType = true;

            if (deviceType == ControllerType.ProController)
            {
                _step = CalibrationStep.Pro_joy_center;
            }
            else
            {
                switch (deviceType)
                {
                    case ControllerType.Wiimote:
                        _step = CalibrationStep.Wiimote_acc_x_center;
                        break;

                    case ControllerType.Nunchuk:
                    case ControllerType.NunchukB:
                        _step = CalibrationStep.Nunchuk_acc_x_center;
                        break;

                    case ControllerType.ClassicController:
                        _step = CalibrationStep.Classic_joy_center;
                        break;

                    case ControllerType.ClassicControllerPro:
                        _step = CalibrationStep.ClassicPro_joy_center;
                        break;

                    case ControllerType.ProController:
                        _step = CalibrationStep.Pro_joy_center;
                        break;

                    default: break;
                }
            }

            UpdateUI();
            _changingType = false;
        }

        void _device_ExtensionChange(object sender, NintrollerExtensionEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                SelectStep(e.controllerType);
            }));
        }

        void _device_StateUpdate(object sender, NintrollerStateEventArgs e)
        {
            if (_changingType) return;

            Dispatcher.Invoke((Action)(() =>
            {
                switch (_step)
                {
                    #region Wiimote Calibrations
                    case CalibrationStep.Wiimote_acc_x_center:
                        group1_center.Value = ((Wiimote)e.state).accelerometer.rawX;
                        break;

                    case CalibrationStep.Wiimote_acc_x_range:
                        if (group1_max.Value == 0)
                        {
                            group1_min.Value = ((Wiimote)e.state).accelerometer.rawX;
                            group1_max.Value = ((Wiimote)e.state).accelerometer.rawX;
                        }
                        else
                        {
                            if (group1_min.Value > ((Wiimote)e.state).accelerometer.rawX) group1_min.Value = ((Wiimote)e.state).accelerometer.rawX;
                            if (group1_max.Value < ((Wiimote)e.state).accelerometer.rawX) group1_max.Value = ((Wiimote)e.state).accelerometer.rawX;
                        }
                        break;

                    case CalibrationStep.Wiimote_acc_y_center:
                        group2_center.Value = ((Wiimote)e.state).accelerometer.rawY;
                        break;

                    case CalibrationStep.Wiimote_acc_y_range:
                        if (group2_max.Value == 0)
                        {
                            group2_min.Value = ((Wiimote)e.state).accelerometer.rawY;
                            group2_max.Value = ((Wiimote)e.state).accelerometer.rawY;
                        }
                        else
                        {
                            if (group2_min.Value > ((Wiimote)e.state).accelerometer.rawY) group2_min.Value = ((Wiimote)e.state).accelerometer.rawY;
                            if (group2_max.Value < ((Wiimote)e.state).accelerometer.rawY) group2_max.Value = ((Wiimote)e.state).accelerometer.rawY;
                        }
                        break;

                    case CalibrationStep.Wiimote_acc_z_center:
                        group3_center.Value = ((Wiimote)e.state).accelerometer.rawZ;
                        break;

                    case CalibrationStep.Wiimote_acc_z_range:
                        if (group3_max.Value == 0)
                        {
                            group3_min.Value = ((Wiimote)e.state).accelerometer.rawZ;
                            group3_max.Value = ((Wiimote)e.state).accelerometer.rawZ;
                        }
                        else
                        {
                            if (group3_min.Value > ((Wiimote)e.state).accelerometer.rawZ) group3_min.Value = ((Wiimote)e.state).accelerometer.rawZ;
                            if (group3_max.Value < ((Wiimote)e.state).accelerometer.rawZ) group3_max.Value = ((Wiimote)e.state).accelerometer.rawZ;
                        }
                        break;
                    #endregion

                    #region Nunchuk Calibration
                    case CalibrationStep.Nunchuk_acc_x_center:
                        group1_center.Value = ((Nunchuk)e.state).accelerometer.rawX;
                        break;

                    case CalibrationStep.Nunchuk_acc_x_range:
                        if (group1_max.Value == 0)
                        {
                            group1_min.Value = ((Nunchuk)e.state).accelerometer.rawX;
                            group1_max.Value = ((Nunchuk)e.state).accelerometer.rawX;
                        }
                        else
                        {
                            if (group1_min.Value > ((Nunchuk)e.state).accelerometer.rawX) group1_min.Value = ((Nunchuk)e.state).accelerometer.rawX;
                            if (group1_max.Value < ((Nunchuk)e.state).accelerometer.rawX) group1_max.Value = ((Nunchuk)e.state).accelerometer.rawX;
                        }
                        break;

                    case CalibrationStep.Nunchuk_acc_y_center:
                        group2_center.Value = ((Nunchuk)e.state).accelerometer.rawY;
                        break;

                    case CalibrationStep.Nunchuk_acc_y_range:
                        if (group2_max.Value == 0)
                        {
                            group2_min.Value = ((Nunchuk)e.state).accelerometer.rawY;
                            group2_max.Value = ((Nunchuk)e.state).accelerometer.rawY;
                        }
                        else
                        {
                            if (group2_min.Value > ((Nunchuk)e.state).accelerometer.rawY) group2_min.Value = ((Nunchuk)e.state).accelerometer.rawY;
                            if (group2_max.Value < ((Nunchuk)e.state).accelerometer.rawY) group2_max.Value = ((Nunchuk)e.state).accelerometer.rawY;
                        }
                        break;

                    case CalibrationStep.Nunchuk_acc_z_center:
                        group3_center.Value = ((Nunchuk)e.state).accelerometer.rawZ;
                        break;

                    case CalibrationStep.Nunchuk_acc_z_range:
                        if (group3_max.Value == 0)
                        {
                            group3_min.Value = ((Nunchuk)e.state).accelerometer.rawZ;
                            group3_max.Value = ((Nunchuk)e.state).accelerometer.rawZ;
                        }
                        else
                        {
                            if (group3_min.Value > ((Nunchuk)e.state).accelerometer.rawZ) group3_min.Value = ((Nunchuk)e.state).accelerometer.rawZ;
                            if (group3_max.Value < ((Nunchuk)e.state).accelerometer.rawZ) group3_max.Value = ((Nunchuk)e.state).accelerometer.rawZ;
                        }
                        break;

                    case CalibrationStep.Nunchuk_acc_done: break;

                    case CalibrationStep.Nunchuk_joy_center:
                        group1_center.Value = ((Nunchuk)e.state).joystick.rawX;
                        group2_center.Value = ((Nunchuk)e.state).joystick.rawY;
                        break;

                    case CalibrationStep.Nunchuk_joy_range:
                        if (group1_min.Value == 0)
                        {
                            group1_min.Value = ((Nunchuk)e.state).joystick.rawX;
                            group2_min.Value = ((Nunchuk)e.state).joystick.rawY;

                            group1_max.Value = ((Nunchuk)e.state).joystick.rawX;
                            group2_max.Value = ((Nunchuk)e.state).joystick.rawY;
                        }
                        else
                        {
                            if (group1_min.Value > ((Nunchuk)e.state).joystick.rawX) group1_min.Value = ((Nunchuk)e.state).joystick.rawX;
                            if (group1_max.Value < ((Nunchuk)e.state).joystick.rawX) group1_max.Value = ((Nunchuk)e.state).joystick.rawX;

                            if (group2_min.Value > ((Nunchuk)e.state).joystick.rawY) group2_min.Value = ((Nunchuk)e.state).joystick.rawY;
                            if (group2_max.Value < ((Nunchuk)e.state).joystick.rawY) group2_max.Value = ((Nunchuk)e.state).joystick.rawY;
                        }
                        break;

                    case CalibrationStep.Nunchuk_joy_deadzone:
                        int nunX = Math.Abs(((Nunchuk)e.state).joystick.rawX - group1_center.Value);
                        int nunY = Math.Abs(((Nunchuk)e.state).joystick.rawY - group2_center.Value);

                        if (nunX > group1_dead.Value) group1_dead.Value = nunX;
                        if (nunY > group2_dead.Value) group2_dead.Value = nunY;
                        break;
                    #endregion

                    #region Classic Controller Calibration
                    case CalibrationStep.Classic_joy_center:
                        group1_center.Value = ((ClassicController)e.state).LJoy.rawX;
                        group2_center.Value = ((ClassicController)e.state).LJoy.rawY;
                        group3_center.Value = ((ClassicController)e.state).RJoy.rawX;
                        group4_center.Value = ((ClassicController)e.state).RJoy.rawY;
                        groupL_min.Value = ((ClassicController)e.state).L.rawValue;
                        groupR_min.Value = ((ClassicController)e.state).R.rawValue;
                        break;

                    case CalibrationStep.Classic_joy_range:
                        if (group1_max.Value == 0)
                        {
                            group1_min.Value = ((ClassicController)e.state).LJoy.rawX;
                            group2_min.Value = ((ClassicController)e.state).LJoy.rawY;
                            group3_min.Value = ((ClassicController)e.state).RJoy.rawX;
                            group4_min.Value = ((ClassicController)e.state).RJoy.rawY;

                            group1_max.Value = ((ClassicController)e.state).LJoy.rawX;
                            group2_max.Value = ((ClassicController)e.state).LJoy.rawY;
                            group3_max.Value = ((ClassicController)e.state).RJoy.rawX;
                            group4_max.Value = ((ClassicController)e.state).RJoy.rawY;

                            groupL_max.Value = ((ClassicController)e.state).L.rawValue;
                            groupR_max.Value = ((ClassicController)e.state).R.rawValue;
                        }
                        else
                        {
                            if (group1_min.Value > ((ClassicController)e.state).LJoy.rawX) group1_min.Value = ((ClassicController)e.state).LJoy.rawX;
                            if (group1_max.Value < ((ClassicController)e.state).LJoy.rawX) group1_max.Value = ((ClassicController)e.state).LJoy.rawX;

                            if (group2_min.Value > ((ClassicController)e.state).LJoy.rawY) group2_min.Value = ((ClassicController)e.state).LJoy.rawY;
                            if (group2_max.Value < ((ClassicController)e.state).LJoy.rawY) group2_max.Value = ((ClassicController)e.state).LJoy.rawY;

                            if (group3_min.Value > ((ClassicController)e.state).RJoy.rawX) group3_min.Value = ((ClassicController)e.state).RJoy.rawX;
                            if (group3_max.Value < ((ClassicController)e.state).RJoy.rawX) group3_max.Value = ((ClassicController)e.state).RJoy.rawX;

                            if (group4_min.Value > ((ClassicController)e.state).RJoy.rawY) group4_min.Value = ((ClassicController)e.state).RJoy.rawY;
                            if (group4_max.Value < ((ClassicController)e.state).RJoy.rawY) group4_max.Value = ((ClassicController)e.state).RJoy.rawY;

                            if (groupL_max.Value < ((ClassicController)e.state).L.rawValue) groupL_max.Value = ((ClassicController)e.state).L.rawValue;
                            if (groupR_max.Value < ((ClassicController)e.state).R.rawValue) groupR_max.Value = ((ClassicController)e.state).R.rawValue;
                        }
                        break;

                    case CalibrationStep.Classic_joy_deadzone:
                        int ccLX = Math.Abs(((ClassicController)e.state).LJoy.rawX - group1_center.Value);
                        int ccLY = Math.Abs(((ClassicController)e.state).LJoy.rawY - group2_center.Value);
                        int ccRX = Math.Abs(((ClassicController)e.state).RJoy.rawX - group3_center.Value);
                        int ccRY = Math.Abs(((ClassicController)e.state).RJoy.rawY - group4_center.Value);

                        if (ccLX > group1_dead.Value) group1_dead.Value = ccLX;
                        if (ccLY > group2_dead.Value) group2_dead.Value = ccLY;
                        if (ccRX > group3_dead.Value) group3_dead.Value = ccRX;
                        if (ccRY > group4_dead.Value) group4_dead.Value = ccRY;
                        break;
                    #endregion

                    #region Classic Controller Pro Calibration
                    case CalibrationStep.ClassicPro_joy_center:
                        group1_center.Value = ((ClassicControllerPro)e.state).LJoy.rawX;
                        group2_center.Value = ((ClassicControllerPro)e.state).LJoy.rawY;
                        group3_center.Value = ((ClassicControllerPro)e.state).RJoy.rawX;
                        group4_center.Value = ((ClassicControllerPro)e.state).RJoy.rawY;
                        break;

                    case CalibrationStep.ClassicPro_joy_range:
                        if (group1_max.Value == 0)
                        {
                            group1_min.Value = ((ClassicControllerPro)e.state).LJoy.rawX;
                            group2_min.Value = ((ClassicControllerPro)e.state).LJoy.rawY;
                            group3_min.Value = ((ClassicControllerPro)e.state).RJoy.rawX;
                            group4_min.Value = ((ClassicControllerPro)e.state).RJoy.rawY;

                            group1_max.Value = ((ClassicControllerPro)e.state).LJoy.rawX;
                            group2_max.Value = ((ClassicControllerPro)e.state).LJoy.rawY;
                            group3_max.Value = ((ClassicControllerPro)e.state).RJoy.rawX;
                            group4_max.Value = ((ClassicControllerPro)e.state).RJoy.rawY;
                        }
                        else
                        {
                            if (group1_min.Value > ((ClassicControllerPro)e.state).LJoy.rawX) group1_min.Value = ((ClassicControllerPro)e.state).LJoy.rawX;
                            if (group1_max.Value < ((ClassicControllerPro)e.state).LJoy.rawX) group1_max.Value = ((ClassicControllerPro)e.state).LJoy.rawX;

                            if (group2_min.Value > ((ClassicControllerPro)e.state).LJoy.rawY) group2_min.Value = ((ClassicControllerPro)e.state).LJoy.rawY;
                            if (group2_max.Value < ((ClassicControllerPro)e.state).LJoy.rawY) group2_max.Value = ((ClassicControllerPro)e.state).LJoy.rawY;
                                                                      
                            if (group3_min.Value > ((ClassicControllerPro)e.state).RJoy.rawX) group3_min.Value = ((ClassicControllerPro)e.state).RJoy.rawX;
                            if (group3_max.Value < ((ClassicControllerPro)e.state).RJoy.rawX) group3_max.Value = ((ClassicControllerPro)e.state).RJoy.rawX;

                            if (group4_min.Value > ((ClassicControllerPro)e.state).RJoy.rawY) group4_min.Value = ((ClassicControllerPro)e.state).RJoy.rawY;
                            if (group4_max.Value < ((ClassicControllerPro)e.state).RJoy.rawY) group4_max.Value = ((ClassicControllerPro)e.state).RJoy.rawY;                        }
                        break;

                    case CalibrationStep.ClassicPro_joy_deadzone:
                        int ccpLX = Math.Abs(((ClassicControllerPro)e.state).LJoy.rawX - group1_center.Value);
                        int ccpLY = Math.Abs(((ClassicControllerPro)e.state).LJoy.rawY - group2_center.Value);
                        int ccpRX = Math.Abs(((ClassicControllerPro)e.state).RJoy.rawX - group3_center.Value);
                        int ccpRY = Math.Abs(((ClassicControllerPro)e.state).RJoy.rawY - group4_center.Value);

                        if (ccpLX > group1_dead.Value) group1_dead.Value = ccpLX;
                        if (ccpLY > group2_dead.Value) group2_dead.Value = ccpLY;
                        if (ccpRX > group3_dead.Value) group3_dead.Value = ccpRX;
                        if (ccpRY > group4_dead.Value) group4_dead.Value = ccpRY;
                        break;
                    #endregion

                    #region Pro Controller Calibration
                    case CalibrationStep.Pro_joy_center:
                        group1_center.Value = ((ProController)e.state).LJoy.rawX;
                        group2_center.Value = ((ProController)e.state).LJoy.rawY;
                        group3_center.Value = ((ProController)e.state).RJoy.rawX;
                        group4_center.Value = ((ProController)e.state).RJoy.rawY;
                        break;

                    case CalibrationStep.Pro_joy_range:
                        if (group1_min.Value == 0)
                        {
                            group1_min.Value = ((ProController)e.state).LJoy.rawX;
                            group2_min.Value = ((ProController)e.state).LJoy.rawY;
                            group3_min.Value = ((ProController)e.state).RJoy.rawX;
                            group4_min.Value = ((ProController)e.state).RJoy.rawY;

                            group1_max.Value = ((ProController)e.state).LJoy.rawX;
                            group2_max.Value = ((ProController)e.state).LJoy.rawY;
                            group3_max.Value = ((ProController)e.state).RJoy.rawX;
                            group4_max.Value = ((ProController)e.state).RJoy.rawY;
                        }
                        else
                        {
                            if (group1_min.Value > ((ProController)e.state).LJoy.rawX) group1_min.Value = ((ProController)e.state).LJoy.rawX;
                            if (group1_max.Value < ((ProController)e.state).LJoy.rawX) group1_max.Value = ((ProController)e.state).LJoy.rawX;

                            if (group2_min.Value > ((ProController)e.state).LJoy.rawY) group2_min.Value = ((ProController)e.state).LJoy.rawY;
                            if (group2_max.Value < ((ProController)e.state).LJoy.rawY) group2_max.Value = ((ProController)e.state).LJoy.rawY;

                            if (group3_min.Value > ((ProController)e.state).RJoy.rawX) group3_min.Value = ((ProController)e.state).RJoy.rawX;
                            if (group3_max.Value < ((ProController)e.state).RJoy.rawX) group3_max.Value = ((ProController)e.state).RJoy.rawX;

                            if (group4_min.Value > ((ProController)e.state).RJoy.rawY) group4_min.Value = ((ProController)e.state).RJoy.rawY;
                            if (group4_max.Value < ((ProController)e.state).RJoy.rawY) group4_max.Value = ((ProController)e.state).RJoy.rawY;
                        }
                        break;

                    case CalibrationStep.Pro_joy_deadzone:
                        int pLX = Math.Abs(((ProController)e.state).LJoy.rawX - group1_center.Value);
                        int pLY = Math.Abs(((ProController)e.state).LJoy.rawY - group2_center.Value);
                        int pRX = Math.Abs(((ProController)e.state).RJoy.rawX - group3_center.Value);
                        int pRY = Math.Abs(((ProController)e.state).RJoy.rawY - group4_center.Value);

                        if (pLX > group1_dead.Value) group1_dead.Value = pLX;
                        if (pLY > group2_dead.Value) group2_dead.Value = pLY;
                        if (pRX > group3_dead.Value) group3_dead.Value = pRX;
                        if (pRY > group4_dead.Value) group4_dead.Value = pRY;
                        break;
                    #endregion
                }
            }));
        }

        private void UpdateUI()
        {
            if (_step != CalibrationStep.ChangeController)
            {
                nextBtn.Content = "Next";
            }

            switch (_step)
            {
                case CalibrationStep.ChangeController:
                    inst.Text = "Calibration for this controller is done. Plug in another extension or click Done to apply the calibrations.";
                    nextBtn.Content = "Done";
                    break;
                    
                case CalibrationStep.Done:
                    inst.Text = "Controller calibration complete. You can adjust the values as desired. Click Done to apply the calibration.";
                    nextBtn.Content = "Done";
                    break;

                #region Wiimote Calibration Steps
                case CalibrationStep.Wiimote_acc_x_center:
                    ResetValues();
                    title.Content = "Wiimote";
                    inst.Text = "Place the Wiimote on a flat surface face down and click Next.";
                    group1_center.Max = group2_center.Max = group3_center.Max = 300;
                    group1_min.Max    = group2_min.Max    = group3_min.Max    = 300;
                    group1_max.Max    = group2_max.Max    = group3_max.Max    = 300;
                    group1_dead.Max   = group2_dead.Max   = group3_dead.Max   = 150;
                    group1.Header = "X-Axis";
                    group1_center.IsEnabled = false;
                    group1_min.IsEnabled    = false;
                    group1_max.IsEnabled    = false;
                    group1_dead.IsEnabled   = false;
                    group1.Visibility = System.Windows.Visibility.Visible;
                    group2.Header = "Y-Axis";
                    group2_center.IsEnabled = false;
                    group2_min.IsEnabled    = false;
                    group2_max.IsEnabled    = false;
                    group2_dead.IsEnabled   = false;
                    group2.Visibility = System.Windows.Visibility.Visible;
                    group3.Header = "Z-Axis";
                    group3_center.IsEnabled = false;
                    group3_min.IsEnabled    = false;
                    group3_max.IsEnabled    = false;
                    group3_dead.IsEnabled   = false;
                    group3.Visibility = System.Windows.Visibility.Visible;
                    group4.Visibility = System.Windows.Visibility.Hidden;
                    groupL.Visibility = System.Windows.Visibility.Hidden;
                    groupR.Visibility = System.Windows.Visibility.Hidden;
                    break;

                case CalibrationStep.Wiimote_acc_x_range:
                    inst.Text = "Rotate the Wiimote so that the buttons are facing to the left and then roll it around so the buttons are facing to the right then click Next.";
                    group1_center.IsEnabled = true;
                    break;

                case CalibrationStep.Wiimote_acc_y_center:
                    inst.Text = "Return the Wiimote to a face down position and click Next.";
                    group1_center.IsEnabled = true;
                    group1_min.IsEnabled    = true;
                    group1_max.IsEnabled    = true;
                    group1_dead.IsEnabled   = true;
                    break;

                case CalibrationStep.Wiimote_acc_y_range:
                    inst.Text = "Move the Wiimote so that it is standing on the top (IR Sensor down) and then move it so that it is standing strait up (extension port down) then click Next.";
                    group2_center.IsEnabled = true;
                    break;

                case CalibrationStep.Wiimote_acc_z_center:
                    inst.Text = "Keep the Wiimote standing up on its extension port then click Next.";
                    group2_center.IsEnabled = true;
                    group2_min.IsEnabled    = true;
                    group2_max.IsEnabled    = true;
                    group2_dead.IsEnabled   = true;
                    break;

                case CalibrationStep.Wiimote_acc_z_range:
                    inst.Text = "Lay the Wiimote down so that its buttons are face up and then rotate it around so its buttons are face down then click Next.";
                    group3_center.IsEnabled = true;
                    break;
                #endregion

                #region Nunchuk Calibration Steps
                case CalibrationStep.Nunchuk_acc_x_center:
                    ResetValues();
                    title.Content = "Nunchuk";
                    inst.Text = "Hold the Nunchuk right side up with the top of the joystick parallel to the ground. Then click Next.";
                    group1_center.Max = group2_center.Max = group3_center.Max = 300;
                    group1_min.Max    = group2_min.Max    = group3_min.Max    = 300;
                    group1_max.Max    = group2_max.Max    = group3_max.Max    = 300;
                    group1_dead.Max   = group2_dead.Max   = group3_dead.Max   = 150;
                    group1_dead.Value = group2_dead.Value = group3_dead.Value = 16;
                    group1.Header = "X-Axis";
                    group1_center.IsEnabled = false;
                    group1_min.IsEnabled    = false;
                    group1_max.IsEnabled    = false;
                    group1_dead.IsEnabled   = false;
                    group1.Visibility = System.Windows.Visibility.Visible;
                    group2.Header = "Y-Axis";
                    group2_center.IsEnabled = false;
                    group2_min.IsEnabled    = false;
                    group2_max.IsEnabled    = false;
                    group2_dead.IsEnabled   = false;
                    group2.Visibility = System.Windows.Visibility.Visible;
                    group3.Header = "Z-Axis";
                    group3_center.IsEnabled = false;
                    group3_min.IsEnabled    = false;
                    group3_max.IsEnabled    = false;
                    group3_dead.IsEnabled   = false;
                    group3.Visibility = System.Windows.Visibility.Visible;
                    group4.Visibility = System.Windows.Visibility.Hidden;
                    groupL.Visibility = System.Windows.Visibility.Hidden;
                    groupR.Visibility = System.Windows.Visibility.Hidden;
                    break;

                case CalibrationStep.Nunchuk_acc_x_range:
                    inst.Text = "Roll the Nunchuk left and right to the desired angles and then click Next.";
                    group1_center.IsEnabled = true;
                    break;

                case CalibrationStep.Nunchuk_acc_y_center:
                    inst.Text = "Return the Nunchuk to the face up position and click Next.";
                    group1_center.IsEnabled = true;
                    group1_min.IsEnabled = true;
                    group1_max.IsEnabled = true;
                    group1_dead.IsEnabled = true;
                    break;

                case CalibrationStep.Nunchuk_acc_y_range:
                    inst.Text = "Tilt the Nunchuk up and down to the desired angles and then click Next.";
                    group2_center.IsEnabled = true;
                    break;

                case CalibrationStep.Nunchuk_acc_z_center:
                    inst.Text = "Rotate the Nunchuk so that the top of the joystick is perpendicular to the ground and then click Next.";
                    group2_center.IsEnabled = true;
                    group2_min.IsEnabled = true;
                    group2_max.IsEnabled = true;
                    group2_dead.IsEnabled = true;
                    break;

                case CalibrationStep.Nunchuk_acc_z_range:
                    inst.Text = "Move the Nunchuk face up and then face down and then click Next.";
                    group3_center.IsEnabled = true;
                    break;

                case CalibrationStep.Nunchuk_acc_done:
                    inst.Text = "Accelerometer calibration is complete, click next to calibrate the joystick.";
                    group3_dead.IsEnabled = true;
                    break;

                case CalibrationStep.Nunchuk_joy_center:
                    ResetValues();
                    inst.Text = "Keep the joystick untouched and click on Next.";
                    group1_center.Max = group2_center.Max = 300;
                    group1_min.Max    = group2_min.Max    = 300;
                    group1_max.Max    = group2_max.Max    = 300;
                    group1_dead.Max   = group2_dead.Max   = 150;
                    group1_dead.Value = group2_dead.Value = 8;
                    group1.Header = "X-Axis";
                    group1_center.IsEnabled = false;
                    group1_min.IsEnabled = false;
                    group1_max.IsEnabled = false;
                    group1_dead.IsEnabled = false;
                    group1.Visibility = System.Windows.Visibility.Visible;
                    group2.Header = "Y-Axis";
                    group2_center.IsEnabled = false;
                    group2_min.IsEnabled = false;
                    group2_max.IsEnabled = false;
                    group2_dead.IsEnabled = false;
                    group2.Visibility = System.Windows.Visibility.Visible;
                    group3.Visibility = System.Windows.Visibility.Hidden;
                    group4.Visibility = System.Windows.Visibility.Hidden;
                    groupL.Visibility = System.Windows.Visibility.Hidden;
                    groupR.Visibility = System.Windows.Visibility.Hidden;
                    break;

                case CalibrationStep.Nunchuk_joy_range:
                    inst.Text = "Move the joystick around in a full circle then click Next.";
                    group1_center.IsEnabled = true;
                    group2_center.IsEnabled = true;
                    break;

                case CalibrationStep.Nunchuk_joy_deadzone:
                    inst.Text = "Carefully wiggle the joystick in a circular motion to find the endges of the dead zone and then click Next.";
                    group1_min.IsEnabled = true;
                    group1_max.IsEnabled = true;
                    group2_min.IsEnabled = true;
                    group2_max.IsEnabled = true;
                    break;
                #endregion

                #region Classic Controller Calibration Steps
                case CalibrationStep.Classic_joy_center:
                    ResetValues();
                    title.Content = "Classic Controller";
                    inst.Text = "Keep both joysticks untouched and click on Next.";
                    group1_center.Max = group2_center.Max = group3_center.Max = group4_center.Max = 100;
                    group1_min.Max    = group2_min.Max    = group3_min.Max    = group4_min.Max    = 100;
                    group1_max.Max    = group2_max.Max    = group3_max.Max    = group4_max.Max    = 100;
                    group1_dead.Max   = group2_dead.Max   = group3_dead.Max   = group4_dead.Max   = 50;
                    group1.Header = "Left X-Axis";
                    group1_center.IsEnabled = false;
                    group1_min.IsEnabled    = false;
                    group1_max.IsEnabled    = false;
                    group1_dead.IsEnabled   = false;
                    group1.Visibility = System.Windows.Visibility.Visible;
                    group2.Header = "Left Y-Axis";
                    group2_center.IsEnabled = false;
                    group2_min.IsEnabled    = false;
                    group2_max.IsEnabled    = false;
                    group2_dead.IsEnabled   = false;
                    group2.Visibility = System.Windows.Visibility.Visible;
                    group3.Header = "Right X-Axis";
                    group3_center.IsEnabled = false;
                    group3_min.IsEnabled    = false;
                    group3_max.IsEnabled    = false;
                    group3_dead.IsEnabled   = false;
                    group3.Visibility = System.Windows.Visibility.Visible;
                    group4.Header = "Right Y-Axis";
                    group4_center.IsEnabled = false;
                    group4_min.IsEnabled    = false;
                    group4_max.IsEnabled    = false;
                    group4_dead.IsEnabled   = false;
                    group4.Visibility = System.Windows.Visibility.Visible;
                    groupL.Header = "Left Trigger";
                    groupL_min.IsEnabled    = false;
                    groupL_max.IsEnabled    = false;
                    groupL.Visibility = System.Windows.Visibility.Visible;
                    groupR.Header = "Right Trigger";
                    groupR_min.IsEnabled    = false;
                    groupR_max.IsEnabled    = false;
                    groupR.Visibility = System.Windows.Visibility.Visible;
                    break;

                case CalibrationStep.Classic_joy_range:
                    inst.Text = "Move both joysticks around in a full circle and press both L & R in completely then click Next.";
                    group1_center.IsEnabled = true;
                    group2_center.IsEnabled = true;
                    group3_center.IsEnabled = true;
                    group4_center.IsEnabled = true;
                    break;

                case CalibrationStep.Classic_joy_deadzone:
                    inst.Text = "Without puting pressure on the joysticks, rub the outside of the joysticks in a circular motion to find the endges of the dead zone and then click Next.";
                    group1_min.IsEnabled = true;
                    group1_max.IsEnabled = true;
                    group2_min.IsEnabled = true;
                    group2_max.IsEnabled = true;
                    group3_min.IsEnabled = true;
                    group3_max.IsEnabled = true;
                    group4_min.IsEnabled = true;
                    group4_max.IsEnabled = true;
                    groupL_min.IsEnabled = true;
                    groupL_max.IsEnabled = true;
                    groupR_min.IsEnabled = true;
                    groupR_max.IsEnabled = true;
                    break;
                #endregion

                #region CCPro & Pro Calibration Steps
                case CalibrationStep.ClassicPro_joy_center:
                    ResetValues();
                    title.Content = "Classic Controller Pro";
                    inst.Text = "Keep both joysticks untouched and click on Next.";
                    group1_center.Max = group2_center.Max = group3_center.Max = group4_center.Max = 100;
                    group1_min.Max    = group2_min.Max    = group3_min.Max    = group4_min.Max    = 100;
                    group1_max.Max    = group2_max.Max    = group3_max.Max    = group4_max.Max    = 100;
                    group1_dead.Max   = group2_dead.Max   = group3_dead.Max   = group4_dead.Max   = 50;
                    group1.Header = "Left X-Axis";
                    group1_center.IsEnabled = false;
                    group1_min.IsEnabled = false;
                    group1_max.IsEnabled = false;
                    group1_dead.IsEnabled = false;
                    group1.Visibility = System.Windows.Visibility.Visible;
                    group2.Header = "Left Y-Axis";
                    group2_center.IsEnabled = false;
                    group2_min.IsEnabled = false;
                    group2_max.IsEnabled = false;
                    group2_dead.IsEnabled = false;
                    group2.Visibility = System.Windows.Visibility.Visible;
                    group3.Header = "Right X-Axis";
                    group3_center.IsEnabled = false;
                    group3_min.IsEnabled = false;
                    group3_max.IsEnabled = false;
                    group3_dead.IsEnabled = false;
                    group3.Visibility = System.Windows.Visibility.Visible;
                    group4.Header = "Right Y-Axis";
                    group4_center.IsEnabled = false;
                    group4_min.IsEnabled = false;
                    group4_max.IsEnabled = false;
                    group4_dead.IsEnabled = false;
                    group4.Visibility = System.Windows.Visibility.Visible;
                    groupL.Visibility = System.Windows.Visibility.Hidden;
                    groupR.Visibility = System.Windows.Visibility.Hidden;
                    break;

                case CalibrationStep.Pro_joy_center:
                    ResetValues();
                    title.Content = "Pro Controller";
                    inst.Text = "Keep both joysticks untouched and click on Next.";
                    group1_center.Max = group2_center.Max = group3_center.Max = group4_center.Max = 4000;
                    group1_min.Max    = group2_min.Max    = group3_min.Max    = group4_min.Max    = 4000;
                    group1_max.Max    = group2_max.Max    = group3_max.Max    = group4_max.Max    = 4000;
                    group1_dead.Max   = group2_dead.Max   = group3_dead.Max   = group4_dead.Max   = 500;
                    group1.Header = "Left X-Axis";
                    group1_center.IsEnabled = false;
                    group1_min.IsEnabled = false;
                    group1_max.IsEnabled = false;
                    group1_dead.IsEnabled = false;
                    group1.Visibility = System.Windows.Visibility.Visible;
                    group2.Header = "Left Y-Axis";
                    group2_center.IsEnabled = false;
                    group2_min.IsEnabled = false;
                    group2_max.IsEnabled = false;
                    group2_dead.IsEnabled = false;
                    group2.Visibility = System.Windows.Visibility.Visible;
                    group3.Header = "Right X-Axis";
                    group3_center.IsEnabled = false;
                    group3_min.IsEnabled = false;
                    group3_max.IsEnabled = false;
                    group3_dead.IsEnabled = false;
                    group3.Visibility = System.Windows.Visibility.Visible;
                    group4.Header = "Right Y-Axis";
                    group4_center.IsEnabled = false;
                    group4_min.IsEnabled = false;
                    group4_max.IsEnabled = false;
                    group4_dead.IsEnabled = false;
                    group4.Visibility = System.Windows.Visibility.Visible;
                    groupL.Visibility = System.Windows.Visibility.Hidden;
                    groupR.Visibility = System.Windows.Visibility.Hidden;
                    break;

                case CalibrationStep.ClassicPro_joy_range:
                case CalibrationStep.Pro_joy_range:
                    inst.Text = "Move both joysticks around in a full circle then click Next.";
                    group1_center.IsEnabled = true;
                    group2_center.IsEnabled = true;
                    group3_center.IsEnabled = true;
                    group4_center.IsEnabled = true;
                    break;

                case CalibrationStep.ClassicPro_joy_deadzone:
                case CalibrationStep.Pro_joy_deadzone:
                    inst.Text = "Without puting pressure on the joysticks, rub the outside of the joysticks in a circular motion to find the endges of the dead zone and then click Next.";
                    group1_min.IsEnabled = true;
                    group1_max.IsEnabled = true;
                    group2_min.IsEnabled = true;
                    group2_max.IsEnabled = true;
                    group3_min.IsEnabled = true;
                    group3_max.IsEnabled = true;
                    group4_min.IsEnabled = true;
                    group4_max.IsEnabled = true;
                    break;
                #endregion
            }
        }

        public void ResetValues()
        {
            group1_center.Value = group1_min.Value = group1_max.Value = group1_dead.Value = 0;
            group2_center.Value = group2_min.Value = group2_max.Value = group2_dead.Value = 0;
            group3_center.Value = group3_min.Value = group3_max.Value = group3_dead.Value = 0;
            group4_center.Value = group4_min.Value = group4_max.Value = group4_dead.Value = 0;
            groupL_min.Value = groupL_max.Value = 4;
            groupR_min.Value = groupR_max.Value = 4;
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_step == CalibrationStep.Done || _step == CalibrationStep.ChangeController)
            {
                // Done with calibrating
            }
            else
            {
                // TODO: Store set calibrated values
                switch (_step)
                {
                    case CalibrationStep.Wiimote_acc_x_center: _step = CalibrationStep.Wiimote_acc_x_range;  break;
                    case CalibrationStep.Wiimote_acc_x_range:  _step = CalibrationStep.Wiimote_acc_y_center; break;
                    case CalibrationStep.Wiimote_acc_y_center: _step = CalibrationStep.Wiimote_acc_y_range;  break;
                    case CalibrationStep.Wiimote_acc_y_range:  _step = CalibrationStep.Wiimote_acc_z_center; break;
                    case CalibrationStep.Wiimote_acc_z_center: _step = CalibrationStep.Wiimote_acc_z_range;  break;
                    case CalibrationStep.Wiimote_acc_z_range:  
                        _step = CalibrationStep.ChangeController;
                        group3_min.IsEnabled  = true;
                        group3_max.IsEnabled  = true;
                        group3_dead.IsEnabled = true;
                        break;

                    case CalibrationStep.Nunchuk_acc_x_center: _step = CalibrationStep.Nunchuk_acc_x_range; break;
                    case CalibrationStep.Nunchuk_acc_x_range: _step = CalibrationStep.Nunchuk_acc_y_center; break;
                    case CalibrationStep.Nunchuk_acc_y_center: _step = CalibrationStep.Nunchuk_acc_y_range; break;
                    case CalibrationStep.Nunchuk_acc_y_range: _step = CalibrationStep.Nunchuk_acc_z_center; break;
                    case CalibrationStep.Nunchuk_acc_z_center: _step = CalibrationStep.Nunchuk_acc_z_range; break;
                    case CalibrationStep.Nunchuk_acc_z_range: 
                        _step = CalibrationStep.Nunchuk_acc_done;
                        group1_dead.IsEnabled = true;
                        group2_dead.IsEnabled = true;
                        group3_dead.IsEnabled = true;
                        break;
                    case CalibrationStep.Nunchuk_acc_done: _step = CalibrationStep.Nunchuk_joy_center; break;
                    case CalibrationStep.Nunchuk_joy_center: _step = CalibrationStep.Nunchuk_joy_range; break;
                    case CalibrationStep.Nunchuk_joy_range: _step = CalibrationStep.Nunchuk_joy_deadzone; break;
                    case CalibrationStep.Nunchuk_joy_deadzone:
                        _step = CalibrationStep.ChangeController;
                        group1_dead.IsEnabled = true;
                        group2_dead.IsEnabled = true;
                        break;

                    case CalibrationStep.Classic_joy_center:   _step = CalibrationStep.Classic_joy_range;    break;
                    case CalibrationStep.Classic_joy_range:    _step = CalibrationStep.Classic_joy_deadzone; break;
                    case CalibrationStep.Classic_joy_deadzone: 
                        _step = CalibrationStep.ChangeController;
                        group1_dead.IsEnabled = true;
                        group2_dead.IsEnabled = true;
                        group3_dead.IsEnabled = true;
                        group4_dead.IsEnabled = true;
                        break;

                    case CalibrationStep.ClassicPro_joy_center:   _step = CalibrationStep.ClassicPro_joy_range;    break;
                    case CalibrationStep.ClassicPro_joy_range:    _step = CalibrationStep.ClassicPro_joy_deadzone; break;
                    case CalibrationStep.ClassicPro_joy_deadzone: 
                        _step = CalibrationStep.ChangeController;
                        group1_dead.IsEnabled = true;
                        group2_dead.IsEnabled = true;
                        group3_dead.IsEnabled = true;
                        group4_dead.IsEnabled = true;
                        break;

                    case CalibrationStep.Pro_joy_center:   _step = CalibrationStep.Pro_joy_range;    break;
                    case CalibrationStep.Pro_joy_range:    _step = CalibrationStep.Pro_joy_deadzone; break;
                    case CalibrationStep.Pro_joy_deadzone:
                        _step = CalibrationStep.Done;
                        group1_dead.IsEnabled = true;
                        group2_dead.IsEnabled = true;
                        group3_dead.IsEnabled = true;
                        group4_dead.IsEnabled = true;
                        break;
                }
            }

            UpdateUI();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void skipBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (_step)
            {
                case CalibrationStep.Wiimote_acc_x_center:    _step = CalibrationStep.Wiimote_acc_x_range;     break;
                case CalibrationStep.Wiimote_acc_x_range:     _step = CalibrationStep.Wiimote_acc_y_center;    break;
                case CalibrationStep.Wiimote_acc_y_center:    _step = CalibrationStep.Wiimote_acc_y_range;     break;
                case CalibrationStep.Wiimote_acc_y_range:     _step = CalibrationStep.Wiimote_acc_z_center;    break;
                case CalibrationStep.Wiimote_acc_z_center:    _step = CalibrationStep.Wiimote_acc_z_range;     break;
                case CalibrationStep.Wiimote_acc_z_range:     _step = CalibrationStep.ChangeController;        break;

                case CalibrationStep.Nunchuk_acc_x_center: _step = CalibrationStep.Nunchuk_acc_x_range; break;
                case CalibrationStep.Nunchuk_acc_x_range: _step = CalibrationStep.Nunchuk_acc_y_center; break;
                case CalibrationStep.Nunchuk_acc_y_center: _step = CalibrationStep.Nunchuk_acc_y_range; break;
                case CalibrationStep.Nunchuk_acc_y_range: _step = CalibrationStep.Nunchuk_acc_z_center; break;
                case CalibrationStep.Nunchuk_acc_z_center: _step = CalibrationStep.Nunchuk_acc_z_range; break;
                case CalibrationStep.Nunchuk_acc_z_range:
                    _step = CalibrationStep.Nunchuk_acc_done;
                    group1_dead.IsEnabled = true;
                    group2_dead.IsEnabled = true;
                    group3_dead.IsEnabled = true;
                    break;
                case CalibrationStep.Nunchuk_acc_done: _step = CalibrationStep.Nunchuk_joy_center; break;
                case CalibrationStep.Nunchuk_joy_center: _step = CalibrationStep.Nunchuk_joy_range; break;
                case CalibrationStep.Nunchuk_joy_range: _step = CalibrationStep.Nunchuk_joy_deadzone; break;
                case CalibrationStep.Nunchuk_joy_deadzone:
                    _step = CalibrationStep.ChangeController;
                    group1_dead.IsEnabled = true;
                    group2_dead.IsEnabled = true;
                    break;

                case CalibrationStep.Classic_joy_center:      _step = CalibrationStep.Classic_joy_range;       break;
                case CalibrationStep.Classic_joy_range:       _step = CalibrationStep.Classic_joy_deadzone;    break;
                case CalibrationStep.Classic_joy_deadzone:    _step = CalibrationStep.ChangeController;        break;

                case CalibrationStep.ClassicPro_joy_center:   _step = CalibrationStep.ClassicPro_joy_range;    break;
                case CalibrationStep.ClassicPro_joy_range:    _step = CalibrationStep.ClassicPro_joy_deadzone; break;
                case CalibrationStep.ClassicPro_joy_deadzone: _step = CalibrationStep.ChangeController;        break;

                case CalibrationStep.Pro_joy_center:          _step = CalibrationStep.Pro_joy_range;           break;
                case CalibrationStep.Pro_joy_range:           _step = CalibrationStep.Pro_joy_deadzone;        break;
                case CalibrationStep.Pro_joy_deadzone:        _step = CalibrationStep.Done;                    break;
            }
        }
    }
}