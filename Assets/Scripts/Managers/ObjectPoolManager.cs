using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        private Dictionary<string, Queue<GameObject>> pool = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public void Preload(GameObject prefab, int amount, Transform parent)
        {
            string key = prefab.name;
            if (!pool.ContainsKey(key))
                pool[key] = new Queue<GameObject>();

            for (int i = 0; i < amount; i++)
            {
                GameObject obj = Instantiate(prefab, parent);
                obj.SetActive(false);
                pool[key].Enqueue(obj);
            }
        }

        public GameObject Get(string key, GameObject prefab, Transform parent)
        {
            GameObject obj;

            if (pool.ContainsKey(key) && pool[key].Count > 0)
            {
                obj = pool[key].Dequeue();
                obj.transform.SetParent(parent, false);
                obj.transform.localPosition = Vector3.zero; 
                obj.SetActive(true);
            }
            else
            {
                obj = Instantiate(prefab, parent);
                obj.name = key;
            }

            return obj;
        }


        public bool HasAvailable(string key)
        {
            return pool.ContainsKey(key) && pool[key].Count > 0;
        }

        public void Return(string key, GameObject obj)
        {
            obj.SetActive(false);
            if (!pool.ContainsKey(key))
                pool[key] = new Queue<GameObject>();

            pool[key].Enqueue(obj);
        }
    }
}