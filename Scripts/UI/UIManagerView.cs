using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEditor;

public class UIManagerView : MonoBehaviour
{
    public GameSetting gameSetting;
    [Header("步數TMP")]
    public TextMeshProUGUI MoveAmountText;
    [Header("分數TMP")]
    public TextMeshProUGUI ScoreText;
    [Header("分數Slider")]
    public Slider ScoreSlider;
    [Header("目標水果1任務數量")]
    public TextMeshProUGUI goalCount1;
    [Header("目標水果1圖片")]
    public Image goal1image;
    [Header("目標水果2任務數量")]
    public TextMeshProUGUI goalCount2;
    [Header("目標水果2圖片")]
    public Image goal2image;
    [Header("道具炸彈")]
    public TextMeshProUGUI itemBomb;
    [Header("道具藥水")]
    public TextMeshProUGUI itemPotion;
    [Header("道具閃電")]
    public TextMeshProUGUI itemLighting;

    //結束面板
    [Header("結束面板")]
    public GameObject endGamePanel;
    [Header("得分星星")]
    public Image[] stars;
    [Header("得分")]
    public TextMeshProUGUI endGameScore;
    [Header("結束標題")]
    public TextMeshProUGUI endGameTitle;

    //暫停面板
    [Header("暫停面板")]
    public GameObject pausePanel;
    //[Header("背景音樂按鈕按下")]
    //public ReactiveCommand OnBgmButtonClick = new ReactiveCommand();
    //[Header("音效按鈕按下")]
    //public ReactiveCommand OnSoundeButtonClick = new ReactiveCommand();

    [Header("背景音樂按鈕")]
    public Toggle bgmButton;
    [Header("音效按鈕")]
    public Toggle soundButton;

    //技能按鈕
    public ReactiveCommand ExplodeButtonClick = new ReactiveCommand();
    public void OnExplodeButtonClick()
    {
        ExplodeButtonClick.Execute();
    }


}
