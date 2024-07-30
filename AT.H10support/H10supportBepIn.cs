using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using EFT.Ballistics;
using EFT.InventoryLogic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using EFT.CameraControl;
using kcp2k;
using System.Net;
using static RootMotion.FinalIK.AimPoser;

namespace AT.H10support
{
    [BepInPlugin("AT.H10support", "AT.H10support", "1.1.0.0")]
    public class H10supportBepIn : BaseUnityPlugin
    {
        public static Object H10Prefab { get; private set; }
        public static Object 鹰击Prefab { get; private set; }
        public static Object 狂飙Prefab { get; private set; }
        public static Object 雷达框Prefab { get; private set; }
        public static ConfigEntry<KeyCode> 召唤;
        public static ConfigEntry<KeyCode> 弹射;
        public static ConfigEntry<KeyCode> 摧毁;
        public static ConfigEntry<KeyCode> 挂载切换;
        public static ConfigEntry<KeyCode> 启动机载雷达;
        public static ConfigEntry<KeyCode> 导弹目标切换;
        public static ConfigEntry<bool> 操作加速度;
        public static ConfigEntry<float> 垂直机动性;
        public static ConfigEntry<float> 翻转机动性;
        public static ConfigEntry<float> 导弹冷却时间;
        public static ConfigEntry<int> 导弹载弹量;
        public static ConfigEntry<int> 机炮载弹量;
        public static ConfigEntry<string> 机炮配置;
        public static ConfigEntry<string> 导弹配置;
        public static ConfigEntry<string> 下挂配置;
        public static string[] 机炮配置列表 = { "普通机炮(AP)", "速射机炮(Speed)", "高爆机炮(HE)" };
        public static string[] 导弹配置列表 = { "无制导火箭弹(Rocket)", "激光制导导弹(Laser guidance)", "红外追踪导弹(Infrared tracking)", "集束导弹(Cluster missile)" };
        public static string[] 下挂配置列表 = { "氢弹(Hydrogen bomb)", "弹射舱(Ejection seat)" };
        public void Awake()
        {
            召唤 = Config.Bind<KeyCode>("按键设置(Key setting)", "召唤按键(Open button)", KeyCode.K, "轰十无人机启动按钮(Drone start button)");
            弹射 = Config.Bind<KeyCode>("按键设置(Key setting)", "弹射按键(Ejection key)", KeyCode.F11, "轰十无人机弹射按钮，需要装备弹射舱，按下一秒后人物会移动到飞机当前位置(The ejection button of the drone needs to be equipped with the Ejection seat, and the character will move to the current position of the aircraft after being pressed for one second)");
            摧毁 = Config.Bind<KeyCode>("按键设置(Key setting)", "摧毁按键(Destroy key)", KeyCode.J, "轰十无人机摧毁按钮(Drone destroy button)");
            挂载切换 = Config.Bind<KeyCode>("按键设置(Key setting)", "挂载切换按键(Weapon switch button)", KeyCode.F, "导弹核弹切换(Missile or hydrogen bomb switching)");
            导弹目标切换 = Config.Bind<KeyCode>("按键设置(Key setting)", "导弹锁定或切换目标(Infrared tracking Missile locks or switches targets)", KeyCode.R, "开启机载雷达后并装备红外追踪导弹，按下后切换目标(Turn on the airborne radar and equip the infrared tracking missile, press and switch targets)");
            启动机载雷达 = Config.Bind<KeyCode>("按键设置(Key setting)", "启动机载雷达(Activated airborne radar)", KeyCode.Tab, "启动后全图AI显示框和距离，可搭配红外追踪弹锁定AI(After starting, the position and distance of all AI are displayed, and the AI can be locked with infrared tracking bullets)");
            操作加速度 = Config.Bind<bool>("飞行系统设置(Flight system setup)", "是否启动操作加速度(Whether to start operating acceleration)", true, "启动后飞机抬升或翻转操作时会有个加速过程(There is an acceleration process when the aircraft lifts or flips after starting)");
            垂直机动性 = Config.Bind("飞行系统设置(Flight system setup)", "垂直机动性参数(Vertical mobility parameters)", 0.2f, new ConfigDescription("飞机进行上下起降的灵敏系数(The sensitivity factor of the aircraft to take off and land)", new AcceptableValueRange<float>(0.01f, 1)));
            翻转机动性 = Config.Bind("飞行系统设置(Flight system setup)", "翻转机动性参数(Rollover maneuverability parameters)", 0.3f, new ConfigDescription("飞机进行左右翻滚的灵敏系数(The sensitivity factor of the aircraft to roll left and right)", new AcceptableValueRange<float>(0.01f, 1)));
            导弹冷却时间 = Config.Bind<float>("武器配置(armament)", "导弹冷却时间(Missile cooling time)", 5, "导弹装填所需时间(Missile loading time)");
            导弹载弹量 = Config.Bind<int>("武器配置(armament)", "导弹载弹量(Missile load)", 24, "导弹载弹数量(Missile load)");
            机炮载弹量 = Config.Bind<int>("武器配置(armament)", "机炮载弹量(Gun load capacity)", 300, "机炮载弹数量(Gun load capacity)");
            机炮配置 = Config.Bind<string>("武器配置(armament)", "选择你的机炮类型(Choose your gun type)", 机炮配置列表[0],
                new ConfigDescription("选择你的机炮类型(Choose your gun type)", new AcceptableValueList<string>(机炮配置列表)));
            导弹配置 = Config.Bind<string>("武器配置(armament)", "选择你的导弹类型(Choose your missile type)", 导弹配置列表[0],
                new ConfigDescription("选择你的导弹类型(Choose your missile type)", new AcceptableValueList<string>(导弹配置列表)));
            下挂配置 = Config.Bind<string>("武器配置(armament)", "选择你的战术武器(Choose your tactical weapon)", 下挂配置列表[0],
                new ConfigDescription("选择你的战术武器(Choose your tactical weapon)", new AcceptableValueList<string>(下挂配置列表)));
        }

        void Start()
        {
            if (H10Prefab == null)
            {
                String filename = Path.Combine(Environment.CurrentDirectory, "BepInEx/plugins/atmod/h10");
                if (!File.Exists(filename))
                    return;

                var H10Bundle = AssetBundle.LoadFromFile(filename);
                if (H10Bundle == null)
                    return;
                H10Prefab = H10Bundle.LoadAsset("Assets/AT模组/轰-10/轰10.prefab");
                鹰击Prefab = H10Bundle.LoadAsset("Assets/AT模组/轰-10/鹰击114.prefab");
                狂飙Prefab = H10Bundle.LoadAsset("Assets/AT模组/轰-10/狂飙514.prefab");
                雷达框Prefab = H10Bundle.LoadAsset("Assets/AT模组/轰-10/轰十雷达框.prefab");
                Console.WriteLine("已加载轰10-太阳黑子");
            }
        }
        void Update()
        {

        }
    }
    internal static class ItemFactory
    {
        private static object instance;

        private static MethodBase methodCreateItem;
        private static Dictionary<string, ItemTemplate> dict = new Dictionary<string, ItemTemplate>();
        public static void Init()
        {
            Type type = (from p in typeof(GameWorld).GetMethod("InitLevel", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).GetParameters()
                         where p.Name == "itemFactory"
                         select p.ParameterType).First<Type>();
            Type type2 = typeof(Singleton<>).MakeGenericType(new Type[]
            {
                type
            });
            instance = type2.GetProperty("Instance").GetValue(type2);
            methodCreateItem = type.GetMethod("CreateItem");
            dict = (Dictionary<string, ItemTemplate>)type.GetField("ItemTemplates").GetValue(ItemFactory.instance);
        }

        public static Item CreateItem(string id, string tpid)
        {
            if (null == ItemFactory.methodCreateItem)
            {
                ItemFactory.Init();
            }
            MethodBase methodBase = ItemFactory.methodCreateItem;
            object obj = ItemFactory.instance;
            object[] array = new object[3];
            array[0] = id;
            array[1] = tpid;
            return (Item)methodBase.Invoke(obj, array);
        }

        public static ItemTemplate GetItemTemplateById(string id)
        {
            if (null == ItemFactory.methodCreateItem)
            {
                ItemFactory.Init();
            }
            return ItemFactory.dict[id];
        }
    }
    internal class Shoot
    {
        private static BallisticsCalculator ballisticsCalculator;
        private static readonly MethodBase methodShoot = typeof(BallisticsCalculator).GetMethod("Shoot");
        private static readonly MethodBase methodCreateShot = typeof(BallisticsCalculator).GetMethod("CreateShot");
        private readonly object Player;
        private readonly Weapon Weapon;
        private readonly Item Bullet;
        public object MakeShot(Vector3 shotPosition, Vector3 shotDirection, float speedFactor)
        {
            object obj = Shoot.methodCreateShot.Invoke(Shoot.ballisticsCalculator, new object[]
            {
                this.Bullet,
                shotPosition,
                shotDirection,
                0,
                this.Player,
                this.Weapon,
                speedFactor,
                0
            });
            Shoot.methodShoot.Invoke(Shoot.ballisticsCalculator, new object[]
            {
                obj
            });
            return obj;
        }

        public static string GenId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 24);
        }

        public Shoot(Player player, string weaponId = "5d52cc5ba4b9367408500062", string bulletId = "5d70e500a4b9364de70d38ce")
        {
            Shoot.ballisticsCalculator = Comfort.Common.Singleton<GameWorld>.Instance._sharedBallisticsCalculator;
            Type parameterType = Shoot.methodCreateShot.GetParameters()[4].ParameterType;
            if (parameterType == typeof(string))
            {
                this.Player = player.ProfileId;
            }
            else if (parameterType == typeof(Player))
            {
                this.Player = player;
            }
            if (this.Player == null)
            {
                throw new Exception("不支持的版本！Player未为null！");
            }
            this.Weapon = (Weapon)ItemFactory.CreateItem(Shoot.GenId(), weaponId);
            this.Bullet = ItemFactory.CreateItem(Shoot.GenId(), bulletId);
        }
    }
}
