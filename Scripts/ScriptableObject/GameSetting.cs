using UnityEngine;

[CreateAssetMenu(fileName = "GameSetting", menuName = "Game/GameSetting")]
public class GameSetting : ScriptableObject
{
    public int moveSteps = 15;

    public int maxScore = 300;
    public int goalObject1Count = 5;
    public int goalObject2Count = 5;

    public int itemBombCount = 1;
    public int itemPositionCount = 1;
    public int itemLightingCount = 1;

    public bool isMusicOn ;
    public bool isSoundOn ;
}
