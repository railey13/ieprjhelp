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

        Vector3 destination = MoveTowardTarget(enemy.transform.position, target.transform.position, enemy.movement);

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