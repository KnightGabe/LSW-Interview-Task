using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SimpleFade : MonoBehaviour
{
    public float fadeDuration = 3;
    private void Start()
    {
        GetComponent<Image>().DOFade(0, 3).From(1);
    }
}
