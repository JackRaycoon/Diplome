using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

   private void InitializeSkillDatabase()
   {
      AddSkillCast("Basic Attack", BasicAttackCast, BasicAttackCalc);
      AddSkillCast("Fire Wave", FireWaveCast, FireWaveCalc);

      AddSkillPassive("Passive Wolf", WolfPassive, WolfReverse);
      //AddSkillPassive(KeyWord.Gigachad, "Test Skill", GigachadEveryTurn);
      //AddSkillPassive(KeyWord.Gigachad, "Gigachad", GigachadPassive, GigachadReverse, "Гигачад своим видом вдохновляет каждого союзника. Все союзники получают +1 к атаке.");
   }

   //Кастер всегда на самой первой позиции листа целей.
   private void AddSkillCast(string name, Action<List<Fighter>> cast,
      Func<List<Fighter>, List<int>> calc)
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

   private void AddSkillPassive(string name, Action<Fighter, List<Fighter>> passive, Action<Fighter, List<Fighter>> reverse)
   {
      Skill skill = new Skill(name);
      skill.passive = passive;
      skill.reverse = reverse;
      skillDatabase.Add(name, skill);
   }

   //Надо ли?
   public enum SkillTYPE
   {
      All_Allies
   }

   public void GigachadEveryTurn(List<Fighter> targets) //первый в targets всегда caster
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
   }

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
      return new List<int> { 1 + max_characteristic / 3 };
   }
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
      return new List<int> { caster.wisdow + caster.bonus_wisdow };
   }
   public void WolfPassive(Fighter caster, List<Fighter> targets)
   {
      foreach(Fighter wolf in targets)
      {
         if (wolf.Data.character_name.ToLower().Contains("wolf") && wolf != caster)
         {
            wolf.bonus_hp += 1;
            wolf.hp += 1;
            wolf.bonus_strengh++;
            wolf.bonus_agility++;
            wolf.bonus_wisdow++;
         }
      }
   }
   public void WolfReverse(Fighter caster, List<Fighter> targets)
   {
      foreach (Fighter wolf in targets)
      {
         if (wolf.Data.character_name.ToLower().Contains("wolf") && wolf != caster)
         {
            wolf.bonus_hp -= 1;
            if(wolf.hp > wolf.max_hp + wolf.bonus_hp) wolf.hp = wolf.max_hp + wolf.bonus_hp;
            wolf.bonus_strengh--;
            wolf.bonus_agility--;
            wolf.bonus_wisdow--;
         }
      }
   }
   public void GigachadDeath()
   {
      Debug.Log("Giga Death");
   }

}
