/* NOTE: * * * * * * * * * * * * * * * * * * * * 
 * After adding WCF support with the current IPC
 * support, it was working with major amounts of
 * lag in the XInput Test. Also, rumble started
 * to work for IPC after switching back. Removing
 * all WCF exclusive code.
 * Refer to XInput Test project for working WCF.
 * * WCF may have been slow due to the injector
 * * timer being set at 100ms. (but IPC was Ok)
 * * * * * * * * * * * * * * * * * * * * * * * */

// For debug build
//#define DEBUG
// For testing x86 & x64 specific binaries
#define x86_64

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using NintrollerLib;

using EasyHook;
// IPC
using System.Runtime.Remoting.Channels.Ipc;
using XAgentCS;
using XAgentCS.Interface;

// TODO: Must disconnect from a process before creating a new one

namespace WiinUSoft
{
    public partial class Main : Form
    {
        #region Supported Games List
        public static List<String> goodList = new List<string>()
        {
            //TODO: Update
            "AlanWake",
            "alan_wakes_american_nightmare",
            "AnomalyWarzoneEarth",
            "Audiosurf",
            "Bastion",
            "Bioshock",
            "Bioshock2",
            "BladeKitten",
            "Blur",
            "castle",
            "Crysis",
            "DustAET",
            "Fallout3",
            "FalloutNV",
            "FUEL",
            "GTAIV",
            "EFLC",
            "JustCause2",
            "LIMBO",
            "Magicka",
            "METAL GEAR RISING REVENGEANCE",
            "PrinceOfPersia_Launcher",
            "Sam3",
            "SamHD",
            "SamHD_TSE",
            "soniccd",
            "sonic",
            "SonicII",
            "TESV",
            "Spelunky",
            "SWTFU2"
        };

        public static List<String> testList = new List<string>()
        {
            "ShippingPC-BmGame",
            "BatmanAC",
            "BFBC2Game",
            "Bioshock",
            "Bioshock2",
            "Crysis",
            "DeadIslandGame",
            "TESV",
            "Fallout3",
            "FalloutNV",
            "FarCry2",
            "FUEL",
            "GTAIV",
            "EFLC",
            "hl2",
            "HMA",
            "HMSC",
            "JustCause2",
            "left4dead",
            "left4dead2",
            "Magic_2012",
            "MassEffect",
            "metro2033",
            "MirrorsEdge",
            "NFS11",
            "Munch",
            "stranger",
            "portal2",
            "PrinceOfPersia_Launcher",
            "RE5DX10",
            "RE5DX9",
            "Sam3",
            "SamHD",
            "SamHD_TSE",
            "BattlefrontII",
            "RepublicHeroes",
            "SWTFU",
            //"SWTFU Launcher",
            "SWTFU2",
            "SuperMeatBoy",
            "trine",
            
            "Audiosurf",
            "BioShockInfinite",
            "BrutalLegend",
            "csgo",
            "DARKSOULS",
            "DarksidersPC",
            "Dead Space",
            "DeadSpace",
            "DukeForever",
            "FEZ",
            "Injustice",
            "MKKE",
            "OrcsMustDie",
            "SaintsRowTheThird",
            "SaintsRowTheThird_DX11",
 //           "config",
            "Sonic Adventure DX",
            "SonicAdventureDX",
 //           "Launcher",
            "sonic2app",
            "soniccd",
//            "ConfigurationTool",
            "SonicGenerations",
            "WormsMayhem",
            "WormsRevolution",

            "QuestViewer",
            "Bastion",
            "Borderlands",
            "Borderlands2",
            "castle",
            "DarkSoulsII",
            "dxhr",
            "dxhrml",
            "DevilMayCry4_DX9",
            "DevilMayCry4_DX10",
            "DustAET",
            "HotlineGL",
            "Magicka",
            "game",
            "METAL GEAR RISING REVENGEANCE",
            "METALGEARRISINGREVENGEANCE",
            "MetroLL",
            "stanley",

            "bf3",
            "TheWolfAmoungUs",
            "Sam_HD",
            "SamHD_TSE_Unrestricted",

            "ACBSP",
            "Blur",
            "braid",
            "LIMBO",

            "SonicGenerations",
            "SonicGenerations.fxpipeline",
            "soniccd",
            "sonic",
            "SonicII",
            "sonic2app",
            "Sonic Adventure DX",

            "AlanWake",
            "alan_wakes_american_nightmare",
            "AnomalyWarzoneEarth",
            "BladeKitten",
            "BrutalLegend",
            "BurnoutParadise",
            "Fable3",
            "GSGameExe",
            "TLR",
            "witn",
            "game",
            "moh",
            "Rayman Origins",
            "SanctumGame-Win32-Shipping",
            "Launcher",
            "Spelunky",
            "TombRaider",
            "tra",
            "trl",
            "tru",

            "braid",
            "Darksiders2",
            "LANLauncher",
            "rf4_launcher",

            "DMC-DevilMayCry",

            "BattleBlockTheater",
            "BlackOps",

            "LaMulanaWin",
            "Jamestown",

            "farcry3",
            "fifa13",
            "Injustice",
            "pes2013",
            "Rayman Legends",
            "SaintsRowIV",
            "witcher2",
            "TombRaider",
            "Darksiders2",
            "dishonored",

            "FORCED",
            "ChildofLight",
            "Transistor",
            "NS3FB",
            "Cave",
            "Shipping-Thief",
            "watch_dogs",

            "Aquaria",
            "Crysis2",
            "DunDefGame",
            "SSFIV",
            "trine2_32bit",
            "WormsReloaded"
        };
        public static void AddGame(string game)
        {
            testList.Add(game);
        }
        #endregion

        internal static XControllers[] xDevices;

        private Dictionary<string, bool> processes;
        private string ChannelName = null;

        //IPC
        public const bool useIPC = false;
        private IpcServerChannel serverChannel;
        internal static InputInterface serverInterface;

        private List<DeviceControl> availableDevices;
        private static int connectedControllers = 0;

        #if DEBUG
        public LogForm logger;
        #endif

            // When this object is created
        public Main()
        {
            xDevices = new XControllers[4];
            for (int x = 0; x < xDevices.Length; x++)
            {
                xDevices[x] = new XControllers();
            }

            processes = new Dictionary<string, bool>();

            InitializeComponent();

            #if DEBUG
            logger = new LogForm();
            logger.Show();
            #endif
        }

        public int ControllerConnected (DeviceControl c)
        {
            #if DEBUG
            logger.logBox.AppendText("Controller Added\n");
            #endif

            flowPanelConnected.Controls.Add(c);
            return connectedControllers++;
        }

        public void MoveUp(DeviceControl c)
        {
            int location = flowPanelConnected.Controls.IndexOf(c);
            if (location > 0)
                flowPanelConnected.Controls.SetChildIndex(c, location - 1);

            foreach (DeviceControl d in flowPanelConnected.Controls)
            {
                d.SetPlayerNum(flowPanelConnected.Controls.IndexOf(d));
            }
        }

        public void MoveDown(DeviceControl c)
        {
            int location = flowPanelConnected.Controls.IndexOf(c);
            if (location < flowPanelConnected.Controls.Count - 1)
                flowPanelConnected.Controls.SetChildIndex(c, location + 1);

            foreach (DeviceControl d in flowPanelConnected.Controls)
            {
                d.SetPlayerNum(flowPanelConnected.Controls.IndexOf(d));
            }
        }

            // When the Main form is loaded
        private void Main_Load(object sender, EventArgs e)
        {
            // List supported paired BT devices
            List<string> supportedDevices = Nintroller.FindControllers();
            availableDevices = new List<DeviceControl>();

            // Form a list of those that are connected
            for (int i = 0; i < supportedDevices.Count; i++)
            {
                Nintroller n = new Nintroller(supportedDevices[i]);
                if (n.ConnectTest())
                {
                    availableDevices.Add(new DeviceControl(n));
                    flowPanelAvailable.Controls.Add(availableDevices[availableDevices.Count - 1]);
                }
            }

            // Test IPC
            if (useIPC)
            {
                for (int p = 0; p < goodList.Count; p++)
                    processes.Add(goodList[p], false);

                // Make sure to sign the DLL & that the Easyhook files are in the build dir
    #if x86
                Config.Register("XAgentCS", "XAgentCS_32.dll");
    #elif x64
                Config.Register("XAgentCS", "XAgentCS_64.dll");
    #else
                Config.Register("XAgentCS", "XAgentCS.dll");
    #endif
                var inputInterface = new InputInterface();
                inputInterface.RemoteMessage += inputInterface_RemoteMessage;
                inputInterface.RumbleEvent += inputInterface_RumbleEvent;
                serverInterface = inputInterface;
                serverChannel = RemoteHooking.IpcCreateServer<InputInterface>(ref ChannelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton, serverInterface);
                timer.Start();
            }
        }

            // When the refresh controllers list button is clicked
        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

            // Check for compatable programs to inject into
        private void timer_Tick(object sender, EventArgs e)
        {
            //foreach (string game in goodList)
            foreach (string game in testList)
            {
                Process[] process = Process.GetProcessesByName(game);
                if (process == null || process.Length < 1)
                {
                    // change the timer's tick if the game was removed from the list.
                    if (processes.ContainsKey(game) && processes[game])
                    {
                        timer.Interval = 2000;
                        Debug.WriteLine("Game Exited");
                    }

                    processes[game] = false;
                    continue;
                }

                if (!processes.ContainsKey(game))
                    continue;

                if (processes[game])
                    continue;
                else
                {
                    #if DEBUG
                    logger.logBox.AppendText("Attempting to Inject into process: " + process[0].ProcessName + "\n");
                    #endif

#if x86
                    RemoteHooking.Inject(
                        process[0].Id,
                        InjectionOptions.Default,
                        "XAgentCS_32.dll",
                        "XAgentCS_32.dll",
                        ChannelName);
#elif x64
                    RemoteHooking.Inject(
                        process[0].Id,
                        InjectionOptions.Default,
                        "XAgentCS_64.dll",
                        "XAgentCS_64.dll",
                        ChannelName);
#else
                    /*RemoteHooking.Inject(
                        process[0].Id,
                        InjectionOptions.Default,
                        typeof(InputInterface).Assembly.Location,
                        typeof(InputInterface).Assembly.Location,
                        ChannelName);*/
                    try
                    {
                        RemoteHooking.Inject(process[0].Id, "XAgentCS.dll", "XAgentCS.dll", ChannelName);
                    }
                    catch (Exception ex)
                    {
                        #if DEBUG
                        logger.logBox.AppendText(ex.Message + "\n");
                        logger.logBox.AppendText(ex.StackTrace + "\n\n");
                        #endif
                    }
#endif

                    /*Console.WriteLine(Application.StartupPath);

                    RemoteHooking.Inject(
                        process[0].Id, 
                        "C:\\XAgentCS32.dll", 
                        "C:\\XAgentCS64.dll", 
                        ChannelName);*/

                    // Inject the library
                    //RemoteHooking.Inject(process[0].Id, "XAgentCS.dll", "XAgentCS.dll", ChannelName);
                    processes[game] = true;

                    // slow the timer to not slowdown the input
                    //timer.Interval = 30000;
                }
            }
        }

            // Make sure the xinput interface is stopped and disconnected
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (DeviceControl d in availableDevices)
            {
                d.RemoveXInput();
            }
        }

        #region Remote Events
        void inputInterface_RemoteMessage(MessageReceivedEventArgs message)
        {
            Console.WriteLine("Remote Message: " + message.Message);

            #if DEBUG
            LogInfo("Message from Assembly:\n" + message.Message + "\n");
            #endif
        }

        void inputInterface_RumbleEvent(RumbleSetEventArgs args)
        {
            xDevices[args.Player].rumble = args.Rumble;

            #if DEBUG
            if (!hasRumbled)
            {
                LogInfo("Rumble Message Has been Recieved.\n");
                hasRumbled = true;
            }
            #endif
        }

        #if DEBUG
        public bool hasRumbled = false;
        private delegate void LogInfoDelegate(string message);
        void LogInfo(string info)
        {
            try
            {
                BeginInvoke(new LogInfoDelegate(Log), info);
            }
            catch
            {
                return;
            }
        }
        void Log(string m)
        {
            logger.logBox.AppendText(m + "\n");
        }
        #endif
        #endregion
    }
}
