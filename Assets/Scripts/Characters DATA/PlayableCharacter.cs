using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : Fighter
{
   public byte currentPhase = 1; //1 - �������, 2 - �����������, 3 - ������

   public PlayableCharacter(string name) : base(name)
   {
      //��������, ����� ��� �� ��������� �� ������� ���� �����, � ��� ������ 1-� ����� � + ����� �������� ���������
      foreach(SkillSO skillSO in Data.skills)
      {
         skills.Add(SkillDB.Instance.GetSkillByName(skillSO.name));
         if (skills.Count == 5) break;
      }
      //
   }

   public new Sprite Portrait
   {
      get
      {
         return currentPhase switch
         {
            1 => Data.portrait_human,
            2 => Data.portrait_halfhuman,
            3 => Data.portrait_monster,
            _ => null,
         };
      }
      private set { }
   }
}
