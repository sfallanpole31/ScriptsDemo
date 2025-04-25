using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using VContainer;

public class Presenter
{
    private readonly GameController gameController;
    private readonly Model model;
    private readonly View view;
    private readonly UIManagerModel uIManagerModel;
    public List<UniTask> animationTasks = new List<UniTask>();//紀錄正在播放動畫的事件

    [Inject]
    public Presenter(GameController controller, Model model, View view, UIManagerModel uIManagerModel,EffectView effectView,GameSetting gameSetting)
    {
        this.gameController = controller;
        this.model = model;
        this.view = view;
        this.uIManagerModel = uIManagerModel;

        Init();

        #region controller
        //Controler選擇水果時 >> Model數據 選中的水果
        controller.selectFruit.Subscribe(fruit =>
        {
            model.selectedFruit = fruit;
            if (controller.isSkilling)
                controller.selectSkillTarget.Execute();
        });

        controller.swappedFruit.Subscribe(fruit =>
        {
            model.swappedFruit = fruit;
        });

        //如果交換事件觸發 Controller >> 播放View動畫、更改Model數據
        controller.swappedAction.Subscribe(async _ =>
        {

            if (model.selectedFruit == null) return;
            if (model.swappedFruit == null) return;
            if (controller.isSwiping) return; // 避免重複交換
            controller.isSwiping = true; // 🔹開始交換，鎖定操作

            await CheckAnimationTask();
            model.ChangePosition(model.selectedFruit, model.swappedFruit);
            var task = view.FruitMoveBehaviour(model.selectedFruit, model.swappedFruit).Preserve();
            animationTasks.Add(task);
            view.EndOfSwipping.Execute();
        });


        #endregion

        #region model
        model.InitGrid.Subscribe(async _ =>
        {
            await CheckAnimationTask();

            model.FruitTypeCheck();

        });

        //Model生成水果參數 >> View 生成水果物件
        model.OnFruitModelCreate.Skip(1).Subscribe(fruit =>
        {
            view.CreateFruit(fruit);
        });

        //Model 刪除水果數據 >> View 物件刪除
        model.OnFruitDestroy.Subscribe(Fruit =>
        {

            view.DestroyFruit(Fruit);
        });
        // Model 配對的水果數據消除 >> View 物件刪除
        model.OnFruitMatchDestroy.Subscribe(Fruit =>
        {

            if (Fruit == null)
                return;

            view.DestroyFruitList(Fruit);

        });

        //Model數據下墜處理 >> View下墜動畫處理
        model.DroppingFruitModel.Subscribe(Data =>
        {
            if (Data.Item1 == null)
                return;

            var task = view.MoveFruitAnimation(Data.Item1, Data.Item2).Preserve();
            animationTasks.Add(task);
        });

        model.OnSpwanCompelete.Subscribe(async _ =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            model.CheckAllFruitMatch();

            if (model.matchedFruits.Count != 0)//是
            {
                uIManagerModel.MinusGoalCount(model.matchedFruits);
                int score = model.AddScoreBasedOnMatch();
                uIManagerModel.AddScore(score);
                model.DestroyMatchFruit();
            }
            else
            {
                controller.isSwiping = false;
                if (uIManagerModel.MoveSteps.Value == 0)
                    uIManagerModel.MoveStepsExhaustedCloseGame.Execute();
            }

        });

        #endregion

        #region view
        //View生成物件 >> 存儲資料到Model
        view.OnFruitObjectCreate.Skip(1).Subscribe(gameObject =>
        {
            if (gameObject == null)
                return;

            Fruit fruit = gameObject.GetComponent<Fruit>();
            model.fruitsObjects[fruit.gridPosition.x, fruit.gridPosition.y] = gameObject;

            // 新增動畫任務到清單
            var task = view.MoveFruitAnimation(gameObject, fruit.worldPosition).Preserve();
            animationTasks.Add(task);

        });

        //View交換動畫結束 >>  Model判斷 整個面板 是否符合消除條件 >> 是: / 否:返回原位置
        view.EndOfSwipping.Subscribe(async _ =>
        {
            await CheckAnimationTask();

            model.CheckAllFruitMatch();
            if (model.matchedFruits.Count != 0)//是
            {
                uIManagerModel.MinusGoalCount(model.matchedFruits);
                int score = model.AddScoreBasedOnMatch();
                uIManagerModel.AddScore(score);
                model.DestroyMatchFruit();
                uIManagerModel.UseStep();
            }
            else //否
            {
                model.ChangePosition(model.selectedFruit, model.swappedFruit);
                var task = view.FruitMoveBehaviour(model.selectedFruit, model.swappedFruit).Preserve();
                animationTasks.Add(task);
                await CheckAnimationTask();
                controller.isSwiping = false;
                if (uIManagerModel.MoveSteps.Value == 0)
                    uIManagerModel.MoveStepsExhaustedCloseGame.Execute();
            }

            await CheckAnimationTask();

        });

        #endregion












    }



    /// <summary>
    /// 將基本設定數據傳至Model
    /// </summary>
    void Init()
    {
        view.SetProperty.Subscribe(property =>
        {
            model.gridWidth = property.width;
            model.gridHeight = property.height;
            model.boxCollider = property.boxCollider2D;
            model.fruitTypes = property.fruitType;
        });

    }

    // 當檢查是否有未執行的任務時，移除已完成的任務
    public async UniTask CheckAnimationTask()
    {
        // 只檢查未完成的任務
        animationTasks.RemoveAll(task => task.Status == UniTaskStatus.Succeeded || task.Status == UniTaskStatus.Faulted);

        if (animationTasks.Count > 0)
        {
            // 等待未完成的任務
            await UniTask.WhenAll(animationTasks);
        }

    }
}
