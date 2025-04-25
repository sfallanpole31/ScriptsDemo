using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using DG.Tweening;

public class View : MonoBehaviour
{
    //[Inject]
    //private readonly Model model;

    [Header("��V���")]
    public int gridWidth = 6;
    [Header("�������")]
    public int gridHeight = 6;
    [Header("���G����")]
    public int fruitTypes = 5;

    [Header("���G�w�s����")]
    public GameObject[] fruitPrefabs;
    [Header("���O�j�pboxCollider")]
    [SerializeField] private BoxCollider2D boxCollider;



    //UniRx 
    public ReactiveProperty<(int width, int height, int fruitType, BoxCollider2D boxCollider2D)> SetProperty = new ReactiveProperty<(int width, int height, int fruitType, BoxCollider2D boxCollider2D)>();
    public ReactiveProperty<GameObject> OnFruitObjectCreate = new ReactiveProperty<GameObject>();
    public ReactiveCommand EndOfSwipping = new ReactiveCommand();

    private void Awake()
    {
        SetProperty.Value = (gridWidth, gridHeight, fruitTypes, boxCollider);

    }


    //�ͦ����G 
    public void CreateFruit((Vector2Int, Vector2, int) data)
    {
        Vector2 startPos = new Vector2(data.Item2.x, data.Item2.y + boxCollider.size.y / gridHeight);
        GameObject gameObject = Instantiate(fruitPrefabs[data.Item3], startPos, Quaternion.identity, this.gameObject.transform);
        Fruit fruit = gameObject.GetComponent<Fruit>();
        fruit.gridPosition = data.Item1;
        fruit.worldPosition = data.Item2;
        fruit.type = data.Item3;

        OnFruitObjectCreate.Value = gameObject;



    }
    //public async void CreateFruit((Vector2Int, Vector2, int) data)
    //{
    //    Vector2 startPos = new Vector2(data.Item2.x, data.Item2.y);


    //    GameObject gameObject = Instantiate(fruitPrefabs[data.Item3], startPos, Quaternion.identity, this.gameObject.transform);
    //    Fruit fruit = gameObject.GetComponent<Fruit>();
    //    fruit.gridPosition = data.Item1;
    //    fruit.worldPosition = data.Item2;
    //    fruit.type = data.Item3;

    //    await MoveFruitAnimation(gameObject, data.Item2);

    //    OnFruitObjectCreate.Value = gameObject;



    //}
    /// <summary>
    /// �洫�ʵe
    /// </summary>
    /// <param name="selectedFruit"></param>
    /// <param name="swappedFruit"></param>
    /// <returns></returns>
    public async UniTask FruitMoveBehaviour(Fruit selectedFruit, Fruit swappedFruit)
    {
        Vector3 selectedStartPos = selectedFruit.transform.position;
        Vector3 swappedStartPos = swappedFruit.transform.position;
        float duration = 0.5f;

        // �ϥ� DOTween ����첾�ʵe
        Tween moveSelected = selectedFruit.transform.DOMove(swappedStartPos, duration);
        Tween moveSwapped = swappedFruit.transform.DOMove(selectedStartPos, duration);

        // ���ݨ�Ӱʵe������
        await UniTask.WhenAll(
            moveSelected.AsyncWaitForCompletion().AsUniTask(),
            moveSwapped.AsyncWaitForCompletion().AsUniTask()
        );
    }

    public void DestroyFruit(GameObject gameObject)
    {
        if (gameObject == null)
            return;

        Destroy(gameObject);  // �P������
    }

    public void DestroyFruitList(GameObject gameObject)
    {
        if (gameObject == null)
            return;

        Destroy(gameObject);  // �P������
    }

    /// <summary>
    /// �U���ʵe �����G���ʨ�s��m�]�a�ʵe�^
    /// </summary>
    public async UniTask MoveFruitAnimation(GameObject fruit, Vector2 targetPos)
    {
        if (fruit == null)
            return;

        float duration = 0.2f;

        // ���� DOTween �첾�ʵe
        await fruit.transform.DOMove(targetPos, duration).SetEase(Ease.Linear).AsyncWaitForCompletion().AsUniTask();
    }


    //�S��

    //����

}
