using UnityEngine;
using UnityEngine.UI;

public class EffectView : MonoBehaviour
{

    [Header("消除特效")]
    public GameObject exolodeEffectPrefab;

    [Range(0,1)]
    [Header("特效大小")]
    [SerializeField] float effectSize = 1f;


    [Header("炸彈特效按鈕")]
    public Button exolodeSkillEffectButton;

    [Header("炸彈特效")]
    public GameObject exolodeSkillEffectPrefab;

    [Range(0, 1)]
    [Header("炸彈特效大小")]
    [SerializeField] float exolodeSkillEffectSize;


    public void SpawnEffect(Vector3 position)
    {
        GameObject gameObject = Instantiate(exolodeEffectPrefab, position,Quaternion.identity);
        gameObject.transform.localScale = new Vector3(effectSize, effectSize, effectSize);
    }

    public void SpawnExolodeSkillEffect( Vector3 position)
    {
        GameObject gameObject = Instantiate(exolodeSkillEffectPrefab, position, Quaternion.identity);
        gameObject.transform.localScale = new Vector3(exolodeSkillEffectSize, exolodeSkillEffectSize, exolodeSkillEffectSize);
    }

}
