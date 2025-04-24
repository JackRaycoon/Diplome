using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillPool", menuName = "Skill Pool", order = 4)]
public class SkillPool : ScriptableObject
{
   public List<SkillSO> activeSkillList;
   public List<SkillSO> passiveSkillList;

   private List<SkillSO> _allSkillList = null;

   public List<SkillSO> allSkillList
   {
      get
      {
         _allSkillList = new();
         foreach (var skill in activeSkillList) _allSkillList.Add(skill);
         foreach (var skill in passiveSkillList) _allSkillList.Add(skill);
         return _allSkillList;
      }
   }
}
