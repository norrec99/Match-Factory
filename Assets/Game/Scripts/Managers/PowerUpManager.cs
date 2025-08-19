using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

public class PowerUpManager : MonoBehaviour
{
    [Header("Vacuum Power-Up Settings")]
    [SerializeField] private Transform vacuumSuckPoint;
    [SerializeField] private Vacuum vacuum;
    [Header("Data")]
    [SerializeField] private int initialPowerupCount;


    private bool isBusy;

    private int vacuumItemsToCollect;
    private int vacuumCounter;
    private int vacuumPowerupCount;

    public static Action<Item> OnVacuumPowerUpUsed;
    public static Action<Item> ItemBackToGameAction;

    private const int MAX_VACUUM_ITEMS = 3;

    private void Awake()
    {
        LoadData();
        ListenEvents();
    }

    private void ListenEvents()
    {
        Vacuum.OnVacuumStarted += VacuumPowerUp;
        InputManager.PowerupClickedAction += OnPowerupClicked;
    }

    private void OnPowerupClicked(Powerup powerup)
    {
        if (isBusy)
        {
            Debug.LogWarning("Power-up is already in use.");
            return;
        }

        switch (powerup.PowerupType)
        {
            case EPowerupType.Vacuum:
                HandleVacuumClicked();
                UpdateVacuumVisuals();
                break;
            default:
                Debug.LogWarning("Unsupported power-up type.");
                break;
        }
    }

    #region Spring Power-Up Logic
    [Button]
    private void SpringPowerUp()
    {
        ItemSpot itemSpot = ItemSpotsManager.Instance.GetRandomOccupiedItemSpot();

        if (itemSpot == null)
        {
            return;
        }

        isBusy = true;

        Item itemToRelease = itemSpot.Item;

        itemSpot.ClearItem();

        itemToRelease.UnassignItemSpot();
        itemToRelease.EnablePhysics();

        itemToRelease.transform.parent = LevelManager.Instance.ItemParent;
        itemToRelease.transform.localPosition = Vector3.up * 3f;
        itemToRelease.transform.localScale = Vector3.one;

        ItemBackToGameAction?.Invoke(itemToRelease);
    }
    #endregion
    #region Vacuum Power-Up Logic
    private void HandleVacuumClicked()
    {
        if (vacuumPowerupCount <= 0)
        {
            // Can add rewarded ad logic here to replenish power-ups
            vacuumPowerupCount = initialPowerupCount;
            SaveData();
        }
        else
        {
            isBusy = true;
            vacuumPowerupCount--;

            SaveData();

            vacuum.Play();
        }
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

        vacuumCounter = 0;

        ItemLevelData goal = (ItemLevelData)greatestGoal;

        List<Item> itemsToVacuum = new List<Item>();

        List<Vector3> pathPoints = new List<Vector3>();

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

            float middlePointY = (itemToVacuum.transform.position.y + vacuumSuckPoint.position.y) / 2 + 2f;

            itemToVacuum.transform.DOMove(vacuumSuckPoint.position, 0.75f).SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    ItemReachedVacuumPoint(itemToVacuum);
                });
            itemToVacuum.transform.DOMoveY(middlePointY, 0.5f);
            itemToVacuum.transform.DOMoveY(vacuumSuckPoint.position.y, 0.25f).SetDelay(0.5f);
            itemToVacuum.transform.DOScale(Vector3.one * 0.2f, 0.5f).SetDelay(0.25f);
        }
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

    private void UpdateVacuumVisuals()
    {
       vacuum.UpdateVisuals(vacuumPowerupCount);
    }
    #endregion

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

    private void LoadData()
    {
        vacuumPowerupCount = PlayerPrefs.GetInt("VacuumPowerupCount", initialPowerupCount);
        UpdateVacuumVisuals();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("VacuumPowerupCount", vacuumPowerupCount);
    }

    private void UnsubscribeEvents()
    {
        Vacuum.OnVacuumStarted -= VacuumPowerUp;
        InputManager.PowerupClickedAction -= OnPowerupClicked;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
