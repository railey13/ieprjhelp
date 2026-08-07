using UnityEngine;

public class ArnfinnUnit : PlayerClass
{
    [Header("Arnfinn Passive")]
    public int speedBonus = 5;
    public int movementBonus = 2;

    [Header("Harder To Target")]
    public int enemyRangePenalty = 1;

    private bool passiveApplied = false;

    private void Start()
    {
        ApplyArnfinnPassive();
    }

    private void ApplyArnfinnPassive()
    {
        if (passiveApplied)
            return;

        // Enzo changes: I only apply this once so his stats dont keep stacking
        speed += speedBonus;
        movement += movementBonus;

        passiveApplied = true;

        Debug.Log(UnitName + " got Arnfinn passive stats");
    }

    public override void BasicAttack(EnemyClass target)
    {
        if (target == null)
            return;

        int finalDamage = SkillUtility.BuildDamage(this, atk, false);

        SkillUtility.DealDamage(this, target, finalDamage, "Arrow Shot", basicAttackSubtype);

        PlayAttackAnimation();
    }

    public int GetEnemyRangePenalty()
    {
        // Enzo changes: enemy AI can use this later if we want enemies to have lower range against Arnfinn
        return enemyRangePenalty;
    }
}
