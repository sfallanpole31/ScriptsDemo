using NUnit.Framework;

using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class FruitTypeCheckTests
{
    private Model model;

    [SetUp]
    public void Setup()
    {
        model = new Model();

        // 設置一個大小為 3x3 的 grid
        model.gridWidth = 8;
        model.gridHeight = 8;

        // 設置 fruitsObjects 和 OnFruitDestroy, OnFruitModelCreate 事件
        model.fruitsObjects = new GameObject[model.gridWidth, model.gridHeight];
        model.OnFruitDestroy = new ReactiveProperty<GameObject>();
        model.OnFruitModelCreate = new ReactiveProperty<(Vector2Int, Vector2, int)>();
        model.OnFruitMatchDestroy = new ReactiveProperty<GameObject>();
        model.boxCollider = new GameObject().AddComponent<BoxCollider2D>();
        model.boxCollider.size = new Vector2(10, 10); // 設置 BoxCollider 大小

        Vector2 min = model.boxCollider.bounds.min; // 取得 BoxCollider 的左下角
        Vector2 size = model.boxCollider.size; // 取得 BoxCollider 的大小
        float cellWidth = size.x / model.gridWidth;  // 單個格子的寬度
        float cellHeight = size.y / model.gridHeight; // 單個格子的高度 (假設擺放在 XZ 平面)
        Vector2 startPos = min + new Vector2(cellWidth / 2, cellHeight / 2);// 設置起始點為 BoxCollider 的左下角

        // 這裡可以先填充一些水果物件進 fruitsObjects，並設定它們的初始狀態
        for (int x = 0; x < model.gridWidth; x++)
        {
            for (int y = 0; y < model.gridHeight; y++)
            {
                // 跳過四個角落
                if ((x == 0 && y == 0) ||
                    (x == 0 && y == model.gridHeight - 1) ||
                    (x == model.gridWidth - 1 && y == 0) ||
                    (x == model.gridWidth - 1 && y == model.gridHeight - 1))
                    continue;

                // 模擬水果物件
                var fruitGameObject = new GameObject();
                var fruit = fruitGameObject.AddComponent<Fruit>();
                fruit.worldPosition = new Vector2(x, y);
                fruit.gridPosition = new Vector2Int(x, y);
                fruit.type = (fruit.gridPosition.x + fruit.gridPosition.y) % model.fruitTypes; // 模擬兩種水果類型 (0 或 1)
                model.fruitsObjects[x, y] = fruitGameObject;
                model.getWorldPosition.Add(new Vector2Int(x, y), startPos + new Vector2(x * cellHeight, y * cellWidth));
            }
        }
    }

    /// <summary>
    ///  確保 fruitsObjects 大小正確
    /// </summary>
    [Test]
    public void Grid_Is_Initialized_Correctly()
    {
        Assert.AreEqual(model.gridWidth, model.fruitsObjects.GetLength(0));
        Assert.AreEqual(model.gridHeight, model.fruitsObjects.GetLength(1));
    }

    /// <summary>
    /// 確保水果種類在正常範圍
    /// </summary>
    [Test]
    public void Fruits_Are_Created_At_Correct_Positions()
    {
        var createdPositions = new List<Vector2Int>();

        foreach(var gameObject in model.fruitsObjects)
        {
            if (gameObject == null)
                continue;

            Fruit fruit = gameObject.GetComponent<Fruit>();
            createdPositions.Add(fruit.gridPosition);
            Assert.IsTrue(fruit.type >= 0 && fruit.type < model.fruitTypes, "水果類型應該在範圍內");
        }
     

        // 2️⃣ 檢查四個角落是否被排除
        Assert.IsFalse(createdPositions.Contains(new Vector2Int(0, 0)), "四個角落不該包含資料");
        Assert.IsFalse(createdPositions.Contains(new Vector2Int(0, model.gridHeight - 1)), "四個角落不該包含資料");
        Assert.IsFalse(createdPositions.Contains(new Vector2Int(model.gridWidth - 1, 0)), "四個角落不該包含資料");
        Assert.IsFalse(createdPositions.Contains(new Vector2Int(model.gridWidth - 1, model.gridHeight - 1)), "四個角落不該包含資料");

        // 3️⃣ 確保事件有正確觸發 (不包含角落的數量)
        int expectedCount = (model.gridWidth * model.gridHeight) - 4;
        Assert.AreEqual(expectedCount, createdPositions.Count, "總水果數量應該少四個");
    }

    /// <summary>
    /// 檢測生成位置是否符合預期
    /// </summary>
    [Test]
    public void World_Position_Is_Calculated_Correctly()
    {


        foreach (var kvp in model.getWorldPosition)
        {
            Vector2Int gridPos = kvp.Key;
            Vector2 worldPos = kvp.Value;

            Vector2 expectedPos = (Vector2)model.boxCollider.bounds.min + new Vector2(
                                   (gridPos.x + 0.5f) * (model.boxCollider.size.x / model.gridWidth),
                                   (gridPos.y + 0.5f) * (model.boxCollider.size.y / model.gridHeight)
                               );

            Assert.AreEqual(expectedPos.x, worldPos.x, 0.001f);
            Assert.AreEqual(expectedPos.y, worldPos.y, 0.001f);
        }
    }



    /// <summary>
    /// 確認水果是否有正確被更換種類
    /// </summary>
    [Test]
    public void FruitTypeCheck_ShouldChangeFruitType_WhenThereIsADuplicate()
    {
        // 模擬 FruitTypeCheck 方法
        model.FruitTypeCheck();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        // 檢查水果是否有被更換類型
        foreach (var fruitGameObject in model.fruitsObjects)
        {
            if (fruitGameObject != null)
            {
                var fruit = fruitGameObject.GetComponent<Fruit>();

                foreach (Vector2Int dir in directions)
                {
                    int newX = fruit.gridPosition.x + dir.x;
                    int newY = fruit.gridPosition.y + dir.y;

                    if (newX >= 0 && newX < model.gridWidth && newY >= 0 && newY < model.gridHeight)
                    {
                        Fruit neighborFruit = model.fruitsObjects[newX, newY]?.GetComponent<Fruit>();
                        if (neighborFruit && fruit.type == neighborFruit.type)
                        {
                            //還是相同種類 測試錯誤
                            Assert.Fail($"水果位置 ({fruit.gridPosition.x}, {fruit.gridPosition.y}) 水果種類還是相同沒有更換.");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 檢查Destroy是否有觸發
    /// </summary>
    [Test]
    public void FruitTypeCheck_ShouldTriggerOnFruitDestroy_WhenChangingFruitType()
    {
        // 記錄 OnFruitDestroy 事件是否被觸發
        bool onFruitDestroyCalled = false;
        model.OnFruitDestroy.Subscribe(_ => onFruitDestroyCalled = true);

        // 模擬 FruitTypeCheck 方法
        model.FruitTypeCheck();

        // 檢查事件是否被觸發
        Assert.IsTrue(onFruitDestroyCalled);
    }

    /// <summary>
    /// 檢查水果數據事件是否被觸發
    /// </summary>
    [Test]
    public void FruitTypeCheck_ShouldTriggerOnFruitModelCreate_WhenChangingFruitType()
    {
        // 記錄 OnFruitModelCreate 事件是否被觸發
        bool onFruitModelCreateCalled = false;
        model.OnFruitModelCreate.Subscribe(_ => onFruitModelCreateCalled = true);

        // 模擬 FruitTypeCheck 方法
        model.FruitTypeCheck();

        // 檢查事件是否被觸發
        Assert.IsTrue(onFruitModelCreateCalled);
    }

    /// <summary>
    ///  檢查交換是否成功
    /// </summary>
    [Test]
    public void Test_ChangePosition_AfterSwap()
    {


        // 取得交換前的位置信息
        Fruit fruit1 = model.fruitsObjects[0, 1].GetComponent<Fruit>();
        Fruit fruit2 = model.fruitsObjects[0, 2].GetComponent<Fruit>();

        Vector2Int oldPosition1 = fruit1.gridPosition;
        Vector2Int oldPosition2 = fruit2.gridPosition;

        // 執行交換
        model.ChangePosition(fruit1, fruit2);

        // 檢查交換後的位置是否互換
        Assert.AreEqual(oldPosition2, fruit1.gridPosition, "fruit1 應該移動到 fruit2 原來的位置");
        Assert.AreEqual(oldPosition1, fruit2.gridPosition, "fruit2 應該移動到 fruit1 原來的位置");
    }

    /// <summary>
    /// 檢查配對的水果是否被消除
    /// </summary>
    [Test]
    public void Test_CheckMatchedFruitDestroy()
    {
        int onEventTriggerCount = 0;
        model.fruitsObjects[0, 1].GetComponent<Fruit>().type = 1;
        model.fruitsObjects[0, 2].GetComponent<Fruit>().type = 1;
        model.fruitsObjects[0, 3].GetComponent<Fruit>().type = 1;
        model.fruitsObjects[0, 4].GetComponent<Fruit>().type = 1;
        model.fruitsObjects[0, 5].GetComponent<Fruit>().type = 1;
        model.OnFruitMatchDestroy.Skip(1).Subscribe(_ => onEventTriggerCount++);

        model.CheckAllFruitMatch();
        model.DestroyMatchFruit();

        // 記錄 OnFruitModelCreate 事件是否被觸發




        // 檢查事件是否被觸發
        Assert.IsTrue(onEventTriggerCount == model.matchedFruits.Count, "應該消除" + model.matchedFruits.Count + "，消除了:" + onEventTriggerCount);
    }

    /// <summary>
    /// 測試水果是否有下墜
    /// </summary>
    [Test]
    public void Test_DroppingFruit()
    {
        model.fruitsObjects[1, 1] = null;
        model.fruitsObjects[2, 1] = null;
        model.fruitsObjects[3, 1] = null;

        model.DroppingFruit();

        Assert.IsTrue(model.fruitsObjects[1, 1] != null && model.fruitsObjects[1, model.gridHeight - 1] == null,"空位沒有下墜的水果，最上方的水果沒有被移動");
        Assert.IsTrue(model.fruitsObjects[2, 1] != null && model.fruitsObjects[2, model.gridHeight - 1] == null, "空位沒有下墜的水果，最上方的水果沒有被移動");
        Assert.IsTrue(model.fruitsObjects[3, 1] != null && model.fruitsObjects[3, model.gridHeight - 1] == null, "空位沒有下墜的水果，最上方的水果沒有被移動");
    }

    /// <summary>
    /// 測試空的位置是否有補齊水果
    /// </summary>
    [Test]
    public void Test_SpawnNewFruits()
    {
        model.fruitsObjects[1, model.gridHeight - 1] = null;
        model.fruitsObjects[2, model.gridHeight - 1] = null;
        model.fruitsObjects[3, model.gridHeight - 1] = null;
   
        int onEventTriggerCount = 0;
        model.OnFruitModelCreate.Skip(1).Subscribe(_ => onEventTriggerCount++);
        model.SpawnNewFruits();
        Assert.IsTrue(onEventTriggerCount == 3, "水果沒有重新生成");


    }

}
