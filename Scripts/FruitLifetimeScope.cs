using UnityEngine;
using VContainer;
using VContainer.Unity;

public class FruitLifetimeScope : LifetimeScope
{
    [SerializeField] private GameSetting gameSetting;

    protected override void Configure(IContainerBuilder builder)
    {
        // 註冊依賴
        builder.Register<Model>(Lifetime.Singleton).AsSelf();
        builder.Register<Fruit>(Lifetime.Transient);  // 每次需要時都會創建一個新的 Fruit 實例
        builder.Register<UIManagerModel>(Lifetime.Singleton).AsSelf();
        builder.Register<Presenter>(Lifetime.Singleton).AsSelf();
        builder.Register<UIManagerPresenter>(Lifetime.Singleton).AsSelf();
        builder.Register<AudioPresenter>(Lifetime.Singleton).AsSelf();
        builder.Register<EffectPresenter>(Lifetime.Singleton).AsSelf();

        // 自動尋找場景中的 GameController、View 並注入
        builder.RegisterComponentInHierarchy<GameController>();
        builder.RegisterComponentInHierarchy<View>();
        builder.RegisterComponentInHierarchy<UIManagerView>();
        builder.RegisterComponentInHierarchy<AudioView>();
        builder.RegisterComponentInHierarchy<EffectView>();


        //ScriptableObject
        builder.RegisterInstance(gameSetting);
    }

    protected override void Awake()
    {
        base.Awake();
        // 解析 Presenter（但建議用 Constructor Injection 代替）
        var presenter = Container.Resolve<Presenter>();
        var uIManagerPresenter = Container.Resolve<UIManagerPresenter>();
        var aduioPresenter = Container.Resolve<AudioPresenter>();
        var effectPresenter = Container.Resolve<EffectPresenter>();

    }
}
