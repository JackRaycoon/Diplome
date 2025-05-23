using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;


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
         if (constitution <= 0)
            return 1;
         return constitution * 5;
      }
   }
   public int bonus_hp; //���� � ��� ���-�� �� �������� ��� ��������

   //����������� ������� (����� �����)
   public Dictionary<Effect, int> effectStacks = new();
   public int poisonStacks = 0;

   public int defence; //�������������� ��������, ������� ����� ��������� ��������
   public int armor; //�������������� ��������, ������� ����� ��������� ��������
   public Skill Intension = null; //��������� ������������ �����
   public Skill prevIntension = null;

   public bool isDead = false;

   //��������������
   public int strengh, agility, wisdow, constitution;
   public int bonus_strengh, bonus_agility, bonus_wisdow; //������ ��������� ���-���

   public List<Buff> buffs = new();

   public Dictionary<Skill, short> cooldowns = new();

   public bool isSpawn;
   public bool isSummon;
   public bool isFear;

   //������ ���������
   public List<Skill> skills = new()
   {
    SkillDB.Instance.GetSkillByName("Basic Attack")
   };
   public List<Skill> skillsBattle; //� ��� ���������� ���� ������, ���� �� ��������� ���-������ � ���

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
         AddSkillWithoutChecks(skillSO);
      }
   }

   public void AddSkill(SkillSO skillData)
   {
      var skill = SkillDB.Instance.GetSkillByName(skillData.name);
      if (!skills.Contains(skill) && CheckSkillCount(skillData))
      {
         skills.Add(skill);
         CheckAddSkillGlobal(skill);
      }
   }
   public void AddSkillWithoutChecks(SkillSO skillData)
   {
      var skill = SkillDB.Instance.GetSkillByName(skillData.name);
      skills.Add(skill);
   }
   public void AddSkill(Skill skill)
   {
      if (!skills.Contains(skill) && CheckSkillCount(skill.skillData))
      {
         skills.Add(skill);
         CheckAddSkillGlobal(skill);
      }
   }
   public void AddSkill(string skillName)
   {
      var skill = SkillDB.Instance.GetSkillByName(skillName);
      if (!skills.Contains(skill) && CheckSkillCount(skill.skillData))
      {
         skills.Add(skill);
         CheckAddSkillGlobal(skill);
      }
   }

   public bool CheckSkillCount(SkillSO data)
   {
      int slots;
      bool isPassive = data.skill_target == SkillSO.SkillTarget.Passive;
      if (isPassive && data.isCurse)
      {
         slots = CurseSkillEmptySlots();
      }
      else if (isPassive)
      {
         slots = PassiveSkillEmptySlots();
      }
      else
      {
         slots = ActiveSkillEmptySlots();
      }
         return slots > 0;
   }

   public int ActiveSkillEmptySlots()
   {
      int slots, cur_count;
      slots = 3 + agility / 3;
      if (slots > 7) slots = 7; //7 �� ������ + 1 �� ����� = �������� 8
      try
      {
         if (SaveLoadController.runInfo.activePotion) slots++;
      }
      catch { };

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
      if (slots > 7) slots = 7; //7 �� ������ + 1 �� ����� = �������� 8
      try
      {
         if (SaveLoadController.runInfo.passivePotion) slots++;
      }
      catch { };

      cur_count = 0;
      foreach (var skill in skills)
      {
         if (skill.skillData.skill_target == SkillSO.SkillTarget.Passive
            && !skill.skillData.isCurse) cur_count++;
      }
      return slots - cur_count;
   }
   public int CurseSkillEmptySlots()
   {
      int slots, cur_count;
      slots = 8;

      cur_count = 0;
      foreach (var skill in skills)
      {
         if (skill.skillData.skill_target == SkillSO.SkillTarget.Passive
            && skill.skillData.isCurse) cur_count++;
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

   public void CastSkill(List<Fighter> targets, Skill skill, bool needCooldown = true)
   {
      bool isDoubleNextAttack = false;
      List<Fighter> doubleTargets = targets;

      //�������� �� ����� � ����������
      for (int i = 0; i < buffs.Count; i++)
      {
         switch (buffs[i])
         {
            case Buff.DoubleNextAttack:
               if (skill.skillData.skill_type != SkillSO.SkillType.Attack) continue;
               isDoubleNextAttack = true;
               doubleTargets = Fight.ChooseTarget(skill, this, Fight.RandomEnemy(this));
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

            case Buff.EchoForest:
               if (skill.skillData.skill_type == SkillSO.SkillType.Attack) continue;
               armor += GetPassiveSkill(Buff.EchoForest).calc(new List<Fighter> { this })[0];
               break;

            case Buff.ProvocationCaster:
               List<Fighter> allyes = new List<Fighter>(Fight.PlayerTeam);
               if (Fight.IsEnemy(this, allyes[0]))
                  allyes = new List<Fighter>(Fight.EnemyTeam);
               foreach (var chara in allyes)
               {
                  if (chara.buffs.Contains(Buff.Provocation))
                     chara.buffs.Remove(Buff.Provocation);
               }
               buffs.Remove(Buff.ProvocationCaster);
               break;
            case Buff.DissolvingShadows:
               buffs.Remove(Buff.DissolvingShadows);
               break;
            case Buff.Accompaniment:
               buffs.Remove(Buff.Accompaniment);
               isDoubleNextAttack = true;
               SacrificeHP(max_hp);
               break;
         }
      }
      skill.Cast(this, targets);
      var cooldown = skill.skillData.cooldown;
      if (cooldown != 0 && needCooldown)
         cooldowns.Add(skill, cooldown);

      //�������� �� ����� ����� ���������� ����������
      if (buffs.Contains(Buff.PainSilencing))
      {
         SacrificeHP((int)((max_hp + bonus_hp) * 0.03f));
      }

      if (isDoubleNextAttack && !isDead)
      {
         Fight.additionalCastSkills.Add(new(doubleTargets, false, skill));
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

      skillsBattle = new(skills);

      //���� ������
      foreach (Skill skill in skillsBattle)
      {
         //skill.battlecry?.Invoke();
         skill.passive?.Invoke(this, allSpawned);
      }

      //����������� �� ���� ������
      foreach (Fighter character in allSpawned)
      {
         if (character != null && character != this)
            foreach (Skill skill in character.skillsBattle)
            {
               //passive
               skill.passive?.Invoke(character, new List<Fighter> { this });
            }
      }

      //����� �� ��������
      foreach (var skill in skillsBattle)
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

      //���������� ������� ��� �������� � �������������� �������
      //int skillCount = 0;
      List<Skill> availableSkills = new();
      foreach (Skill skill in skillsBattle)
      {
         if (skill.skillData.skill_target != SkillSO.SkillTarget.Passive
            && !cooldowns.Keys.Contains(skill))
            availableSkills.Add(skill);
      }
      if (buffs.Contains(Buff.NoAttack))
         availableSkills.Remove(SkillDB.Instance.GetSkillByName("Basic Attack"));
      availableSkills.Remove(prevIntension);
      //bool reroll;
      var count = availableSkills.Count;
      if (count > 0)
      {
         Intension = availableSkills[Random.Range(0, count)];
      }
      else 
      {
         if (cooldowns.Keys.Contains(prevIntension))
         {
            Intension = null;
            //Fight.skipTurn = true;
         }
      }
   }

   public void Death()
   {
      if(this == SaveLoadController.runInfo.PlayerTeam[0])
      {
         //�������� �� ����������:
         int chance = (strengh + bonus_strengh) * Fight.procentPerOneCharacteristic;
         if (chance > Fight.limitProcent) chance = Fight.limitProcent;
         int res = Random.Range(0, 100);
         if(res < chance)
         {
            hp = 1;
            return;
         }
      }

      //����� ����� �������
      foreach (var buff in buffs)
      {
         switch (buff)
         {
            case Buff.AngelicGrace:
               hp = max_hp + bonus_hp;
               buffs.Remove(Buff.AngelicGrace);
               effectStacks.Remove(Effect.AngelicGrace);
               var grace = SkillDB.Instance.GetSkillByName("Angelic Grace");
               skills.Remove(grace);
               skillsBattle.Remove(grace);
               return;
         }
      }

      isDead = true;

      //����� ��� ������
      foreach (var buff in buffs)
      {
         switch (buff)
         {
            case Buff.ProvocationCaster:
               List<Fighter> allyes = new List<Fighter>(Fight.PlayerTeam);
               if (Fight.IsEnemy(this, allyes[0]))
                  allyes = new List<Fighter>(Fight.EnemyTeam);
               foreach (var chara in allyes)
               {
                  if (chara.buffs.Contains(Buff.Provocation))
                     chara.buffs.Remove(Buff.Provocation);
               }
               buffs.Remove(Buff.ProvocationCaster);
               break;
         }
      }

      foreach (Skill skill in skillsBattle)
      {
         skill.death?.Invoke(new List<Fighter> { this });
         skill.reverse?.Invoke(this, Fight.AllCharacter);
      }
   }

   public Skill GetPassiveSkill(Buff buff)
   {
      foreach(var skill in skillsBattle)
      {
         if (skill.skillData.passiveBuff == buff)
            return skill;
      }
      return null;
   }

   public void TakeDmg(Fighter takeFrom, int dmg, SkillSO.SkillElement element, int counterRecursion = 0)
   {
      if (counterRecursion > 10000) return;
      //Buffs for DmgCaster, not include poison and curse
      if (takeFrom != null)
         foreach (var buff in takeFrom.buffs)
         {
            switch (buff)
            {
               case Buff.OldFightersPlate:
                  break;
            }
         }

      //Buffs
      foreach (var buff in buffs)
      {
         switch (buff)
         {
            case Buff.OldFightersPlate:
               dmg -= GetPassiveSkill(buff).calc(new List<Fighter> { this })[0];
               break;
            case Buff.Provocation:
               dmg = 0;
               break;
            case Buff.DissolvingShadows:
               if (element != SkillSO.SkillElement.Light)
                  dmg = 0;
               break;
            case Buff.EchoPain:
               switch(Random.Range(0,3))
               {
                  case 0:
                     bonus_strengh++;
                     break;
                  case 1:
                     bonus_agility++;
                     break;
                  case 2:
                     bonus_wisdow++;
                     break;
               }
               break;
            case Buff.PainSilencing:
               if(dmg > 1)
                  dmg = 1;
               break;
            case Buff.AngelicPower:
               int heal = dmg / 3;
               if (heal < 1) heal = 1;
               TakeHeal(this, heal, counterRecursion);
               var enemy = Fight.RandomEnemy(this);
               enemy.TakeDmg(takeFrom, heal * 2, SkillSO.SkillElement.Light, counterRecursion);
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

   public void SacrificeHP(int _hp)
   {
      int dmg = _hp;
      if (dmg <= 0) dmg = 1;

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

   public int TakeHeal(Fighter takeFrom, int heal, int counterRecursion = 0)
   {
      if (counterRecursion > 10000) return 0;
      int excess = 0;
      if (heal >= 0)
      {
         excess = heal - (max_hp + bonus_hp - hp);
         hp += heal;
         if (hp > max_hp + bonus_hp)
         {
            hp = max_hp + bonus_hp;
         }
      }
      if (excess < 0) excess = 0;

      //Buffs for HealCaster
      if (takeFrom != null)
         foreach (var buff in takeFrom.buffs)
         {
            switch (buff)
            {
               case Buff.VengefulLight:
                  var enemy = Fight.RandomEnemy(takeFrom);
                  enemy.TakeDmg(takeFrom, excess, SkillSO.SkillElement.Light, counterRecursion);
                  break;
            }
         }

      return excess;
   }

   public void FullHeal()
   {
      hp = max_hp + bonus_hp;
   }

   internal Skill BuffToSkill(Buff buff)
   {
      string name = "";
      switch (buff)
      {
         case Buff.DoubleNextAttack:
            name = "Waiting";
            break;
         case Buff.OldFightersPlate:
            name = "Old Fighter's Chest";
            break;
         case Buff.BestialInstinctBuff:
            name = "Bestial Instinct";
            break;
         case Buff.Corpseless:
            name = "Corpseless";
            break;
         case Buff.CurseDestruction:
            name = "Curse of Destruction";
            break;
         case Buff.CurseVictim:
            name = "Curse of Victim";
            break;
         case Buff.WeightMemories:
            name = "The Weight of Memories";
            break;
         case Buff.CallPack:
            name = "Call of the Pack";
            break;
         case Buff.EchoPain:
            name = "Echo of Pain";
            break;
         case Buff.EchoForest:
            name = "Echo of Forest";
            break;
         case Buff.Provocation:
            name = "Provocation";
            break;
         case Buff.Poison:
            name = "Poison";
            break;
         case Buff.DissolvingShadows:
            name = "Dissolving in the Shadows";
            break;
         case Buff.PainSilencing:
            name = "Pain Silencing";
            break;
         case Buff.SacrificialChant:
            name = "Sacrificial Chant";
            break;
         case Buff.Accompaniment:
            name = "Accompaniment";
            break;
         case Buff.NoAttack:
            name = "No Attack";
            break;
         case Buff.VengefulLight:
            name = "Vengeful Light";
            break;
         case Buff.AngelicGrace:
            name = "Angelic Grace";
            break;
         case Buff.AngelicPower:
            name = "Angelic Power";
            break;
         case Buff.ScarBetrayal:
            name = "Scar of Betrayal";
            break;
      }
      if (name == "") return null;
      return SkillDB.Instance.GetSkillByName(name);
   }

   public enum Effect
   {
      AngelicGrace,

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
      ScreamIntoVoid,
      Corpseless,
      CursedHand,
      TouchingMystery,
      CurseDestruction,
      WeightMemories,
      CallPack,
      EchoHope,
      EchoPain,
      EchoForest,
      Provocation,
      ProvocationCaster,
      Poison,
      DissolvingShadows,
      PainSilencing,
      SacrificialChant,
      CurseVictim,
      Accompaniment,
      NoAttack,
      WanderingMusician,
      VengefulLight,
      AngelicGrace,
      AngelicPower,
      AngelicGuardian,
      ScarBetrayal,

   }
}
