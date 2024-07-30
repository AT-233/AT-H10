using Comfort.Common;
using EFT;
using UnityEngine;
using HarmonyLib;
using EFT.CameraControl;


namespace AT.H10support
{
    public class H10support : MonoBehaviour
    {
        public static float 飞行速度;
        public GameObject 轰10模型;
        public Transform 摄像机位置;
        public static float 上下角度;
        public static float 左右翻转角度;
        public static float 左右平移参数;
        private float 垂直机动性参数;
        private float 翻转机动性参数;
        private static GameObject 玩家游戏对象;
        private static GameObject 主摄像机;
        public static GameWorld gameWorld;
        public static bool 牢大;

        void Start()
        {
            //A前，W右
            牢大 = false;
            上下角度 = 0f;
            左右翻转角度 = 0f;
            飞行速度 = 0.03f;
            垂直机动性参数 = H10supportBepIn.垂直机动性.Value;
            翻转机动性参数 = H10supportBepIn.翻转机动性.Value;
            左右平移参数 = 0f;
            if (gameWorld == null)
            {
                gameWorld = Singleton<GameWorld>.Instance;
            }
            主摄像机 = GameObject.Find("FPS Camera");
            玩家游戏对象 = gameWorld.MainPlayer.gameObject;
            玩家游戏对象.GetComponent<PlayerOwner>().enabled = false;
            //玩家游戏对象.GetComponent<SimpleCharacterController>().enabled = false;
            gameWorld.MainPlayer.PointOfView = EPointOfView.ThirdPerson;
            var playerBody = Traverse.Create(gameWorld.MainPlayer).Field<PlayerBody>("_playerBody").Value;
            playerBody.PointOfView.Value = EPointOfView.FreeCamera;
            gameWorld.MainPlayer.GetComponent<PlayerCameraController>().UpdatePointOfView();
            //玩家游戏对象.transform.position = 摄像机位置.transform.position;
            //玩家游戏对象.transform.rotation = 摄像机位置.transform.rotation;
            //玩家游戏对象.transform.parent = transform;
            主摄像机.transform.position = 摄像机位置.transform.position;
            主摄像机.transform.rotation = 摄像机位置.transform.rotation;
            主摄像机.transform.parent = 摄像机位置.transform;
            //玩家游戏对象.layer = LayerMask.NameToLayer("Grass");
        }
        void FixedUpdate()
        {
            控制参数();
            //gameWorld.MainPlayer.Transform.position = 摄像机位置.transform.position;
            //gameWorld.MainPlayer.Transform.rotation = 摄像机位置.transform.rotation;           
            if (牢大)
            {
                主摄像机.transform.parent = null;
                gameWorld.MainPlayer.PointOfView = EPointOfView.FirstPerson;
                玩家游戏对象.GetComponent<PlayerOwner>().enabled = true;
                玩家游戏对象.GetComponent<SimpleCharacterController>().enabled = true;
                玩家游戏对象.layer = LayerMask.NameToLayer("Player");
                Destroy(this.gameObject);
            }
            if(H10voice.弹射)
            {
                主摄像机.transform.parent = null;
                gameWorld.MainPlayer.PointOfView = EPointOfView.FirstPerson;
                玩家游戏对象.GetComponent<PlayerOwner>().enabled = true;
                玩家游戏对象.GetComponent<SimpleCharacterController>().enabled = true;
                玩家游戏对象.layer = LayerMask.NameToLayer("Player");
                var position = new Vector3(transform.position.x, transform.position.y+0.3f, transform.position.z);
                玩家游戏对象.gameObject.transform.SetPositionAndRotation(position, Quaternion.Euler(0, transform.rotation.y, 0));
                Destroy(this.gameObject);
            }
        }
        // Start is called before the first frame update
        private void 控制参数()
        {
            float fMouseX = Input.GetAxis("Mouse X");
            float fMouseY = Input.GetAxis("Mouse Y");
            if (Input.GetKey(KeyCode.Space)) 上下角度 += 0.15f;
            if(H10supportBepIn.操作加速度.Value)
            {
                if (fMouseY > 0) 上下角度 = 上下角度 + fMouseY * 垂直机动性参数;
                if (fMouseY < 0) 上下角度 = 上下角度 + fMouseY * 垂直机动性参数;
            }
            else
            {
                if (fMouseY > 0) 上下角度 = fMouseY * 垂直机动性参数 * 10;
                if (fMouseY < 0) 上下角度 = fMouseY * 垂直机动性参数 * 10;
            }
            if (fMouseY == 0 && 上下角度 > 0 && !Input.GetKey(KeyCode.Space))
            {
                上下角度 = 上下角度 - 0.1F;
                if (上下角度 < 0) 上下角度 = 0;
            }
            if (fMouseY == 0 && 上下角度 < 0 && !Input.GetKey(KeyCode.Space))
            {
                上下角度 = 上下角度 + 0.1F;
                if (上下角度 > 0) 上下角度 = 0;
            }
            if (上下角度 < -1.2F) 上下角度 = -1.2F;
            if (上下角度 > 1.2F) 上下角度 = 1.2F;
            if (H10supportBepIn.操作加速度.Value)
            {
                if (fMouseX > 0) 左右翻转角度 = 左右翻转角度 + fMouseX * 翻转机动性参数;
                if (fMouseX < 0) 左右翻转角度 = 左右翻转角度 + fMouseX * 翻转机动性参数;
            }
            else
            {
                if (fMouseX > 0) 左右翻转角度 = fMouseX * 翻转机动性参数 * 10;
                if (fMouseX < 0) 左右翻转角度 = fMouseX * 翻转机动性参数 * 10;
            }
            if (fMouseX == 0 && 左右翻转角度 > 0)
            {
                左右翻转角度 = 左右翻转角度 - 0.1F;
                if (左右翻转角度 < 0) 左右翻转角度 = 0;
            }
            if (fMouseX == 0 && 左右翻转角度 < 0)
            {
                左右翻转角度 = 左右翻转角度 + 0.1F;
                if (左右翻转角度 > 0) 左右翻转角度 = 0;
            }
            if (左右翻转角度 > 1.8F) 左右翻转角度 = 1.8F;
            if (左右翻转角度 < -1.8F) 左右翻转角度 = -1.8F;

            if (左右平移参数 < 0 && !Input.GetKey(KeyCode.A))
            {
                左右平移参数 += 0.02f;
                if (左右平移参数 > 0) 左右平移参数 = 0;
            }
            if (左右平移参数 > 0 && !Input.GetKey(KeyCode.D))
            {
                左右平移参数 -= 0.02f;
                if (左右平移参数 < 0) 左右平移参数 = 0;
            }
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && 飞行速度 < 0.241F)
            {
                飞行速度 += 0.0004f;
                if (飞行速度 > 0.241F) 飞行速度 = 0.241F;
            }
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && 飞行速度 > 0.241F)
            {
                飞行速度 -= 0.0006f;
                if (飞行速度 < 0.241F) 飞行速度 = 0.241F;
            }
            if (Input.GetKey(KeyCode.W))
            {
                飞行速度 = 飞行速度 + 0.0006f;
                if (飞行速度 > 0.505F) 飞行速度 = 0.505F;
            }
            if (Input.GetKey(KeyCode.S))
            {
                飞行速度 = 飞行速度 - 0.0009f;
                if (飞行速度 < 0.03f) 飞行速度 = 0.03f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                左右平移参数 = 左右平移参数 + 0.01f;
                if (左右平移参数 > 0.25F) 左右平移参数 = 0.25F;
            }
            if (Input.GetKey(KeyCode.A))
            {
                左右平移参数 = 左右平移参数 - 0.01f;
                if (左右平移参数 < -0.25F) 左右平移参数 = -0.25F;
            }
            模型联控参数();
            transform.position += transform.forward * 飞行速度;
            transform.Rotate(Vector3.forward, -左右翻转角度, Space.Self);
            transform.Rotate(Vector3.right, -上下角度, Space.Self);
            transform.Rotate(Vector3.up, 左右平移参数, Space.Self);
        }
        private void 模型联控参数()
        {
            轰10模型.transform.localPosition = new Vector3(0, -0.006f, 0.12f + (飞行速度 / 0.3F) * 0.05f);
            轰10模型.transform.localEulerAngles = new Vector3((上下角度 / 1.2f) * -2F, (左右平移参数 / 0.25F) * 2f, (左右翻转角度 / 1.8f) * -4F);
        }
    }
}
