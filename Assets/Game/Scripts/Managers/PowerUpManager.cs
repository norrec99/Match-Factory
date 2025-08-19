using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static Action<Item> OnVacuumPowerUpUsed;

    private const int MAX_VACUUM_ITEMS = 3;

    [Button]
    private void VacuumPowerUp()
    {
        Item[] items = LevelManager.Instance.Items;
        ItemLevelData[] goals = GoalManager.Instance.Goals;

        ItemLevelData? greatestGoal = GetGreatestGoal(goals);
        if (greatestGoal == null)
        {
            Debug.LogWarning("No goals available to vacuum.");
            return;
        }

        ItemLevelData goal = (ItemLevelData)greatestGoal;

        List<Item> itemsToVacuum = new List<Item>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].ItemType == goal.itemPrefab.ItemType)
            {
                itemsToVacuum.Add(items[i]);

                if (itemsToVacuum.Count >= MAX_VACUUM_ITEMS)
                {
                    break;
                }
            }
        }
        
        for (int i = itemsToVacuum.Count - 1; i >= 0; i--)
        {
            OnVacuumPowerUpUsed?.Invoke(itemsToVacuum[i]);
            Destroy(itemsToVacuum[i].gameObject); 
        }
    }

    private ItemLevelData? GetGreatestGoal(ItemLevelData[] goals)
    {
        int maxAmount = 0;
        int greatestGoalIndex = -1;
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].amount > maxAmount)
            {
                maxAmount = goals[i].amount;
                greatestGoalIndex = i;
            }
        }

        if (greatestGoalIndex <= -1)
        {
            return null;
        }
        
        return goals[greatestGoalIndex];
    }
}
