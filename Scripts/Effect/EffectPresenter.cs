using UnityEngine;
using VContainer;
using UniRx;

public class EffectPresenter 
{
    [Inject]
    public EffectPresenter(Model model,EffectView effectView,GameController controller,UIManagerModel uIManagerModel)
    {
        model.OnFruitMatchDestroy.Skip(1).Subscribe(GameObject =>
        {
            effectView.SpawnEffect(GameObject.transform.position);
        });

        controller.selectSkillTarget.Subscribe(_ =>
        {
            if (controller.selectFruit.Value == null)
                return;

            model.SelectSkillTarget();
            uIManagerModel.MinusGoalCount(model.matchedFruits);
            int score = model.AddScoreBasedOnMatch();
            uIManagerModel.AddScore(score);
            model.DestroyMatchFruit();
            uIManagerModel.UseExplodeSkill();
            effectView.SpawnExolodeSkillEffect( controller.selectFruit.Value.gameObject.transform.position);
            controller.isSkilling = false;
        });
    }

}
