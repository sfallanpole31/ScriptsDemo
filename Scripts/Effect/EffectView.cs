using UnityEngine;
using UnityEngine.UI;

public class EffectView : MonoBehaviour
{

    [Header("�����S��")]
    public GameObject exolodeEffectPrefab;

    [Range(0,1)]
    [Header("�S�Ĥj�p")]
    [SerializeField] float effectSize = 1f;


    [Header("���u�S�ī��s")]
    public Button exolodeSkillEffectButton;

    [Header("���u�S��")]
    public GameObject exolodeSkillEffectPrefab;

    [Range(0, 1)]
    [Header("���u�S�Ĥj�p")]
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
