using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AT.H10support
{
    public class H10HUD : MonoBehaviour
    {
        public Text 速度;
        public Text 高度;
        public Text 弹药文本;
        public Text 机炮备弹;
        public Text 导弹备弹;
        public Text 炸弹备弹;
        public Image 导弹图标;
        public Image 导弹图标2;
        public Image 炸弹图标;
        public Image 炸弹图标2;
        public Image 水平仪背景;
        public Image 水平仪;
        public Image 选择箭头;
        public Image 弹药指示;
        public Image 准星显示;
        public Image 锁定框1;
        public Image 锁定框2;
        public static Vector2 锁定框1坐标;
        private float 当前速度;
        private float 当前高度;
        private float 一号冷却;
        private float 二号冷却;
        private float 导弹冷却;
        private int 最大机炮弹药;
        public static int 机炮弹药;
        public static int 导弹弹药;
        public static int 核弹弹药;
        public static bool 导弹还是核弹;
        public static bool 一号导弹;
        public static bool 二号导弹;
        public static bool 一号核弹;
        public static bool 二号核弹;
        private string 机炮名;
        private string 一号挂载名;
        private string 二号挂载名;
        void Start()
        {
            导弹弹药 = H10supportBepIn.导弹载弹量.Value;
            核弹弹药 = 2;
            一号冷却 = 0;
            二号冷却 = 0;
            导弹冷却 = H10supportBepIn.导弹冷却时间.Value;
            一号导弹 = true;
            二号导弹 = true;
            一号核弹 = true;
            二号核弹 = true;
            锁定框1坐标 = new Vector2(0, 0);
            导弹还是核弹 = true;
            if (H10supportBepIn.机炮配置.Value == "普通机炮(AP)")
            {
                机炮名 = "25-1";
                机炮弹药 = H10supportBepIn.机炮载弹量.Value;
                最大机炮弹药 = 机炮弹药;
            }
            if (H10supportBepIn.机炮配置.Value == "速射机炮(Speed)")
            {
                机炮名 = "QJK99";
                机炮弹药 = H10supportBepIn.机炮载弹量.Value * 2;
                最大机炮弹药 = 机炮弹药;
            }
            if (H10supportBepIn.机炮配置.Value == "高爆机炮(HE)")
            {
                机炮名 = "ZPT-99";
                机炮弹药 = H10supportBepIn.机炮载弹量.Value / 10;
                最大机炮弹药 = 机炮弹药;
            }
            if (H10supportBepIn.导弹配置.Value == "无制导火箭弹(Rocket)") 一号挂载名 = "火蛇-70";
            if (H10supportBepIn.导弹配置.Value == "激光制导导弹(Laser guidance)") 一号挂载名 = "蓝箭-7";
            if (H10supportBepIn.导弹配置.Value == "红外追踪导弹(Infrared tracking)") 一号挂载名 = "鹰击-9E";
            if (H10supportBepIn.导弹配置.Value == "集束导弹(Cluster missile)") 一号挂载名 = "天雷-500";
            if (H10supportBepIn.下挂配置.Value == "氢弹(Hydrogen bomb)") 二号挂载名 = "狂飙一号";
            if (H10supportBepIn.下挂配置.Value == "弹射舱(Ejection seat)") { 二号挂载名 = "弹射器"; 核弹弹药 = 1; }
            弹药文本.GetComponent<Text>().text = 机炮名 + "\n" + 一号挂载名 + "\n" + 二号挂载名;
        }
        void FixedUpdate()
        {
            //Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, 1 << 12 | 1 << 18 | 1 << 11 | 1 << 8);
            //if (Input.GetKey(KeyCode.Y))
            //{ Console.WriteLine(hitInfo.collider.name); }
            选择目标();
            机炮备弹.GetComponent<Text>().text = 机炮弹药.ToString();
            导弹备弹.GetComponent<Text>().text = 导弹弹药.ToString();
            炸弹备弹.GetComponent<Text>().text = 核弹弹药.ToString();
            弹药指示.fillAmount = (float)机炮弹药 / 最大机炮弹药;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (!弹药指示.GetComponent<Image>().enabled) 弹药指示.GetComponent<Image>().enabled = true;
                if (!准星显示.GetComponent<Image>().enabled) 准星显示.GetComponent<Image>().enabled = true;
            }
            else
            {
                if (弹药指示.GetComponent<Image>().enabled) 弹药指示.GetComponent<Image>().enabled = false;
                if (准星显示.GetComponent<Image>().enabled) 准星显示.GetComponent<Image>().enabled = false;
            }
            当前速度 = (int)((H10support.飞行速度) * 3800 + 1);
            速度.GetComponent<Text>().text = 当前速度.ToString();
            当前高度 = (int)(transform.position.y * 10);
            高度.GetComponent<Text>().text = 当前高度.ToString();
            水平仪背景.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -transform.eulerAngles.z);
            if (transform.eulerAngles.x > 90) 水平仪.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 298 * -((360 - transform.eulerAngles.x) / 90), 0);
            if (transform.eulerAngles.x < 90) 水平仪.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 298 * (transform.eulerAngles.x / 90), 0);
            if (Input.GetKeyDown(H10supportBepIn.挂载切换.Value))
            {
                导弹还是核弹 = !导弹还是核弹;
                if (导弹还是核弹) 选择箭头.GetComponent<RectTransform>().anchoredPosition = new Vector3(-42.5f, -0.8F, 0);
                if (!导弹还是核弹) 选择箭头.GetComponent<RectTransform>().anchoredPosition = new Vector3(-42.5f, -17, 0);
            }
            if (!一号导弹)
            {
                一号冷却 += Time.deltaTime;
                导弹图标.fillAmount = 一号冷却 / 导弹冷却;
                if (一号冷却 > 导弹冷却)
                {
                    一号冷却 = 导弹冷却;
                    导弹图标.fillAmount = 一号冷却 / 导弹冷却;
                    一号导弹 = true;
                    一号冷却 = 0;
                }
            }
            if (!二号导弹)
            {
                二号冷却 += Time.deltaTime;
                导弹图标2.fillAmount = 二号冷却 / 导弹冷却;
                if (二号冷却 > 导弹冷却)
                {
                    二号冷却 = 导弹冷却;
                    导弹图标2.fillAmount = 二号冷却 / 导弹冷却;
                    二号导弹 = true;
                    二号冷却 = 0;
                }
            }
            if (!一号核弹) 炸弹图标.fillAmount = 0;
            if (!二号核弹) 炸弹图标2.fillAmount = 0;
        }
        private void 选择目标()
        {
            if (H10Weapon.导弹准备发射)
            { 
                锁定框1.GetComponent<Image>().enabled = true;
                锁定框1.transform.position = 锁定框1坐标;
            }
            else 锁定框1.GetComponent<Image>().enabled = false;
        }
    }
}
