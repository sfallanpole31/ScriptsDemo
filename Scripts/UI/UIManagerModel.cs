using UnityEngine;
using UniRx;
using System.Collections.Generic;


public class UIManagerModel
{
    //UniRx
    public ReactiveProperty<int> MoveSteps { get; private set; } = new ReactiveProperty<int>(15);
    public ReactiveProperty<int> CurrentScore { get; private set; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<float> ScorePercentage { get; private set; } = new ReactiveProperty<float>(0f);
    public ReactiveProperty<int> MaxScore { get; private set; } = new ReactiveProperty<int>(300);
    public ReactiveProperty<int> GoalObject1Count { get; private set; } = new ReactiveProperty<int>(5);
    public ReactiveProperty<int> GoalObject2Count { get; private set; } = new ReactiveProperty<int>(5);
    public ReactiveProperty<int> ItemBombCount { get; private set; } = new ReactiveProperty<int>(1);
    public ReactiveProperty<int> ItemPositionCount { get; private set; } = new ReactiveProperty<int>(1);
    public ReactiveProperty<int> ItemLightingCount { get; private set; } = new ReactiveProperty<int>(1);
    public ReactiveProperty<int> goal1type { get; private set; } = new ReactiveProperty<int>();
    public ReactiveProperty<int> goal2type { get; private set; } = new ReactiveProperty<int>();

    //event
    public ReactiveCommand MoveStepsExhaustedCloseGame = new ReactiveCommand();

    public void Init(GameSetting gameSetting)
    {
        MoveSteps.Value = gameSetting.moveSteps;

        MaxScore.Value = gameSetting.maxScore;
        GoalObject1Count.Value = gameSetting.goalObject1Count;
        GoalObject2Count.Value = gameSetting.goalObject2Count;
        ItemBombCount.Value = gameSetting.itemBombCount;
        ItemPositionCount.Value = gameSetting.itemPositionCount;
        ItemLightingCount.Value = gameSetting.itemLightingCount;
    }

    public void CalculatePercentage()
    {
        if (CurrentScore.Value > MaxScore.Value)
            ScorePercentage.Value = 1f;
        else
            ScorePercentage.Value = (float)CurrentScore.Value / MaxScore.Value;
    }

    public void UseStep()
    {
        if (MoveSteps.Value > 0)
            MoveSteps.Value--;


    }

    public void AddScore(int addScore)
    {
        CurrentScore.Value += addScore;
    }


    public void SetGoal1Type(int type)
    {
        goal1type.Value = type;
    }

    public void SetGoal2Type(int type)
    {
        goal2type.Value = type;
    }

    public void MinusGoalCount(List<GameObject> matchedFruits)
    {
        foreach (GameObject gameObject in matchedFruits)
        {
            Fruit fruit = gameObject.GetComponent<Fruit>();
            if (GoalObject1Count.Value != 0)
            {
                if (fruit.type == goal1type.Value)
                {
                    GoalObject1Count.Value--;
                }
            }
            if (GoalObject2Count.Value != 0)
            {
                if (fruit.type == goal2type.Value)
                {
                    GoalObject2Count.Value--;
                }
            }

        }

    }

    public void UseExplodeSkill()
    {
        if (ItemBombCount.Value > 0)
            ItemBombCount.Value--;
    }

}
