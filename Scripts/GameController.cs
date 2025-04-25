using UnityEngine;
using UnityEngine.SceneManagement;


//第三方插件
using UniRx;
using VContainer;
using Cysharp.Threading.Tasks;

public class GameController : MonoBehaviour
{
    private Model model;

    [Inject]
    public void Construct(Model model)
    {
        this.model = model;
    }

    public bool isSwiping = false;
    public bool isSkilling = false;
    public float swipeThreshold = 50f; // 設定移動多少像素才算有效滑動
    private Vector2 startPos;

    //UniRx事件
    public ReactiveProperty<Fruit> selectFruit;
    public ReactiveProperty<Fruit> swappedFruit;
    public ReactiveCommand swappedAction = new ReactiveCommand();
    public ReactiveCommand selectSkillTarget = new ReactiveCommand();


    private void Start()
    {
        model.CreateGrid();
        ListenForInput().Forget(); // 啟動異步輸入監聽
    }

    private async UniTaskVoid ListenForInput()
    {
        while (true)
        {
            await SelectFruit();   // 等待選擇水果
            await SwappedFruit();  // 等待交換水果
        }
    }

    private async UniTask SelectFruit()
    {
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0) & !isSwiping); // 等待滑鼠按下
        startPos = Input.mousePosition;


        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            //Debug.Log("點擊到 2D 物件：" + hit.collider.gameObject.name);
            selectFruit.Value = hit.collider.gameObject.GetComponent<Fruit>();
        }
    }

    private async UniTask SwappedFruit()
    {
        await UniTask.WaitUntil(() => Input.GetMouseButtonUp(0) & !isSwiping); // 等待滑鼠放開

        if (selectFruit.Value != null)
        {
            int x = selectFruit.Value.gridPosition.x;
            int y = selectFruit.Value.gridPosition.y;
            Vector2 endPos = Input.mousePosition;
            Vector2 delta = endPos - startPos;

            Vector2Int direction = Vector2Int.zero;

            // 判斷滑動方向
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) // 水平滑動
            {
                direction = delta.x > swipeThreshold ? Vector2Int.right :
                            delta.x < -swipeThreshold ? Vector2Int.left :
                            Vector2Int.zero;
            }
            else // 垂直滑動
            {
                direction = delta.y > swipeThreshold ? Vector2Int.up :
                            delta.y < -swipeThreshold ? Vector2Int.down :
                            Vector2Int.zero;
            }

            // 嘗試交換水果
            if (direction != Vector2Int.zero)
            {
                TrySwap(x + direction.x, y + direction.y, direction);
            }
        }

    }

    // 嘗試交換水果
    void TrySwap(int newX, int newY, Vector2Int direction)
    {
        if (newX >= 0 && newX < model.gridWidth && newY >= 0 && newY < model.gridHeight)
        {
            if (model.fruitsObjects[newX, newY])
            {
                swappedFruit.Value = model.fruitsObjects[newX, newY].GetComponent<Fruit>();
                //Debug.Log($"滑動方向: {direction}");
                swappedAction.Execute();
            }
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(0);
    }



}
