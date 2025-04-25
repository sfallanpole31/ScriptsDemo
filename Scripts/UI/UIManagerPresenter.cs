using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using VContainer;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManagerPresenter
{
    //private readonly GameController gameController;
    private readonly UIManagerModel UIManagerModel;
    private readonly UIManagerView UIManagerView;
    private readonly View view;
    private readonly Model _model;
    private readonly Presenter _presenter;


    [Inject]
    public UIManagerPresenter(Model model, View view,UIManagerModel uIManagerModel, UIManagerView uIManagerView, GameController gameController, GameSetting gameSetting, EffectView effectView)
    {
        UIManagerModel = uIManagerModel;
        UIManagerView = uIManagerView;
        _model = model;
        this.view = view;


        //初始化
        UIManagerModel.Init(UIManagerView.gameSetting);


        model.InitGrid.Subscribe(_ =>
        {
            //設定目標種類
            int type1 = Random.Range(0, _model.fruitTypes);
            int type2;

            do
            {
                type2 = Random.Range(0, _model.fruitTypes);
            }
            while (type2 == type1);

            UIManagerModel.SetGoal1Type(type1);
            UIManagerModel.SetGoal2Type(type2);
        });



        UIManagerModel.goal1type.Subscribe(type =>
        {
            UIManagerView.goal1image.sprite = view.fruitPrefabs[type].GetComponentInChildren<SpriteRenderer>().sprite;
        });

        UIManagerModel.goal2type.Subscribe(type =>
        {
            UIManagerView.goal2image.sprite = view.fruitPrefabs[type].GetComponentInChildren<SpriteRenderer>().sprite;
        });

        UIManagerModel.MoveSteps.Subscribe(moveSteps =>
        {
            UIManagerView.MoveAmountText.text = moveSteps.ToString();
        });

        UIManagerModel.CurrentScore.Subscribe(score =>
        {
            UIManagerView.ScoreText.text = score.ToString();
            UIManagerModel.CalculatePercentage();
        });

        UIManagerModel.ScorePercentage.Subscribe(value =>
        {
            UIManagerView.ScoreSlider.value = value;
        });

        UIManagerModel.GoalObject1Count.Subscribe(value =>
        {
            UIManagerView.goalCount1.text = value.ToString();
        });

        UIManagerModel.GoalObject2Count.Subscribe(value =>
        {
            UIManagerView.goalCount2.text = value.ToString();
        });

        UIManagerModel.ItemBombCount.Subscribe(value =>
        {
            UIManagerView.itemBomb.text = value.ToString();
        });

        UIManagerModel.ItemPositionCount.Subscribe(value =>
        {
            UIManagerView.itemPotion.text = value.ToString();
        });

        UIManagerModel.ItemLightingCount.Subscribe(value =>
        {
            UIManagerView.itemLighting.text = value.ToString();
        });

        //步數用盡 結束遊戲
        UIManagerModel.MoveStepsExhaustedCloseGame.Subscribe(_ =>
        {

            UIManagerView.endGamePanel.SetActive(true);

            int starCount = 0;

            if (UIManagerModel.CurrentScore.Value >= UIManagerModel.MaxScore.Value)
                starCount = 3;
            else if (UIManagerModel.CurrentScore.Value >= UIManagerModel.MaxScore.Value * 2 / 3)
                starCount = 2;
            else if (UIManagerModel.CurrentScore.Value >= UIManagerModel.MaxScore.Value * 1 / 3)
                starCount = 1;
            else
                starCount = 0;

            for (int i = 0; i < UIManagerView.stars.Length; i++)
            {
                UIManagerView.stars[i].gameObject.SetActive(i < starCount);
                UIManagerView.endGameScore.text = uIManagerModel.CurrentScore.Value.ToString();

                if (uIManagerModel.GoalObject1Count.Value != 0 || uIManagerModel.GoalObject2Count.Value != 0)
                    UIManagerView.endGameTitle.text = "FAIL!";
                else
                    UIManagerView.endGameTitle.text = "COMPLETE!";
            }

        });

        UIManagerView.ExplodeButtonClick.Subscribe(_ =>
        {
            Color normalColor = new Color(1f, 1f, 1f);
            Color selectedColor = new Color(0.302f, 0.302f, 0.302f);


            if (gameController.isSkilling == false)
            {
                if (uIManagerModel.ItemBombCount.Value > 0)
                {
                    gameController.isSkilling = true;
                    gameController.selectFruit.Value = null;
                    ColorBlock colors = effectView.exolodeSkillEffectButton.colors;
                    colors.selectedColor = selectedColor;
                    effectView.exolodeSkillEffectButton.colors = colors;
                }
            }
            else if (gameController.isSkilling == true)
            {
                gameController.isSkilling = false;
                gameController.selectFruit.Value = null;
                ColorBlock colors = effectView.exolodeSkillEffectButton.colors;
                colors.selectedColor = normalColor;
                effectView.exolodeSkillEffectButton.colors = colors;
            }



        });

    }

}
