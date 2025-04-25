using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Model
{
    //��V���
    public int gridWidth = 6;
    //�������
    public int gridHeight = 6;
    //���G����
    public int fruitTypes = 5;
    //���O�j�p
    public BoxCollider2D boxCollider;
    //�����G�ƾ�
    public GameObject[,] fruitsObjects;
    //�襤���G
    public Fruit selectedFruit;
    //�������G
    public Fruit swappedFruit;
    //�ŦX���󪺤��G����
    public List<GameObject> matchedFruits = new List<GameObject>();
    //�@�ɦ�m
    public Dictionary<Vector2Int, Vector2> getWorldPosition = new Dictionary<Vector2Int, Vector2>();

    //UniRx
    public ReactiveCommand InitGrid = new ReactiveCommand();
    public ReactiveProperty<(Vector2Int, Vector2, int)> OnFruitModelCreate = new ReactiveProperty<(Vector2Int, Vector2, int)>();
    public ReactiveProperty<GameObject> OnFruitDestroy = new ReactiveProperty<GameObject>();
    public ReactiveProperty<GameObject> OnFruitMatchDestroy = new ReactiveProperty<GameObject>();
    public ReactiveProperty<(GameObject, Vector2)> DroppingFruitModel = new ReactiveProperty<(GameObject, Vector2)>();
    public ReactiveCommand OnSpwanCompelete = new ReactiveCommand();

    /// <summary>
    /// ��l�ƥͦ����O
    /// </summary>
    public void CreateGrid()
    {
        //fruitsObjects = new Fruit[gridWidth, gridHeight];
        fruitsObjects = new GameObject[gridWidth, gridHeight];
        Vector2 min = boxCollider.bounds.min; // ���o BoxCollider �����U��
        Vector2 size = boxCollider.size; // ���o BoxCollider ���j�p

        float cellWidth = size.x / gridWidth;  // ��Ӯ�l���e��
        float cellHeight = size.y / gridHeight; // ��Ӯ�l������ (���]�\��b XZ ����)

        Vector2 startPos = min + new Vector2(cellWidth / 2, cellHeight / 2);// �]�m�_�l�I�� BoxCollider �����U��

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // ���L�|�Ө���
                if ((x == 0 && y == 0) ||
                    (x == 0 && y == gridHeight - 1) ||
                    (x == gridWidth - 1 && y == 0) ||
                    (x == gridWidth - 1 && y == gridHeight - 1))
                    continue;

                var type = UnityEngine.Random.Range(0, fruitTypes);

                //����G�ѼƶǨ�VIEW�h�ͦ�����A�⪫��Ǩ�Model������}�C
                OnFruitModelCreate.Value = (new Vector2Int(x, y), startPos + new Vector2(x * cellHeight, y * cellWidth), type);
                getWorldPosition.Add(new Vector2Int(x, y), startPos + new Vector2(x * cellHeight, y * cellWidth));

            }
        }
        InitGrid.Execute();

    }

    /// <summary>
    /// �ˬd���G�����O�_���ơA�Y���Ƨ����G����
    /// </summary>
    public void FruitTypeCheck()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (!fruitsObjects[x, y]) continue;

                Fruit currentFruit = fruitsObjects[x, y].GetComponent<Fruit>();
                if (currentFruit.worldPosition == Vector2.zero) continue;

                // �ˬd�O�_�P�۾F���G��������
                bool hasDuplicate = false;
                foreach (Vector2Int dir in directions)
                {
                    int newX = x + dir.x;
                    int newY = y + dir.y;

                    if (newX >= 0 && newX < gridWidth && newY >= 0 && newY < gridHeight)
                    {
                        Fruit neighborFruit = fruitsObjects[newX, newY]?.GetComponent<Fruit>();
                        if (neighborFruit && currentFruit.type == neighborFruit.type)
                        {
                            hasDuplicate = true;
                            break;
                        }
                    }
                }

                // �p�G�����ƪ������A�N�󴫤��G����
                if (hasDuplicate)
                {
                    currentFruit.type = ChangeTypes(currentFruit.type, x, y);//�󴫺���

                    //�ƥ��¸��
                    Vector2Int newGridPosition = currentFruit.gridPosition;
                    Vector2 newPosition = currentFruit.worldPosition;
                    int newType = currentFruit.type;

                    OnFruitDestroy.Value = currentFruit.gameObject;//�R���ª���
                    OnFruitModelCreate.Value = (newGridPosition, newPosition, newType);//�s�W�s���G

                }
            }
        }
    }

    /// <summary>
    /// �����G�����A�T�O�s���������P�W�U���k���G���ۦP
    /// </summary>
    private int ChangeTypes(int exclude, int x, int y)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        HashSet<int> neighborTypes = new HashSet<int>();

        // �����۾F���G������
        foreach (Vector2Int dir in directions)
        {
            int newX = x + dir.x;
            int newY = y + dir.y;

            if (newX >= 0 && newX < gridWidth && newY >= 0 && newY < gridHeight)
            {
                Fruit neighborFruit = fruitsObjects[newX, newY]?.GetComponent<Fruit>();
                if (neighborFruit != null)
                {
                    neighborTypes.Add(neighborFruit.type);
                }
            }
        }

        int newType;
        do
        {
            newType = UnityEngine.Random.Range(0, fruitTypes);
        } while (newType == exclude || neighborTypes.Contains(newType));  // �T�O�s�������P�F����G�ۦP

        return newType;
    }

    /// <summary>
    /// Model�ƾڧ�� �洫��̰}�C������m
    /// </summary>
    /// <param name="select"></param>
    /// <param name="swap"></param>
    /// <returns></returns>
    public void ChangePosition(Fruit select, Fruit swap)
    {
        // �����洫��m�ƾ�
        Vector2Int selectGrid = select.gridPosition;
        Vector2Int swapGrid = swap.gridPosition;

        Vector3 selectWorld = select.worldPosition;
        Vector3 swapWorld = swap.worldPosition;

        // ��s���G��m
        select.gridPosition = swapGrid;
        select.worldPosition = swapWorld;

        swap.gridPosition = selectGrid;
        swap.worldPosition = selectWorld;

        // ��s����Ѧ�
        fruitsObjects[selectGrid.x, selectGrid.y] = swap.gameObject;
        fruitsObjects[swapGrid.x, swapGrid.y] = select.gameObject;

    }

    /// <summary>
    /// �ˬd��ӭ��O���G
    /// </summary>
    public void CheckAllFruitMatch()
    {
        matchedFruits.Clear();
        VerticalCheck();
        HorizontalCheck();
    }

    /// <summary>
    /// �����ˬd
    /// </summary>
    private void VerticalCheck()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            int y = 0;

            while (y < gridHeight)
            {
                if (fruitsObjects[x, y] == null)
                {
                    y = y + 1;
                    continue;
                }
                Fruit currentFruit = fruitsObjects[x, y].GetComponent<Fruit>();
                List<GameObject> tempMatchedFruits = new List<GameObject>();
                tempMatchedFruits.Add(fruitsObjects[x, y]);

                int matchCount = 1;
                int nextY = y + 1;

                // �ˬd���U�Ӫ����G�O�_�ۦP
                while (nextY < gridHeight)
                {
                    if (fruitsObjects[x, nextY] == null)
                        break;

                    Fruit nextFruit = fruitsObjects[x, nextY].GetComponent<Fruit>();

                    if (nextFruit.type == currentFruit.type)
                    {
                        tempMatchedFruits.Add(fruitsObjects[x, nextY]);
                        matchCount++;
                        nextY++;
                    }
                    else
                    {
                        break;
                    }
                }

                // �p�G�ǰt�ƶq�j�󵥩� 3�A�h�[�J matchedFruits
                if (matchCount >= 3)
                {
                    matchedFruits.AddRange(tempMatchedFruits);
                }

                // ���ʨ�U�@�Ӥ��ǰt�����G�~���ˬd
                y = nextY;
            }
        }
    }

    /// <summary>
    /// �����ˬd
    /// </summary>
    private void HorizontalCheck()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            int x = 0;

            while (x < gridWidth)
            {
                if (fruitsObjects[x, y] == null)
                {
                    x = x + 1;
                    continue;
                }

                Fruit currentFruit = fruitsObjects[x, y].GetComponent<Fruit>();
                List<GameObject> tempMatchedFruits = new List<GameObject>();
                tempMatchedFruits.Add(fruitsObjects[x, y]);

                int matchCount = 1;
                int nextX = x + 1;

                // �ˬd���U�Ӫ����G�O�_�ۦP
                while (nextX < gridWidth)
                {
                    if (fruitsObjects[nextX, y] == null)
                        break;

                    Fruit nextFruit = fruitsObjects[nextX, y].GetComponent<Fruit>();

                    if (nextFruit.type == currentFruit.type)
                    {
                        tempMatchedFruits.Add(fruitsObjects[nextX, y]);
                        matchCount++;
                        nextX++;
                    }
                    else
                    {
                        break;
                    }
                }

                // �p�G�ǰt�ƶq�j�󵥩� 3�A�h�[�J matchedFruits
                if (matchCount >= 3)
                {
                    matchedFruits.AddRange(tempMatchedFruits);
                }

                // ���ʨ�U�@�Ӥ��ǰt�����G�~���ˬd
                x = nextX;
            }
        }
    }

    /// <summary>
    /// �����t�諸���G
    /// </summary>
    /// <returns></returns>
    public void DestroyMatchFruit()
    {

        foreach (var fruit in matchedFruits)
        {

            // �������� `GameObject[,]` �����ޥΨó]�� null �γB�z
            for (int x = 0; x < fruitsObjects.GetLength(0); x++)
            {
                for (int y = 0; y < fruitsObjects.GetLength(1); y++)
                {
                    if (fruitsObjects[x, y] == fruit)
                    {
                        fruitsObjects[x, y] = null; // ��s�ޥ�
                        OnFruitMatchDestroy.Value = fruit;
                    }
                }
            }
        }



        DroppingFruit();
        SpawnNewFruits();

    }

    /// <summary>
    /// �N�a�Ť��G�U�Y
    /// </summary>
    /// <returns></returns>
    public void DroppingFruit()
    {

        // 2. ���W�誺���G���U��
        for (int x = 0; x < gridWidth; x++) // �M���C�@�C
        {
            for (int y = 0; y < gridHeight; y++) // �q�������W�ˬd
            {
                if (fruitsObjects[x, y] == null) // �o�{�Ů�
                {
                    // ���L�|�Ө���
                    if ((x == 0 && y == 0) ||
                        (x == 0 && y == gridHeight - 1) ||
                        (x == gridWidth - 1 && y == 0) ||
                        (x == gridWidth - 1 && y == gridHeight - 1))
                        continue;

                    for (int searchY = y + 1; searchY < gridHeight; searchY++) // ���W�M��D�Ť��G
                    {
                        if (fruitsObjects[x, searchY] != null)
                        {
                            // ����ʵe
                            Vector2 newPosition = getWorldPosition[new Vector2Int(x, y)];
                            DroppingFruitModel.Value = (fruitsObjects[x, searchY], newPosition);

                            // �洫��m
                            fruitsObjects[x, y] = fruitsObjects[x, searchY];
                            fruitsObjects[x, searchY] = null;
                            fruitsObjects[x, y].GetComponent<Fruit>().gridPosition = new Vector2Int(x, y);

                            break;
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// ���ͷs�����G�ɺ��ů�
    /// </summary>
    public void SpawnNewFruits()
    {
        int width = fruitsObjects.GetLength(0);
        int height = fruitsObjects.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                // ���L�|�Ө���
                if ((x == 0 && y == 0) ||
                    (x == 0 && y == gridHeight - 1) ||
                    (x == gridWidth - 1 && y == 0) ||
                    (x == gridWidth - 1 && y == gridHeight - 1))
                    continue;

                if (fruitsObjects[x, y] == null) // �p�G�٦��ů�
                {
                    var type = UnityEngine.Random.Range(0, fruitTypes);
                    Vector2 position = getWorldPosition[new Vector2Int(x, y)];
                    OnFruitModelCreate.Value = (new Vector2Int(x, y), position, type);

                }
            }
        }
        OnSpwanCompelete.Execute();
    }

    /// <summary>
    /// �̭n���������G�ƶq �ӵ���
    /// </summary>
    /// <param name="matchedFruits"></param>
    public int AddScoreBasedOnMatch()
    {
        int matchCount = matchedFruits.Count;

        if (matchCount == 3)
        {
            return 10;
        }
        else if (matchCount == 4)
        {
            return 20;
        }
        else if (matchCount == 5)
        {
            return 30;
        }
        else if (matchCount > 5)
        {
            return 50;
        }
        else
        {
            return 0;
        }
    }


    public void SelectSkillTarget()
    {
        if (selectedFruit == null)
            return;

        int rows = gridHeight;
        int cols = gridWidth;
        int x = selectedFruit.gridPosition.x;
        int y = selectedFruit.gridPosition.y;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < rows && ny >= 0 && ny < cols)
                {
                    if (fruitsObjects[nx, ny] != null)
                        matchedFruits.Add(fruitsObjects[nx, ny]);
                }
            }
        }

    }

}
