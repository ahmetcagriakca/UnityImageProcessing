using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour
{
    [SerializeField] Image image;

    public float animationSpeed = 1f;
    public float rotationSpeed = 1f;

    const float maxFillAmount = 0.5f;

    private void Awake()
    {
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Radial360;
    }

    private void Update()
    {
        image.transform.Rotate(Vector3.forward, -360f * Time.deltaTime * rotationSpeed);
    }

    IEnumerator DOFillAmount(Image target, float endValue, float duration)
    {
        yield return target.DOFillAmount(endValue, duration).SetEase(Ease.Linear);
    }
    IEnumerator Animate()
    {

        yield return DoFillAmount1();

    }

    private IEnumerator DoFillAmount1()
    {
        image.fillClockwise = true;
        image.transform.localScale = Vector3.one;
        image.fillAmount = 0;

        yield return DOTweenModuleUI.DOFillAmount(image, maxFillAmount, animationSpeed);

        yield return DoFillAmount2();
    }

    private IEnumerator DoFillAmount2()
    {
        image.fillClockwise = false;
        image.transform.localScale = new Vector3(-1, -1, 1);
        yield return DOTweenModuleUI.DOFillAmount(image, maxFillAmount, animationSpeed);

        yield return DoFillAmount2();
    }


    private IEnumerator DoFillAmount3()
    {
        image.fillClockwise = true;

        yield return DOTweenModuleUI.DOFillAmount(image, maxFillAmount, animationSpeed);
        yield return DoFillAmount4();
    }
    private IEnumerator DoFillAmount4()
    {
        image.fillClockwise = false;
        image.transform.localScale = Vector3.one;

        yield return DOTweenModuleUI.DOFillAmount(image, 0, animationSpeed);

        yield return Animate();
    }
    private void OnEnable()
    {
        image.fillAmount = 0;
        image.fillClockwise = true;

        StartCoroutine(Animate());
    }


    private void OnDisable()
    {
        image.DOKill();
    }

}