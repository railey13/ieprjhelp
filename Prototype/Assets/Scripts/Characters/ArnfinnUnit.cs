using UnityEngine;

public class ArnfinnUnit : PlayerClass
{
    [Header("Fast passive")]
    public int speedBonus = 5;
    public int movementBonus = 2;

    [Header("Harder to target")]
    public int enemyRangePenalty = 1;

    private bool applied = false;

    private void Start()
    {
        if (applied)
            return;

        // Enzo changes: simple stat passive for Arnfinn
        speed += speedBonus;
        movement += movementBonus;

        applied = true;

        Debug.Log(UnitName + " got Arnfinn speed passive");
    }
}
