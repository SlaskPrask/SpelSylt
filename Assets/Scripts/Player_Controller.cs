using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : Entity_Controller
{
    protected override void AwakeInit()
    {
        body.drag = SerializedData.GetStat(PlayerStats.DECELERATION);
        transform.localScale = Vector3.one * SerializedData.GetStat(PlayerStats.SIZE);
    }

    private void FixedUpdate()
    {
        body.AddForce(move * SerializedData.GetStat(PlayerStats.ACCELERATION));
        body.velocity = Vector2.ClampMagnitude(body.velocity, SerializedData.GetStat(PlayerStats.MAX_SPEED));
    }

    public void OnMove(InputValue val)
    {
        move = val.Get<Vector2>();
    }
}
