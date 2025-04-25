using UnityEngine;

public class Quit : MonoBehaviour
{
    public void ExitGame()
    {
            #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false; // 編輯器中停止遊戲
            #else
                    Application.Quit(); // 打包後退出遊戲
            #endif
    }
}
