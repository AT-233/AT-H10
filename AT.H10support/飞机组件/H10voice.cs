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
    public class H10voice : MonoBehaviour
    {
        public GameObject 音效组件;
        public AudioClip[] 音效;
        private float 提示音间隔;
        private float 提示音间隔2;
        private bool 是否提示;
        private bool 是否提示2;
        public static bool 弹射 = false;
        void Start()
        {
            提示音间隔 = 0;
            提示音间隔2 = 0;
            是否提示 = true;
            是否提示2 = true;
            弹射 = false;
        }
        void FixedUpdate()
        {
            if (H10supportBepIn.下挂配置.Value == "弹射舱(Ejection seat)")
            {
                if (Input.GetKeyDown(H10supportBepIn.弹射.Value) && 是否提示2)
                {
                    音效组件.GetComponent<AudioSource>().clip = 音效[1];
                    音效组件.GetComponent<AudioSource>().Play();
                    是否提示2 = false;
                }
                if (!是否提示2)
                {
                    提示音间隔2 += Time.deltaTime;
                    if (提示音间隔2 > 1)
                    {
                        弹射 = true;
                    }
                }
            }
            if (H10Weapon.机头下方距离 > 0 && H10Weapon.机头下方距离 < 0.8f && 是否提示 && 是否提示2)
            {
                音效组件.GetComponent<AudioSource>().clip = 音效[0];
                音效组件.GetComponent<AudioSource>().Play();
                是否提示 = false;
            }
            if (!是否提示)
            {
                提示音间隔 += Time.deltaTime;
                if (提示音间隔 > 0.8f)
                {
                    是否提示 = true;
                    提示音间隔 = 0;
                }
            }
        }
    }
}
