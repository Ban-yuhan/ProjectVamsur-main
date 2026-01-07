using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerDash : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer playerSpriteRenderer;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private PlayerMovement movementScriptToDisable;

    [SerializeField]
    private KeyCode dashKey = KeyCode.LeftControl;

    [SerializeField]
    private float dashSpeed = 15.0f;

    [SerializeField]
    private float dashDurationSec = 0.2f;

    [SerializeField]
    private float dashCooldownSec = 0.5f;

    [SerializeField]
    private bool allowVerticalDash = true;

    [SerializeField]
    private GameObject afterImagePrefab;

    [SerializeField]
    private float afterImageIntervalSec = 0.05f;

    [SerializeField]
    private float afterImageLifeSec = 0.15f;

    [SerializeField]
    private float afterImageStartAlpha = 0.5f;

    [SerializeField]
    private float afterImageEndAlpha = 0.0f;

    [SerializeField]
    private PlayerWeaponStatsRuntime stats;


    private bool isDashing;
    private float dashEndTime;
    private float nextDashReadyTime;
    private Vector2 dashDirection;
    private Vector2 lastMoveDirection;
    private float afterImageNextSpawnTime;

    private List<DashAfterImage> afterImagePool = new List<DashAfterImage>();

    public event Action OnDashStarted;

    public bool IsDashing()
    {
        return isDashing;
    }

    private void Awake()
    {
        lastMoveDirection = Vector2.right;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLastMoveDirection();

        if(Input.GetKeyDown(dashKey) == true)
        {
            TryStartDash();
        }
    }

    private void FixedUpdate()
    {
        if (stats == null)
        {
            return;
        }

        if (isDashing == false)
        {
            return;
        }

        if(Time.time >= dashEndTime)
        {
            EndDash();
        }

        dashSpeed = stats.GetDashSpeed();

        if(body != null)
        {
            body.linearVelocity = dashDirection * dashSpeed;
        }

        TrySpawnAfterImage();
    }

    void StartDash()
    {
        if(stats == null)
        {
            return;
        }

        dashDurationSec = stats.GetDashDurationSec();

        isDashing = true;

        dashEndTime = Time.time + dashDurationSec;
        nextDashReadyTime = Time.time + dashCooldownSec;

        afterImageNextSpawnTime = Time.time;

        if(movementScriptToDisable != null)
        {
            movementScriptToDisable.enabled = false;
        }

        dashSpeed = stats.GetDashSpeed();

        if(body != null)
        {
            body.linearVelocity = dashDirection * dashSpeed;
        }

        if(OnDashStarted != null)
        {
            OnDashStarted.Invoke();
        }
    }

    void EndDash()
    {
        isDashing = false;

        if(movementScriptToDisable != null)
        {
            movementScriptToDisable.enabled = true;
        }

        if(body != null)
        {
            body.linearVelocity = Vector2.zero;
        }
    }

    Vector2 ChooseDashDirection()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if(allowVerticalDash == false)
        {
            y = 0.0f;
        }

        Vector2 inputDir = new Vector2(x, y);

        if(inputDir.sqrMagnitude > 0.01f)
        {
            return inputDir;
        }

        return lastMoveDirection;
    }

    void TryStartDash()
    {
        if(isDashing == true)
        {
            return;
        }

        if(Time.time < nextDashReadyTime)
        {
            return;
        }

        if(body == null)
        {
            return;
        }

        dashDirection = ChooseDashDirection().normalized;

        StartDash();
    }

    void UpdateLastMoveDirection()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (allowVerticalDash == false)
        {
            y = 0.0f;
        }

        Vector2 inputDir = new Vector2(x, y);

        if (inputDir.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = inputDir.normalized;
        }
    }

    DashAfterImage GetAfterImageFromPool()
    {
        for(int i=0; i<afterImagePool.Count; ++i)
        {
            DashAfterImage item = afterImagePool[i];

            if(item.gameObject.activeSelf == false)
            {
                return item;
            }
        }

        GameObject obj = Instantiate(afterImagePrefab);
        if(obj == null)
        {
            return null;
        }

        DashAfterImage img = obj.GetComponent<DashAfterImage>();
        obj.SetActive(false);
        afterImagePool.Add(img);

        return img;
    }

    void TrySpawnAfterImage()
    {
        if(Time.time < afterImageNextSpawnTime)
        {
            return;
        }

        afterImageNextSpawnTime = Time.time + afterImageIntervalSec;

        DashAfterImage img = GetAfterImageFromPool();
        if(img == null)
        {
            return;
        }

        Transform t = img.transform;
        t.position = transform.position;
        t.rotation = transform.rotation;
        t.localScale = transform.localScale;

        Sprite sprite = playerSpriteRenderer.sprite;
        bool flipX = playerSpriteRenderer.flipX;
        bool flipY = playerSpriteRenderer.flipY;
        string layerName = playerSpriteRenderer.sortingLayerName;
        int order = playerSpriteRenderer.sortingOrder;

        img.gameObject.SetActive(true);

        img.Initialize(sprite, flipX, flipY, layerName, order, afterImageLifeSec, afterImageStartAlpha, afterImageEndAlpha);        
    }
}
