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
    public class H10RC : MonoBehaviour
    {
        private static GameWorld gameWorld;
        private GameObject H10;
        void Update()
        {
            if (gameWorld == null)
            {
                gameWorld = Singleton<GameWorld>.Instance;
                Console.WriteLine("世界找到");
                return;
            }
            if (Input.GetKeyDown(H10supportBepIn.召唤.Value) && H10 == null)
            {
                var Prefab = Instantiate(H10supportBepIn.H10Prefab, new Vector3(gameWorld.MainPlayer.Transform.position.x, gameWorld.MainPlayer.Transform.position.y + 1.8f, gameWorld.MainPlayer.Transform.position.z), gameWorld.MainPlayer.Transform.rotation);
                H10 = Prefab as GameObject;
            }
        }
    }
}
