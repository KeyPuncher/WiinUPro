using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private NintrollerLib.ControllerType deviceType = NintrollerLib.ControllerType.Wiimote;
        private Shape currentSelection;

        private ConfigWindow()
        {
            InitializeComponent();
        }

        public ConfigWindow(Dictionary<string, string> mappings, NintrollerLib.ControllerType type)
        {
            InitializeComponent();

            // Copy the mappings over
            map = mappings.ToDictionary(entry => entry.Key, entry => entry.Value);
            deviceType = type;
            GetShapes();

            switch (deviceType)
            {
                case NintrollerLib.ControllerType.ProController:
                    ProGrid.Visibility = System.Windows.Visibility.Visible;
                    CCGrid.Visibility = System.Windows.Visibility.Hidden;
                    break;

                case NintrollerLib.ControllerType.ClassicController:
                    ProGrid.Visibility = System.Windows.Visibility.Hidden;
                    CCGrid.Visibility = System.Windows.Visibility.Visible;
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
            #endregion

            #region Pro Clickables
            deviceShapes = new Dictionary<Shape, string>();
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
            // TODO: add other controller maps

            if (deviceType == NintrollerLib.ControllerType.ProController)
            {
                currentSelection = pro_homeClick;
            }
            else if (deviceType == NintrollerLib.ControllerType.ClassicController)
            {
                currentSelection = cc_homeClick;
            }

            device_MouseDown(currentSelection, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));
        }

        private void MoveSelector(Shape s, Ellipse selector, Line guide)
        {
            double top = Canvas.GetTop(((UIElement)s.Parent));
            top += s.Margin.Top;
            top -= selector.Height / 2;
            top += s.Height / 2;

            double left = Canvas.GetLeft((UIElement)(s.Parent));
            left += s.Margin.Left;
            left -= selector.Width / 2;
            left += s.Width / 2;

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

            if (mapValues.ContainsKey(s) && map.ContainsKey(deviceShapes[currentSelection]))
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
    }
}
