using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playersInfo : MonoBehaviour
{
    public static playersInfo instance;
    public selectedPlayerInfo selectedPlayer; // se�ilen oyuncu bilgileri men�den gelecek
    public List<Transform> playerPrefabs; // oyuncu prefablar� oyun ekran�nda �a�r�lacak
    public int levelID;
    public int lastCheckID = -1;   
    private void Awake()
    {
        instance = this; 
        DontDestroyOnLoad(this.gameObject); // sahneler aras� ge�i�te bu obje silinmesin
        SceneManager.LoadScene("menu");
    }        
    [System.Serializable]
    public class selectedPlayerInfo
    {
        public string name;
        public int id;
        public int level;
        public float speed;
        public float health; 
    }
}
