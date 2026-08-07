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
            return intent;
        }

        float distanceFromCurrent = Vector3.Distance(enemy.transform.position, target.transform.position);
        float tooCloseDistance = enemy.range * 0.5f;
        bool canHitFromCurrentPosition = distanceFromCurrent <= enemy.range + rangeBuffer;

        Vector3 destination;
        bool willAttack = true;

        if (canHitFromCurrentPosition)
        {
            if (distanceFromCurrent <= tooCloseDistance)
            {
                destination = MoveAwayFromTarget(enemy.transform.position, target.transform.position, enemy.movement);
            }
            else
            {
                destination = enemy.transform.position;
            }
        }
        else
        {
            // move closer toward the target — now stops minStoppingDistance short instead of walking into them
            destination = MoveTowardTarget(enemy.transform.position, target.transform.position, enemy.movement, minStoppingDistance);
        }

        intent.targetPlayer = target;
        intent.destination = destination;
        intent.willAttack = true;
        intent.willSkill = false;
        intent.chosenSkill = null;

        return intent;
    }
}