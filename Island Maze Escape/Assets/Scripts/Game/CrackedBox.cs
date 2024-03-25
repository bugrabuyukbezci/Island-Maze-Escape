using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBox : MonoBehaviour
{
    public List<Rigidbody> allParts;
    bool isCracked;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // kutuya çarpan þey oyuncu mu
        {
            if (!isCracked)
            {                
                isCracked = true;

                Transform crackParticle = Instantiate(GameManager.instance.crackedPrefab);
                crackParticle.position = transform.position;
                Destroy(crackParticle.gameObject, 2f);
                for (int i = 0; i < allParts.Count; i++)
                {
                    allParts[i].isKinematic = false;
                    allParts[i].AddExplosionForce(200, allParts[i].position, 100); // patlama için force 
                }
            }
        }
    }
}
