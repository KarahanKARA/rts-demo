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

        public void Preload(GameObject prefab, int amount)
        {
            string key = prefab.name;
            if (!pool.ContainsKey(key))
                pool[key] = new Queue<GameObject>();

            for (int i = 0; i < amount; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pool[key].Enqueue(obj);
            }
        }

        public GameObject Get(string key, GameObject prefab, Transform parent)
        {
            if (pool.ContainsKey(key) && pool[key].Count > 0)
            {
                GameObject obj = pool[key].Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(parent, false);
                return obj;
            }

            GameObject newObj = Instantiate(prefab, parent);
            newObj.name = key;
            return newObj;
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