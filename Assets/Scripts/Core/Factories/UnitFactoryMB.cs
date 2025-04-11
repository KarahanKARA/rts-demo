using System.Collections;
using Data.Units;
using Managers;
using UnityEngine;

namespace Core.Factories
{
    public class UnitFactoryMB : MonoBehaviour, IUnitFactory
    {
        public static UnitFactoryMB Instance { get; private set; }

        [SerializeField] private Transform unitParent;
        [SerializeField] private int initialCount = 35;
        [SerializeField] private int extraBatchCount = 20;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => ObjectPoolManager.Instance != null);

            var allUnits = Resources.LoadAll<UnitData>("ScriptableObjects/Units");
            foreach (var unit in allUnits)
            {
                ObjectPoolManager.Instance.Preload(unit.prefab, initialCount);
            }
        }

        public GameObject CreateUnit(UnitData data, Vector3 position)
        {
            string key = data.prefab.name;

            if (!ObjectPoolManager.Instance.HasAvailable(key))
            {
                ObjectPoolManager.Instance.Preload(data.prefab, extraBatchCount);
            }

            GameObject go = ObjectPoolManager.Instance.Get(key, data.prefab, unitParent);
            go.transform.position = position;
            go.transform.rotation = Quaternion.identity;

            return go;
        }
    }
}