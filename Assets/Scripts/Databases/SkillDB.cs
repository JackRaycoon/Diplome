using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using static UnityEngine.GraphicsBuffer;

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

      //Active
      AddSkillCast("Raise Shields", RaiseShieldsCast, RaiseShieldsCalc);
      AddSkillCast("Fire Wave", FireWaveCast, FireWaveCalc);
      AddSkillCast("Healing Wounds", HealingWoundsCast, HealingWoundsCalc);
      AddSkillCast("Blade of the Wind", BladeWindCast, BladeWindCalc);
      AddSkillCast("Purple Haze", PurpleHazeCast, PurpleHazeCalc);
      AddSkillCast("Surge of Darkness", SurgeDarknessCast, SurgeDarknessCalc);
      AddSkillCast("Poisoned Thorn", PoisonedThornCast, PoisonedThornCalc);
      AddSkillCast("Bloody Note", BloodyNoteCast, BloodyNoteCalc);
      AddSkillCast("Dissonance of Pain", DissonancePainCast, DissonancePainCalc);
      AddSkillCast("Discreet Chord", DiscreetChordCast, DiscreetChordCalc);
      AddSkillCast("Distorted Anthem", DistortedAnthemCast, DistortedAnthemCalc);
      AddSkillCast("Resonance of the Chord", ResonanceChordCast, ResonanceChordCalc);
      AddSkillCast("Crescendo Finale", CrescendoFinaleCast, CrescendoFinaleCalc);

      AddSkillCast("Provocation", ProvocationCast);
      AddSkillCast("Waiting", WaitingCast);
      AddSkillCast("Last Octave", LastOctaveCast);
      AddSkillCast("Dissolving in the Shadows", DissolvingShadowsCast);
      AddSkillCast("Summon the Shadow", SummonShadowCast);


      //Active Curses
      AddSkillCast("Curse of Destruction", CurseDestructionCast);
      AddSkillCast("Curse of Victim", CurseVictimCast);

      //Special
      AddSkillPassive("Empty");
      AddSkillPassive("Poison", PoisonCalc);
      AddSkillPassive("Corpseless");
      AddSkillPassive("No Attack");

      //Passive
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
      AddSkillPassive("Cursed Hand");
      AddSkillPassive("Pain Silencing");
      AddSkillPassive("Accompaniment");

      //Death
      AddSkillDeath("Sacrificial Chant", SacrificialChantDeath);

      //Echo
      AddSkillPassive("Echo of Hope");
      AddSkillPassive("Echo of Pain");
      AddSkillPassive("Echo of Forest", EchoForestCalc);

      //Passive Curses
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

   //Basic Attack
   private void BasicAttackCast(List<Fighter> targets)
   {
      int dmg = BasicAttackCalc(targets)[0];
      var caster = targets[0];
      targets.RemoveAt(0); //Убираем кастера
      foreach (Fighter target in targets)
      {
         if (!target.isDead)
         {
            target.TakeDmg(dmg, SkillSO.SkillElement.Physical);
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
   private List<int> BasicAttackCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      /*var max_characteristic = Mathf.Max(caster.strengh + caster.bonus_strengh, 
         caster.agility + caster.bonus_agility, 
         caster.wisdow + caster.bonus_wisdow);*/
      int strengh = caster.strengh + caster.bonus_strengh;
      return new List<int> { 1 + strengh };
   }

   //Healing Wounds
   private void HealingWoundsCast(List<Fighter> targets)
   {
      int heal = HealingWoundsCalc(targets)[0];
      var target = targets[1];
      if (!target.isDead)
      {
         if (SaveLoadController.runInfo.globalBuffs.Contains(RunInfo.GlobalBuff.TouchingMystery))
         {
            if (heal >= 0)
            {
               var excess = target.TakeHeal(heal);
               if (excess > 0)
               {
                  var armor = excess / 2;
                  target.armor += (armor == 0) ? 1 : armor;
               }
            }
         }
         else
            target.TakeHeal(heal);
      }
   }
   private List<int> HealingWoundsCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      return new List<int> { caster.wisdow + caster.bonus_wisdow };
   }

   //Waiting
   private void WaitingCast(List<Fighter> targets)
   {
      var caster = targets[0];
      caster.buffs.Add(Fighter.Buff.DoubleNextAttack);
   }

   //Curse of Destraction
   private void CurseDestructionCast(List<Fighter> targets)
   {
      var target = targets[1];
      target.buffs.Add(Fighter.Buff.CurseDestruction);
   }

   //Curse of Victim
   private void CurseVictimCast(List<Fighter> targets)
   {
      var target = targets[1];
      target.buffs.Add(Fighter.Buff.CurseVictim);
   }

   //Raise Shields
   private void RaiseShieldsCast(List<Fighter> targets)
   {
      int armor = RaiseShieldsCalc(targets)[0];
      if (!targets[0].isDead) targets[0].armor += armor;
   }
   private List<int> RaiseShieldsCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int defence = caster.defence;
      if (defence == 0) defence = 1;
      return new List<int> { defence };
   }

   //Fire Wave
   private void FireWaveCast(List<Fighter> targets)
   {
      int dmg = FireWaveCalc(targets)[0];
      targets.RemoveAt(0); //Убираем кастера
      foreach (Fighter target in targets)
      {
         if (!target.isDead) target.TakeDmg(dmg, SkillSO.SkillElement.Fire);
      }
   }
   private List<int> FireWaveCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      var sum = caster.strengh + caster.bonus_strengh + 
         caster.wisdow + caster.bonus_wisdow;
      return new List<int> { sum / 2 };
   }

   //Blade of the Wind
   private void BladeWindCast(List<Fighter> targets)
   {
      var list = BladeWindCalc(targets);
      int dmg = list[0];
      int count = list[1];

      var target = targets[1];
      for (int i = 0; i < count; i++)
      {
         if (!target.isDead) target.TakeDmg(dmg, SkillSO.SkillElement.Wind);
         else
         {
            target = Fight.RandomEnemy(targets[0]);
            if (target.isDead) break;
            i--;
         }
      }
   }
   private List<int> BladeWindCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      var sumWisdow = caster.wisdow + caster.bonus_wisdow;
      var sumAgility = caster.agility + caster.bonus_agility;
      return new List<int> { sumWisdow, 1 + sumAgility / 5};
   }

   //Summon the Shadow
   private void SummonShadowCast(List<Fighter> targets)
   {
      var caster = targets[0];
      var target = Fight.RandomEnemy(caster);
      bool isEnemyCast = Fight.IsEnemy(Fight.PlayerTeam[0], caster);

      if (target.isDead) return;
      Fighter shadow = new("Summons/Shadow")
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

      foreach(var skill in target.skillsBattle)
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
   private void PurpleHazeCast(List<Fighter> targets)
   {
      var list = PurpleHazeCalc(targets);
      int dmg = list[0];

      var caster = targets[0];
      targets.RemoveAt(0); //Убираем кастера
      foreach (Fighter target in targets)
      {
         if (!target.isDead)
         {
            target.TakeDmg(dmg, SkillSO.SkillElement.Dark);

            var pool = GetPoolByName("CurseSkills");
            var listSk = pool.activeSkillList;
            var skill = Instance.GetSkillByName(listSk[Random.Range(0, listSk.Count)].name);
            Fight.additionalCastSkills.Insert(0, new(new List<Fighter> { target }, false, skill));
            //caster.CastSkill(new List<Fighter> { caster, target }, false, skill);
         }
      }
   }
   private List<int> PurpleHazeCalc(List<Fighter> targets)
   {
      return new List<int> { ((int)SaveLoadController.runInfo.currentLocation + 1) * 10 };
   }

   //Surge of Darkness
   private void SurgeDarknessCast(List<Fighter> targets)
   {
      var list = SurgeDarknessCalc(targets);
      int dmg = list[0];
      int chance = list[1];

      var caster = targets[0];
      var target = targets[1];

      if (!target.isDead)
      {
         target.TakeDmg(dmg, SkillSO.SkillElement.Dark);

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
   private List<int> SurgeDarknessCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int wisdow = caster.wisdow + caster.bonus_wisdow;
      int dmg = 1 + wisdow;
      int chance = wisdow * 10;
      if (chance > 100) chance = 100;
      return new List<int> { dmg, chance };
   }

   //Poisoned Thorn
   private void PoisonedThornCast(List<Fighter> targets)
   {
      var list = PoisonedThornCalc(targets);
      int dmg = list[0];
      int poison = list[1];

      var caster = targets[0];
      var target = targets[1];

      if (!target.isDead)
      {
         target.TakeDmg(dmg, SkillSO.SkillElement.Poison);

         if (!target.buffs.Contains(Fighter.Buff.Poison))
            target.buffs.Add(Fighter.Buff.Poison);
         target.poisonStacks += poison;
      }
   }
   private List<int> PoisonedThornCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int wisdow = caster.wisdow + caster.bonus_wisdow;
      int agility = caster.agility + caster.bonus_agility;
      return new List<int> { agility, wisdow };
   }

   //Bloody Note
   private void BloodyNoteCast(List<Fighter> targets)
   {
      var list = BloodyNoteCalc(targets);
      int dmg = list[0];

      var caster = targets[0];
      var target = targets[1];

      if (!target.isDead)
      {
         caster.SacrificeHP(dmg);
         target.TakeDmg(dmg, SkillSO.SkillElement.Physical);
      }
   }
   private List<int> BloodyNoteCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int hp = (int)((caster.max_hp + caster.bonus_hp) * 0.2f);
      if (hp <= 0) hp = 1;
      return new List<int> { hp };
   }

   //Dissonance of Pain
   private void DissonancePainCast(List<Fighter> targets)
   {
      var list = DissonancePainCalc(targets);
      int dmg = list[0];

      var caster = targets[0];
      targets.Remove(caster);
      caster.SacrificeHP(dmg);
      foreach (var target in targets)
      {
         if (!target.isDead)
         {
            target.TakeDmg(dmg, SkillSO.SkillElement.Physical);
            if (Random.Range(0, 100) < dmg)
            {
               var pool = GetPoolByName("CurseSkills");
               var listSk = pool.activeSkillList;
               var skill = Instance.GetSkillByName(listSk[Random.Range(0, listSk.Count)].name);
               Fight.additionalCastSkills.Insert(0, new(new List<Fighter> { target }, false, skill));
            }
         }
      }
   }
   private List<int> DissonancePainCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int hp = (int)(caster.hp * 0.4f);
      if (hp <= 0) hp = 1;
      return new List<int> { hp };
   }

   //Discreet Chord
   private void DiscreetChordCast(List<Fighter> targets)
   {
      var list = DiscreetChordCalc(targets);
      int dmg = list[0];

      var caster = targets[0];
      targets.Remove(caster);
      caster.SacrificeHP(dmg);
      int i = 0;
      while(dmg > 0 && i < 10000)
      {
         var target = targets[Random.Range(0, targets.Count)];
         if (!target.isDead)
         {
            target.armor++;
            dmg--;
         }
         i++;
      }
   }
   private List<int> DiscreetChordCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int hp = (int)((caster.max_hp + caster.bonus_hp) * 0.3f);
      if (hp <= 0) hp = 1;
      return new List<int> { hp };
   }

   //Distorted Anthem
   private void DistortedAnthemCast(List<Fighter> targets)
   {
      var list = DistortedAnthemCalc(targets);
      int dmg = list[0];

      var caster = targets[0];
      targets.Remove(caster);
      caster.SacrificeHP(dmg);
      foreach(var target in targets)
      {
         var skill = Instance.GetSkillByName("Curse of Victim");
         Fight.additionalCastSkills.Insert(0, new(new List<Fighter> { target }, false, skill));
      }
   }
   private List<int> DistortedAnthemCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int hp = (int)((caster.max_hp + caster.bonus_hp) * 0.25f);
      if (hp <= 0) hp = 1;
      return new List<int> { hp };
   }

   //Resonance of the Chord
   private void ResonanceChordCast(List<Fighter> targets)
   {
      var list = ResonanceChordCalc(targets);
      int count = list[0];

      var caster = targets[0];
      targets.Remove(caster);
      var target = targets[Random.Range(0, targets.Count)];
      for (int i = 0; i < count && targets.Count > 0; i++)
      {
         if (!target.isDead)
         {
            switch (Random.Range(0, 3))
            {
               case 0:
                  target.bonus_strengh++;
                  break;
               case 1:
                  target.bonus_agility++;
                  break;
               case 2:
                  target.bonus_wisdow++;
                  break;
            }
            target = targets[Random.Range(0, targets.Count)];
         }
         else
         {
            targets.Remove(target);
            target = targets[Random.Range(0, targets.Count)];
            i--;
         }
      }
   }
   private List<int> ResonanceChordCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int wisdow = caster.wisdow + caster.bonus_wisdow;
      return new List<int> { wisdow + 1 };
   }

   //Crescendo Finale
   private void CrescendoFinaleCast(List<Fighter> targets)
   {
      var list = CrescendoFinaleCalc(targets);
      int count = Random.Range(0, list[0] + 1);

      var caster = targets[0];
      targets.Remove(caster);
      var target = targets[Random.Range(0, targets.Count)];

      List<Fighter> allies = new(Fight.EnemyTeam);
      if (!allies.Contains(caster))
         allies = new(Fight.PlayerTeam);

      for (int i = 0; i < count && targets.Count > 0; i++)
      {
         if (!target.isDead)
         {
            if (allies.Contains(target))
            {
               target.armor += target.TakeHeal(1);
            }
            else
            {
               target.TakeDmg(1, SkillSO.SkillElement.Physical);
            }
            target = targets[Random.Range(0, targets.Count)];
         }
         else
         {
            targets.Remove(target);
            target = targets[Random.Range(0, targets.Count)];
            i--;
         }
      }
   }
   private List<int> CrescendoFinaleCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      int wisdow = caster.wisdow + caster.bonus_wisdow;
      return new List<int> { 1 + wisdow * 5 };
   }

   //Last Octave
   private void LastOctaveCast(List<Fighter> targets)
   {
      var caster = targets[0];
      int hp = caster.hp;

      targets.Remove(caster);
      caster.SacrificeHP(hp);
      foreach (var target in targets)
      {
         target.TakeHeal(hp);
         if (Fight.AlreadyTurn.Contains(target))
         {
            Fight.AlreadyTurn.Remove(target);
            //Кулдауны
            var keys = new List<Skill>(target.cooldowns.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
               Skill skill = keys[i];
               target.cooldowns[skill]--;

               if (target.cooldowns[skill] == 0)
               {
                  target.cooldowns.Remove(skill);
               }
            }
            target.MakeIntention();
         }
      }
   }

   //Provocation
   private void ProvocationCast(List<Fighter> targets)
   {
      var caster = targets[0];
      targets.Remove(caster);
      caster.buffs.Add(Fighter.Buff.ProvocationCaster);
      foreach(var target in targets)
      {
         if (!target.isDead)
         {
            if(!target.buffs.Contains(Fighter.Buff.ProvocationCaster))
               target.buffs.Add(Fighter.Buff.Provocation);
         }
      }   
   }

   //Dissolving in the Shadows
   private void DissolvingShadowsCast(List<Fighter> targets)
   {
      var caster = targets[0];
      caster.buffs.Add(Fighter.Buff.DoubleNextAttack);
      caster.buffs.Add(Fighter.Buff.DissolvingShadows);
   }

   //Call of the Pack
   private void CallPackPassive(Fighter caster, List<Fighter> targets)
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
   private void CallPackReverse(Fighter caster, List<Fighter> targets)
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
   private List<int> OldFightersChestCalc(List<Fighter> targets)
   {
      return new List<int> { (int)SaveLoadController.runInfo.currentLocation + 1 };
   }

   //Bestial Instinct
   private List<int> BestialInstinctCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      var agility = caster.agility + caster.bonus_agility;
      return new List<int> { 25 + Math.Clamp(agility / 2 - 2, 0, 50) };
   }

   //Poison
   private List<int> PoisonCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      return new List<int> { caster.poisonStacks };
   }

   //Echo of Forest
   private List<int> EchoForestCalc(List<Fighter> targets)
   {
      var caster = targets[0];
      var agility = caster.agility + caster.bonus_agility;
      return new List<int> { agility };
   }

   //Heart of the Darkness
   private void HeartDarknessPassive(Fighter caster, List<Fighter> targets)
   {
      int darkBonus = SaveLoadController.runInfo.badKarma / 5;
      caster.bonus_hp += SaveLoadController.runInfo.badKarma;
      caster.hp += SaveLoadController.runInfo.badKarma;
      caster.bonus_strengh+= darkBonus;
      caster.bonus_agility += darkBonus;
      caster.bonus_wisdow += darkBonus;
   }
   private void HeartDarknessReverse(Fighter caster, List<Fighter> targets)
   {
      int darkBonus = SaveLoadController.runInfo.badKarma / 3;
      caster.bonus_hp -= darkBonus * 4;
      if (caster.hp > caster.max_hp + caster.bonus_hp) caster.hp = caster.max_hp + caster.bonus_hp;
      caster.bonus_strengh -= darkBonus;
      caster.bonus_agility -= darkBonus;
      caster.bonus_wisdow -= darkBonus;
   }

   //Sacrificial Chant
   private void SacrificialChantDeath(List<Fighter> targets)
   {
      var caster = targets[0];
      targets = new(Fight.EnemyTeam);
      if (!Fight.EnemyTeam.Contains(caster))
         targets = new(Fight.PlayerTeam);
      targets.Remove(caster);
      foreach(var target in targets)
      {
         if (!target.isDead)
         {
            int heal = (caster.max_hp + caster.bonus_hp) / 2;
            if (heal <= 0) heal = 1;

            var excess = target.TakeHeal(heal);
            if (excess > 0)
            {
               var armor = excess / 2;
               target.armor += (armor == 0) ? 1 : armor;
            }
         }
      }
   }
}
