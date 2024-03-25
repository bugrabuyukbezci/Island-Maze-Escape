using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public List<sounds> SFX;
    public List<sounds> Musics;
    public bool isOpenSFX = true;
    public bool isOpenMusic = true;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject); // bu obje bütün sahneleri silinmede dolaþsýn
    }   
    void Start()
    {
        for(int i = 0; i < SFX.Count; i++)
        {
            GameObject newSound = new GameObject(SFX[i].name);
            newSound.transform.SetParent(this.transform);
            AudioSource newSource = newSound.AddComponent<AudioSource>();
            SFX[i].thisSource = newSource;
            newSource.volume = SFX[i].volume;
            newSource.pitch = SFX[i].pitch;
            newSource.clip = SFX[i].clip;
            newSource.loop = SFX[i].isLoop;
            newSource.playOnAwake = SFX[i].playOnAwake;
        }
        for (int i = 0; i < Musics.Count; i++)
        {
            GameObject newSound = new GameObject(Musics[i].name);
            newSound.transform.SetParent(this.transform);
            AudioSource newSource = newSound.AddComponent<AudioSource>();
            Musics[i].thisSource = newSource;
            newSource.volume = Musics[i].volume;
            newSource.pitch = Musics[i].pitch;
            newSource.clip = Musics[i].clip;
            newSource.loop = Musics[i].isLoop;
            newSource.playOnAwake = Musics[i].playOnAwake;
        }
    } 
    public void playClip(string name)
    {
        for(int i = 0; i < SFX.Count; i++)
        {
            if(SFX[i].name == name)
            {
                if (isOpenSFX)
                    SFX[i].thisSource.Play();
                break;
            }
        }
    }
    public void playMusic(string name)
    {
        for (int i = 0; i < Musics.Count; i++)
        {
            if (Musics[i].name == name)
            {
                if (isOpenMusic)
                    Musics[i].thisSource.Play();
                break;
            }
        }
    }
    [System.Serializable] 
    public class sounds
    {
        public string name;
        public AudioClip clip;
        public float volume = 1;
        public float pitch;
      [HideInInspector]  public AudioSource thisSource; 
        public bool isLoop;
        public bool playOnAwake;
    }
    
}
