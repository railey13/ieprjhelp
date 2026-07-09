using System.Collections.Generic;
using UnityEngine;

public class Ranged1 : EnemyIntentBehavior
{
    public override EnemyIntent CalculateIntent(EnemyClass enemy, List<PlayerClass> players, int turnNumber, float rangeBuffer)
    {
        EnemyIntent intent = new EnemyIntent { enemy = enemy };

        PlayerClass target = FindNearestPlayer(enemy.transform.position, players);
        if (target == null)
        {
            // no valid target leave targetPlayer null so the caller knows to skip this enemy
            return intent;
        }

        // can they be hit
        float distanceFromCurrent = Vector3.Distance(enemy.transform.position, target.transform.position);
        float tooCloseDistance = enemy.range * 0.5f; // arbitrary threshold for being too close to the target
        bool canHitFromCurrentPosition = distanceFromCurrent <= enemy.range + rangeBuffer;

        Vector3 destination;
        bool willAttack = true; // always aiming at the nearest target, attack fails if not in range.

        if (canHitFromCurrentPosition)
        {
            if(tooCloseDistance >= distanceFromCurrent)
            {
                // too close, move away from the target
                Vector3 awayFromTarget = (enemy.transform.position - target.transform.position).normalized; // opposite direction from the target
                float moveDistance = Mathf.Min(enemy.movement, tooCloseDistance - distanceFromCurrent);
                destination = enemy.transform.position + awayFromTarget * moveDistance;
            }
            else
            {
                // stay put, aim and shoot
                destination = enemy.transform.position;
                
            }
            // stay put, aim and shoot
            destination = enemy.transform.position;
        }
        else
        {
            // move closer toward the target
            Vector3 toTarget = target.transform.position - enemy.transform.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;
            Vector3 direction = toTarget.normalized;

            float moveDistance = Mathf.Min(distance, enemy.movement);
            destination = enemy.transform.position + direction * moveDistance;
        }

        intent.targetPlayer = target;
        intent.destination = destination;
        intent.willAttack = willAttack;
        intent.willSkill = false;   
        intent.chosenSkill = null;

        return intent;
    }
}