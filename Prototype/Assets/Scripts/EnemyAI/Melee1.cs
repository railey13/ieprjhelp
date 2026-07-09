using System.Collections.Generic;
using UnityEngine;

public class Melee1 : EnemyIntentBehavior
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

        Vector3 toTarget = target.transform.position - enemy.transform.position;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;
        Vector3 direction = toTarget.normalized;

        float moveDistance = Mathf.Min(distance, enemy.movement);
        Vector3 destination = enemy.transform.position + direction * moveDistance;

        // will they be in range to attack after moving?
        float distanceAfterMove = Vector3.Distance(destination, target.transform.position);
        bool willAttack = distanceAfterMove <= enemy.range + rangeBuffer;
        bool willSkill = false;
        Skill chosenSkill = null;

        foreach (SkillState state in enemy.skillStates)
        {
            if (!state.IsReady(turnNumber))
                continue;

            chosenSkill = state.skill;
            willSkill = distanceAfterMove <= chosenSkill.range + rangeBuffer;

            if (willSkill)
            {
                willAttack = false;
                break;
            }
        }

        intent.targetPlayer = target;
        intent.destination = destination;
        intent.willAttack = willAttack;
        intent.willSkill = willSkill;
        intent.chosenSkill = chosenSkill;

        return intent;
    }
}