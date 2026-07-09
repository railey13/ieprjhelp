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


        Debug.Log($"Range: {enemy.range}");
        Debug.Log($"Too Close Distance: {tooCloseDistance}");
        Debug.Log($"Distance From Current: {distanceFromCurrent}");
        Debug.Log($"Enemy Pos: {enemy.transform.position}");
        Debug.Log($"Player Pos: {target.transform.position}");
        Debug.Log($"Can Hit: {canHitFromCurrentPosition}");
        Debug.Log($"Should Retreat: {distanceFromCurrent <= tooCloseDistance}");
        if (canHitFromCurrentPosition)
        {
            if (distanceFromCurrent <= tooCloseDistance)
            {
                // too close, move away from the target
                Debug.Log("RUNNING AWAY");
                Vector3 awayFromTarget = enemy.transform.position - target.transform.position;
                awayFromTarget.y = 0f;
                awayFromTarget = awayFromTarget.normalized;
                float moveDistance = enemy.movement;
                destination = enemy.transform.position + awayFromTarget * moveDistance;
            }
            else
            {
                // stay put, aim and shoot
                destination = enemy.transform.position;
                Debug.Log("STAYING PUT");
            }
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
        intent.willAttack = true;
        intent.willSkill = false;
        intent.chosenSkill = null;

        return intent;
    }
}