using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : Entity_Controller
{
    private MaterialPropertyBlock matProps;
    private float angle;
    private float animationTime;
    public float baseAnimTime = 10;

    protected override void AwakeInit()
    {
        body.drag = SerializedData.GetStat(PlayerStats.DECELERATION);
        transform.localScale = Vector3.one * SerializedData.GetStat(PlayerStats.SIZE);
        matProps = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(matProps);
    }

    private void FixedUpdate()
    {
        body.AddForce(move * SerializedData.GetStat(PlayerStats.ACCELERATION));
        body.velocity = Vector2.ClampMagnitude(body.velocity, SerializedData.GetStat(PlayerStats.MAX_SPEED));
    }


    private void CalculateAnimationTime(float mag)
    {
        if (mag < .05f)
        {
            animationTime = Mathf.Max(animationTime - Time.deltaTime * baseAnimTime, 0f);
        }
        else
        {
            animationTime = (animationTime + Time.deltaTime * baseAnimTime * mag) % (Mathf.PI * 2);
        }
    }

    private void LateUpdate()
    {
        float velMag = body.velocity.magnitude;
        Vector2 velocity;
        if (velMag < 0.05f)
        {
            velocity = Vector2.zero;
        }
        else
        {
            velocity = body.velocity.normalized;
            float desiredAngle = Mathf.Atan2(body.velocity.y, body.velocity.x) * Mathf.Rad2Deg - 90;
            angle = Mathf.LerpAngle(angle, desiredAngle, Time.deltaTime * 10);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        CalculateAnimationTime(velMag);
        matProps.SetVector("_Velocity", velocity);
        matProps.SetFloat("_AnimationTime", animationTime);
        renderer.SetPropertyBlock(matProps);
    }

    public void OnMove(InputValue val)
    {
        move = val.Get<Vector2>();
    }
}
