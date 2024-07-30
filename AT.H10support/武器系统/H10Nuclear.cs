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
    public class H10Nuclear : MonoBehaviour
    {
        public GameObject 艺术就是爆炸;
        Shoot 核弹参数 = new Shoot(Singleton<GameWorld>.Instance.MainPlayer, "H10unclear", "0001aaa20aa1145141919810h10unclear");
        private float 初始速度 = 0f;
        private float 摧毁时间 = 0f;
        private float 核弹距离 = 0f;
        private bool 核爆;
        void Start()
        {
            if (H10support.飞行速度 != 0)
            {
                初始速度 = H10support.飞行速度;
                摧毁时间 = 45;
                核弹距离 = H10Weapon.机头下方距离;
                核爆 = true;
            }
        }
        void FixedUpdate()
        {
            摧毁时间 -= Time.deltaTime;
            初始速度 += Time.deltaTime * 0.5F;
            核弹距离 -= 初始速度;
            if (摧毁时间 < 0) Destroy(this.gameObject);
            if (核弹距离 != 0f)
            {
                if (核弹距离 > 0.8f)
                {
                    transform.position += transform.forward * 初始速度;
                    transform.position -= transform.up * 0.5f;
                }
                if (核弹距离 < 0.8f)
                {
                    if (核爆)
                    {
                        if (艺术就是爆炸 != null) 艺术就是爆炸.SetActive(true);
                        if (H10support.gameWorld.AllAlivePlayersList.Count >= 2)
                        {
                            for (int i = 1; i < H10support.gameWorld.AllAlivePlayersList.Count; i++)//获取全部AI
                            {
                                核弹参数.MakeShot(new Vector3(H10support.gameWorld.AllAlivePlayersList[i].Transform.position.x, H10support.gameWorld.AllAlivePlayersList[i].Transform.position.y + 0.8f, H10support.gameWorld.AllAlivePlayersList[i].Transform.position.z), H10support.gameWorld.AllAlivePlayersList[i].Transform.up, 1f);
                            }
                            核爆 = false;
                        }
                    }
                }
            }
        }
    }
}
