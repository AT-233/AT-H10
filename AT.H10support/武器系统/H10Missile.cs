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
    public class H10Missile : MonoBehaviour
    {
        public GameObject 烟雾组件;
        public static Transform 锁定目标点;
        private float 导弹速度 = 0f;
        private float 摧毁时间 = 0f;
        private float 导弹距离 = 0f;
        Shoot 火箭弹参数 = new Shoot(Singleton<GameWorld>.Instance.MainPlayer, "H10missile", "0001aaa10aa1145141919810h10yj114"); //缓存射击预设
        Shoot 激光制导导弹参数 = new Shoot(Singleton<GameWorld>.Instance.MainPlayer, "H10missile1", "0001aaa10aa1145141919810h10yj114");
        Shoot 集束火箭弹参数 = new Shoot(Singleton<GameWorld>.Instance.MainPlayer, "H10missile3", "0001aa40aaa1145141919810h10gunClusterBomb");
        void Start()
        {
            if (H10support.飞行速度 != 0 && H10Weapon.导弹类型 == 0)
            {
                导弹速度 = H10support.飞行速度 + 0.1f;
                摧毁时间 = 5;
                导弹距离 = H10Weapon.机头距离;
                烟雾组件.SetActive(false);
            }
            if (H10support.飞行速度 != 0 && H10Weapon.导弹类型 == 1)
            {
                导弹速度 = H10support.飞行速度 + 0.02f;
                摧毁时间 = 15;
                导弹距离 = H10Weapon.机头距离;
                烟雾组件.SetActive(true);
            }
            if (H10support.飞行速度 != 0 && H10Weapon.导弹类型 == 2)
            {
                导弹速度 = H10support.飞行速度 + 0.15f;
                摧毁时间 = 15;
                导弹距离 = H10Weapon.机头距离;
                烟雾组件.SetActive(true);
            }
            if (H10support.飞行速度 != 0 && H10Weapon.导弹类型 == 3)
            {
                导弹速度 = H10support.飞行速度 + 0.02f;
                摧毁时间 = 10;
                导弹距离 = H10Weapon.机头距离;
                烟雾组件.SetActive(true);
            }
        }
        void FixedUpdate()
        {
            if (H10Weapon.导弹类型 == 0)
            {
                摧毁时间 -= Time.deltaTime;
                导弹速度 += Time.deltaTime * 0.5F;
                导弹距离 -= 导弹速度;
                transform.position += transform.forward * 导弹速度;
                if (摧毁时间 < 0) Destroy(this.gameObject);
                if (导弹距离 > 0f && 导弹距离 < 0.8f)
                {
                    火箭弹参数.MakeShot(transform.position, transform.forward, 1f);
                    Destroy(this.gameObject);
                }
            }
            if (H10Weapon.导弹类型 == 1)
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit 目标点, Mathf.Infinity, 1 << 12 | 1 << 18 | 1 << 11 | 1 << 8);
                Physics.Raycast(transform.position, transform.forward, out RaycastHit 导弹头, Mathf.Infinity, 1 << 12 | 1 << 18 | 1 << 11 | 1 << 8);
                Vector3 mousePositionInWorld = 目标点.point;
                if (目标点.distance != 0) transform.LookAt(mousePositionInWorld);
                if (导弹头.distance != 0) 导弹距离 = 导弹头.distance;
                else 导弹距离 -= 导弹速度;
                if (导弹距离 < 0.8f && 导弹距离 > 0)
                {
                    激光制导导弹参数.MakeShot(transform.position, transform.forward, 1f);
                    Destroy(this.gameObject);
                }
                导弹速度 += Time.deltaTime * 0.1F;
                transform.position += transform.forward * 导弹速度;
            }
            if (H10Weapon.导弹类型 == 2)
            {
                Physics.Raycast(transform.position, transform.forward, out RaycastHit 导弹头, Mathf.Infinity, 1 << 12 | 1 << 18 | 1 << 11 | 1 << 8);
                if (H10Weapon.导弹准备发射) transform.LookAt(锁定目标点);
                if (导弹头.distance != 0) 导弹距离 = 导弹头.distance;
                else 导弹距离 -= 导弹速度;
                if (导弹距离 < 0.8f && 导弹距离 > 0)
                {
                    激光制导导弹参数.MakeShot(transform.position, transform.forward, 1f);
                    Destroy(this.gameObject);
                }
                导弹速度 += Time.deltaTime * 0.1F;
                transform.position += transform.forward * 导弹速度;
            }
            if (H10Weapon.导弹类型 == 3)
            {
                摧毁时间 -= Time.deltaTime;
                导弹速度 += Time.deltaTime * 0.3F;
                导弹距离 -= 导弹速度;
                transform.position += transform.forward * 导弹速度;
                if (摧毁时间 < 0) Destroy(this.gameObject);
                if (导弹距离 > 0f && 导弹距离 < 0.8f)
                {
                    集束火箭弹参数.MakeShot(transform.position, transform.forward, 1f);
                    Destroy(this.gameObject);
                }
            }
        }
        public void 锁定坐标(GameObject 锁定目标)
        {
            锁定目标点 = 锁定目标.transform;
        }
    }
}
