using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Resolve/Darrene Counter")]
public class DarreneCounterSkill : Skill
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

        // Enzo changes: this just arms the counter then it triggers when Darrene gets hit
        darrene.SetCounterReady();

        Debug.Log(darrene.UnitName + " is ready to counter the next hit");

        SkillUtility.NotifySkillUsed(user, target, this);
    }
}
