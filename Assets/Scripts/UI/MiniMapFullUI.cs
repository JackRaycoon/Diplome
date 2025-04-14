using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFullUI : MonoBehaviour
{
   public GameObject fullMapAnalogue;

   [NonSerialized]public Room room = null;
   [NonSerialized]public Corridor corridor = null;
}
