using System.Collections.Generic;
using UnityEngine;

public struct EnemyIntent
{
    public EnemyClass enemy;
    public PlayerClass targetPlayer;
    public Vector3 destination;
    public bool willAttack;
    public bool willSkill;
    public Skill chosenSkill;
}

public abstract class EnemyIntentBehavior
{
    public abstract EnemyIntent CalculateIntent(EnemyClass enemy, List<PlayerClass> players, int turnNumber, float rangeBuffer);
    protected PlayerClass FindNearestPlayer(Vector3 fromPosition, List<PlayerClass> players)
    {
        PlayerClass nearest = null;
        float nearestDist = float.MaxValue;

        foreach (PlayerClass p in players)
        {
            if (p == null || p.hp <= 0) continue;

            float dist = Vector3.Distance(fromPosition, p.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = p;
            }
        }

        return nearest;
    }

    // moves as far as possible toward target, capped by movement range,
    // and never overshoots past the target itself
    protected Vector3 MoveTowardTarget(Vector3 fromPosition, Vector3 targetPosition, float movement)
    {
        Vector3 toTarget = targetPosition - fromPosition;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        if (distance <= 0.0001f) return fromPosition; // already on top of target, avoid NaN direction

        Vector3 direction = toTarget.normalized;
        float moveDistance = Mathf.Min(distance, movement);

        return fromPosition + direction * moveDistance;
    }
    protected Vector3 MoveAwayFromTarget(Vector3 fromPosition, Vector3 targetPosition, float movement)
    {
        Vector3 awayFromTarget = fromPosition - targetPosition;
        awayFromTarget.y = 0f;

        if (awayFromTarget.sqrMagnitude <= 0.0001f)
        {
            // exactly on top of target, no defined direction to retreat in — pick something rather than NaN
            awayFromTarget = Vector3.forward;
        }
        else
        {
            awayFromTarget.Normalize();
        }

        return fromPosition + awayFromTarget * movement;
    }

}