using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerControl : MonoBehaviour
{
    public enum bonusType { banana,Cherry,watermelon,hamburger,cheese,spike,Finish};
    public bonusType types;
   
    void Update()
    {
        if (types != bonusType.spike)
            transform.Rotate(new Vector3(0, 2, 0));
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            switch (types)
            {
                case bonusType.banana:
                    GameManager.instance.healthUpdate(-20);
                    Destroy(gameObject);
                    break;
                case bonusType.cheese:
                    GameManager.instance.healthUpdate(10);
                    Destroy(gameObject);
                    break;
                case bonusType.Cherry:
                    GameManager.instance.healthUpdate(-10);
                    Destroy(gameObject);
                    break;
                case bonusType.hamburger:
                    GameManager.instance.healthUpdate(-5);
                    Destroy(gameObject);
                    break;
                case bonusType.watermelon:
                    GameManager.instance.healthUpdate(-10);
                    Destroy(gameObject);
                    break;
                case bonusType.spike:
                    GameManager.instance.healthUpdate(10);
                    Debug.Log("ÇAlýþtýýý");
                   // Destroy(gameObject);
                    break;
                case bonusType.Finish:
                    GameManager.instance.finishTrigger();
                    this.GetComponent<MeshRenderer>().enabled = false;
                    break;
            }
           
        }
    }
   
}
