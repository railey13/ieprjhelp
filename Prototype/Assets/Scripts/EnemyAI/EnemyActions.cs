using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyActions : MonoBehaviour {
    private TurnBasedSystemV3 turnSystem = null;
    private List<EnemyIntent> enemyIntents = new List<EnemyIntent>();

    public EnemyActions(TurnBasedSystemV3 system) {
        turnSystem = system;
    }

    public IEnumerator MoveEnemy(EnemyClass enemy, Vector3 destination) {
        Animator animator = enemy.GetComponent<Animator>();

        if (animator != null)
            animator.SetBool("IsRunning", true);

        while (Vector3.Distance(enemy.transform.position, destination) > 0.05f) {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                destination,
                5f * Time.deltaTime); // Movement speed

            yield return null;
        }

        enemy.transform.position = destination;

        if (animator != null)
            animator.SetBool("IsRunning", false);
    }

    public void CalculateEnemyIntents() {
        enemyIntents.Clear();

        if (!turnSystem) {
            Debug.LogError("TurnBasedSytem is null inside EnemyAI.");
            return;
        }

        foreach (EnemyClass enemy in turnSystem.GetEnemyList) {
            if (enemy == null || enemy.hp <= 0) continue;
            if (enemy.intentBehavior == null) continue;

            EnemyIntent intent = enemy.intentBehavior.CalculateIntent(enemy, turnSystem.GetPlayerList, turnSystem.GetTurnNumber, turnSystem.GetRangeBuffer);

            if (intent.targetPlayer == null) continue;

            enemyIntents.Add(intent);

            enemy.ShowIntent(intent.willAttack, intent.willSkill, intent.targetPlayer);
            EnemyIntentDisplay display = enemy.GetComponent<EnemyIntentDisplay>();
            if (display != null)
                display.UpdateIntent(intent.destination, enemy.range);
        }
    }

    private float GetEffectiveEnemyRange(float originalRange, PlayerClass target) {
        if (target is ArnfinnUnit arnfinn) {
            // Enzo changes: Arnfinn is harder to target so enemies lose range against him
            return Mathf.Max(1f, originalRange - arnfinn.enemyRangePenalty);
        }

        return originalRange;
    }

    public IEnumerator EnemyTakeTurn(EnemyClass enemy) {
        // false if battle is done
        if (!turnSystem.IsBattleRunning) {
            yield break;
        }

        var players = turnSystem.GetPlayerList;

        Debug.Log(enemy.UnitName + " acts. players.Count = " + players.Count);
        players.RemoveAll(p => p == null || p.hp <= 0);

        // find this enemy's pre-calculated intent
        EnemyIntent intent = enemyIntents.Find(i => i.enemy == enemy);

        // if no intent found, skip
        if (intent.enemy == null) {
            turnSystem.EndTurn();
            yield break;
        }

        // move to the pre-calculated destination regardless of where players moved
        yield return StartCoroutine(MoveEnemy(enemy, intent.destination));
        Debug.Log(enemy.UnitName + " moves to  position");

        if (intent.targetPlayer == null || intent.targetPlayer.hp <= 0) {
            Debug.Log(enemy.UnitName + " target is dead, action cancelled");
            StartCoroutine(EndTurnAfterDelay(0.5f));
            yield break;
        }

        float distance = Vector3.Distance(enemy.transform.position, intent.targetPlayer.transform.position);

        // attack the pre-calculated target if it's still alive
        if (intent.willSkill && intent.chosenSkill != null) {
            float effectiveSkillRange = GetEffectiveEnemyRange(intent.chosenSkill.range, intent.targetPlayer);

            if (intent.targetPlayer != null && intent.targetPlayer.hp > 0 && distance <= effectiveSkillRange + turnSystem.GetRangeBuffer) {
                Debug.Log(enemy.UnitName + " used skill against " + intent.targetPlayer.UnitName);

                int targetHpBefore = intent.targetPlayer.hp;
                intent.chosenSkill.Use(enemy, intent.targetPlayer);

                if (SpecialTurnRunner.Instance != null) {
                    SpecialTurnRunner.Instance.ReportSkillUse(
                        enemy,
                        intent.targetPlayer,
                        intent.chosenSkill,
                        targetHpBefore
                    );
                }

                SkillState usedState =
                enemy.skillStates.Find(s => s.skill == intent.chosenSkill);

                if (usedState != null) {
                    usedState.MarkUsed(turnSystem.GetTurnNumber);
                }
            }
        }
        else if (intent.willAttack) {

            float effectiveAttackRange = GetEffectiveEnemyRange(enemy.range, intent.targetPlayer);

            if (intent.targetPlayer != null && intent.targetPlayer.hp > 0 && distance <= effectiveAttackRange + turnSystem.GetRangeBuffer) {
                Debug.Log(enemy.UnitName + " attacks " + intent.targetPlayer.UnitName);
                DamageInfo info = new DamageInfo {
                    category = enemy.basicAttackCategory,
                    subtype = enemy.basicAttackSubtype,
                    hitCount = enemy.hitCount
                };
                enemy.PlayAttackAnimation();

                yield return new WaitForSeconds(0.7f);
                float dmg = DamageCalculator.CalculateDamage(enemy, intent.targetPlayer, info);
                intent.targetPlayer.TakeDamage(dmg, enemy.basicAttackSubtype);

                if (turnSystem.GetBattleLogger != null)
                    turnSystem.GetBattleLogger.AddEntry($"{enemy.UnitName} attacked {intent.targetPlayer.UnitName} for {dmg} damage.");
            }
            else {
                Debug.Log(enemy.UnitName + " target is dead, attack cancelled");
            }
        }

        turnSystem.WinLoseState();

        if (!turnSystem.IsBattleRunning) {
            Debug.Log("Game over — halting turn loop");
            yield break;
        }

        StartCoroutine(EndTurnAfterDelay(0.5f));
        //turnSystem.EndTurn();
    }

    private IEnumerator EndTurnAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        turnSystem.EndTurn();
    }
}
