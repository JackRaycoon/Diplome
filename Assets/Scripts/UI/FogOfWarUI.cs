using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarUI : MonoBehaviour
{
   public bool isFogOfWar; //������������ ����� ��������� �� �������� ������, �� ����������� ����������
   public bool isUnlocked; //����������� ����� ������ ��������������� �� ���� ������ (��� ���)

   public Room room = null;
   public Corridor corridor = null;

   public GameObject fullMapAnalogue;
}
