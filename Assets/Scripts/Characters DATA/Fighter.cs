using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fighter
{
   //������
   public CharacterSO Data;
   //public int lvl = 1;

   //������ ��������������
   public int hp;
   public int max_hp
   {
      get
      {
         return constitution * 5;
      }
   }
   public int bonus_hp; //���� � ��� ���-�� �� �������� ��� ��������
   public int defence; //�������������� ��������, ������� ����� ��������� ��������
   public int armor; //�������������� ��������, ������� ����� ��������� ��������
   public Skill Intension = null; //��������� ������������ �����
   public Skill prevIntension = null;

   public bool isDead = false;

   //��������������
   public int strengh, agility, wisdow, constitution;
   public int bonus_strengh, bonus_agility, bonus_wisdow; //������ ��������� ���-���

   public List<Buff> buffs = new();

   public bool isSpawn;

   //������ ���������
   public List<Skill> skills = new List<Skill>
{
    SkillDB.Instance.GetSkillByName("Basic Attack")
};

   public Fighter(string name)
   {
      Data = Resources.Load<CharacterSO>("CharData/" + name);
      Fill(Data);
   }
   public Fighter(CharacterSO data)
   {
      Data = data;
      Fill(Data);
   }

   private void Fill(CharacterSO Data)
   {
      constitution = Data.constitution;
      hp = max_hp;
      strengh = Data.strengh;
      agility = Data.agility;
      wisdow = Data.wisdow;
      defence = Data.defence;
      armor = defence * 2;

      if (Data.isEnemy)
      {
         foreach (SkillSO skillSO in Data.skills)
         {
            AddSkill(skillSO);
         }
      }
   }

   public void AddSkill(SkillSO skillData)
   {
      var skill = SkillDB.Instance.GetSkillByName(skillData.name);
      if (!skills.Contains(skill))
      {
         skills.Add(skill);
         CheckAddSkillGlobal(skill);
      }
   }
   public void AddSkill(Skill skill)
   {
      if (!skills.Contains(skill))
      {
         skills.Add(skill);
         CheckAddSkillGlobal(skill);
      }
   }
   public void AddSkill(string skillName)
   {
      var skill = SkillDB.Instance.GetSkillByName(skillName);
      if (!skills.Contains(skill))
      {
         skills.Add(skill);
         CheckAddSkillGlobal(skill);
      }
   }

   public void CheckAddSkillGlobal(Skill skill)
   {
      switch (skill.skillData.globalPassiveBuff)
      {
         case RunInfo.GlobalBuff.AmuletWind:
            var pool = SkillDB.Instance.GetPoolByName("WindSkills");
            var list = pool.skillList;
            AddSkill(list[Random.Range(0, list.Count)]);
            return;
      }
   }

   public void CastSkill(List<Fighter> targets, Skill skill = null)
   {
      skill ??= Intension;

      bool isDoubleNextAttack = false;

      //�������� �� ����� � ����������
      for (int i = 0; i < buffs.Count; i++)
      {
         switch (buffs[i])
         {
            case Buff.DoubleNextAttack:
               if (skill.skillData.skill_type != SkillSO.SkillType.Attack) continue;
               isDoubleNextAttack = true;
               buffs.RemoveAt(i);
               i--;
               break;
            case Buff.BestialInstinct:
               if (skill.skillData.skill_type == SkillSO.SkillType.Attack) 
               {
                  if (buffs.Contains(Buff.BestialInstinctBuff))
                     buffs.Remove(Buff.BestialInstinctBuff);
                  continue; 
               }
               if (!buffs.Contains(Buff.BestialInstinctBuff))
                  buffs.Add(Buff.BestialInstinctBuff);
               break;
         }
      }
      skill.Cast(this, targets);

      if (isDoubleNextAttack)
      {
         CastSkill(Fight.ChooseTarget(skill, this, Fight.RandomEnemy(this)), skill);
      }
   }

   public Sprite Portrait
   {
      get
      {
         if (this is PlayableCharacter) return (this as PlayableCharacter).Portrait;
         return Data.portrait_monster;
      }
      private set { }
   }

   public void Spawn()
   {
      if (isSpawn) return;

      //��� ��� ����������� �� ����
      var allSpawned = new List<Fighter>();
      foreach (Fighter character in Fight.AllCharacter)
         if (character.isSpawn) allSpawned.Add(character);

      //���� ������
      foreach (Skill skill in skills)
      {
         skill.battlecry?.Invoke();
         skill.passive?.Invoke(this, allSpawned);
      }

      //����������� �� ���� ������
      foreach (Fighter character in allSpawned)
      {
         if (character != null && character != this)
            foreach (Skill skill in character.skills)
            {
               //passive
               skill.passive?.Invoke(character, new List<Fighter> { this });
            }
      }
      isSpawn = true;
   }

   public void Death()
   {
      isDead = true;

      foreach(Skill skill in skills)
      {
         skill.death?.Invoke();
         skill.reverse?.Invoke(this, Fight.AllCharacter);
      }
   }

   public Skill GetPassiveSkill(Buff buff)
   {
      foreach(var skill in skills)
      {
         if (skill.skillData.passiveBuff == buff)
            return skill;
      }
      return null;
   }

   public void TakeDmg(int dmg)
   {
      //Buffs
      foreach(var buff in buffs)
      {
         switch (buff)
         {
            case Buff.OldFightersPlate:
               dmg -= GetPassiveSkill(buff).calc(new List<Fighter> { this })[0];
               break;
         }
      }

      if (dmg < 0) return;
      if(armor > 0)
      { 
         var armor_cur = armor;
         armor -= dmg;
         dmg -= armor_cur;
         if (armor < 0) armor = 0;
      }
      if (dmg >= 0)
      {
         hp -= dmg;
         if (hp <= 0)
         {
            hp = 0;
            Death();
         }
      }
   }

   public void TakeHeal(int heal)
   {
      if (heal >= 0)
      {
         hp += heal;
         if (hp > max_hp + bonus_hp)
         {
            hp = max_hp + bonus_hp;
         }
      }
   }

   public enum Buff
   {
      None,
      DoubleNextAttack,
      OldFightersPlate,
      QuickRebuff,
      BestialInstinct,
      BestialInstinctBuff, // ������������� ����� ���������� �� ��������� �����
      QuietBlessing,

   }
}
