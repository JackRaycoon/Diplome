using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class MiniMapFullUI : MonoBehaviour
{
   public GameObject fullMapAnalogue;

   [IgnoreDataMember] public Room room = null;
   [IgnoreDataMember] public Corridor corridor = null;
}
