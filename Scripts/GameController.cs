using UnityEngine;
using UnityEngine.SceneManagement;


//�ĤT�贡��
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
    public float swipeThreshold = 50f; // �]�w���ʦh�ֹ����~�⦳�ķư�
    private Vector2 startPos;

    //UniRx�ƥ�
    public ReactiveProperty<Fruit> selectFruit;
    public ReactiveProperty<Fruit> swappedFruit;
    public ReactiveCommand swappedAction = new ReactiveCommand();
    public ReactiveCommand selectSkillTarget = new ReactiveCommand();


    private void Start()
    {
        model.CreateGrid();
        ListenForInput().Forget(); // �Ұʲ��B��J��ť
    }

    private async UniTaskVoid ListenForInput()
    {
        while (true)
        {
            await SelectFruit();   // ���ݿ�ܤ��G
            await SwappedFruit();  // ���ݥ洫���G
        }
    }

    private async UniTask SelectFruit()
    {
        await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0) & !isSwiping); // ���ݷƹ����U
        startPos = Input.mousePosition;


        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            //Debug.Log("�I���� 2D ����G" + hit.collider.gameObject.name);
            selectFruit.Value = hit.collider.gameObject.GetComponent<Fruit>();
        }
    }

    private async UniTask SwappedFruit()
    {
        await UniTask.WaitUntil(() => Input.GetMouseButtonUp(0) & !isSwiping); // ���ݷƹ���}

        if (selectFruit.Value != null)
        {
            int x = selectFruit.Value.gridPosition.x;
            int y = selectFruit.Value.gridPosition.y;
            Vector2 endPos = Input.mousePosition;
            Vector2 delta = endPos - startPos;

            Vector2Int direction = Vector2Int.zero;

            // �P�_�ưʤ�V
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) // �����ư�
            {
                direction = delta.x > swipeThreshold ? Vector2Int.right :
                            delta.x < -swipeThreshold ? Vector2Int.left :
                            Vector2Int.zero;
            }
            else // �����ư�
            {
                direction = delta.y > swipeThreshold ? Vector2Int.up :
                            delta.y < -swipeThreshold ? Vector2Int.down :
                            Vector2Int.zero;
            }

            // ���ե洫���G
            if (direction != Vector2Int.zero)
            {
                TrySwap(x + direction.x, y + direction.y, direction);
            }
        }

    }

    // ���ե洫���G
    void TrySwap(int newX, int newY, Vector2Int direction)
    {
        if (newX >= 0 && newX < model.gridWidth && newY >= 0 && newY < model.gridHeight)
        {
            if (model.fruitsObjects[newX, newY])
            {
                swappedFruit.Value = model.fruitsObjects[newX, newY].GetComponent<Fruit>();
                //Debug.Log($"�ưʤ�V: {direction}");
                swappedAction.Execute();
            }
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(0);
    }



}
