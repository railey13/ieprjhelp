using UnityEngine;

public class VaericUnit : PlayerClass, IOnSkillUsedPassive, IOutgoingDamageModifier
{
    public int maxHp = 0;
    public int selfDamagePerAction = 5;

    [Header("Revive")]
    public bool hasRevived = false;
    public int reviveAtkBoost = 10;
    public float revivedDamageMultiplier = 1.3f;

    private bool revivedBoostActive = false;

    private void Start()
    {
        if (maxHp <= 0)
            maxHp = hp;
    }

    public override void BasicAttack(EnemyClass target)
    {
        if (target == null)
            return;

        int finalDamage = SkillUtility.BuildDamage(this, atk, false);

        SkillUtility.DealDamage(this, target, finalDamage, "Basic Attack");

        // Enzo changes: Vaeric also loses hp from basic attacks
        TakeDamage(selfDamagePerAction);
    }

    public void OnSkillUsed(UnitClass user, UnitClass target, Skill skill)
    {
        if (user != this)
            return;

        // Enzo changes: Vaeric loses hp after his skills resolve
        TakeDamage(selfDamagePerAction);

        Debug.Log(UnitName + " lost " + selfDamagePerAction + " HP from using a skill");
    }

    public int ModifyOutgoingDamage(int damage)
    {
        if (!revivedBoostActive)
            return damage;

        return Mathf.RoundToInt(damage * revivedDamageMultiplier);
    }

    public override void TakeDamage(int damage)
    {
        hp -= damage;

        Debug.Log(UnitName + " took " + damage + " damage. Current HP: " + hp);

        HitEffect hitEffect = GetComponent<HitEffect>();
        if (hitEffect != null)
            hitEffect.PlayHitEffect();

        if (hp <= 0)
        {
            if (!hasRevived)
            {
                hasRevived = true;
                revivedBoostActive = true;
                hp = maxHp;
                atk += reviveAtkBoost;

                Debug.Log(UnitName + " revived once and got stronger");
                return;
            }

            hp = 0;
            Die();
        }
    }
}
