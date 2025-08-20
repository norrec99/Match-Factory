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
    [Header("Fan Power-Up Settings")]
    [SerializeField] private Fan fan;
    [SerializeField] private float fanMagnitiude;
    [Header("Spring Power-Up Settings")]
    [SerializeField] private Spring spring;
    [Header("Freeze Power-Up Settings")]
    [SerializeField] private Freeze freeze;
    [SerializeField] private float freezeDuration = 10f;
    [Header("Data")]
    [SerializeField] private int initialPowerupCount;


    private bool isBusy;

    private int vacuumItemsToCollect;
    private int vacuumCounter;
    private int vacuumPowerupCount;
    private int springPowerupCount;
    private int fanPowerupCount;
    private int freezePowerupCount;

    public static Action<Item> OnVacuumPowerUpUsed;
    public static Action<Item> ItemBackToGameAction;

    private const int MAX_VACUUM_ITEMS = 3;

    private const string VACUUM_KEY = "VacuumPowerupCount";
    private const string SPRING_KEY = "SpringPowerupCount";
    private const string FAN_KEY = "FanPowerupCount";
    private const string FREEZE_KEY = "FreezePowerupCount";


    private void Awake()
    {
        LoadVacuumData();
        LoadSpringData();
        LoadFanData();
        LoadFreezeData();
        ListenEvents();
    }

    private void ListenEvents()
    {
        Vacuum.OnVacuumStarted += VacuumPowerUp;
        Vacuum.OnVacuumEnded += OnVacuumEnded;
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
            case EPowerupType.Spring:
                HandleSpringClicked();
                UpdateSpringVisuals();
                break;
            case EPowerupType.Fan:
                HandleFanClicked();
                UpdateFanVisuals();
                break;
            case EPowerupType.FreezeGun:
                HandleFreezeClicked();
                UpdateFreezeVisuals();
                break;
            default:
                Debug.LogWarning("Unsupported power-up type.");
                break;
        }
    }

    #region Freeze Power-Up Logic
    private void HandleFreezeClicked()
    {
        if (freezePowerupCount <= 0)
        {
            // Can add rewarded ad logic here to replenish power-ups
            freezePowerupCount = initialPowerupCount;
            SaveFreezeData();
        }
        else
        {
            freezePowerupCount--;

            SaveFreezeData();

            FreezePowerUp();
        }
    }
    private void UpdateFreezeVisuals()
    {
        freeze.UpdateVisuals(freezePowerupCount);
    }
    [Button]
    private void FreezePowerUp()
    {
        TimerManager.Instance.FreezeTimer(freezeDuration);
    }
    #endregion
    #region Fan Power-Up Logic
    private void HandleFanClicked()
    {
        if (fanPowerupCount <= 0)
        {
            // Can add rewarded ad logic here to replenish power-ups
            fanPowerupCount = initialPowerupCount;
            SaveFanData();
        }
        else
        {
            fanPowerupCount--;

            SaveFanData();

            FanPowerUp();
        }
    }
    private void UpdateFanVisuals()
    {
        fan.UpdateVisuals(fanPowerupCount);
    }
    [Button]
    private void FanPowerUp()
    {
        Item[] items = LevelManager.Instance.Items;

        foreach (Item item in items)
        {
            item.ApplyRandomForce(fanMagnitiude);
        }
    }
    #endregion
    #region Spring Power-Up Logic
    private void HandleSpringClicked()
    {
        if (springPowerupCount <= 0)
        {
            // Can add rewarded ad logic here to replenish power-ups
            springPowerupCount = initialPowerupCount;
            SaveSpringData();
        }
        else
        {
            springPowerupCount--;

            SaveSpringData();

            SpringPowerUp();
        }
    }
    private void UpdateSpringVisuals()
    {
       spring.UpdateVisuals(springPowerupCount);
    }
    [Button]
    private void SpringPowerUp()
    {
        ItemSpot itemSpot = ItemSpotsManager.Instance.GetRandomOccupiedItemSpot();

        if (itemSpot == null)
        {
            return;
        }

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
            SaveVacuumData();
        }
        else
        {
            isBusy = true;
            vacuumPowerupCount--;

            SaveVacuumData();

            vacuum.Play();
        }
    }
    private void OnVacuumEnded()
    {
        isBusy = false;
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
            // itemToVacuum.transform.DOScale(Vector3.one * 0.2f, 0.5f).SetDelay(0.5f);
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

    private void LoadVacuumData()
    {
        vacuumPowerupCount = PlayerPrefs.GetInt(VACUUM_KEY, initialPowerupCount);
        UpdateVacuumVisuals();
    }
    private void SaveVacuumData()
    {
        PlayerPrefs.SetInt(VACUUM_KEY, vacuumPowerupCount);
    }
    private void LoadSpringData()
    {
        springPowerupCount = PlayerPrefs.GetInt(SPRING_KEY, initialPowerupCount);
        UpdateSpringVisuals();
    }
    private void SaveSpringData()
    {
        PlayerPrefs.SetInt(SPRING_KEY, springPowerupCount);
    }
    private void LoadFanData()
    {
        fanPowerupCount = PlayerPrefs.GetInt(FAN_KEY, initialPowerupCount);
        UpdateFanVisuals();
    }
    private void SaveFanData()
    {
        PlayerPrefs.SetInt(FAN_KEY, fanPowerupCount);
    }
    private void LoadFreezeData()
    {
        freezePowerupCount = PlayerPrefs.GetInt(FREEZE_KEY, initialPowerupCount);
        UpdateFreezeVisuals();
    }
    private void SaveFreezeData()
    {
        PlayerPrefs.SetInt(FREEZE_KEY, freezePowerupCount);
    }

    private void UnsubscribeEvents()
    {
        Vacuum.OnVacuumStarted -= VacuumPowerUp;
        Vacuum.OnVacuumEnded -= OnVacuumEnded;
        InputManager.PowerupClickedAction -= OnPowerupClicked;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
