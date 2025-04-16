using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Skill
{
   public SkillSO skillData;

   public Action<List<Fighter>> cast; //Применение скилла
   public Func<List<Fighter>, List<int>> calc; //Рассчёт скилла (урон, длительность и тд)
   public Action battlecry; //Применяется автоматически при появлении на поле
   public Action death; //Применяется автоматически при смерти персонажа
   public Action<List<Fighter>> every_turn; //Каждый ход какое-то действие

   public Action<Fighter, List<Fighter>> passive; //Пассивный эффект "пока я жив вы получаете то-то"
   public Action<Fighter, List<Fighter>> reverse; //Отмена пассивного эффекта, когда его источник пропадает

   public Skill(string name)
   {
      skillData = Resources.Load<SkillSO>("SkillData/" + name);
   }
   /*public Skill(SkillSO skillSO, Fighter _skillOwner)
   {
      skillData = skillSO;
      skillOwner = _skillOwner;
   }*/
   public Sprite Icon
   {
      get
      {
          return skillData.icon;
      }
      private set { }
   }

   public string Name
   {
      get
      {
         return $"<b><size=55>{skillData._name}</size></b>";
      }
      private set { }
   }

   /*public string Description
   {
      get
      {
         if(Fight.isEnemyTurn)
            return string.Format(skillData.description, calc(new List<Fighter> { Fight.EnemyUITeam[0] })[0]);
         else
            return string.Format(skillData.description, calc(new List<Fighter> { Fight.SelectedCharacter() })[0]);
      }
      private set { }
   }*/
   public string Description(Fighter fighter = null)
   {
      if (calc == null) return skillData.description;
      if (fighter != null) 
         return string.Format(skillData.description, calc(new List<Fighter> { fighter })[0]);

      if (Fight.isEnemyTurn)
         return string.Format(skillData.description, calc(new List<Fighter> { Fight.EnemyUITeam[0] })[0]);
      else
         return string.Format(skillData.description, calc(new List<Fighter> { Fight.SelectedCharacter() })[0]);
   }

   public void Cast(Fighter caster, List<Fighter> targets)
   {
      var list = new List<Fighter>(targets);
      list.Insert(0, caster);
      cast?.Invoke(list);
   }
}
