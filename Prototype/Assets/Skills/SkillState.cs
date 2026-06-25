using UnityEngine;

[System.Serializable]
public class SkillState
{
    public Skill skill;
    public int lastUsedTurn = -999;

    public bool IsReady(int currentTurn)
    {
        return currentTurn - lastUsedTurn >= skill.cooldown;
    }

    public void MarkUsed(int currentTurn)
    {
        lastUsedTurn = currentTurn;
    }
}