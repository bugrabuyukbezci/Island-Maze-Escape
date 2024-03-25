using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }
    public List<level> levelList;
    public CinemachineVirtualCamera playerCam;
    public float gameTimer;
    public Image timerFillAmount;
    level selectedLevel;
    public Text timerTxt;
    public bool timerStart;
    public bool gameStart; 
    float playerHealth; 
    float playerHealthFirst;
    public Image healthBar;
    public Text healthTxt;
    bool timerHealthStart;
    float timerHealth=10; 
    public Image damagePanel;
    public Transform crackedPrefab;
    PlayerMovemet selectedPlayer;
    bool isGameOver;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    bool isPaused;
    public GameObject miniMapCam;
    public GameObject winPanel;    
    IEnumerator Start()
    {
        Time.timeScale = 1;
        damagePanel.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        winPanel.GetComponent<CanvasGroup>().alpha = 0;
        gameOverPanel.GetComponent<CanvasGroup>().alpha = 0;
        pausePanel.GetComponent<CanvasGroup>().alpha = 0;
        yield return new WaitForEndOfFrame(); 
        selectedLevel = Instantiate(levelList[playersInfo.instance.levelID]);
        selectedLevel.transform.position = Vector3.zero;
        selectedPlayer = Instantiate
            (playersInfo.instance.playerPrefabs
            [playersInfo.instance.selectedPlayer.id]
            .GetComponent<PlayerMovemet>());
        miniMapCam.transform.SetParent(selectedPlayer.transform);
        miniMapCam.transform.localPosition = new Vector3(0, 20, 0);
        selectedPlayer.speed = playersInfo.instance.selectedPlayer.speed;
        if (playersInfo.instance.lastCheckID == -1)
            selectedPlayer.transform.position = selectedLevel.spawnPoint.position; 
        else
        {
            for(int i = 0; i < selectedLevel.checkPoints.Count; i++)
            {
                if(selectedLevel.checkPoints[i].checkID == playersInfo.instance.lastCheckID)
                {
                    selectedPlayer.transform.position = selectedLevel.checkPoints[i].transform.position;
                    break; 
                }
            }
        }
        playerCam.Follow = selectedPlayer.transform; 
        selectedPlayer.playerNameTxt.text = "Lv." + (playersInfo.instance.selectedPlayer.level+1)
            + " " + playersInfo.instance.selectedPlayer.name;        
        playerHealth = playersInfo.instance.selectedPlayer.health;
        playerHealthFirst = playerHealth;
        healthUpdate(0);// baþlangýçta hasar almayacak
        gameTimer = selectedLevel.levelTime;
        timerFillAmount.fillAmount = gameTimer / selectedLevel.levelTime;
        timerTxt.text = (int)gameTimer + "s"; 
        yield return new WaitForSeconds(3f); 
        gameStart = true;
        timerStart = true;
    }
    public void healthUpdate(int damage) // saðlýk güncelleme
    {
        if (isGameOver)
            return;

        if(playerHealth - damage > 0)
        {
            if (damage > 0)
            {
                SoundManager.instance.playClip("damage");
                playerHealth -= damage;
            }
            else
            {
                if (playerHealth - damage <= playerHealthFirst)
                {
                    playerHealth -= damage;
                }
                else
                {
                    playerHealth = playerHealthFirst;
                }
            }
            healthBar.DOFillAmount(playerHealth / playerHealthFirst, 1f);
            healthTxt.text = playerHealth + "/" + playerHealthFirst; 
            if (damage > 0) 
            {
                damagePanel.gameObject.SetActive(true);
                damagePanel.color = new Color(1, 1, 1, 0f);
                damagePanel.DOFade(.7f, .2f).OnComplete(() =>
                {
                    damagePanel.DOFade(0f, 0.2f).SetDelay(0.15f).OnComplete(() =>
                    {
                        damagePanel.gameObject.SetActive(false);
                    });
                });
            }
            // hasar alma iþlemleri
        }
        else
        {
            selectedPlayer.animator.Play("die", -1, 0);
            isGameOver = true;
            gameStart = false;
            timerHealthStart = false;
            playerHealth = 0;
            healthBar.DOFillAmount(playerHealth / playerHealthFirst, 1f);
            healthTxt.text = playerHealth + "/" + playerHealthFirst; 
            DOVirtual.DelayedCall(2f,()=> {
                gameOverPanel.GetComponent<CanvasGroup>().DOFade(1f, .5f);
                gameOverPanel.SetActive(true);
            });
        }
    }

    public void nextButton()
    {
        if (playersInfo.instance.levelID <= 8)
            playersInfo.instance.levelID++; 
        restartOnClick();
    }
    public void finishTrigger()
    {
        if (isGameOver) 
            return;
        isGameOver = true;
        gameStart = false;
        timerHealthStart = false;
        timerStart = false;
        playersInfo.instance.lastCheckID = -1; 
        PlayerPrefs.SetInt("LevelLock" + (playersInfo.instance.levelID + 1), 1); 
        
        selectedPlayer.animator.Play("Cheering", -1, 0);
        DOVirtual.DelayedCall(2f, () => {
            winPanel.GetComponent<CanvasGroup>().DOFade(1f, .5f);
            winPanel.SetActive(true);
        });
    }
    public void pauseButtonOnClick()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
            pausePanel.GetComponent<CanvasGroup>().DOFade(1f, .5f).SetUpdate(true);
        }
        else
        {
            pausePanel.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).SetUpdate(true).OnComplete(() => {
                Time.timeScale = 1;
                pausePanel.SetActive(false);

            });
          
        }
    }    
    void Update()
    {
        if (timerStart)
        {
            if (gameTimer > 0)
            {
                gameTimer -= Time.deltaTime;
                timerTxt.text = (int)gameTimer + "s"; 
                timerFillAmount.fillAmount = gameTimer / selectedLevel.levelTime;
            }
            else
            {
                // zamanýn bittiði yer. :/
                timerStart = false;
                timerHealthStart = true;
                healthUpdate(10);
                Debug.Log("<color=green><b>Oyuncunun canýný azalt!</b></color>");
            }           
        }
        if (timerHealthStart)
        {
            if (timerHealth > 0) 
            {
                timerHealth -= Time.deltaTime;
            }
            else
            {
                timerHealth = 10;
                healthUpdate(10);
            }
        }
    }
    public void restartOnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("game");
    }
    public void menuOnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
    }
}
