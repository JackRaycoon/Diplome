using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSkillStructure
{
   public List<Fighter> targets;
   public bool needCooldown = true;
   public Skill skill = null;

   public CastSkillStructure(List<Fighter> targets, bool needCooldown, Skill skill)
   {
      this.targets = targets;
      this.needCooldown = needCooldown;
      this.skill = skill;
   }
}
