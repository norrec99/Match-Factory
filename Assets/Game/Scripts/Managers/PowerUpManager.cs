using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

public class PowerUpManager : MonoBehaviour
{
    [Header("Vacuum Power-Up Settings")]
    [SerializeField] private Transform vacuumSuckPoint;


    private bool isBusy;
    private int vacuumItemsToCollect;
    private int vacuumCounter;

    public static Action<Item> OnVacuumPowerUpUsed;

    private const int MAX_VACUUM_ITEMS = 3;

    private void Awake()
    {
        ListenEvents();
    }

    private void ListenEvents()
    {
        Vacuum.OnVacuumStarted += VacuumPowerUp;
    }

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

        if (isBusy)
        {
            Debug.LogWarning("Power-up is already in use.");
            return;
        }
        isBusy = true;
        vacuumCounter = 0;


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

        vacuumItemsToCollect = itemsToVacuum.Count;

        for (int i = itemsToVacuum.Count - 1; i >= 0; i--)
        {
            itemsToVacuum[i].DisablePhysics();

            Item itemToVacuum = itemsToVacuum[i];

            itemToVacuum.transform.DOMove(vacuumSuckPoint.position, 0.5f).SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    ItemReachedVacuumPoint(itemToVacuum);
                });
            itemToVacuum.transform.DOScale(Vector3.one * 0.5f, 0.2f).SetDelay(0.1f);
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

    private void ItemReachedVacuumPoint(Item item)
    {
        vacuumCounter++;

        if (vacuumCounter >= vacuumItemsToCollect)
        {
           isBusy = false;
        }

        OnVacuumPowerUpUsed?.Invoke(item);

        Destroy(item.gameObject);
    }
    
    private void UnsubscribeEvents()
    {
        Vacuum.OnVacuumStarted -= VacuumPowerUp;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
