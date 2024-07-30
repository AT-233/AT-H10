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
    public class H10animation : MonoBehaviour
    {
        public GameObject 左方向舵;
        public GameObject 右方向舵;
        public GameObject 左襟翼;
        public GameObject 右襟翼;
        public GameObject 左副翼;
        public GameObject 右副翼;
        public GameObject 左缘缝翼;
        public GameObject 右缘缝翼;
        public GameObject 左升降舵;
        public GameObject 右升降舵;
        public GameObject 左发动机喷口;
        public GameObject 右发动机喷口;
        public GameObject 减速板;
        private float 减速板归位;
        void Start()
        {
            减速板归位 = 0;
        }
        void FixedUpdate()
        {
            左方向舵.transform.localEulerAngles = new Vector3(0, (H10support.左右平移参数 / 0.25F) * 15, 0);
            右方向舵.transform.localEulerAngles = new Vector3(0, (H10support.左右平移参数 / 0.25F) * 15, 0);
            左襟翼.transform.localEulerAngles = new Vector3((H10support.左右翻转角度 / 1.8f) * 15, 0, 0);
            左副翼.transform.localEulerAngles = new Vector3((H10support.左右翻转角度 / 1.8f) * 15, 0, 0);
            右襟翼.transform.localEulerAngles = new Vector3((H10support.左右翻转角度 / 1.8f) * -15, 0, 0);
            右副翼.transform.localEulerAngles = new Vector3((H10support.左右翻转角度 / 1.8f) * -15, 0, 0);
            左升降舵.transform.localEulerAngles = new Vector3(((H10support.上下角度 / 1.8f) + (H10support.左右翻转角度 / 1.8f)) * 15, 0, 0);
            右升降舵.transform.localEulerAngles = new Vector3(((H10support.上下角度 / 1.8f) - (H10support.左右翻转角度 / 1.8f)) * 15, 0, 0);

            if (!Input.GetKey(KeyCode.S) && 减速板归位 > 0)
            {
                减速板归位 -= 1F;
                if (减速板归位 < 0F) 减速板归位 = 0F;
            }
            if (Input.GetKey(KeyCode.S) && 减速板归位 < 60)
            {
                减速板归位 += 0.5F;
                if (减速板归位 > 60F) 减速板归位 = 60F;
            }
            减速板.transform.localEulerAngles = new Vector3(减速板归位, 0, 0);
            减速板.GetComponent<AudioSource>().pitch = (0.2F + ((H10support.飞行速度 - 0.03F) / 0.475F) * 1.8F);
            左发动机喷口.transform.localScale = new Vector3(1, 1, 0.4f + ((H10support.飞行速度 - 0.03F) / 0.475F) * 1.6F);
            右发动机喷口.transform.localScale = new Vector3(1, 1, 0.4f + ((H10support.飞行速度 - 0.03F) / 0.475F) * 1.6F);
        }
    }
}
