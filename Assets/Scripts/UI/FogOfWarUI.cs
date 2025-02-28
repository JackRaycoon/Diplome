using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarUI : MonoBehaviour
{
   public bool isFogOfWar; //Рассеивается когда проходишь на соседней клетке, не обязательно соединённой
   public bool isUnlocked; //Открывается когда прошёл непосредственно по этой клетке (был там)

   public Room room = null;
   public Corridor corridor = null;

   public GameObject fullMapAnalogue;
}
