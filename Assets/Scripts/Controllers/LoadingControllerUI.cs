using UnityEngine;
public class LoadingControllerUI : MonoBehaviour
{

    protected virtual void Awake()
    {
    }

    public void Show(bool enabled)
    {
        gameObject.SetActive(enabled);
    }

}