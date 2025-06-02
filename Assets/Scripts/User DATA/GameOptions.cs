using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class GameOptions
{
   [DataMember]
   public short bgmVolume = 100, sfxVolume = 100;
}
