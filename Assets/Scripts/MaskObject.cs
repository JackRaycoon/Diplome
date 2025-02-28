using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskObject : MonoBehaviour
{
   public GameObject[] maskObj;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in maskObj)
      {
         go.GetComponent<SkinnedMeshRenderer>().material.renderQueue = 3002;
      }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
