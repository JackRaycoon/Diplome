using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomState
{
   //public Vector2Int position;
   //public Enemy enemy; //��� ���� � �������? ���� ����� ���, �� null
   public bool[] doorOpened = {false, false, false, false}; //������� �� ���������� �����
   public short eventCount = 0; //���������� ����������� � ������� �������, �� 1 ������� �� ����� 2-� �������
   //public List<ItemData> itemsInRoom;

   public RoomState(bool[] doorOpenedMas)
   {
      doorOpened = doorOpenedMas;
   }
}