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

   public Action<List<Fighter>> cast; //���������� ������
   public Func<List<Fighter>, List<int>> calc; //������� ������ (����, ������������ � ��)
   public Action<List<Fighter>> death; //����������� ������������� ��� ������ ���������

   public Action<Fighter, List<Fighter>> passive; //��������� ������ "���� � ��� �� ��������� ��-��"
   public Action<Fighter, List<Fighter>> reverse; //������ ���������� �������, ����� ��� �������� ���������

   public Skill(string name, bool isPassive)
   {
      skillData = Resources.Load<SkillSO>($"SkillData/{(isPassive ? "Passive" : "Active")}/" + name);
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
         return $"<size=60>{skillData._name}</size>\n";
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
   public string Description(bool needCooldown, Fighter fighter = null)
   {
      var description = skillData.description;
      //�������� �� ���������� �����, ���������� ��������
      foreach (var buff in SaveLoadController.runInfo.globalBuffs)
      {
         switch (buff)
         {
            case RunInfo.GlobalBuff.TouchingMystery:
               if (skillData.name == "Healing Wounds" && (fighter == null || fighter.buffs.Contains(Buff.TouchingMystery)) )
                  description += ". �������� ����������� ������� ���������� ������";
               break;
            case RunInfo.GlobalBuff.CursedHand:
               if (skillData.name == "Basic Attack" && (fighter == null || fighter.buffs.Contains(Buff.CursedHand)) )
                  description += ". ����������� �� ���������� ��������� ���������";
               break;
         }
      }

      if(needCooldown)
         description += $"\n����� ��������������: {skillData.cooldown}";

      if (calc == null) return description;
      
      List<int> values;
      if (fighter != null)
         values = calc(new List<Fighter> { fighter });
      /*else if (Fight.isEnemyTurn)
         values = calc(new List<Fighter> { Fight.EnemyUITeam[0] });
      else
         values = calc(new List<Fighter> { Fight.SelectedCharacter() });*/
      else
      {
         values = calc(new List<Fighter> { SaveLoadController.runInfo.PlayerTeam[0] });
      }

      return string.Format(description, values.Cast<object>().ToArray());
   }


   public void Cast(Fighter caster, List<Fighter> targets)
   {
      //�������� �� ����� � �����, �� ������� ���, �� ���� ��� ��������� �����
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

                  Debug.Log("������ ��: " + target.Data.character_name);
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
