using UnityEngine;

public class BossWarning : MonoBehaviour
{
    [SerializeField]
    private float warningSeconds = 10.0f;

    [SerializeField]
    private GameObject panelWarning;

    [SerializeField]
    private float blinkLifetime = 0.25f;

    private float warningTimer = 0.0f;
    private float blinkTimer = 0.0f;

    private bool warning = false;

    void Start()
    {
        if(panelWarning != null)
        {
            panelWarning.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(warning == false)
        {
            return;
        }

        blinkTimer += Time.deltaTime;
        if(blinkTimer >= blinkLifetime)
        {
            panelWarning.SetActive(!panelWarning.activeSelf);
            blinkTimer = 0.0f;
        }

        warningTimer += Time.deltaTime;
        if(warningTimer >= warningSeconds)
        {
            panelWarning.SetActive(false);
            warning = false;
        }
    }

    public void StartWarning()
    {
        blinkTimer = 0.0f;
        warningTimer = 0.0f;
        warning = true;
    }
}
