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
}