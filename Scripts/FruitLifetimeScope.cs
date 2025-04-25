using UnityEngine;
using VContainer;
using VContainer.Unity;

public class FruitLifetimeScope : LifetimeScope
{
    [SerializeField] private GameSetting gameSetting;

    protected override void Configure(IContainerBuilder builder)
    {
        // ���U�̿�
        builder.Register<Model>(Lifetime.Singleton).AsSelf();
        builder.Register<Fruit>(Lifetime.Transient);  // �C���ݭn�ɳ��|�Ыؤ@�ӷs�� Fruit ���
        builder.Register<UIManagerModel>(Lifetime.Singleton).AsSelf();
        builder.Register<Presenter>(Lifetime.Singleton).AsSelf();
        builder.Register<UIManagerPresenter>(Lifetime.Singleton).AsSelf();
        builder.Register<AudioPresenter>(Lifetime.Singleton).AsSelf();
        builder.Register<EffectPresenter>(Lifetime.Singleton).AsSelf();

        // �۰ʴM��������� GameController�BView �ê`�J
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
        // �ѪR Presenter�]����ĳ�� Constructor Injection �N���^
        var presenter = Container.Resolve<Presenter>();
        var uIManagerPresenter = Container.Resolve<UIManagerPresenter>();
        var aduioPresenter = Container.Resolve<AudioPresenter>();
        var effectPresenter = Container.Resolve<EffectPresenter>();

    }
}
