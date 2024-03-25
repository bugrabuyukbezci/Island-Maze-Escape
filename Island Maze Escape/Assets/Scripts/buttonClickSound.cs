using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonClickSound : MonoBehaviour
{    
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(() => {
            SoundManager.instance.playClip("click");
        });
    }
}
