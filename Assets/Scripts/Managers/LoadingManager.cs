using UnityEngine;

public class LoadingManager : SingleInstance<LoadingManager>
{
    [SerializeField] LoadingControllerUI LoadingControllerUi;
    private async void Awake()
    {
        ShowLoading(false);
    }

    public void ShowLoading(bool enabled)
    {
        LoadingControllerUi.Show(enabled);
    }
}
