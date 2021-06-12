using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Player_Controller : Entity_Controller
{
    private MaterialPropertyBlock matProps;
    private float angle;
    private float animationTime;
    public float baseAnimTime = 10;
    private float velMagnitude;
    [SerializeField]private bool isAbsorbing;
    private List<GameObject> overlappedPowerups;
    public PowerUpContainer powerupVisuals;
    private float gotoSize;
    private Coroutine sizeRoutine;
    public UnityEvent<float, float> onHealthChange = new UnityEvent<float, float>();
    private bool dead;
    protected override void AwakeInit()
    {
        body.drag = SerializedData.GetStat(PlayerStats.DECELERATION);
        gotoSize = SerializedData.GetStat(PlayerStats.SIZE);
        transform.localScale = Vector3.one * gotoSize;
        matProps = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(matProps);
        isAbsorbing = false;
        overlappedPowerups = new List<GameObject>();
        dead = false;
    }

    private void FixedUpdate()
    {
        if (dead)
        {
            body.velocity = Vector2.zero;
            return;
        }
        body.AddForce(move * SerializedData.GetStat(PlayerStats.ACCELERATION));
        body.velocity = Vector2.ClampMagnitude(body.velocity, SerializedData.GetStat(PlayerStats.MAX_SPEED));
    }

    private void Update()
    {
        if (dead)
        {
            return;
        }

        velMagnitude = body.velocity.magnitude;
        HandleShots();
        HandleAbsorb();
    }

    private void HandleAbsorb()
    {
        if (isAbsorbing)
        {
            for (int i = overlappedPowerups.Count - 1; i >= 0; i--)
            {
                gotoSize += .3f;
                AbsorbPower(overlappedPowerups[i].GetComponent<PowerUp_Object>());
                Destroy(overlappedPowerups[i]);
            }
            overlappedPowerups.Clear();
            powerupVisuals.UpdateScales();
            if (sizeRoutine != null)
            {
                StopCoroutine(sizeRoutine);
            }
            sizeRoutine = StartCoroutine(UpdateSize());
        }
    }

    private void AbsorbPower(PowerUp_Object power)
    {
        SerializedData.AddPowerUp(power.powerup.power);
        powerupVisuals.CreatePowerUpVisual(power.powerup.sprite);
    }

    private void HandleShots()
    {
        if (shoot.magnitude > .1f)
        {
            Vector2 shotDir;
            if (velMagnitude > .05f)
            {
                shotDir = (shoot + body.velocity.normalized * .1f).normalized;
            }
            else
            {
                shotDir = shoot;
            }

            List<PowerUp_ExtraShot> shots = SerializedData.GetShots();
            for (int i = 0; i < shots.Count; i++)
            {
                shots[i].Shoot(this, shotDir);
            }
        }
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

    private void OnShoot(InputValue val)
    {
        shoot = val.Get<Vector2>();
    }

    private void LateUpdate()
    {
        if (dead)
        {
            return;
        }


        Vector2 velocity;
        if (velMagnitude < 0.05f)
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
        CalculateAnimationTime(velMagnitude);
        matProps.SetVector("_Velocity", velocity);
        matProps.SetFloat("_AnimationTime", animationTime);
        renderer.SetPropertyBlock(matProps);
    }

    public void OnMove(InputValue val)
    {
        move = val.Get<Vector2>();
    }

    public void OnAbsorb(InputValue val)
    {
        isAbsorbing = val.Get<float>() > .5f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 8: //power up
                
                if (!overlappedPowerups.Contains(collision.gameObject))
                {
                    overlappedPowerups.Add(collision.gameObject);
                }
                break;
            case 9: //Enemy Bullet
            case 11: //Neutral Damage
                Damage(.5f);
                break;
            default:
                break;
        }
    }

    public void Damage(float value)
    {
        float hp = SerializedData.GetStat(PlayerStats.CURRENT_HP);
        float maxHP = SerializedData.GetStat(PlayerStats.MAX_HP);
        hp = Mathf.Clamp(hp - value, 0f, maxHP);
        SerializedData.UpdateStat(PlayerStats.CURRENT_HP, hp);
        onHealthChange.Invoke(hp, maxHP);

        if (hp <= 0f)
        {
            dead = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 8: //power up

                if (overlappedPowerups.Contains(collision.gameObject))
                {
                    overlappedPowerups.Remove(collision.gameObject);
                }
                break;
            default:
                break;
        }
    }

    IEnumerator UpdateSize()
    {
        float t = 0;

        do
        {
            transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, gotoSize, t / .25f);

            t += Time.deltaTime;
            yield return null;

        } while (t < .25f);
    }
}
