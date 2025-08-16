using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [SerializeField] private float goUpDistance;
    [SerializeField] private float goUpDuration;
    [SerializeField] private float smashDuration;

    [SerializeField] private ParticleSystem mergeParticles; // Particle system to play when items are merged

    private Sequence sequence;

    private void Awake()
    {
        ListenEvents();

    }

    private void ListenEvents()
    {
        ItemSpotsManager.ItemsMergeStartedAction += OnItemsMergeStarted;
    }

    private void OnItemsMergeStarted(List<Item> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 targetPosition = items[i].transform.position + items[i].transform.up * goUpDistance;

            Action callback = null;

            if (i == 0)
            {
                callback = () => SmashItems(items);
            }

            items[i].transform.DOMove(targetPosition, goUpDuration)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }
    }

    private void SmashItems(List<Item> items)
    {
        items.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x)); // Sort items by their x position

        float targetPositionX = items[1].transform.position.x;

        sequence?.Kill(true);
        sequence = DOTween.Sequence();
        sequence.Append(items[0].transform.DOMoveX(targetPositionX, smashDuration).SetEase(Ease.InBack));
        sequence.Join(items[2].transform.DOMoveX(targetPositionX, smashDuration).SetEase(Ease.InBack));

        sequence.OnComplete(() =>
        {
            FinalizeMerge(items);
        });
    }

    private void FinalizeMerge(List<Item> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject); // Destroy the items after merging

        }
        if (mergeParticles != null)
        {
            ParticleSystem particles = Instantiate(mergeParticles, items[1].transform.position, Quaternion.identity);
            particles.Play();
        }
    }

    private void UnsubscribeEvents()
    {
        ItemSpotsManager.ItemsMergeStartedAction -= OnItemsMergeStarted;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
