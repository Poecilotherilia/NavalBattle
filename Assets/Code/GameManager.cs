using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
	public class GameManager : MonoBehaviour
	{
        private static GameManager _instance;
        public static GameManager Instance {
            get {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
                }
                return _instance;
            }
        }
        public static int MAX_BROAD_SIZE = 9;
        private List<Model> m_ModelList
            = new List<Model>();

        public checherBoardViewPreference checherBoardViewPreference;
        private ChessFlashItemView chessFlashItemView = null;
        public ChessFlashItemView ChessFlashItemView {
            get {
                if (chessFlashItemView == null) {
                    GameObject chessPreb =
                        Resources.Load("Prefab/chessFlash") as GameObject; ;
                    GameObject currentObject =
                        Instantiate(chessPreb) as GameObject;
                    currentObject.SetActive(false);
                    chessFlashItemView = currentObject.GetComponent<ChessFlashItemView>();
                }
                return chessFlashItemView;
            }
        }

        void Awake()
        {
            //this.gameObject.AddComponent<VPCollectorManage>();
            this.gameObject.AddComponent<UpdateManager>();
            this.gameObject.AddComponent<NetManager>();
            this.gameObject.AddComponent<ObjectPoolManager>();
            this.gameObject.AddComponent<UIManager>();
            this.Prepare();
        }

        public static T GetModel<T>()
        where T : Model ,new()
        {
            foreach (var m in GameManager.Instance.m_ModelList)
            {
                if (m is T)
                {
                    return (T)m;
                }
            }
            T model = new T();
            GameManager.Instance.m_ModelList.Add(model);
            return model;
        }

        public void Prepare() {
            GameObject prepare = new GameObject(typeof(PrepareViewModel).ToString());
            prepare.transform.parent = this.gameObject.transform;
            prepare.AddComponent<PrepareViewModel>();
        }

        public void StartGame(bool isPlayer1) {
            GameObject gameRoom = new GameObject(typeof(GameRoomViewModel).ToString());
            gameRoom.transform.parent = this.gameObject.transform;
            gameRoom.AddComponent<GameRoomViewModel>();
            gameRoom.GetComponent<GameRoomViewModel>().StartGame(isPlayer1);
        }
    }
}
