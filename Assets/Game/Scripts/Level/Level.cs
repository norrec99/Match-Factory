using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private ItemPlacer itemPlacer;
    [SerializeField] private float levelDuration;

    public float LevelDuration => levelDuration;


    public ItemLevelData[] GetGoals()
    {
        return itemPlacer.GetGoals();
    }
}
