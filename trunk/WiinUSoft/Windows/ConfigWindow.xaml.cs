using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

using NintrollerLib;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public bool result = false;
        public Dictionary<string, string> map;
        private Dictionary<string, Shape> mapShapes;
        private Dictionary<Shape, string> mapValues;
        private Dictionary<Shape, string> deviceShapes;
        private ControllerType deviceType = ControllerType.Wiimote;
        private Shape currentSelection;

        private ConfigWindow()
        {
            InitializeComponent();
        }

        public ConfigWindow(Dictionary<string, string> mappings, ControllerType type)
        //public ConfigWindow(Dictionary<string, string> mappings, ControllerType type)
        {
            InitializeComponent();

            // Copy the mappings over
            map = mappings.ToDictionary(entry => entry.Key, entry => entry.Value);
            deviceType = type;
            GetShapes();

            RotateTransform rt = new RotateTransform(-90);
            // other views should already be hidden except the pro controller
            switch (deviceType)
            {
                case ControllerType.ProController:
                    ProGrid.Visibility = Visibility.Visible;
                    break;

                case ControllerType.ClassicController:
                    ProGrid.Visibility = Visibility.Hidden;
                    CCGrid.Visibility = Visibility.Visible;
                    WmGrid.Visibility = Visibility.Visible;

                    // Reposition Wiimote Grid
                    Canvas.SetLeft(WmGrid, 70);
                    Canvas.SetTop(WmGrid, -50);
                    WmGrid.RenderTransform = rt;
                    break;

                case ControllerType.ClassicControllerPro:
                    ProGrid.Visibility = Visibility.Hidden;
                    CCPGrid.Visibility = Visibility.Visible;
                    WmGrid.Visibility = Visibility.Visible;

                    // Reposition Wiimote Grid
                    Canvas.SetLeft(WmGrid, 70);
                    Canvas.SetTop(WmGrid, -50);
                    WmGrid.RenderTransform = rt;
                    break;

                case ControllerType.Nunchuk:
                case ControllerType.NunchukB:
                    ProGrid.Visibility = Visibility.Hidden;
                    NkGrid.Visibility = Visibility.Visible;
                    WmGrid.Visibility = Visibility.Visible;
                    break;

                case ControllerType.Wiimote:
                    ProGrid.Visibility = Visibility.Hidden;
                    WmGrid.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void GetShapes()
        {
            #region Xbox Clickables
            mapShapes = new Dictionary<string,Shape>();
            mapShapes.Add(Inputs.Xbox360.A     , x_aClick);
            mapShapes.Add(Inputs.Xbox360.B     , x_bClick);
            mapShapes.Add(Inputs.Xbox360.X     , x_xClick);
            mapShapes.Add(Inputs.Xbox360.Y     , x_yClick);
            mapShapes.Add(Inputs.Xbox360.LB    , x_lbClick);
            mapShapes.Add(Inputs.Xbox360.LT    , x_ltClick);
            mapShapes.Add(Inputs.Xbox360.LS    , x_lsClick);
            mapShapes.Add(Inputs.Xbox360.RB    , x_rbClick);
            mapShapes.Add(Inputs.Xbox360.RT    , x_rtClick);
            mapShapes.Add(Inputs.Xbox360.RS    , x_rsClick);
            mapShapes.Add(Inputs.Xbox360.UP    , x_upClick);
            mapShapes.Add(Inputs.Xbox360.DOWN  , x_downClick);
            mapShapes.Add(Inputs.Xbox360.LEFT  , x_leftClick);
            mapShapes.Add(Inputs.Xbox360.RIGHT , x_rightClick);
            mapShapes.Add(Inputs.Xbox360.LUP   , x_lupClick);
            mapShapes.Add(Inputs.Xbox360.LDOWN , x_ldownClick);
            mapShapes.Add(Inputs.Xbox360.LLEFT , x_lleftClick);
            mapShapes.Add(Inputs.Xbox360.LRIGHT, x_lrightClick);
            mapShapes.Add(Inputs.Xbox360.RUP   , x_rupClick);
            mapShapes.Add(Inputs.Xbox360.RDOWN , x_rdownClick);
            mapShapes.Add(Inputs.Xbox360.RLEFT , x_rleftClick);
            mapShapes.Add(Inputs.Xbox360.RRIGHT, x_rrightClick);
            mapShapes.Add(Inputs.Xbox360.START , x_startClick);
            mapShapes.Add(Inputs.Xbox360.BACK  , x_backClick);
            mapShapes.Add(Inputs.Xbox360.GUIDE , x_guideClick);
            mapShapes.Add("", x_noneClick);
            #endregion

            #region Xbox Mappings
            mapValues = new Dictionary<Shape, string>();
            mapValues.Add(x_aClick      ,Inputs.Xbox360.A);
            mapValues.Add(x_bClick      ,Inputs.Xbox360.B);
            mapValues.Add(x_xClick      ,Inputs.Xbox360.X);
            mapValues.Add(x_yClick      ,Inputs.Xbox360.Y);
            mapValues.Add(x_lbClick     ,Inputs.Xbox360.LB);
            mapValues.Add(x_ltClick     ,Inputs.Xbox360.LT);
            mapValues.Add(x_lsClick     ,Inputs.Xbox360.LS);
            mapValues.Add(x_rbClick     ,Inputs.Xbox360.RB);
            mapValues.Add(x_rtClick     ,Inputs.Xbox360.RT);
            mapValues.Add(x_rsClick     ,Inputs.Xbox360.RS);
            mapValues.Add(x_upClick     ,Inputs.Xbox360.UP);
            mapValues.Add(x_downClick   ,Inputs.Xbox360.DOWN);
            mapValues.Add(x_leftClick   ,Inputs.Xbox360.LEFT);
            mapValues.Add(x_rightClick  ,Inputs.Xbox360.RIGHT);
            mapValues.Add(x_lupClick    ,Inputs.Xbox360.LUP);
            mapValues.Add(x_ldownClick  ,Inputs.Xbox360.LDOWN);
            mapValues.Add(x_lleftClick  ,Inputs.Xbox360.LLEFT);
            mapValues.Add(x_lrightClick ,Inputs.Xbox360.LRIGHT);
            mapValues.Add(x_rupClick    ,Inputs.Xbox360.RUP);
            mapValues.Add(x_rdownClick  ,Inputs.Xbox360.RDOWN);
            mapValues.Add(x_rleftClick  ,Inputs.Xbox360.RLEFT);
            mapValues.Add(x_rrightClick ,Inputs.Xbox360.RRIGHT);
            mapValues.Add(x_startClick  ,Inputs.Xbox360.START);
            mapValues.Add(x_backClick   ,Inputs.Xbox360.BACK);
            mapValues.Add(x_guideClick  ,Inputs.Xbox360.GUIDE);
            mapValues.Add(x_noneClick, "");
            #endregion

#if MouseMode
            // Mouse Mode
            mapShapes.Add("MouseMode", mouseClick);
            mapValues.Add(mouseClick, "MouseMode");
#endif

            deviceShapes = new Dictionary<Shape, string>();
            if (deviceType == ControllerType.ProController)
            {
                #region Pro Clickables
                deviceShapes.Add(pro_aClick     , Inputs.ProController.A);
                deviceShapes.Add(pro_bClick     , Inputs.ProController.B);
                deviceShapes.Add(pro_xClick     , Inputs.ProController.X);
                deviceShapes.Add(pro_yClick     , Inputs.ProController.Y);
                deviceShapes.Add(pro_lClick     , Inputs.ProController.L);
                deviceShapes.Add(pro_zlClick    , Inputs.ProController.ZL);
                deviceShapes.Add(pro_lsClick    , Inputs.ProController.LS);
                deviceShapes.Add(pro_rClick     , Inputs.ProController.R);
                deviceShapes.Add(pro_zrClick    , Inputs.ProController.ZR);
                deviceShapes.Add(pro_rsClick    , Inputs.ProController.RS);
                deviceShapes.Add(pro_upClick    , Inputs.ProController.UP);
                deviceShapes.Add(pro_downClick  , Inputs.ProController.DOWN);
                deviceShapes.Add(pro_leftClick  , Inputs.ProController.LEFT);
                deviceShapes.Add(pro_rightClick , Inputs.ProController.RIGHT);
                deviceShapes.Add(pro_lupClick   , Inputs.ProController.LUP);
                deviceShapes.Add(pro_ldownClick , Inputs.ProController.LDOWN);
                deviceShapes.Add(pro_lleftClick , Inputs.ProController.LLEFT);
                deviceShapes.Add(pro_lrightClick, Inputs.ProController.LRIGHT);
                deviceShapes.Add(pro_rupClick   , Inputs.ProController.RUP);
                deviceShapes.Add(pro_rdownClick , Inputs.ProController.RDOWN);
                deviceShapes.Add(pro_rleftClick , Inputs.ProController.RLEFT);
                deviceShapes.Add(pro_rrightClick, Inputs.ProController.RRIGHT);
                deviceShapes.Add(pro_startClick , Inputs.ProController.START);
                deviceShapes.Add(pro_selectClick, Inputs.ProController.SELECT);
                deviceShapes.Add(pro_homeClick  , Inputs.ProController.HOME);
                #endregion
            }
            else
            {
                #region Wiimote Clickalbes
                deviceShapes.Add(wm_aClick,         Inputs.Wiimote.A);
                deviceShapes.Add(wm_bClick,         Inputs.Wiimote.B);
                deviceShapes.Add(wm_upClick,        Inputs.Wiimote.UP);
                deviceShapes.Add(wm_downClick,      Inputs.Wiimote.DOWN);
                deviceShapes.Add(wm_leftClick,      Inputs.Wiimote.LEFT);
                deviceShapes.Add(wm_rightClick,     Inputs.Wiimote.RIGHT);
                deviceShapes.Add(wm_oneClick,       Inputs.Wiimote.ONE);
                deviceShapes.Add(wm_twoClick,       Inputs.Wiimote.TWO);
                deviceShapes.Add(wm_homeClick,      Inputs.Wiimote.HOME);
                deviceShapes.Add(wm_selectClick,    Inputs.Wiimote.MINUS);
                deviceShapes.Add(wm_startClick,     Inputs.Wiimote.PLUS);
                deviceShapes.Add(wm_accXClick,      Inputs.Wiimote.ACC_SHAKE_X);
                deviceShapes.Add(wm_accYClick,      Inputs.Wiimote.ACC_SHAKE_Y);
                deviceShapes.Add(wm_accZClick,      Inputs.Wiimote.ACC_SHAKE_Z);
                deviceShapes.Add(wm_aRollClick,     Inputs.Wiimote.TILT_RIGHT);
                deviceShapes.Add(wm_aRollNegClick,  Inputs.Wiimote.TILT_LEFT);
                deviceShapes.Add(wm_aPitchClick,    Inputs.Wiimote.TILT_UP);
                deviceShapes.Add(wm_aPitchNegClick, Inputs.Wiimote.TILT_DOWN);
                deviceShapes.Add(wm_irRightClick,   Inputs.Wiimote.IR_RIGHT);
                deviceShapes.Add(wm_irLeftClick,    Inputs.Wiimote.IR_LEFT);
                deviceShapes.Add(wm_irUpClick,      Inputs.Wiimote.IR_UP);
                deviceShapes.Add(wm_irDownClick,    Inputs.Wiimote.IR_DOWN);
                #endregion

                if (deviceType == ControllerType.ClassicController)
                {
                    #region Classic Clickables
                    deviceShapes.Add(cc_aClick     , Inputs.ClassicController.A);
                    deviceShapes.Add(cc_bClick     , Inputs.ClassicController.B);
                    deviceShapes.Add(cc_xClick     , Inputs.ClassicController.X);
                    deviceShapes.Add(cc_yClick     , Inputs.ClassicController.Y);
                    deviceShapes.Add(cc_lClick     , Inputs.ClassicController.LT);
                    deviceShapes.Add(cc_zlClick    , Inputs.ClassicController.ZL);
                    deviceShapes.Add(cc_rClick     , Inputs.ClassicController.RT);
                    deviceShapes.Add(cc_zrClick    , Inputs.ClassicController.ZR);
                    deviceShapes.Add(cc_upClick    , Inputs.ClassicController.UP);
                    deviceShapes.Add(cc_downClick  , Inputs.ClassicController.DOWN);
                    deviceShapes.Add(cc_leftClick  , Inputs.ClassicController.LEFT);
                    deviceShapes.Add(cc_rightClick , Inputs.ClassicController.RIGHT);
                    deviceShapes.Add(cc_lupClick   , Inputs.ClassicController.LUP);
                    deviceShapes.Add(cc_ldownClick , Inputs.ClassicController.LDOWN);
                    deviceShapes.Add(cc_lleftClick , Inputs.ClassicController.LLEFT);
                    deviceShapes.Add(cc_lrightClick, Inputs.ClassicController.LRIGHT);
                    deviceShapes.Add(cc_rupClick   , Inputs.ClassicController.RUP);
                    deviceShapes.Add(cc_rdownClick , Inputs.ClassicController.RDOWN);
                    deviceShapes.Add(cc_rleftClick , Inputs.ClassicController.RLEFT);
                    deviceShapes.Add(cc_rrightClick, Inputs.ClassicController.RRIGHT);
                    deviceShapes.Add(cc_startClick , Inputs.ClassicController.START);
                    deviceShapes.Add(cc_selectClick, Inputs.ClassicController.SELECT);
                    deviceShapes.Add(cc_homeClick  , Inputs.ClassicController.HOME);
                    #endregion
                }
                else if (deviceType == ControllerType.ClassicControllerPro)
                {
                    #region Classic Pro Clickables
                    deviceShapes.Add(ccp_aClick     , Inputs.ClassicControllerPro.A);
                    deviceShapes.Add(ccp_bClick     , Inputs.ClassicControllerPro.B);
                    deviceShapes.Add(ccp_xClick     , Inputs.ClassicControllerPro.X);
                    deviceShapes.Add(ccp_yClick     , Inputs.ClassicControllerPro.Y);
                    deviceShapes.Add(ccp_lClick     , Inputs.ClassicControllerPro.L);
                    deviceShapes.Add(ccp_zlClick    , Inputs.ClassicControllerPro.ZL);
                    deviceShapes.Add(ccp_rClick     , Inputs.ClassicControllerPro.R);
                    deviceShapes.Add(ccp_zrClick    , Inputs.ClassicControllerPro.ZR);
                    deviceShapes.Add(ccp_upClick    , Inputs.ClassicControllerPro.UP);
                    deviceShapes.Add(ccp_downClick  , Inputs.ClassicControllerPro.DOWN);
                    deviceShapes.Add(ccp_leftClick  , Inputs.ClassicControllerPro.LEFT);
                    deviceShapes.Add(ccp_rightClick , Inputs.ClassicControllerPro.RIGHT);
                    deviceShapes.Add(ccp_lupClick   , Inputs.ClassicControllerPro.LUP);
                    deviceShapes.Add(ccp_ldownClick , Inputs.ClassicControllerPro.LDOWN);
                    deviceShapes.Add(ccp_lleftClick , Inputs.ClassicControllerPro.LLEFT);
                    deviceShapes.Add(ccp_lrightClick, Inputs.ClassicControllerPro.LRIGHT);
                    deviceShapes.Add(ccp_rupClick   , Inputs.ClassicControllerPro.RUP);
                    deviceShapes.Add(ccp_rdownClick , Inputs.ClassicControllerPro.RDOWN);
                    deviceShapes.Add(ccp_rleftClick , Inputs.ClassicControllerPro.RLEFT);
                    deviceShapes.Add(ccp_rrightClick, Inputs.ClassicControllerPro.RRIGHT);
                    deviceShapes.Add(ccp_startClick , Inputs.ClassicControllerPro.START);
                    deviceShapes.Add(ccp_selectClick, Inputs.ClassicControllerPro.SELECT);
                    deviceShapes.Add(ccp_homeClick  , Inputs.ClassicControllerPro.HOME);
                    #endregion
                }
                else if (deviceType == ControllerType.Nunchuk 
                      || deviceType == ControllerType.NunchukB)
                {
                    #region Nunchuk Clickables
                    deviceShapes.Add(nk_cClick, Inputs.Nunchuk.C);
                    deviceShapes.Add(nk_zClick, Inputs.Nunchuk.Z);
                    deviceShapes.Add(nk_upClick, Inputs.Nunchuk.UP);
                    deviceShapes.Add(nk_downClick, Inputs.Nunchuk.DOWN);
                    deviceShapes.Add(nk_leftClick, Inputs.Nunchuk.LEFT);
                    deviceShapes.Add(nk_rightClick, Inputs.Nunchuk.RIGHT);
                    // TODO: other Nunchuk Clickables (Acc) (not for 1st release)
                    deviceShapes.Add(nk_aRightClick, Inputs.Nunchuk.ACC_SHAKE_X);
                    deviceShapes.Add(nk_aUpClick, Inputs.Nunchuk.ACC_SHAKE_Y);
                    deviceShapes.Add(nk_aForwardClick, Inputs.Nunchuk.ACC_SHAKE_Z);
                    deviceShapes.Add(nk_aRollClick, Inputs.Nunchuk.TILT_LEFT);
                    deviceShapes.Add(nk_aRollNegClick, Inputs.Nunchuk.TILT_RIGHT);
                    deviceShapes.Add(nk_aPitchClick, Inputs.Nunchuk.TILT_UP);
                    deviceShapes.Add(nk_aPitchNegClick, Inputs.Nunchuk.TILT_DOWN);
                    #endregion
                }
            }
            // TODO: add other controller maps (Balance Board, Musicals) (not for 1st release)

            if (deviceType == ControllerType.ProController)
            {
                currentSelection = pro_homeClick;
            }
            else if (deviceType == ControllerType.ClassicController)
            {
                currentSelection = cc_homeClick;
            }
            else if (deviceType == ControllerType.ClassicControllerPro)
            {
                currentSelection = ccp_homeClick;
            }
            else
            {
                currentSelection = wm_homeClick;
            }

            device_MouseDown(currentSelection, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));
        }

        private void MoveSelector(Shape s, Ellipse selector, Line guide)
        {
            double top = 0;
            double left = 0;

            if (s.Name.StartsWith("wm") && deviceType != ControllerType.Wiimote
                                        && deviceType != ControllerType.Nunchuk
                                        && deviceType != ControllerType.NunchukB)
            {
                top = 0;// Canvas.GetLeft((UIElement)s.Parent) - 50;
                top += WmGrid.Width - s.Margin.Left;
                top += selector.Width / 2;
                top -= s.Width / 2;
                top -= 22;

                left = Canvas.GetTop((UIElement)s.Parent);
                left += s.Margin.Top;
                left += selector.Height / 2;
                left += s.Height / 2;
                left += 20;
            }
            else
            {
                top = Canvas.GetTop(((UIElement)s.Parent));
                top += s.Margin.Top;
                top -= selector.Height / 2;
                top += s.Height / 2;

                left = Canvas.GetLeft((UIElement)(s.Parent));
                left += s.Margin.Left;
                left -= selector.Width / 2;
                left += s.Width / 2;
            }

            Canvas.SetTop(selector, top);
            Canvas.SetLeft(selector, left);

            Canvas.SetLeft(guide, Canvas.GetLeft(selector) + selector.Width / 2);
            guide.Y2 = Canvas.GetTop(selector) - selector.Height / 2 - selector.StrokeThickness;

            if (guide == guideMap)
            {
                guideMeet.X2 = Canvas.GetLeft(guide);
            }
            else
            {
                guideMeet.X1 = Canvas.GetLeft(guide);
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Shape clickArea = (Shape)sender;
            clickArea.StrokeThickness = 1;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Shape clickArea = (Shape)sender;
            clickArea.StrokeThickness = 0;
        }

        private void device_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Shape s = (Shape)sender;
            deviceLabel.Content = s.ToolTip;
            MoveSelector((Shape)sender, selectionDevice, guideDevice);
            currentSelection = s;

            if (deviceShapes.ContainsKey(s) && map.ContainsKey(deviceShapes[s]))
            {
                MoveSelector(mapShapes[map[deviceShapes[s]]], selectionMap, guideMap);
                mapLabel.Content = mapShapes[map[deviceShapes[s]]].ToolTip;
            }
        }

        private void map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Shape s = (Shape)sender;
            mapLabel.Content = s.ToolTip;
            MoveSelector((Shape)sender, selectionMap, guideMap);

            if (deviceShapes.ContainsKey(currentSelection) && mapValues.ContainsKey(s) && map.ContainsKey(deviceShapes[currentSelection]))
            {
                map[deviceShapes[currentSelection]] = mapValues[s];
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            result = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            result = false;
            Close();
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            map = Holders.XInputHolder.GetDefaultMapping(deviceType).ToDictionary(entry => entry.Key, entry => entry.Value);
            result = true;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Profile newProfile = new Profile(deviceType);
            foreach (KeyValuePair<string, string> item in map)
            {
                newProfile.controllerMapKeys.Add(item.Key);
                newProfile.controllerMapValues.Add(item.Value);
            }

            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = deviceType.ToString() + "_profile";
            dialog.DefaultExt = ".wsp";
            dialog.Filter = App.PROFILE_FILTER;

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Profile));
                
                using (FileStream stream = File.Create(dialog.FileName))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    serializer.Serialize(writer, newProfile);
                    writer.Close();
                    stream.Close();
                }
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = deviceType.ToString() + "_profile";
            dialog.DefaultExt = ".wsp";
            dialog.Filter = App.PROFILE_FILTER;

            bool? doLoad = dialog.ShowDialog();
            Profile loadedProfile = null;

            if (doLoad == true && dialog.CheckFileExists)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Profile));

                    using (FileStream stream = File.OpenRead(dialog.FileName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadedProfile = serializer.Deserialize(reader) as Profile;
                        reader.Close();
                        stream.Close();
                    }
                }
                catch (Exception err)
                {
                    var c = MessageBox.Show("Could not open the file \"" + err.Message + "\".", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (loadedProfile != null)
                {
                    if (loadedProfile.profileType != deviceType)
                    {
                        MessageBoxResult m = MessageBox.Show("Profile is not for this controller type. Load anyway?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        
                        if (m != MessageBoxResult.Yes)
                        {
                            doLoad = false;
                        }
                    }

                    if (doLoad == true)
                    {
                        for (int i = 0; i < Math.Min(loadedProfile.controllerMapKeys.Count, loadedProfile.controllerMapValues.Count); i++)
                        {
                            if (map.ContainsKey(loadedProfile.controllerMapKeys[i]))
                            {
                                map[loadedProfile.controllerMapKeys[i]] = loadedProfile.controllerMapValues[i];
                            }
                            else
                            {
                                map.Add(loadedProfile.controllerMapKeys[i], loadedProfile.controllerMapValues[i]);
                            }
                        }

                        result = true;
                        Close();
                    }
                }
            }
            else if (doLoad == true && !dialog.CheckFileExists)
            {
                var a = MessageBox.Show("Could not find the file \"" + dialog.FileName + "\".", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
