using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SkillDB
{
   private static SkillDB instance;
   public static SkillDB Instance
   {
      get
      {
         if (instance == null)
         {
            instance = new SkillDB();
         }
         return instance;
      }
   }
   private SkillDB()
   {
      // Инициализация базы данных заклинаний
      InitializeSkillDatabase();
   }

   private Dictionary<string, Skill> skillDatabase = new();

   public Skill GetSkillByName(string name)
   {
      if (skillDatabase.ContainsKey(name))
      {
         return skillDatabase[name];
      }
      else
      {
         // Обработка случая, когда заклинание не найдено
         return null;
      }
   }

   public SkillPool GetPoolByName(string name)
   {
      return Resources.Load<SkillPool>("SkillData/Pools/" + name);
   }

   private void InitializeSkillDatabase()
   {
      AddSkillCast("Basic Attack", BasicAttackCast, BasicAttackCalc);
      AddSkillCast("Raise Shields", RaiseShieldsCast, RaiseShieldsCalc);
      AddSkillCast("Fire Wave", FireWaveCast, FireWaveCalc);
      AddSkillCast("Healing Wounds", HealingWoundsCast, HealingWoundsCalc);
      AddSkillCast("Waiting", WaitingCast);
      AddSkillCast("Blade of the Wind", BladeWindCast, BladeWindCalc);
      AddSkillCast("Summon the Shadow", SummonShadowCast);
      AddSkillCast("Purple Haze", PurpleHazeCast, PurpleHazeCalc);
      AddSkillCast("Curse of Destruction", CurseDestructionCast);
      AddSkillCast("Surge of Darkness", SurgeDarknessCast, SurgeDarknessCalc);

      AddSkillPassive("Empty");
      AddSkillPassive("Call of the Pack", CallPackPassive, CallPackReverse);
      AddSkillPassive("Old Fighter's Chest", OldFightersChestCalc);
      AddSkillPassive("Bestial Instinct", BestialInstinctCalc);
      AddSkillPassive("Quick Rebuff");
      AddSkillPassive("Silent Blood");
      AddSkillPassive("Amulet of the Wind");
      AddSkillPassive("Trace of Ancient Route");
      AddSkillPassive("Quiet Blessing");
      AddSkillPassive("Touching the Mystery");
      AddSkillPassive("Scream Into the Void");
      AddSkillPassive("Heart of Darkness", HeartDarknessPassive, HeartDarknessReverse);
      AddSkillPassive("Corpseless");
      AddSkillPassive("Cursed Hand");
      AddSkillPassive("The Weight of Memories");
   }

   //Кастер всегда на самой первой позиции листа целей.
   private void AddSkillCast(string name, Action<List<Fighter>> cast,
      Func<List<Fighter>, List<int>> calc = null)
   {
      Skill skill = new(name, false)
      {
         cast = cast,
         calc = calc
      };
      skillDatabase.Add(name, skill);
   }
   private void AddSkillDeath(string name, Action<List<Fighter>> death)
   {
      Skill skill = new(name, true)
      {
         death = death
      };
      skillDatabase.Add(name, skill);
   }

   private void AddSkillEvTurn(string name, Action<List<Fighter>> everyTurn)
   {
      Skill skill = new(name, true)
      {
         every_turn = everyTurn
      };
      skillDatabase.Add(name, skill);
   }

   private void AddSkillPassive(string name, Action<Fighter, List<Fighter>> passive, Action<Fighter, List<Fighter>> reverse, Func<List<Fighter>, List<int>> calc = null)
   {
      Skill skill = new(name, true)
      {
         passive = passive,
         reverse = reverse,
         calc = calc
      };
      skillDatabase.Add(name, skill);
   }
   private void AddSkillPassive(string name, Func<List<Fighter>, List<int>> calc)
   {
      Skill skill = new(name, true)
      {
         calc = calc
      };
      skillDatabase.Add(name, skill);
   }
   private void AddSkillPassive(string name)
   {
      Skill skill = new(name, true);
      skillDatabase.Add(name, skill);
   }

   //Надо ли?
   public enum SkillTYPE
   {
      All_Allies
   }

   /*public void EveryTurn(List<Fighter> targets) //первый в targets всегда caster
   {
      //var Board = GameScript.EnemyBoard;
      //foreach (Card card in GameScript.PlayerBoard) if (card == caster) { Board = GameScript.PlayerBoard; break; }
      //foreach (Card target in Board)
      //{
      //   if (target != null && target != caster && UnityEngine.Random.Range(1, 101) <= 30)
      //   {
      //      target.ATK++;
      //   }
      //}
   }*/

   //Basic Attack
   public void BasicAttackCast(List<Fighter> targets)
   {
      int dmg = BasicAttackCalc(targets)[0];
      var caster = targets[0];
      targets.RemoveAt(0); //Убираем кастера
      foreach (Fighter target in targets)
      {
         if (!target.isDead)
         {
            target.TakeDmg(dmg);
            if (caster.buffs.Contains(Fighter.Buff.CursedHand)
               && SaveLoadController.runInfo.globalBuffs.Contains(RunInfo.GlobalBuff.CursedHand))
            {
               //Применяем к врагу случайный скилл из пула проклятий
               var pool = GetPoolByName("CurseSkills");
               var list = pool.activeSkillList;
               var skill = Instance.GetSkillByName(list[Random.Range(0,list.Count)].name);

               Fight.additionalCastSkills.Insert(0, new(new List<Fighter> { target }, false, skill));
               //caster.CastSkill(new List<Fighter> { caster, target }, false, skill);
            }
         }
      }
   }
   public List<int> BasicAttackCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      /*var max_characteristic = Mathf.Max(caster.strengh + caster.bonus_strengh, 
         caster.agility + caster.bonus_agility, 
         caster.wisdow + caster.bonus_wisdow);*/
      int strengh = caster.strengh + caster.bonus_strengh;
      return new List<int> { 1 + strengh };
   }

   //Healing Wounds
   public void HealingWoundsCast(List<Fighter> targets)
   {
      int heal = HealingWoundsCalc(targets)[0];
      var target = targets[1];
      if (!target.isDead)
      {
         if (SaveLoadController.runInfo.globalBuffs.Contains(RunInfo.GlobalBuff.TouchingMystery))
         {
            if (heal >= 0)
            {
               var needHP = target.max_hp + target.bonus_hp - target.hp;
               var excess = heal - needHP;
               if(excess > 0)
               {
                  var armor = excess / 2;
                  target.armor += (armor == 0) ? 1 : armor;
               }
            }
         }
         target.TakeHeal(heal);
      }
   }
   public List<int> HealingWoundsCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      return new List<int> { caster.wisdow + caster.bonus_wisdow };
   }

   //Waiting
   public void WaitingCast(List<Fighter> targets)
   {
      var caster = targets[0];
      caster.buffs.Add(Fighter.Buff.DoubleNextAttack);
   }

   //Curse of Destraction
   public void CurseDestructionCast(List<Fighter> targets)
   {
      var target = targets[1];
      target.buffs.Add(Fighter.Buff.CurseDestruction);
   }

   //Raise Shields
   public void RaiseShieldsCast(List<Fighter> targets)
   {
      int armor = RaiseShieldsCalc(targets)[0];
      if (!targets[0].isDead) targets[0].armor += armor;
   }
   public List<int> RaiseShieldsCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int defence = caster.defence;
      if (defence == 0) defence = 1;
      return new List<int> { defence };
   }

   //Fire Wave
   public void FireWaveCast(List<Fighter> targets)
   {
      int dmg = FireWaveCalc(targets)[0];
      targets.RemoveAt(0); //Убираем кастера
      foreach (Fighter target in targets)
      {
         if (!target.isDead) target.TakeDmg(dmg);
      }
   }
   public List<int> FireWaveCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      var sum = caster.strengh + caster.bonus_strengh + 
         caster.wisdow + caster.bonus_wisdow;
      return new List<int> { sum / 2 };
   }

   //Blade of the Wind
   public void BladeWindCast(List<Fighter> targets)
   {
      var list = BladeWindCalc(targets);
      int dmg = list[0];
      int count = list[1];

      var target = targets[1];
      for (int i = 0; i < count; i++)
      {
         if (!target.isDead) target.TakeDmg(dmg);
         else
         {
            target = Fight.RandomEnemy(targets[0]);
            if (target.isDead) break;
            i--;
         }
      }
   }
   public List<int> BladeWindCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      var sumWisdow = caster.wisdow + caster.bonus_wisdow;
      var sumAgility = caster.agility + caster.bonus_agility;
      return new List<int> { sumWisdow, 1 + sumAgility / 5};
   }

   //Summon the Shadow
   public void SummonShadowCast(List<Fighter> targets)
   {
      var caster = targets[0];
      var target = Fight.RandomEnemy(caster);
      bool isEnemyCast = Fight.IsEnemy(Fight.PlayerTeam[0], caster);

      if (target.isDead) return;
      Fighter shadow = new("Shadow")
      {
         strengh = target.strengh / 2,
         bonus_strengh = target.bonus_strengh / 2,

         agility = target.agility / 2,
         bonus_agility = target.bonus_agility / 2,

         wisdow = target.wisdow / 2,
         bonus_wisdow = target.bonus_wisdow / 2,

         constitution = (target.constitution) / 2,
         bonus_hp = target.bonus_hp / 2
      };
      if (shadow.constitution == 0) shadow.constitution = 1;
      shadow.FullHeal();

      foreach(var skill in target.skills)
      {
         shadow.AddSkill(skill);
      }

      if (isEnemyCast)
      {
         if (Fight.EnemyTeam.Count < 6)
         {
            Fight.EnemyTeam.Add(shadow);
            shadow.Spawn();
         }
      }
      else
      {
         if (Fight.PlayerTeam.Count < 6)
         {
            Fight.PlayerTeam.Add(shadow as PlayableCharacter);
            shadow.Spawn();
         }
      }
   }

   //Purple Haze
   public void PurpleHazeCast(List<Fighter> targets)
   {
      var list = PurpleHazeCalc(targets);
      int dmg = list[0];

      var caster = targets[0];
      targets.RemoveAt(0); //Убираем кастера
      foreach (Fighter target in targets)
      {
         if (!target.isDead)
         {
            target.TakeDmg(dmg);

            var pool = GetPoolByName("CurseSkills");
            var listSk = pool.activeSkillList;
            var skill = Instance.GetSkillByName(listSk[Random.Range(0, listSk.Count)].name);
            Fight.additionalCastSkills.Insert(0, new(new List<Fighter> { target }, false, skill));
            //caster.CastSkill(new List<Fighter> { caster, target }, false, skill);
         }
      }
   }
   public List<int> PurpleHazeCalc(List<Fighter> targets)
   {
      return new List<int> { ((int)SaveLoadController.runInfo.currentLocation + 1) * 10 };
   }

   //Surge of Darkness
   public void SurgeDarknessCast(List<Fighter> targets)
   {
      var list = SurgeDarknessCalc(targets);
      int dmg = list[0];
      int chance = list[1];

      var caster = targets[0];
      var target = targets[1];

      if (!target.isDead)
      {
         target.TakeDmg(dmg);

         if(Random.Range(0, 100) < chance)
         {
            var pool = GetPoolByName("CurseSkills");
            var listSk = pool.activeSkillList;
            var skill = Instance.GetSkillByName(listSk[Random.Range(0, listSk.Count)].name);
            Fight.additionalCastSkills.Insert(0, new(new List<Fighter> { target }, false, skill));
            //caster.CastSkill();
         }
      }
   }
   public List<int> SurgeDarknessCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int wisdow = caster.wisdow + caster.bonus_wisdow;
      int dmg = 1 + wisdow;
      int chance = wisdow * 10;
      if (chance > 100) chance = 100;
      return new List<int> { dmg, chance };
   }

   //Call of the Pack
   public void CallPackPassive(Fighter caster, List<Fighter> targets)
   {
      foreach(Fighter wolf in targets)
      {
         if (Fight.IsEnemy(caster, wolf)) continue;
         if (wolf.Data.name.ToLower().Contains("wolf") && wolf != caster)
         {
            wolf.buffs.Add(Fighter.Buff.CallPack);
            wolf.bonus_hp += 1;
            wolf.hp += 1;
            wolf.bonus_strengh++;
            wolf.bonus_agility++;
            wolf.bonus_wisdow++;
         }
      }
   }
   public void CallPackReverse(Fighter caster, List<Fighter> targets)
   {
      foreach (Fighter wolf in targets)
      {
         if (Fight.IsEnemy(caster, wolf)) continue;
         if (wolf.Data.name.ToLower().Contains("wolf") && wolf != caster)
         {
            wolf.buffs.Remove(Fighter.Buff.CallPack);
            wolf.bonus_hp -= 1;
            if(wolf.hp > wolf.max_hp + wolf.bonus_hp) wolf.hp = wolf.max_hp + wolf.bonus_hp;
            wolf.bonus_strengh--;
            wolf.bonus_agility--;
            wolf.bonus_wisdow--;
         }
      }
   }

   //Old Fighter's Chest
   public List<int> OldFightersChestCalc(List<Fighter> targets)
   {
      return new List<int> { (int)SaveLoadController.runInfo.currentLocation + 1 };
   }

   //Bestial Instinct
   public List<int> BestialInstinctCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      var agility = caster.agility + caster.bonus_agility;
      return new List<int> { 25 + Math.Clamp(agility / 2 - 2, 0, 50) };
   }

   //Heart of the Darkness
   public void HeartDarknessPassive(Fighter caster, List<Fighter> targets)
   {
      int darkBonus = SaveLoadController.runInfo.badKarma / 5;
      caster.bonus_hp += SaveLoadController.runInfo.badKarma;
      caster.hp += SaveLoadController.runInfo.badKarma;
      caster.bonus_strengh+= darkBonus;
      caster.bonus_agility += darkBonus;
      caster.bonus_wisdow += darkBonus;
   }
   public void HeartDarknessReverse(Fighter caster, List<Fighter> targets)
   {
      int darkBonus = SaveLoadController.runInfo.badKarma / 3;
      caster.bonus_hp -= darkBonus * 4;
      if (caster.hp > caster.max_hp + caster.bonus_hp) caster.hp = caster.max_hp + caster.bonus_hp;
      caster.bonus_strengh -= darkBonus;
      caster.bonus_agility -= darkBonus;
      caster.bonus_wisdow -= darkBonus;
   }
}
