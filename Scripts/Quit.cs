using UnityEngine;

public class Quit : MonoBehaviour
{
    public void ExitGame()
    {
            #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false; // �s�边������C��
            #else
                    Application.Quit(); // ���]��h�X�C��
            #endif
    }
}
