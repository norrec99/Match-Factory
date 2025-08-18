using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private ItemPlacer itemPlacer;

    public ItemLevelData[] GetGoals()
    {
        return itemPlacer.GetGoals();
    }
}
