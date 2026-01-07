using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject panelClear;

    [SerializeField]
    private GameObject panelGameOver;

    [SerializeField]
    private GameObject panelPause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(panelClear != null)
        {
            panelClear.SetActive(false);
        }

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        SetPanelPauseVisible(false);
    }

    public void OpenClear()
    {
        if (panelClear != null)
        {
            panelClear.SetActive(true);
        }
    }

    public void OpenGameOver()
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
    }

    public void SetPanelPauseVisible(bool visible)
    {
        if (panelPause != null)
        {
            panelPause.SetActive(visible);
        }
    }
}
