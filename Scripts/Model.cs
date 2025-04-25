using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Model
{
    //橫向欄位
    public int gridWidth = 6;
    //垂直欄位
    public int gridHeight = 6;
    //水果種類
    public int fruitTypes = 5;
    //面板大小
    public BoxCollider2D boxCollider;
    //全水果數據
    public GameObject[,] fruitsObjects;
    //選中水果
    public Fruit selectedFruit;
    //替換水果
    public Fruit swappedFruit;
    //符合條件的水果物件
    public List<GameObject> matchedFruits = new List<GameObject>();
    //世界位置
    public Dictionary<Vector2Int, Vector2> getWorldPosition = new Dictionary<Vector2Int, Vector2>();

    //UniRx
    public ReactiveCommand InitGrid = new ReactiveCommand();
    public ReactiveProperty<(Vector2Int, Vector2, int)> OnFruitModelCreate = new ReactiveProperty<(Vector2Int, Vector2, int)>();
    public ReactiveProperty<GameObject> OnFruitDestroy = new ReactiveProperty<GameObject>();
    public ReactiveProperty<GameObject> OnFruitMatchDestroy = new ReactiveProperty<GameObject>();
    public ReactiveProperty<(GameObject, Vector2)> DroppingFruitModel = new ReactiveProperty<(GameObject, Vector2)>();
    public ReactiveCommand OnSpwanCompelete = new ReactiveCommand();

    /// <summary>
    /// 初始化生成面板
    /// </summary>
    public void CreateGrid()
    {
        //fruitsObjects = new Fruit[gridWidth, gridHeight];
        fruitsObjects = new GameObject[gridWidth, gridHeight];
        Vector2 min = boxCollider.bounds.min; // 取得 BoxCollider 的左下角
        Vector2 size = boxCollider.size; // 取得 BoxCollider 的大小

        float cellWidth = size.x / gridWidth;  // 單個格子的寬度
        float cellHeight = size.y / gridHeight; // 單個格子的高度 (假設擺放在 XZ 平面)

        Vector2 startPos = min + new Vector2(cellWidth / 2, cellHeight / 2);// 設置起始點為 BoxCollider 的左下角

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // 跳過四個角落
                if ((x == 0 && y == 0) ||
                    (x == 0 && y == gridHeight - 1) ||
                    (x == gridWidth - 1 && y == 0) ||
                    (x == gridWidth - 1 && y == gridHeight - 1))
                    continue;

                var type = UnityEngine.Random.Range(0, fruitTypes);

                //把水果參數傳到VIEW去生成物件，把物件傳到Model的物件陣列
                OnFruitModelCreate.Value = (new Vector2Int(x, y), startPos + new Vector2(x * cellHeight, y * cellWidth), type);
                getWorldPosition.Add(new Vector2Int(x, y), startPos + new Vector2(x * cellHeight, y * cellWidth));

            }
        }
        InitGrid.Execute();

    }

    /// <summary>
    /// 檢查水果種類是否重複，若重複更改水果種類
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

                // 檢查是否與相鄰水果類型重複
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

                // 如果有重複的類型，就更換水果類型
                if (hasDuplicate)
                {
                    currentFruit.type = ChangeTypes(currentFruit.type, x, y);//更換種類

                    //備份舊資料
                    Vector2Int newGridPosition = currentFruit.gridPosition;
                    Vector2 newPosition = currentFruit.worldPosition;
                    int newType = currentFruit.type;

                    OnFruitDestroy.Value = currentFruit.gameObject;//刪除舊物件
                    OnFruitModelCreate.Value = (newGridPosition, newPosition, newType);//新增新水果

                }
            }
        }
    }

    /// <summary>
    /// 更改水果種類，確保新的類型不與上下左右水果不相同
    /// </summary>
    private int ChangeTypes(int exclude, int x, int y)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        HashSet<int> neighborTypes = new HashSet<int>();

        // 收集相鄰水果的類型
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
        } while (newType == exclude || neighborTypes.Contains(newType));  // 確保新類型不與鄰近水果相同

        return newType;
    }

    /// <summary>
    /// Model數據更改 交換兩者陣列中的位置
    /// </summary>
    /// <param name="select"></param>
    /// <param name="swap"></param>
    /// <returns></returns>
    public void ChangePosition(Fruit select, Fruit swap)
    {
        // 直接交換位置數據
        Vector2Int selectGrid = select.gridPosition;
        Vector2Int swapGrid = swap.gridPosition;

        Vector3 selectWorld = select.worldPosition;
        Vector3 swapWorld = swap.worldPosition;

        // 更新水果位置
        select.gridPosition = swapGrid;
        select.worldPosition = swapWorld;

        swap.gridPosition = selectGrid;
        swap.worldPosition = selectWorld;

        // 更新物件參考
        fruitsObjects[selectGrid.x, selectGrid.y] = swap.gameObject;
        fruitsObjects[swapGrid.x, swapGrid.y] = select.gameObject;

    }

    /// <summary>
    /// 檢查整個面板水果
    /// </summary>
    public void CheckAllFruitMatch()
    {
        matchedFruits.Clear();
        VerticalCheck();
        HorizontalCheck();
    }

    /// <summary>
    /// 垂直檢查
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

                // 檢查接下來的水果是否相同
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

                // 如果匹配數量大於等於 3，則加入 matchedFruits
                if (matchCount >= 3)
                {
                    matchedFruits.AddRange(tempMatchedFruits);
                }

                // 移動到下一個不匹配的水果繼續檢查
                y = nextY;
            }
        }
    }

    /// <summary>
    /// 水平檢查
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

                // 檢查接下來的水果是否相同
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

                // 如果匹配數量大於等於 3，則加入 matchedFruits
                if (matchCount >= 3)
                {
                    matchedFruits.AddRange(tempMatchedFruits);
                }

                // 移動到下一個不匹配的水果繼續檢查
                x = nextX;
            }
        }
    }

    /// <summary>
    /// 消除配對的水果
    /// </summary>
    /// <returns></returns>
    public void DestroyMatchFruit()
    {

        foreach (var fruit in matchedFruits)
        {

            // 找到對應的 `GameObject[,]` 中的引用並設為 null 或處理
            for (int x = 0; x < fruitsObjects.GetLength(0); x++)
            {
                for (int y = 0; y < fruitsObjects.GetLength(1); y++)
                {
                    if (fruitsObjects[x, y] == fruit)
                    {
                        fruitsObjects[x, y] = null; // 更新引用
                        OnFruitMatchDestroy.Value = fruit;
                    }
                }
            }
        }



        DroppingFruit();
        SpawnNewFruits();

    }

    /// <summary>
    /// 將懸空水果下墜
    /// </summary>
    /// <returns></returns>
    public void DroppingFruit()
    {

        // 2. 讓上方的水果往下掉
        for (int x = 0; x < gridWidth; x++) // 遍歷每一列
        {
            for (int y = 0; y < gridHeight; y++) // 從底部往上檢查
            {
                if (fruitsObjects[x, y] == null) // 發現空格
                {
                    // 跳過四個角落
                    if ((x == 0 && y == 0) ||
                        (x == 0 && y == gridHeight - 1) ||
                        (x == gridWidth - 1 && y == 0) ||
                        (x == gridWidth - 1 && y == gridHeight - 1))
                        continue;

                    for (int searchY = y + 1; searchY < gridHeight; searchY++) // 往上尋找非空水果
                    {
                        if (fruitsObjects[x, searchY] != null)
                        {
                            // 執行動畫
                            Vector2 newPosition = getWorldPosition[new Vector2Int(x, y)];
                            DroppingFruitModel.Value = (fruitsObjects[x, searchY], newPosition);

                            // 交換位置
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
    /// 產生新的水果補滿空缺
    /// </summary>
    public void SpawnNewFruits()
    {
        int width = fruitsObjects.GetLength(0);
        int height = fruitsObjects.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                // 跳過四個角落
                if ((x == 0 && y == 0) ||
                    (x == 0 && y == gridHeight - 1) ||
                    (x == gridWidth - 1 && y == 0) ||
                    (x == gridWidth - 1 && y == gridHeight - 1))
                    continue;

                if (fruitsObjects[x, y] == null) // 如果還有空缺
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
    /// 依要消除的水果數量 來給分
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
