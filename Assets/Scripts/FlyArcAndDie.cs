using System;
using DG.Tweening;
using UnityEngine;

public class FlyArcAndDie : MonoBehaviour
{

    public static FlyArcAndDie Instance;

    private void Awake()
    {
        Instance = this;


    }

    public void FlyAndDie(
       Transform model,
       Vector3 direction,
       Action action = null,
       float distance = 10f,
       float height = 2f,
       float duration = 1.2f

   )
    {
        // Tính target point (điểm rơi xuống)
        Vector3 target = model.position - direction.normalized * distance + Vector3.right;

        // Tween bay theo arc
        model.DOJump(target, height, 1, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                action?.Invoke();
            });
    }
}
