using System.Collections.Generic;
using UnityEngine;
namespace game
{
    public class ObjectPool
    {
        private Queue<GameObject> m_PoolQueue;


        private PoolType m_PoolName;

        protected Transform m_Parent;

        // 需要缓存的对象
        private GameObject prefab;

        // 最大容量
        private int m_MaxCount;

        protected const int m_DefaultMaxCount = 100;

        public GameObject Prefab {
            get {
                return prefab;
            }
            set {
                prefab = value;
            }
        }

        public ObjectPool()
        {
            m_MaxCount = m_DefaultMaxCount;
            m_PoolQueue = new Queue<GameObject>();
        }

        public virtual void Init(PoolType poolName, Transform transform)
        {
            m_PoolName = poolName;
            m_Parent = transform;
        }

        public virtual GameObject Get(Vector3 pos)
        {
            GameObject returnObj;
            if (m_PoolQueue.Count > 0)
            {
                returnObj = m_PoolQueue.Dequeue();
            }
            else
            {
                // 池中没有可分配对象了，新生成一个
                returnObj = GameObject.Instantiate<GameObject>(prefab);
                returnObj.transform.SetParent(m_Parent);
                returnObj.SetActive(false);
            }

            returnObj.transform.position = pos;
            returnObj.SetActive(true);

            return returnObj;
        }

        // "销毁对象" 其实是回收对象
        public virtual void Remove(GameObject obj)
        {
            if (m_PoolQueue.Contains(obj))
            {
                return;
            }

            if (m_PoolQueue.Count > m_MaxCount)
            {
                // 对象池已满 直接销毁
                GameObject.Destroy(obj);
            }
            else
            {
                // 放入对象池
                m_PoolQueue.Enqueue(obj);
                obj.SetActive(false);
            }
        }

        public virtual void Destroy()
        {
            m_PoolQueue.Clear();
        }


    }
}