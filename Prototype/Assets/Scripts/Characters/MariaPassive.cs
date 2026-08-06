using UnityEngine;

public class MariaPassive : MonoBehaviour, IOnSkillUsedPassive
{
    [Header("Maria Passive")]
    public int castsNeededToEnhance = 3;
    public int currentCasts = 0;

    public bool IsEnhanced { get; private set; } = false;

    public void OnSkillUsed(UnitClass user, UnitClass target, Skill skill)
    {
        if (user == null)
            return;

        if (user.gameObject != gameObject)
            return;

        if (IsEnhanced)
            return;

        currentCasts++;

        Debug.Log(user.UnitName + " passive count: " + currentCasts + "/" + castsNeededToEnhance);

        if (currentCasts >= castsNeededToEnhance)
        {
            IsEnhanced = true;

            Debug.Log(user.UnitName + "'s support skills are now enhanced");
        }
    }

    public void ResetEnhanceProgress()
    {
        // Enzo changes: just here in case I need to reset her passive for testing
        currentCasts = 0;
        IsEnhanced = false;
    }
}
