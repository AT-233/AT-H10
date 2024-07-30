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
    public class H10Weapon : MonoBehaviour
    {
        public AudioClip[] 机炮音效;
        public AudioClip[] 导弹音效;
        public GameObject 机炮发射点;
        public GameObject 导弹炸弹发射点;
        public GameObject 机头检测点;
        public GameObject 左翼检测点;
        public GameObject 右翼检测点;
        public static float 机头距离 = 0f;
        public static float 机头下方距离 = 0f;
        public static float 左翼距离 = 0f;
        public static float 右翼距离 = 0f;
        private Vector3 导弹锁定坐标;
        private float 射击间隔时间 = 0f;
        private float 导弹射击间隔时间 = 0f;
        private float 射击间隔时间上限 = 0f;
        private float 速射音效间隔时间 = 0f;
        private bool 速射机炮开火 = true;
        private bool 允许机炮开火;
        private bool 允许导弹发射=true;
        private bool 雷达已开启 = false;
        public static bool 导弹准备发射 = false;
        private float timer = 0;
        private int AI标记系数 = 0;
        public static int 机炮类型 = 0;
        public static int 导弹类型 = 0;
        private GameObject[] 雷达目标;
        private bool[] 雷达目标可被锁定;
        Shoot 机炮参数 = new Shoot(Singleton<GameWorld>.Instance.MainPlayer, "H10gun", "0001aa10aaa1145141919810h10gun"); //缓存射击预设
        Shoot 速射机炮参数 = new Shoot(Singleton<GameWorld>.Instance.MainPlayer, "H10gun1", "0001aa20aaa1145141919810h10gunspeed");
        Shoot 高爆机炮参数 = new Shoot(Singleton<GameWorld>.Instance.MainPlayer, "H10gun2", "0001aa30aaa1145141919810h10gunhe");
        Shoot 碰撞参数 = new Shoot(Singleton<GameWorld>.Instance.MainPlayer, "H10collision", "0001aaa10aa1145141919810h10yj114");
        void 机炮发射()
        {
            机炮参数.MakeShot(机炮发射点.transform.position, 机炮发射点.transform.forward, 1f);//从左到右是射击坐标，射击角度，
        }
        void 速射机炮发射()
        {
            速射机炮参数.MakeShot(左翼检测点.transform.position, 机炮发射点.transform.forward, 1f);
            速射机炮参数.MakeShot(右翼检测点.transform.position, 机炮发射点.transform.forward, 1f);
        }
        void 高爆机炮发射()
        {
            高爆机炮参数.MakeShot(机炮发射点.transform.position, 机炮发射点.transform.forward, 1f);//从左到右是射击坐标，射击角度，
        }
        void Start()
        {
            导弹锁定坐标 = new Vector3(0, 0, 0);
            雷达目标 = new GameObject[999];
            雷达目标可被锁定 = new bool[999];
            if (H10supportBepIn.机炮配置.Value == "普通机炮(AP)")
            {
                机炮类型 = 0;
                射击间隔时间上限 = 0.15f;
            }
            if (H10supportBepIn.机炮配置.Value == "速射机炮(Speed)")
            {
                机炮类型 = 1;
                射击间隔时间上限 = 0.09f;
                速射音效间隔时间 = 0.18f;
            }
            if (H10supportBepIn.机炮配置.Value == "高爆机炮(HE)")
            {
                机炮类型 = 2;
                射击间隔时间上限 = 0.5f;
            }
            if (H10supportBepIn.导弹配置.Value == "无制导火箭弹(Rocket)") 导弹类型 = 0;
            if (H10supportBepIn.导弹配置.Value == "激光制导导弹(Laser guidance)") 导弹类型 = 1;
            if (H10supportBepIn.导弹配置.Value == "红外追踪导弹(Infrared tracking)") 导弹类型 = 2;
            if (H10supportBepIn.导弹配置.Value == "集束导弹(Cluster missile)") 导弹类型 = 3;
            机炮发射点.GetComponent<AudioSource>().clip = 机炮音效[机炮类型];
            导弹炸弹发射点.GetComponent<AudioSource>().clip = 导弹音效[导弹类型];
        }
        void FixedUpdate()
        {
            机炮发射参数();
            开启机载雷达();
            if (H10HUD.导弹还是核弹) 导弹发射参数();
            if (!H10HUD.导弹还是核弹) 核弹发射参数();
            距离检测();
        }
        private void 机炮发射参数()
        {
            if (Input.GetKey(KeyCode.Mouse0) && 允许机炮开火 && H10HUD.机炮弹药 > 0)
            {
                if (机炮类型 == 0) 机炮发射();
                if (机炮类型 == 1) 速射机炮发射();
                if (机炮类型 == 2) 高爆机炮发射();
                if(机炮类型 != 1) 机炮发射点.GetComponent<AudioSource>().Play();
                if(机炮类型 == 1&&速射机炮开火)
                {
                    机炮发射点.GetComponent<AudioSource>().Play();
                    速射机炮开火 = false;
                }
                H10HUD.机炮弹药--;
                允许机炮开火 = false;
            }
            if (!允许机炮开火)
            {
                射击间隔时间 += Time.deltaTime;
                if (射击间隔时间 > 射击间隔时间上限)
                {
                    允许机炮开火 = true;
                    射击间隔时间 = 0F;
                }
            }
            if (!速射机炮开火)
            {
                速射音效间隔时间 += Time.deltaTime;
                if (速射音效间隔时间 > 0.18f)
                {
                    速射机炮开火 = true;
                    速射音效间隔时间 = 0F;
                }
            }
        }
        private void 导弹发射参数()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && !H10HUD.二号导弹 && H10HUD.一号导弹 && H10HUD.导弹弹药 > 0 && 允许导弹发射)
            {
                导弹炸弹发射点.GetComponent<AudioSource>().Play();
                var 导弹 = Instantiate(H10supportBepIn.鹰击Prefab, transform.position, transform.rotation);
                var 导弹实体 = 导弹 as GameObject;
                if (导弹类型 == 2)
                {
                    var 传送坐标给导弹头 = 导弹实体.GetComponent<H10Missile>();
                    int 目标 = AI标记系数;
                    传送坐标给导弹头.锁定坐标(H10support.gameWorld.AllAlivePlayersList[目标].gameObject);
                }
                H10HUD.导弹弹药--;
                H10HUD.一号导弹 = false;
                允许导弹发射 = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && H10HUD.二号导弹 && !H10HUD.一号导弹 && H10HUD.导弹弹药 > 0 && 允许导弹发射)
            {
                导弹炸弹发射点.GetComponent<AudioSource>().Play();
                var 导弹 = Instantiate(H10supportBepIn.鹰击Prefab, transform.position, transform.rotation);
                var 导弹实体 = 导弹 as GameObject;
                if (导弹类型 == 2)
                {
                    var 传送坐标给导弹头 = 导弹实体.GetComponent<H10Missile>();
                    int 目标 = AI标记系数;
                    传送坐标给导弹头.锁定坐标(H10support.gameWorld.AllAlivePlayersList[目标].gameObject);
                }
                H10HUD.导弹弹药--;
                H10HUD.二号导弹 = false;
                允许导弹发射 = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse1) && H10HUD.一号导弹 && H10HUD.二号导弹 && H10HUD.导弹弹药 > 0 && 允许导弹发射)
            {
                导弹炸弹发射点.GetComponent<AudioSource>().Play();
                var 导弹 = Instantiate(H10supportBepIn.鹰击Prefab, transform.position, transform.rotation);
                var 导弹实体 = 导弹 as GameObject;
                if (导弹类型 == 2)
                {
                    var 传送坐标给导弹头 = 导弹实体.GetComponent<H10Missile>();
                    int 目标 = AI标记系数;
                    传送坐标给导弹头.锁定坐标(H10support.gameWorld.AllAlivePlayersList[目标].gameObject);
                }
                H10HUD.导弹弹药--;
                H10HUD.一号导弹 = false;
                允许导弹发射 = false;
            }
            if (!允许导弹发射)
            {
                导弹射击间隔时间 += Time.deltaTime;
                if (导弹射击间隔时间 > 0.1F)
                {
                    允许导弹发射 = true;
                    导弹射击间隔时间 = 0F;
                }
            }
        }
        private void 核弹发射参数()
        {
            if (H10supportBepIn.下挂配置.Value == "氢弹(Hydrogen bomb)")
            {
                if (Input.GetKeyDown(KeyCode.Mouse1) && !H10HUD.一号核弹 && H10HUD.二号核弹 && H10HUD.核弹弹药 > 0)
                {
                    var Nuclear = Instantiate(H10supportBepIn.狂飙Prefab, transform.position, new Quaternion(0, 0, 0, 0));
                    H10HUD.核弹弹药--;
                    H10HUD.二号核弹 = false;
                }
                if (Input.GetKeyDown(KeyCode.Mouse1) && H10HUD.一号核弹 && H10HUD.二号核弹 && H10HUD.核弹弹药 > 0)
                {
                    var Nuclear1 = Instantiate(H10supportBepIn.狂飙Prefab, transform.position, new Quaternion(0, 0, 0, 0));
                    H10HUD.核弹弹药--;
                    H10HUD.一号核弹 = false;
                }
            }
        }
        private void 距离检测()
        {
            Physics.Raycast(机头检测点.transform.position, 机头检测点.transform.forward, out RaycastHit 坤头, Mathf.Infinity, 1 << 12 | 1 << 18 | 1 << 11 | 1 << 8);
            机头距离 = 坤头.distance;
            Physics.Raycast(机头检测点.transform.position, -机头检测点.transform.up, out RaycastHit 下坤头, Mathf.Infinity, 1 << 12 | 1 << 18 | 1 << 11 | 1 << 8);
            机头下方距离 = 下坤头.distance;
            if ((机头距离 < 0.32F && 机头距离 > 0 && !H10support.牢大) || (Input.GetKeyDown(H10supportBepIn.摧毁.Value) && !H10support.牢大))
            {
                碰撞参数.MakeShot(机头检测点.transform.position, 机头检测点.transform.forward, 1f);
                H10support.牢大 = true;
                if (雷达目标 != null)
                {
                    for (int i = 1; i < 100; i++)
                    {
                        Destroy(雷达目标[i]);
                    }
                    导弹准备发射 = false;
                }
            }
        }
        private void 开启机载雷达()
        {
            if (Input.GetKeyDown(H10supportBepIn.启动机载雷达.Value) && H10support.gameWorld.AllAlivePlayersList.Count >= 2)
            {
                雷达已开启 = !雷达已开启;
                if(雷达已开启)
                {
                    for (int i = 1; i < H10support.gameWorld.AllAlivePlayersList.Count; i++)
                    {
                        雷达目标[i] = Instantiate(H10supportBepIn.雷达框Prefab, new Vector3(H10support.gameWorld.AllAlivePlayersList[i].Transform.position.x, H10support.gameWorld.AllAlivePlayersList[i].Transform.position.y + 0.8f, H10support.gameWorld.AllAlivePlayersList[i].Transform.position.z), H10support.gameWorld.AllAlivePlayersList[i].Transform.rotation) as GameObject;
                        雷达目标[i].transform.parent = H10support.gameWorld.AllAlivePlayersList[i].gameObject.transform;
                    }
                }
                else
                {
                    if (雷达目标 != null)
                    {
                        for (int i = 1; i < 100; i++)//获取全部AI
                        {
                            Destroy(雷达目标[i]);
                        }
                        导弹准备发射 = false;
                    }
                }
            }
            if (雷达已开启)
            {
                if(导弹类型==2)
                {
                    if (AI标记系数 >= (H10support.gameWorld.AllAlivePlayersList.Count)) AI标记系数 = 0;
                    for (int i = 1; i < H10support.gameWorld.AllAlivePlayersList.Count; i++)
                    {
                        Vector3 当前AI坐标 = H10support.gameWorld.AllAlivePlayersList[i].Transform.position;
                        雷达目标可被锁定[i] = 目标在屏幕内(当前AI坐标);
                    }
                    if (Input.GetKeyDown(H10supportBepIn.导弹目标切换.Value))
                    {
                        //Console.WriteLine(导弹准备发射);
                        //Console.WriteLine(AI标记系数);
                        if (AI标记系数 >= (H10support.gameWorld.AllAlivePlayersList.Count)) AI标记系数 = 0;
                        for (int i = AI标记系数+1; i <= H10support.gameWorld.AllAlivePlayersList.Count; i++)
                        {
                            if (雷达目标可被锁定[i])
                            {
                                AI标记系数 = i;
                                导弹准备发射 = true;
                                break;
                            }
                            if (i >= H10support.gameWorld.AllAlivePlayersList.Count)
                            {
                                AI标记系数 = 0;
                                break;
                            }
                        }
                    }
                    if (导弹准备发射)
                    {
                        if(!目标在屏幕内(H10support.gameWorld.AllAlivePlayersList[AI标记系数].Transform.position))
                        {
                            导弹准备发射 = false;
                            AI标记系数 = 0;
                        }
                        H10HUD.锁定框1坐标 = Camera.main.WorldToScreenPoint(new Vector3(H10support.gameWorld.AllAlivePlayersList[AI标记系数].Transform.position.x, H10support.gameWorld.AllAlivePlayersList[AI标记系数].Transform.position.y + 0.8f, H10support.gameWorld.AllAlivePlayersList[AI标记系数].Transform.position.z));
                    }
                }
                timer += Time.deltaTime;
                if (timer >= 1.5f)
                {
                    if (雷达目标 != null)
                    {
                        for (int i = 1; i < 100; i++)//获取全部AI
                        {
                            Destroy(雷达目标[i]);
                        }
                    }
                    for (int i = 1; i < H10support.gameWorld.AllAlivePlayersList.Count; i++)
                    {
                        雷达目标[i] = Instantiate(H10supportBepIn.雷达框Prefab, new Vector3(H10support.gameWorld.AllAlivePlayersList[i].Transform.position.x, H10support.gameWorld.AllAlivePlayersList[i].Transform.position.y + 0.8f, H10support.gameWorld.AllAlivePlayersList[i].Transform.position.z), H10support.gameWorld.AllAlivePlayersList[i].Transform.rotation) as GameObject;
                        雷达目标[i].transform.parent = H10support.gameWorld.AllAlivePlayersList[i].gameObject.transform;
                    }
                    timer = 0;
                }
                if (!H10support.gameWorld.MainPlayer.ActiveHealthController.IsAlive  || (Input.GetKeyDown(H10supportBepIn.弹射.Value) && (H10supportBepIn.下挂配置.Value == "弹射舱(Ejection seat)")))
                {
                    雷达已开启 = false;
                    if (雷达目标 != null)
                    {
                        for (int i = 1; i < 100; i++)//获取全部
                        {
                            Destroy(雷达目标[i]);
                        }
                    }
                    雷达目标可被锁定 = new bool[100];
                    AI标记系数 = 0;
                }
            }
        }
        private bool 目标在屏幕内(Vector3 输入坐标)
        {
            Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(输入坐标);
            return viewPortPosition.z > 0 &&
                   viewPortPosition.x >= 0 && viewPortPosition.x <= 1 &&
                   viewPortPosition.y >= 0 && viewPortPosition.y <= 1;
        }
        private bool 无障碍(Vector3 输入坐标)
        {
            RaycastHit hit;
            Physics.Raycast(输入坐标, 机头检测点.transform.position - 输入坐标, out hit, Vector3.Distance(输入坐标, 机头检测点.transform.position), 1 << 12 | 1 << 18 | 1 << 11 | 1 << 8);
            return hit.collider == null;
        }
    }
}
