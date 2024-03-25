using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // aray�z k�t�phanesi
using DG.Tweening;// dotween kullanmak i�in k�t�phane dahil ediyoruz.

public class menuController : MonoBehaviour
{ 
    public Transform playButton;
    public Transform logo;
    public Transform buttonGroup;
    public Transform levelSelectPanel;
    public levelSelectBT levelBtPrefab;
    public Transform content;
    public ScrollRect levelScroll;
    public Text moneytxt;
    int money;
    public List<levelSelectBT> allSelectBt = new List<levelSelectBT>();
    public CanvasGroup bgImg;
    public CanvasGroup characterPanel;
    public List<CharacterClass> characterList;
    public Transform startPoint; // karakterin ba�lang�� poziyonu
    public Transform centerPoint;// karakterin orta poziyonu
    public Transform exitPoint; // karakterin ��k�� poziyonu
    int selectCharID;
    public Button marketLeftBt;
    public Button marketRightBt;
    public Text playerNameTxt; // player isminin yazaca�� text
    public GameObject buyBtObj; // sat�n alma  butonunun objesi
    public GameObject selectBtObj; // se�im butonun objesi
    Text buyTxt; // oyuncunun fiyat� yazacak
    int selectedPlayerID; 
    #region info panel
    public Text levelTxt;
    public Image lvlFillAmount;
    public Text healthTxt;
    public Image healthFillAmount;
    public Text speedTxt;
    public Image speedFillAmount;
    int maxLevel = 5;
    int maxSpeed = 20;
    int maxHealth = 200;
    #endregion
    public GameObject upgradeButton;
    
    void Start()
    {
        Time.timeScale = 1;
        playersInfo.instance.lastCheckID = -1;
        SoundManager.instance.playMusic("mainMusic");
        
#if TestMode
        Debug.Log("TEst mode aktif");
#else
        Debug.Log("TEst mode pasif");
#endif
        selectedPlayerID = PlayerPrefs.GetInt("SelectedPlayer");      
        buyBtObj.GetComponent<Button>().onClick.AddListener(buyButtonOnClick); 
        buyTxt = buyBtObj.transform.GetChild(0).GetComponent<Text>(); // sat�n alma butonunn 0. �ocu�unun text componenti
        marketLeftBt.interactable = false;
        characterPanel.gameObject.SetActive(false);
        characterPanel.alpha = 0;
        if (!PlayerPrefs.HasKey("totalMoney"))
            PlayerPrefs.SetInt("totalMoney", 10000);
        money = PlayerPrefs.GetInt("totalMoney");
        moneytxt.text = money.ToString() + "$";
        if (PlayerPrefs.HasKey("LevelLock0") == false) // haf�zada bu parametre var m�?
        {
            PlayerPrefs.SetInt("LevelLock0", 1); // parametreye 1 setle. Kilit a�ma i�lemi
        }
        // level butonlar� olu�tur
        bool isBuyBt=false;
        for (int i = 0; i < 10; i++)
        {
            levelSelectBT btClone = Instantiate(levelBtPrefab); 
            btClone.transform.SetParent(content); 
            btClone.levelID.text = (i + 1).ToString(); 
            int levelID = i;
            allSelectBt.Add(btClone);
           
            if(PlayerPrefs.GetInt("LevelLock" + levelID) == 0) // seviye kilitli
            {
                if (!isBuyBt)
                {
                    isBuyBt = true;
                    btClone.buyBt.gameObject.SetActive(true); // sat�n alma butonu kilitli seviyelerde g�r�necek
                    btClone.buyBt.onClick.AddListener(() => {
                        Debug.Log(levelID + ". Buy Butonuna bas�ld�.!");
                        // sat�n alma i�lemi control
                        if(money >= 1000) // haf�zadaki para 1000'den b�y�k veya e�it mi
                        {
                            // sat�n alma i�lemi ger�ekle�ecek
                            money -= 1000; //money = money - 1000; ayn� i�lem
                            PlayerPrefs.SetInt("totalMoney", money);
                            moneytxt.text = money.ToString() + "$";
                            moneytxt.transform.DOScale(new Vector3(1.1f, 1.1f), .5f).OnComplete(()=> {
                                moneytxt.transform.DOScale(new Vector3(1f, 1f), .5f).SetDelay(.2f);
                            });
                            // butonu aktifle�tirme i�lemi
                            allSelectBt[levelID].GetComponent<CanvasGroup>().alpha = 1f;
                            allSelectBt[levelID].buyBt.gameObject.SetActive(false);
                            allSelectBt[levelID].GetComponent<Button>().interactable = true;
                            PlayerPrefs.SetInt("LevelLock" + levelID, 1); 
                            // butonu aktifle�tirme i�lemi

                            // buton kayd�rma i�lemi
                            if(levelID < 9)
                            {
                                allSelectBt[levelID + 1].buyBt.gameObject.SetActive(true);
                                allSelectBt[levelID + 1].buyBt.onClick.AddListener(() => {
                                    if (money >= 1000) 
                                    {
                                        // sat�n alma i�lemi ger�ekle�ecek
                                        money -= 1000; 
                                        PlayerPrefs.SetInt("totalMoney", money);
                                        moneytxt.text = money.ToString() + "$";
                                        moneytxt.transform.DOScale(new Vector3(1.1f, 1.1f), .5f).OnComplete(() => {
                                            moneytxt.transform.DOScale(new Vector3(1f, 1f), .5f).SetDelay(.2f);
                                        });                                        
                                        allSelectBt[levelID+1].GetComponent<CanvasGroup>().alpha = 1f;
                                        allSelectBt[levelID+1].buyBt.gameObject.SetActive(false);
                                        allSelectBt[levelID+1].GetComponent<Button>().interactable = true;
                                        PlayerPrefs.SetInt("LevelLock" + (levelID+1), 1);
                                    }
                                });
                            }                            
                        }
                        else
                        {
                            // yetersiz para i�in kullan�c�ya bildirim ver.
                        }
                    });
                }
                else
                {
                    btClone.buyBt.gameObject.SetActive(false);
                }
                btClone.GetComponent<CanvasGroup>().alpha = 0.5f; 
                btClone.GetComponent<Button>().interactable = false;// butonun t�klanabilirlik �zelli�i kapal�
            }
            else // kilitli de�il
            {
                btClone.buyBt.gameObject.SetActive(false); 
                btClone.GetComponent<CanvasGroup>().alpha = 1f;
                btClone.GetComponent<Button>().interactable = true;
            }

            btClone.GetComponent<Button>().onClick.AddListener(() => {
                playersInfo.instance.levelID = levelID;
                UnityEngine.SceneManagement.SceneManager.LoadScene("game");
                Debug.Log((levelID + 1) + ". B�l�me t�kland�.!");
                upgradeControl();
                setPlayerInfos();
                SoundManager.instance.playClip("click");
            });
        }

        levelScroll.horizontalNormalizedPosition = 0f; 

        levelSelectPanel.gameObject.SetActive(false);
        levelSelectPanel.localScale = Vector3.zero;

        playButton.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo);

        logo.DOLocalMoveY(299f, 1f).OnComplete(()=> {
            logo.DORotate(new Vector3(0, 0, 4), 1f).SetLoops(-1, LoopType.Yoyo);
        }).SetDelay(.5f);

        buttonGroup.DOLocalMoveY(-132.7881f, 1f).SetEase(Ease.OutBounce).SetDelay(.5f);

        selectCharID = PlayerPrefs.GetInt("SelectedPlayer"); 
        for(int i = 0; i < characterList.Count; i++)
        {
            if(i< selectCharID)
            {
                characterList[i].player.position = new Vector3(exitPoint.position.x,
                characterList[i].player.position.y,
                characterList[i].player.position.z);
            }
            else if(i == selectCharID)
            {
                characterList[i].player.position = new Vector3(centerPoint.position.x,
                characterList[i].player.position.y,
                characterList[i].player.position.z);
            }
            else
            {
                characterList[i].player.position = new Vector3(startPoint.position.x,
                characterList[i].player.position.y,
                characterList[i].player.position.z);
            }
        }
        if(selectCharID == 0) 
        {
            marketLeftBt.interactable = false;
            marketRightBt.interactable = true;
        }else if(selectCharID == characterList.Count - 1) 
        {
            marketRightBt.interactable = false;
            marketLeftBt.interactable = true;
        }
        else 
        {
            marketRightBt.interactable = true;
            marketLeftBt.interactable = true;
        }
        CharacterControl();
        // upgarade i�lemleri
        for(int i = 0; i < characterList.Count; i++)
        {
            characterList[i].firstHealth = characterList[i].health;
            characterList[i].firstSpeed = characterList[i].speed;
            characterList[i].Level = PlayerPrefs.GetInt("Player" + i + "Level");
        }        
    }    

    public void upgradeOnClick()
    {
        int veriableLevel = PlayerPrefs.GetInt("Player" + selectCharID + "Level"); // se�ili karakterin haf�zaki leveli
        if(veriableLevel < characterList[selectCharID].maxLevel-1)
        {
            if(money >= 1000) 
            {
                money -= 1000; 
                PlayerPrefs.SetInt("totalMoney", money);            
                moneytxt.text = money.ToString() + "$"; 
                moneytxt.transform.DOScale(new Vector3(1.1f, 1.1f), .5f).OnComplete(() => {
                    moneytxt.transform.DOScale(new Vector3(1f, 1f), .5f).SetDelay(.2f);
                });
                veriableLevel++;
                PlayerPrefs.SetInt("Player" + selectCharID + "Level", veriableLevel); 
                characterList[selectCharID].Level = veriableLevel;
                upgradeControl();
            }
            else
            {
                Debug.Log("Yetersizpara");
            }
        }

    }
    void upgradeControl()
    {
        int veriableLevel = PlayerPrefs.GetInt("Player" + selectCharID + "Level");        

        characterList[selectCharID].health = characterList[selectCharID].firstHealth + (10 * veriableLevel); 
        characterList[selectCharID].speed = characterList[selectCharID].firstSpeed + (.5f * veriableLevel); 

        lvlFillAmount.DOFillAmount(((float)characterList[selectCharID].Level+1) / (float)maxLevel, 1f);
        healthFillAmount.DOFillAmount((float)characterList[selectCharID].health / (float)maxHealth, 1f);
        speedFillAmount.DOFillAmount((float)characterList[selectCharID].speed / (float)maxSpeed, 1f);

        levelTxt.text = "LEVEL " + (characterList[selectCharID].Level+1) + "/" + maxLevel;
        healthTxt.text = "HEALTH " + characterList[selectCharID].health + "/" + maxHealth;
        speedTxt.text = "SPEED " + characterList[selectCharID].speed + "/" + maxSpeed;

        if(veriableLevel == characterList[selectCharID].maxLevel - 1 || characterList[selectCharID].isLocked) 
        {
            upgradeButton.SetActive(false);
        }
        else
        {
            upgradeButton.SetActive(true);
        }

    }
    void CharacterControl()
    {
        if (!PlayerPrefs.HasKey("Character0")) 
        {
            PlayerPrefs.SetInt("Character0", 1); // 0. karakterin kilidini a� ve haf�zaya kaydet.
        }
        for(int i = 0; i < characterList.Count; i++) 
        {
            if (PlayerPrefs.GetInt("Character" + i) == 0) 
            {
                characterList[i].isLocked = true;
            }
            else
            {
                characterList[i].isLocked = false;
            }
            characterList[i].player.position = new Vector3(startPoint.position.x, 
                characterList[i].player.position.y, 
                characterList[i].player.position.z);
        }
    }
    public void marketButtonClick()
    {
        bgImg.DOFade(0f, .5f).OnComplete(()=> {
            bgImg.gameObject.SetActive(false);
            characterPanel.gameObject.SetActive(true);
            characterPanel.DOFade(1f, .5f);
            for (int i = 0; i < characterList.Count; i++)
            {
                if (i < selectCharID)
                {
                    characterList[i].player.DOMoveX(exitPoint.position.x, 1f);
                }
                else if (i == selectCharID)
                {
                    characterList[i].player.DOMoveX(centerPoint.position.x, 1f);
                }
                else
                {
                    characterList[i].player.DOMoveX(startPoint.position.x, 1f);
                }
            }            
        });
        getPlayerInfo();
    }
    public void charLeftOnclick()
    {
        marketRightBt.interactable = true;

        characterList[selectCharID].player.DOMoveX(startPoint.position.x, 1f); // id'yi de�i�tirmeden karakteri sa�a �ek

        if (selectCharID > 0)
        {
            selectCharID--; 
        }
        characterList[selectCharID].player.DOMoveX(centerPoint.position.x, 1f); // id'yi de�i�tirdikten sonra �imdiki karakteri ortaya �ek
        if (selectCharID == 0)
        {
            marketLeftBt.interactable = false;
        }
        Debug.Log("Select Char ID : " + selectCharID);
        getPlayerInfo();
    }
    public void charRightOnclick()
    {
        marketLeftBt.interactable = true;

        characterList[selectCharID].player.DOMoveX(exitPoint.position.x, 1f); // id'yi de�i�tirmeden karakteri sola �ek

        if(selectCharID < characterList.Count - 1)
        {
            selectCharID++; // id'yi artt�r
        }

        characterList[selectCharID].player.DOMoveX(centerPoint.position.x, 1f); // artt�r�lan id'deki karakteri ortaya �ek
        if (selectCharID == characterList.Count - 1)
            marketRightBt.interactable = false;
        Debug.Log("Select Char ID : " + selectCharID);
        getPlayerInfo();
    }
    void getPlayerInfo()
    {
        playerNameTxt.text = characterList[selectCharID].name;
        if (characterList[selectCharID].isLocked) // oyuncu kilitli mi?
        {
            buyBtObj.SetActive(true);
            buyTxt.text = "BUY - $" + characterList[selectCharID].price; // se�ili karakterin fiyat�n� yazd�r
            selectBtObj.SetActive(false);
        }
        else
        {
            buyBtObj.SetActive(false);
            if(selectCharID == selectedPlayerID)            
                selectBtObj.SetActive(false);            
            else            
                selectBtObj.SetActive(true); 
        }        
        upgradeControl();

    }
    public void buyButtonOnClick()
    {
        if(money >= characterList[selectCharID].price) // param sat�n almaya yetiyorsa
        {
            buyBtObj.SetActive(false);
            selectedPlayerID = selectCharID;
            PlayerPrefs.SetInt("SelectedPlayer", selectedPlayerID); // se�ili oyuncuuyu haf�zaya yaz
            money -= characterList[selectCharID].price; // paray� azalt
            PlayerPrefs.SetInt("totalMoney", money);    // azalan paray� haf�zaya kaydet        
            moneytxt.text = money.ToString() + "$"; // paray� ekrana yaz
            moneytxt.transform.DOScale(new Vector3(1.1f, 1.1f), .5f).OnComplete(() => {
                moneytxt.transform.DOScale(new Vector3(1f, 1f), .5f).SetDelay(.2f);
            });
            characterList[selectCharID].isLocked = false;
            PlayerPrefs.SetInt("Character" + selectCharID, 1);// karakterin kilidini a�mak i�in 1 olarak kaydediyoruz.
            upgradeButton.SetActive(true); // upgrade butonunu aktifle�tir
        }
        else
        {
            //TODO : e�er para yetersiz ise para k�rm�z� yan�p s�ns�n ve yine b�y�me ve k���lme yaps�n.
            Debug.Log("PARA YOK!");
        }
    }
    public void selectButtonOnClick()
    {
        PlayerPrefs.SetInt("SelectedPlayer", selectCharID); // se�ili oyuncuuyu haf�zaya yaz
        selectedPlayerID = selectCharID;
        selectBtObj.gameObject.SetActive(false);
        setPlayerInfos();
    }
    void setPlayerInfos()
    {
        playersInfo.instance.selectedPlayer.name = characterList[selectCharID].name;
        playersInfo.instance.selectedPlayer.id = characterList[selectCharID].id;
        playersInfo.instance.selectedPlayer.speed = characterList[selectCharID].speed;
        playersInfo.instance.selectedPlayer.health = characterList[selectCharID].health;
        playersInfo.instance.selectedPlayer.level = characterList[selectCharID].Level;
    }
    public void marketBackButtonClick()
    {
        characterPanel.DOFade(0f,.5f).OnComplete(()=> {
            characterPanel.gameObject.SetActive(false);
            bgImg.gameObject.SetActive(true);
            bgImg.DOFade(1f, .5f);
        });
    }
    public void playButtonClick()
    {
        levelSelectPanel.gameObject.SetActive(true);
        levelSelectPanel.DOScale(Vector3.one, .7f).SetEase(Ease.OutBounce).SetUpdate(true);
        // setupdate timescale 0 ikende �al���r.
    }
    public void selectPanelBack()
    {
        levelSelectPanel.DOScale(Vector3.zero,.7f).SetEase(Ease.OutBounce).OnComplete(()=> {
            levelSelectPanel.gameObject.SetActive(false);
        });
    }
    bool moneyWait;
    void Update()
    {
        if(Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.K) && !moneyWait)
        {
            money += 5000;
            PlayerPrefs.SetInt("totalMoney", money);
            moneyWait = true;
            moneytxt.text = money.ToString() + "$";
            DOVirtual.DelayedCall(3f, () => { // 3 sanyie sonra �al��maya yarayan dotween �zelli�i
                moneyWait = false; 
            });
        }
    }
    [System.Serializable] 
    public class CharacterClass
    {
        public string name; // ismi
        public int id; // id'si       
        public int price; // fiyat�
        public float speed; // h�z�
        public float health; // sa�l�k
        public int Level = 1; // default 1. levelden ba�las�n.
        public int maxLevel; // seviyenin ula�abilece�i maksimum de�er
        public bool isLocked; // karakter kilitli mi de�il mi?
        public Transform player; // oyuncunun transformu
        public float firstHealth;
        public float firstSpeed;
    }
}
