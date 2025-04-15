using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillPool", menuName = "Skill Pool", order = 4)]
public class SkillPool : ScriptableObject
{
   public List<SkillSO> skillList;
}
