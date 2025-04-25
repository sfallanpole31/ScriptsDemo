using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEditor;

public class UIManagerView : MonoBehaviour
{
    public GameSetting gameSetting;
    [Header("�B��TMP")]
    public TextMeshProUGUI MoveAmountText;
    [Header("����TMP")]
    public TextMeshProUGUI ScoreText;
    [Header("����Slider")]
    public Slider ScoreSlider;
    [Header("�ؼФ��G1���ȼƶq")]
    public TextMeshProUGUI goalCount1;
    [Header("�ؼФ��G1�Ϥ�")]
    public Image goal1image;
    [Header("�ؼФ��G2���ȼƶq")]
    public TextMeshProUGUI goalCount2;
    [Header("�ؼФ��G2�Ϥ�")]
    public Image goal2image;
    [Header("�D�㬵�u")]
    public TextMeshProUGUI itemBomb;
    [Header("�D���Ĥ�")]
    public TextMeshProUGUI itemPotion;
    [Header("�D��{�q")]
    public TextMeshProUGUI itemLighting;

    //�������O
    [Header("�������O")]
    public GameObject endGamePanel;
    [Header("�o���P�P")]
    public Image[] stars;
    [Header("�o��")]
    public TextMeshProUGUI endGameScore;
    [Header("�������D")]
    public TextMeshProUGUI endGameTitle;

    //�Ȱ����O
    [Header("�Ȱ����O")]
    public GameObject pausePanel;
    //[Header("�I�����֫��s���U")]
    //public ReactiveCommand OnBgmButtonClick = new ReactiveCommand();
    //[Header("���ī��s���U")]
    //public ReactiveCommand OnSoundeButtonClick = new ReactiveCommand();

    [Header("�I�����֫��s")]
    public Toggle bgmButton;
    [Header("���ī��s")]
    public Toggle soundButton;

    //�ޯ���s
    public ReactiveCommand ExplodeButtonClick = new ReactiveCommand();
    public void OnExplodeButtonClick()
    {
        ExplodeButtonClick.Execute();
    }


}
