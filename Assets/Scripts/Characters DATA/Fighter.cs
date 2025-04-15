using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
   public int armor; //дополнительное здоровье, которое может превышать максимум
   public int armor_current; //дополнительное здоровье, которое может превышать максимум
   public Skill Intension = null; //Намерение использовать скилл
   public Skill prevIntension = null;

   public bool isDead = false;

   //Характеристики
   public int strengh, agility, wisdow, constitution;
   public int bonus_strengh, bonus_agility, bonus_wisdow; //боевые изменения хар-тик

   public bool isSpawn;

   //Умения персонажа
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
      armor = Data.armor;
      armor_current = armor;

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
         skills.Add(skill);
   }
   public void AddSkill(Skill skill)
   {
      if (!skills.Contains(skill))
         skills.Add(skill);
   }
   public void AddSkill(string skillName)
   {
      var skill = SkillDB.Instance.GetSkillByName(skillName);
      if (!skills.Contains(skill))
         skills.Add(skill);
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

   public void TakeDmg(int dmg)
   {
      if(armor_current > 0)
      { 
         var armor_cur = armor_current;
         armor_current -= dmg;
         dmg -= armor_cur;
         if (armor_current < 0) armor_current = 0;
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


}
