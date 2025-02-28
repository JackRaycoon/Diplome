using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameSettings : MonoBehaviour
{
   private void Awake()
   {
      Application.targetFrameRate = 45;
      QualitySettings.vSyncCount = 0;
   }
   void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
