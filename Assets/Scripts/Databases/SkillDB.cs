using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using System.Numerics;

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

      AddSkillPassive("Call of the Pack", CallPackPassive, CallPackReverse);
      AddSkillPassive("Old Fighter's Chest", OldFightersChestCalc);
      AddSkillPassive("Bestial Instinct", BestialInstinctCalc);
      AddSkillPassive("Quick Rebuff");
      AddSkillPassive("Silent Blood");
      AddSkillPassive("Amulet of the Wind");
      AddSkillPassive("Trace of Ancient Route");
      //AddSkillPassive(KeyWord.Gigachad, "Test Skill", GigachadEveryTurn);
   }

   //Кастер всегда на самой первой позиции листа целей.
   private void AddSkillCast(string name, Action<List<Fighter>> cast,
      Func<List<Fighter>, List<int>> calc = null)
   {
      Skill skill = new Skill(name);
      skill.cast = cast;
      skill.calc = calc;
      skillDatabase.Add(name, skill);
   }
   private void AddSkillDeath(string name, Action death)
   {
      Skill skill = new Skill(name);
      skill.death = death;
      skillDatabase.Add(name, skill);
   }

   private void AddSkillEvTurn(string name, Action<List<Fighter>> everyTurn)
   {
      Skill skill = new Skill(name);
      skill.every_turn = everyTurn;
      skillDatabase.Add(name, skill);
   }

   private void AddSkillPassive(string name, Action<Fighter, List<Fighter>> passive, Action<Fighter, List<Fighter>> reverse, Func<List<Fighter>, List<int>> calc = null)
   {
      Skill skill = new Skill(name);
      skill.passive = passive;
      skill.reverse = reverse;
      skill.calc = calc;
      skillDatabase.Add(name, skill);
   }
   private void AddSkillPassive(string name, Func<List<Fighter>, List<int>> calc)
   {
      Skill skill = new Skill(name);
      skill.calc = calc;
      skillDatabase.Add(name, skill);
   }
   private void AddSkillPassive(string name)
   {
      Skill skill = new Skill(name);
      skillDatabase.Add(name, skill);
   }

   //Надо ли?
   public enum SkillTYPE
   {
      All_Allies
   }

   /*public void GigachadEveryTurn(List<Fighter> targets) //первый в targets всегда caster
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
      targets.RemoveAt(0); //Убираем кастера
      foreach (Fighter target in targets)
      {
         if(!target.isDead) target.TakeDmg(dmg);
      }
   }
   public List<int> BasicAttackCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      var max_characteristic = Mathf.Max(caster.strengh + caster.bonus_strengh, 
         caster.agility + caster.bonus_agility, 
         caster.wisdow + caster.bonus_wisdow);
      return new List<int> { 1 + max_characteristic / 2 };
   }

   //Healing Wounds
   public void HealingWoundsCast(List<Fighter> targets)
   {
      int heal = HealingWoundsCalc(targets)[0];
      if (!targets[0].isDead) targets[0].TakeHeal(heal);
   }
   public List<int> HealingWoundsCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      return new List<int> { caster.wisdow + caster.bonus_wisdow };
   }

   //Waiting
   public void WaitingCast(List<Fighter> targets)
   {
      //Будем вешать бафф
      var caster = targets[0];
      caster.buffs.Add(Fighter.Buff.DoubleNextAttack);
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
      return new List<int> { caster.defence };
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

   //Call of the Pack
   public void CallPackPassive(Fighter caster, List<Fighter> targets)
   {
      foreach(Fighter wolf in targets)
      {
         if (wolf.Data.name.ToLower().Contains("wolf") && wolf != caster)
         {
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
         if (wolf.Data.name.ToLower().Contains("wolf") && wolf != caster)
         {
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
}
