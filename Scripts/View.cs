using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using DG.Tweening;

public class View : MonoBehaviour
{
    //[Inject]
    //private readonly Model model;

    [Header("橫向欄位")]
    public int gridWidth = 6;
    [Header("垂直欄位")]
    public int gridHeight = 6;
    [Header("水果種類")]
    public int fruitTypes = 5;

    [Header("水果預製物件")]
    public GameObject[] fruitPrefabs;
    [Header("面板大小boxCollider")]
    [SerializeField] private BoxCollider2D boxCollider;



    //UniRx 
    public ReactiveProperty<(int width, int height, int fruitType, BoxCollider2D boxCollider2D)> SetProperty = new ReactiveProperty<(int width, int height, int fruitType, BoxCollider2D boxCollider2D)>();
    public ReactiveProperty<GameObject> OnFruitObjectCreate = new ReactiveProperty<GameObject>();
    public ReactiveCommand EndOfSwipping = new ReactiveCommand();

    private void Awake()
    {
        SetProperty.Value = (gridWidth, gridHeight, fruitTypes, boxCollider);

    }


    //生成水果 
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
    /// 交換動畫
    /// </summary>
    /// <param name="selectedFruit"></param>
    /// <param name="swappedFruit"></param>
    /// <returns></returns>
    public async UniTask FruitMoveBehaviour(Fruit selectedFruit, Fruit swappedFruit)
    {
        Vector3 selectedStartPos = selectedFruit.transform.position;
        Vector3 swappedStartPos = swappedFruit.transform.position;
        float duration = 0.5f;

        // 使用 DOTween 執行位移動畫
        Tween moveSelected = selectedFruit.transform.DOMove(swappedStartPos, duration);
        Tween moveSwapped = swappedFruit.transform.DOMove(selectedStartPos, duration);

        // 等待兩個動畫都完成
        await UniTask.WhenAll(
            moveSelected.AsyncWaitForCompletion().AsUniTask(),
            moveSwapped.AsyncWaitForCompletion().AsUniTask()
        );
    }

    public void DestroyFruit(GameObject gameObject)
    {
        if (gameObject == null)
            return;

        Destroy(gameObject);  // 銷毀物件
    }

    public void DestroyFruitList(GameObject gameObject)
    {
        if (gameObject == null)
            return;

        Destroy(gameObject);  // 銷毀物件
    }

    /// <summary>
    /// 下落動畫 讓水果移動到新位置（帶動畫）
    /// </summary>
    public async UniTask MoveFruitAnimation(GameObject fruit, Vector2 targetPos)
    {
        if (fruit == null)
            return;

        float duration = 0.2f;

        // 執行 DOTween 位移動畫
        await fruit.transform.DOMove(targetPos, duration).SetEase(Ease.Linear).AsyncWaitForCompletion().AsUniTask();
    }


    //特效

    //音效

}
