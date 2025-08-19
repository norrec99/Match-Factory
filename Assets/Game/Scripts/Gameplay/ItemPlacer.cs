using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemPlacer : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ItemLevelData[] itemDatas;
    [Header("Spawn Settings")]
    [SerializeField] private BoxCollider spawnZone;
    [SerializeField] private int seed;

    private Item[] items;

    public ItemLevelData[] GetGoals()
    {
        List<ItemLevelData> goals = new List<ItemLevelData>();
        foreach (var itemData in itemDatas)
        {
            if (itemData.isGoal)
            {
                goals.Add(itemData);
            }
        }

        return goals.ToArray();
    }

    public Item[] GetItems()
    {
        if (items == null)
        {
            items = GetComponentsInChildren<Item>();
        }

        return items;
    }


#if UNITY_EDITOR
    [Button]
    private void GenerateItems()
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.SetParent(null);
            DestroyImmediate(child.gameObject);
        }

        Random.InitState(seed);

        for (int i = 0; i < itemDatas.Length; i++)
        {
            ItemLevelData itemData = itemDatas[i];
            for (int j = 0; j < itemData.amount; j++)
            {
                Vector3 spawnPosition = GetSpawnPosition();
                Item item = PrefabUtility.InstantiatePrefab(itemData.itemPrefab, transform) as Item;

                item.transform.position = spawnPosition;
                item.transform.rotation = Quaternion.Euler(Random.onUnitSphere * 360f);
            }
        }
    }

    private Vector3 GetSpawnPosition()
    {
        float x = Random.Range(-spawnZone.size.x / 2, spawnZone.size.x / 2);
        float y = Random.Range(-spawnZone.size.y / 2, spawnZone.size.y / 2);
        float z = Random.Range(-spawnZone.size.z / 2, spawnZone.size.z / 2);

        Vector3 localPosition = spawnZone.center + new Vector3(x, y, z);
        Vector3 spawnPosition = spawnZone.transform.TransformPoint(localPosition);

        return spawnPosition;
    }
#endif
}
