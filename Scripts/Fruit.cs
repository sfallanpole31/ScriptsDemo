using UnityEngine;

public class Fruit : MonoBehaviour
{
    public Vector2Int gridPosition;  // 使用 [SerializeField] 顯示私有變量
    public Vector2 worldPosition;
    public int type;

    public void Init(Vector2Int gridPos, Vector2 worldPos, int type)
    {
        gridPosition = gridPos;
        worldPosition = worldPos;
        this.type = type;
    }
}