using System.Linq.Expressions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Rigidbody2D body;
    public GameManager gameManager;

    private Vector2 inputDirection;

    private Vector2 knockbackVelocity;
    public float knockbackDamping = 8.0f;

    public AnimationController animController;
    public SpriteRenderer sr;

    // Update is called once per frame
    void Update()
    {
        bool canMove = false;
        if(gameManager != null)
        {
            canMove = gameManager.IsPlaying();
        }

        if(canMove == true)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            Vector2 raw = new Vector2(x, y);
            if(raw.sqrMagnitude > 1.0f)
            {
                raw = raw.normalized;
            }

            inputDirection = raw;
        }
        else
        {
            inputDirection = Vector2.zero;
        }

        if(animController != null)
        {
            animController.PlayIdleOrMove(inputDirection.magnitude);
        }

        UpdateDirection();
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = inputDirection * moveSpeed;

        if(knockbackVelocity.sqrMagnitude > 0.0001f)
        {
            float damp = Mathf.Exp(-knockbackDamping * Time.fixedDeltaTime);
            knockbackVelocity = knockbackVelocity * damp;
        }
        else
        {
            knockbackVelocity = Vector2.zero;
        }

        Vector2 finalVel = targetVelocity + knockbackVelocity;
        //body.linearVelocity = targetVelocity;
        body.linearVelocity = finalVel;
    }

    public void SetMoveSpeed(float value)
    {
        moveSpeed = value;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void AddKnockback(Vector2 force)
    {
        knockbackVelocity += force;
    }

    void UpdateDirection()
    {
        if(sr != null)
        {
            if(inputDirection.x > 0.0f)
            {
                sr.flipX = false;
            }
            else if(inputDirection.x < 0.0f)
            {
                sr.flipX = true;
            }
        }
    }
}
