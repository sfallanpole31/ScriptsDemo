using UnityEngine;

public class Fruit : MonoBehaviour
{
    public Vector2Int gridPosition;  // �ϥ� [SerializeField] ��ܨp���ܶq
    public Vector2 worldPosition;
    public int type;

    public void Init(Vector2Int gridPos, Vector2 worldPos, int type)
    {
        gridPosition = gridPos;
        worldPosition = worldPos;
        this.type = type;
    }
}