using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace game
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance {
            get {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(UIManager)) as UIManager;
                }
                return _instance;
            }
        }
        private Queue<GameObject> m_UIQueue = new  Queue<GameObject>();
        private const int MAX_UI_COUNT = 3;
        private Transform m_UIparent;
        void Awake()
        {
            m_UIparent = GameObject.Find("UICanvas").transform;
        }

        public GameObject GetUI(string UIName)
        {
            GameObject ui;
            bool isAlreadyIn = false;
            foreach (GameObject item in m_UIQueue) {
                if (item.name.Equals(UIName)) {
                    isAlreadyIn = true;
                    ui = item;
                    ui.SetActive(true);
                    return ui;
                }
            }
            if (!isAlreadyIn)
            {
                if (m_UIQueue.Count >= MAX_UI_COUNT)
                {
                    ui = m_UIQueue.Dequeue();
                    GameObject.Destroy(ui);
                }
                GameObject prefab =
                        Resources.Load("UI/" + UIName) as GameObject;
                ui =
                    Instantiate(prefab, this.m_UIparent) as GameObject;
                ui.name = UIName;
                m_UIQueue.Enqueue(ui);
                return ui;
            }
            return new GameObject();
        }

        public bool RemoveUI(string UIName) {
            foreach (GameObject item in m_UIQueue)
            {
                if (item.name.Equals(UIName))
                {
                    item.SetActive(false);
                    return true;
                }
            }
            return false;
        }
    }
}
