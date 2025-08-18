using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private ItemLevelData[] goals;
    [SerializeField] private GoalCard goalCardPrefab;
    [SerializeField] private Transform goalCardParent;

    private List<GoalCard> goalCards = new List<GoalCard>();

    void Awake()
    {
        ListenEvents();
    }

    private void ListenEvents()
    {
        LevelManager.OnLevelSpawned += OnLevelSpawned;
        ItemSpotsManager.ItemPickedUpAction += OnItemPickedUp;
    }

    private void OnLevelSpawned(Level level)
    {
        goals = level.GetGoals();
        GenerateGoalCards();
    }

    private void GenerateGoalCards()
    {
        foreach (var goal in goals)
        {
            GenerateGoalCard(goal);
        }
    }

    private void GenerateGoalCard(ItemLevelData goal)
    {
        GoalCard goalCard = Instantiate(goalCardPrefab, goalCardParent);
        goalCard.SetGoalCard(goal.amount);
        goalCards.Add(goalCard);
    }

    private void OnItemPickedUp(Item item)
    {
        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].itemPrefab.ItemType != item.ItemType)
            {
                continue; // Skip if item type does not match
            }

            goals[i].amount--;
            if (goals[i].amount <= 0)
            {
                CompleteGoal(i);
            }
            else
            {
                goalCards[i].UpdateGoalCard(goals[i].amount);
            }
            break;
        }
    }

    private void CompleteGoal(int index)
    {
        Debug.Log($"Goal achieved for item type: {goals[index].itemPrefab.ItemType}");
        goalCards[index].CompleteGoalCard();
        CheckForLevelCompletion();
    }

    private void CheckForLevelCompletion()
    {
        foreach (var goal in goals)
        {
            if (goal.amount > 0)
            {
                return; // Not all goals are completed
            }
        }

        Debug.Log("All goals completed! Level can be considered finished.");
        // Trigger level completion logic here, e.g., load next level or show completion UI
    }

    private void UnsubscribeEvents()
    {
        LevelManager.OnLevelSpawned -= OnLevelSpawned;
        ItemSpotsManager.ItemPickedUpAction -= OnItemPickedUp;
    }

    void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
