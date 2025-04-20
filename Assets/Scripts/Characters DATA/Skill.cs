using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static Fighter;
using UnityEditor.Experimental.GraphView;
using static UnityEngine.GraphicsBuffer;

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

      List<int> values;
      if (fighter != null)
         values = calc(new List<Fighter> { fighter });
      else if (Fight.isEnemyTurn)
         values = calc(new List<Fighter> { Fight.EnemyUITeam[0] });
      else
         values = calc(new List<Fighter> { Fight.SelectedCharacter() });

      return string.Format(skillData.description, values.Cast<object>().ToArray());
   }


   public void Cast(Fighter caster, List<Fighter> targets)
   {
      //Проверка на баффы у целей, на промахи тут, на урон при получении урона
      for (int i = 0; i < targets.Count; i++)
      {
         var target = targets[i];
         for (int j = 0; j < target.buffs.Count; j++)
         {
            var buff = target.buffs[j];
            switch (buff)
            {
               case Buff.BestialInstinctBuff:
                  if (skillData.skill_type != SkillSO.SkillType.Attack) continue;
                  if (UnityEngine.Random.Range(1, 101) >
                     target.GetPassiveSkill(Buff.BestialInstinct).calc(new List<Fighter> { target })[0]) continue;

                  Debug.Log("Промах по: " + target.Data.character_name);
                  targets.Remove(target);
                  i--;
                  break;
            }
         }
      }
      var list = new List<Fighter>(targets);
      list.Insert(0, caster);
      cast?.Invoke(list);
   }
}
