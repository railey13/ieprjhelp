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
    // How close an enemy is allowed to get to a target before stopping, to avoid overlapping.
    protected const float minStoppingDistance = 1.5f;

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

    protected Vector3 CalculateMoveDestination(Vector3 fromPosition, Vector3 targetPosition, float movement, int direction, float stoppingDistance = 0f)
    {
        Vector3 delta = (targetPosition - fromPosition) * direction;
        delta.y = 0f;

        float distance = delta.magnitude;

        if (distance <= 0.0001f)
        {
            // no defined direction 
            return fromPosition + Vector3.forward * movement;
        }

        Vector3 dir = delta.normalized;

        float moveDistance;
        if (direction > 0)
        {
            // moving toward the target: never overshoot past it, and stop short by stoppingDistance
            float maxTravel = Mathf.Max(0f, distance - stoppingDistance);
            moveDistance = Mathf.Min(maxTravel, movement);
        }
        else
        {
            // moving away from the target: always travel the full movement distance
            moveDistance = movement;
        }

        return fromPosition + dir * moveDistance;
    }

    protected Vector3 MoveTowardTarget(Vector3 fromPosition, Vector3 targetPosition, float movement, float stoppingDistance = 0f)
    {
        return CalculateMoveDestination(fromPosition, targetPosition, movement, 1, stoppingDistance);
    }

    protected Vector3 MoveAwayFromTarget(Vector3 fromPosition, Vector3 targetPosition, float movement)
    {
        return CalculateMoveDestination(fromPosition, targetPosition, movement, -1);
    }

}