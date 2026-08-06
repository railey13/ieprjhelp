using UnityEngine;

public class MariaPassive : MonoBehaviour, IOnSkillUsedPassive
{
    public int castsNeededToEnhance = 3;
    public int currentCasts = 0;

    public bool IsEnhanced { get; private set; } = false;

    public void OnSkillUsed(UnitClass user, UnitClass target, Skill skill)
    {
        if (IsEnhanced)
            return;

        currentCasts++;

        Debug.Log(user.UnitName + " passive count: " + currentCasts + "/" + castsNeededToEnhance);

        if (currentCasts >= castsNeededToEnhance)
        {
            IsEnhanced = true;

            Debug.Log(user.UnitName + "'s skills are now enhanced");
        }
    }
}
