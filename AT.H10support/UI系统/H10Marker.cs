using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static RootMotion.FinalIK.AimPoser;
using Object = UnityEngine.Object;

namespace AT.H10support
{
    public class H10Marker : MonoBehaviour
    {
        private GameObject 本体;
        public Text 距离;
        public Image 框;
        private int 当前距离;
        private Transform 当前AI坐标;
        private bool 锁定敌人;
        private Vector2 屏幕显示坐标;

        void Start()
        {
            当前AI坐标 = transform;
            if (本体 == null)
            {
                本体 = GameObject.Find("FPS Camera");//找到摄像机
            }
        }
        void Update()
        {
            if (目标在屏幕内(当前AI坐标.position))
            {
                if(!框.GetComponent<Image>().enabled) 框.GetComponent<Image>().enabled = true;
                if (!距离.GetComponent<Text>().enabled) 距离.GetComponent<Text>().enabled = true;
            }
            else
            {
                if (框.GetComponent<Image>().enabled) 框.GetComponent<Image>().enabled = false;
                if (距离.GetComponent<Text>().enabled) 距离.GetComponent<Text>().enabled = false;
            }
            float dis = Vector3.Distance(本体.transform.position, transform.position);
            当前距离 = (int)(dis);
            距离.GetComponent<Text>().text = 当前距离.ToString();
            float minX = 框.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;
            float minY = 框.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;
            Vector2 vector2 = Camera.main.WorldToScreenPoint(当前AI坐标.position);
            屏幕显示坐标 = new Vector2(vector2.x * Screen.width / Camera.main.pixelWidth, vector2.y * Screen.height / Camera.main.pixelHeight);
            屏幕显示坐标 = Camera.main.WorldToScreenPoint(当前AI坐标.position);
            //锁定框1坐标.x = Camera.main.WorldToScreenPoint(输入锁定坐标).x * Screen.width / Camera.main.pixelWidth;
            //锁定框1坐标.y = Camera.main.WorldToScreenPoint(输入锁定坐标).y * Screen.height / Camera.main.pixelHeight;
            if (Vector3.Dot((当前AI坐标.position - 本体.transform.position), 本体.transform.forward) < 0)
            {
                if (框.GetComponent<Image>().enabled) 框.GetComponent<Image>().enabled = false;
                if (屏幕显示坐标.x < Screen.width / 2)
                {
                    屏幕显示坐标.x = maxX;
                }
                else 屏幕显示坐标.x = minX;
            }
            屏幕显示坐标.x = Mathf.Clamp(屏幕显示坐标.x, minX, maxX);
            屏幕显示坐标.y = Mathf.Clamp(屏幕显示坐标.y, minY, maxY);
            框.transform.position = 屏幕显示坐标;
            if(无障碍(当前AI坐标.position)) 框.color = Color.green;
            else 框.color = Color.yellow;
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
            Physics.Raycast(输入坐标, 本体.transform.position - 输入坐标, out hit, Vector3.Distance(本体.transform.position, 输入坐标), 1 << 12 | 1 << 18 | 1 << 11 | 1 << 8);
            return hit.collider == null;
        }
    }
}
