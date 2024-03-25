using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkPointControl : MonoBehaviour
{
    public int checkID;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(playersInfo.instance.lastCheckID < checkID)
            {
                Debug.Log("Check ID : " + checkID);
                playersInfo.instance.lastCheckID = checkID;
                SoundManager.instance.playClip("checkPoint");
            }
        }
    }
}
