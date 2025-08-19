using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private ItemPlacer itemPlacer;
    [SerializeField] private float levelDuration;

    public float LevelDuration => levelDuration;

    public Transform ItemParent => itemPlacer.transform;


    public ItemLevelData[] GetGoals()
    {
        return itemPlacer.GetGoals();
    }

    public Item[] GetItems()
    {
        return itemPlacer.GetItems();
    }
}
