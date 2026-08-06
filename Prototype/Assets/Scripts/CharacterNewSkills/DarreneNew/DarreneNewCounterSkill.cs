using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Darrene New/Counter Skill")]
public class DarreneNewCounterSkill : Skill
{
    public override void Use(UnitClass user, UnitClass target)
    {
        if (user == null)
            return;

        DarreneUnit darrene = user as DarreneUnit;

        if (darrene == null)
        {
            Debug.Log(SkillName + " can only be used by Darrene");
            return;
        }

        // Enzo changes: this just prepares the counter so it triggers when Darrene gets hit
        darrene.SetCounterReady();

        Debug.Log(darrene.UnitName + " is ready to counter");

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}
