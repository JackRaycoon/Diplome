using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class Fighter
{
   //Данные
   public CharacterSO Data;
   //public int lvl = 1;

   //Боевые характеристики
   public int hp;
   public int max_hp
   {
      get
      {
         return constitution * 5;
      }
   }
   public int bonus_hp; //если в бою чьи-то хп повысили или понизили
   public int defence; //дополнительное здоровье, которое может превышать максимум
   public int armor; //дополнительное здоровье, которое может превышать максимум
   public Skill Intension = null; //Намерение использовать скилл
   public Skill prevIntension = null;

   public bool isDead = false;

   //Характеристики
   public int strengh, agility, wisdow, constitution;
   public int bonus_strengh, bonus_agility, bonus_wisdow; //боевые изменения хар-тик

   public List<Buff> buffs = new();

   public Dictionary<Skill, short> cooldowns = new();

   public bool isSpawn;
   public bool isSummon;
   public bool isFear;

   //Умения персонажа
   public List<Skill> skills = new()
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

      //skills = new();
      foreach (SkillSO skillSO in Data.skills)
      {
         AddSkill(skillSO);
      }
   }

   public void AddSkill(SkillSO skillData)
   {
      var skill = SkillDB.Instance.GetSkillByName(skillData.name);
      if (!skills.Contains(skill) && CheckSkillCount(skillData.skill_target))
      {
         skills.Add(skill);
         CheckAddSkillGlobal(skill);
      }
   }
   public void AddSkill(Skill skill)
   {
      if (!skills.Contains(skill) && CheckSkillCount(skill.skillData.skill_target))
      {
         skills.Add(skill);
         CheckAddSkillGlobal(skill);
      }
   }
   public void AddSkill(string skillName)
   {
      var skill = SkillDB.Instance.GetSkillByName(skillName);
      if (!skills.Contains(skill) && CheckSkillCount(skill.skillData.skill_target))
      {
         skills.Add(skill);
         CheckAddSkillGlobal(skill);
      }
   }

   public bool CheckSkillCount(SkillSO.SkillTarget st)
   {
      int slots;
      bool isPassive = st == SkillSO.SkillTarget.Passive;
      if (isPassive)
      {
         slots = PassiveSkillEmptySlots();
         //slots = 2 + wisdow / 2;
         //if (slots > 7) slots = 7; //7 от статов + 1 от зелья = максимум 8

         //cur_count = 0;
         //foreach(var skill in skills)
         //{
         //   if (skill.skillData.skill_target == SkillSO.SkillTarget.Passive) cur_count++;
         //}
      }
      else
      {
         slots = ActiveSkillEmptySlots();
         //slots = 3 + agility / 5;
         //if (slots > 5) slots = 5; //5 от статов + 1 от зелья = максимум 6

         //cur_count = 0;
         //foreach (var skill in skills)
         //{
         //   if (skill.skillData.skill_target != SkillSO.SkillTarget.Passive) cur_count++;
         //}
      }
         return slots > 0;
   }

   public int ActiveSkillEmptySlots()
   {
      int slots, cur_count;
      slots = 3 + agility / 5;
      if (slots > 5) slots = 5;

      cur_count = 0;
      foreach (var skill in skills)
      {
         if (skill.skillData.skill_target != SkillSO.SkillTarget.Passive) cur_count++;
      }
      return slots - cur_count;
   }

   public int PassiveSkillEmptySlots()
   {
      int slots, cur_count;
      slots = 2 + wisdow / 2;
      if (slots > 7) slots = 7; //7 от статов + 1 от зелья = максимум 8

      cur_count = 0;
      foreach (var skill in skills)
      {
         if (skill.skillData.skill_target == SkillSO.SkillTarget.Passive) cur_count++;
      }
      return slots - cur_count;
   }

   public void CheckAddSkillGlobal(Skill skill)
   {
      switch (skill.skillData.globalPassiveBuff)
      {
         case RunInfo.GlobalBuff.AmuletWind:
            var pool = SkillDB.Instance.GetPoolByName("WindSkills");
            var list = pool.activeSkillList;
            AddSkill(list[Random.Range(0, list.Count)]);
            return;
         case RunInfo.GlobalBuff.HeartDarkness:
            pool = SkillDB.Instance.GetPoolByName("DarkSkills");
            list = pool.activeSkillList;
            AddSkill(list[Random.Range(0, list.Count)]);
            return;
         case RunInfo.GlobalBuff.CursedHand:
            AddSkill(SkillDB.Instance.GetSkillByName("Purple Haze"));
            return;
      }
   }

   public void CastSkill(List<Fighter> targets, bool needCooldown = true, Skill skill = null)
   {
      skill ??= Intension;

      bool isDoubleNextAttack = false;

      //Проверка на баффы у кастующего
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
      var cooldown = skill.skillData.cooldown;
      if (cooldown != 0 && needCooldown)
         cooldowns.Add(skill, cooldown);

      if (isDoubleNextAttack)
      {
         CastSkill(Fight.ChooseTarget(skill, this, Fight.RandomEnemy(this)), false, skill);
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

      //Все уже появившиеся на поле
      var allSpawned = new List<Fighter>();
      foreach (Fighter character in Fight.AllCharacter)
         if (character.isSpawn) allSpawned.Add(character);

      //Свои абилки
      foreach (Skill skill in skills)
      {
         skill.battlecry?.Invoke();
         skill.passive?.Invoke(this, allSpawned);
      }

      //Действующие на него абилки
      foreach (Fighter character in allSpawned)
      {
         if (character != null && character != this)
            foreach (Skill skill in character.skills)
            {
               //passive
               skill.passive?.Invoke(character, new List<Fighter> { this });
            }
      }

      //Баффы от пассивок
      foreach (var skill in skills)
      {
         var skillData = skill.skillData;
         if (skillData.passiveBuff != Buff.None)
         {
            buffs.Add(skillData.passiveBuff);
         }
      }

      if(Fight.EnemyTeam.Contains(this))
         MakeIntention();
      isSpawn = true;
   }

   public void MakeIntention()
   {
      prevIntension = Intension;

      //Количество скиллов без пассивок и перезаряжаемых скиллов
      //int skillCount = 0;
      List<Skill> availableSkills = new();
      foreach (Skill skill in skills)
      {
         if (skill.skillData.skill_target != SkillSO.SkillTarget.Passive
            && !cooldowns.Keys.Contains(skill))
            availableSkills.Add(skill);
      }
      //bool reroll;
      var count = availableSkills.Count;
      if (count > 0)
         do
         {
            Intension = availableSkills[Random.Range(0, count)];
         } while (count > 1 && Intension == prevIntension);
      /*do
         {
            var _skills = skills;
            Intension = _skills[Random.Range(0, _skills.Count)];
            if (prevIntension != null)
               reroll = Intension.skillData.skill_target == SkillSO.SkillTarget.Passive ||
                        cooldowns.Keys.Contains(Intension) ||
                        (skillCount > 1 && Intension == prevIntension);
            else reroll = Intension.skillData.skill_target == SkillSO.SkillTarget.Passive ||
                  cooldowns.Keys.Contains(Intension);
         } while (reroll);*/
   }

   public void Death()
   {
      if(this == SaveLoadController.runInfo.PlayerTeam[0])
      {
         //Проверка на бессмертие:
         int chance = (strengh + bonus_strengh) * Fight.procentPerOneCharacteristic;
         if (chance > Fight.limitProcent) chance = Fight.limitProcent;
         int res = Random.Range(0, 100);
         if(res < chance)
         {
            hp = 1;
            return;
         }
      }
      isDead = true;

      foreach(Skill skill in skills)
      {
         skill.death?.Invoke(new List<Fighter> { this });
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

   public void FullHeal()
   {
      hp = max_hp + bonus_hp;
   }

   public enum Buff
   {
      None,
      DoubleNextAttack,
      OldFightersPlate,
      QuickRebuff,
      BestialInstinct,
      BestialInstinctBuff, // накладывается когда применяешь не атакующий навык
      QuietBlessing,
      ScreamIntoVoid,
      Corpseless,
      CursedHand,
      TouchingMystery,
      CurseDestruction,
   }
}
