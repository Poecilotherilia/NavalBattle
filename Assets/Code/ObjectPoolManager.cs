using System;
using System.Collections.Generic;
using UnityEngine;
namespace game
{
    public enum PoolType
    {
        Chess
    }

    public class ObjectPoolManager : MonoBehaviour
    {
        private static ObjectPoolManager _instance;
        public static ObjectPoolManager Instance {
            get {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(ObjectPoolManager)) as ObjectPoolManager;
                }
                return _instance;
            }
        }

        private Dictionary<PoolType, ObjectPool> m_PoolDic;

        private Transform m_RootPoolTrans;

        void Awake()
        {
            m_PoolDic = new Dictionary<PoolType, ObjectPool>();

            // 根对象池
            GameObject go = new GameObject(typeof(ObjectPoolManager).ToString());
            m_RootPoolTrans = go.transform;
        }

        // 创建一个新的对象池
        public T CreateObjectPool<T>(PoolType poolName ,Transform parent = null) where T : ObjectPool, new()
        {
            if (m_PoolDic.ContainsKey(poolName))
            {
                return m_PoolDic[poolName] as T;
            }

            GameObject obj = new GameObject(poolName.ToString());
            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }
            else {
                obj.transform.SetParent(m_RootPoolTrans);
            }
            T pool = new T();
            pool.Init(poolName, obj.transform);
            m_PoolDic.Add(poolName, pool);
            return pool;
        }

        public GameObject GetGameObject(PoolType poolName, Vector3 position)
        {
            if (m_PoolDic.ContainsKey(poolName))
            {
                return m_PoolDic[poolName].Get(position);
            }
            return null;
        }

        public void RemoveGameObject(PoolType poolName, GameObject go)
        {
            if (m_PoolDic.ContainsKey(poolName))
            {
                m_PoolDic[poolName].Remove(go);
            }
        }

        // 销毁所有对象池
        public void Destroy()
        {
            m_PoolDic.Clear();
            GameObject.Destroy(m_RootPoolTrans);

        }
    }
}