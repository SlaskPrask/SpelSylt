using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using FMODUnity;

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
    [HideInInspector] public UnityEvent<float, float> onHealthChange = new UnityEvent<float, float>();
    [HideInInspector] public UnityEvent<PowerUp_Object> onAbsorbedItem = new UnityEvent<PowerUp_Object>();
    [HideInInspector] public UnityEvent onNext = new UnityEvent();
    [HideInInspector] public UnityEvent<int> onNumberInput = new UnityEvent<int>();
    [HideInInspector] public UnityEvent<int> onDigest = new UnityEvent<int>();
    
    public bool dead;
    private float invincibility;
    
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
        invincibility = 0f;
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
        if (isAbsorbing && overlappedPowerups.Count > 0)
        {
            Stack<byte> delete = new Stack<byte>();
            for (byte i = 0; i < overlappedPowerups.Count; i++)
            {
                if (SerializedData.PowerCount >= 9)
                    break;

                gotoSize += .7f;
                PowerUp_Object obj = overlappedPowerups[i].GetComponent<PowerUp_Object>();
                AbsorbPower(obj);
                RuntimeManager.PlayOneShotAttached("event:/SFX/Absorb", gameObject);
                onAbsorbedItem.Invoke(obj);
                Destroy(overlappedPowerups[i]);
                delete.Push(i);
            }

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

    public void ForceDigest(int index)
    {
        SerializedData.RemovePowerUp(index);
        powerupVisuals.RemovePowerUp(index);
        Heal(1);
        onDigest.Invoke(index);
        gotoSize -= .7f;
        if (sizeRoutine != null)
        {
            StopCoroutine(sizeRoutine);
        }
        sizeRoutine = StartCoroutine(UpdateSize());
        RuntimeManager.PlayOneShotAttached("Event:/SFX/Digest", gameObject);
    }

    public void OnDigest(InputValue val)
    {
        int powers = SerializedData.PowerCount;
        if (powers == 0)
            return;

        if (SerializedData.GetStat(PlayerStats.SELECTED_SLOT) == powers - 1 || SerializedData.SelectedPowerupType() == PowerUpType.KEY_ITEM)
        {
            //Error noise
            RuntimeManager.PlayOneShot("Event:/SFX/NoPoop");
            return;
        }
        else
        {
            SerializedData.RemoveSelectedPowerUp();
            powerupVisuals.RemovePowerUp((int)SerializedData.GetStat(PlayerStats.SELECTED_SLOT));
            Heal(1);
            onDigest.Invoke((int)SerializedData.GetStat(PlayerStats.SELECTED_SLOT));
            gotoSize -= .7f;
            if (sizeRoutine != null)
            {
                StopCoroutine(sizeRoutine);
            }
            sizeRoutine = StartCoroutine(UpdateSize());
            RuntimeManager.PlayOneShotAttached("Event:/SFX/Digest", gameObject);
        }
    }

    public void OnNextItem(InputValue val)
    {
        onNext.Invoke();
    }

    public void On_1(InputValue val)
    {
        onNumberInput.Invoke(0);
    }

    public void On_2(InputValue val)
    {
        onNumberInput.Invoke(1);
    }


    public void On_3(InputValue val)
    {
        onNumberInput.Invoke(2);
    }

    public void On_4(InputValue val)
    {
        onNumberInput.Invoke(3);
    }

    public void On_5(InputValue val)
    {
        onNumberInput.Invoke(4);
    }

    public void On_6(InputValue val)
    {
        onNumberInput.Invoke(5);
    }

    public void On_7(InputValue val)
    {
        onNumberInput.Invoke(6);
    }

    public void On_8(InputValue val)
    {
        onNumberInput.Invoke(7);
    }

    public void On_9(InputValue val)
    {
        onNumberInput.Invoke(8);
    }

    public void OnPause(InputValue val)
    {
        GameManager.PauseGame();
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
            case 10: //Enemy
            case 11: //Neutral Damage
                if (invincibility > 0f)
                    return;

                if (collision.TryGetComponent<IDamageSource>(out IDamageSource dmgSource))
                {
                    Vector2 knockDir = transform.position - collision.transform.position;
                    Damage(dmgSource.GetDamage(), 10, knockDir.normalized);
                }
                else
                    return;

                break;
            default:
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 8: //power up
                break;
            case 9: //Enemy Bullet
            case 10: //Enemy
            case 11: //Neutral Damage
                if (invincibility > 0f)
                    return;

                if (collision.TryGetComponent<IDamageSource>(out IDamageSource dmgSource))
                {
                    Vector2 knockDir = transform.position - collision.transform.position;
                    Damage(dmgSource.GetDamage(), 10, knockDir.normalized);
                }
                else
                    return;
                break;
            default:
                break;
        }
    }

    public void Damage(float value, float knockback, Vector2 knockDir)
    {

        float hp = SerializedData.GetStat(PlayerStats.CURRENT_HP);
        float prevHp = hp;
        float maxHP = SerializedData.GetStat(PlayerStats.MAX_HP);
        
        hp = Mathf.Clamp(hp - value, 0f, maxHP);
        SerializedData.UpdateStat(PlayerStats.CURRENT_HP, hp);
        onHealthChange.Invoke(hp, maxHP);

        if (hp <= 0f && !dead)
        {
            RuntimeManager.PlayOneShotAttached("Event:/SFX/BlobDeath2", gameObject);
            dead = true;
        }
        else if (hp < prevHp)
        {
            RuntimeManager.PlayOneShotAttached("Event:/SFX/GoopHurt", gameObject);
            body.AddForce(knockDir * Mathf.Max(knockback - SerializedData.GetStat(PlayerStats.KNOCKBACK_RESISTANCE), 0f), ForceMode2D.Impulse);
            StartCoroutine(Invincibility());
        }
    }

    public void Heal(float value)
    {
        float hp = SerializedData.GetStat(PlayerStats.CURRENT_HP);
        float prevHp = hp;
        float maxHP = SerializedData.GetStat(PlayerStats.MAX_HP);

        hp = Mathf.Clamp(hp + value, 0, maxHP);
        SerializedData.UpdateStat(PlayerStats.CURRENT_HP, hp);
        onHealthChange.Invoke(hp, maxHP);
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
        SerializedData.UpdateStat(PlayerStats.SIZE, gotoSize);
        float t = 0;

        do
        {
            transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, gotoSize, t / .25f);
            powerupVisuals.UpdateScales();
            t += Time.deltaTime;
            yield return null;

        } while (t < .25f);
    }

    IEnumerator Invincibility()
    {
        invincibility = SerializedData.GetStat(PlayerStats.INVINCIBILITY_TIME);
        Color c;
        int count = 0;
        while (invincibility > 0f)
        {
            c = renderer.color;
            //.5->0->.5->1 repeat
            invincibility -= Time.deltaTime;
            count = (count + 1) % 4;
            yield return null;
            c.a = Mathf.Max((count * .5f - count / 3) - .25f, 0f); //0 .5 1 .5 
            renderer.color = c;
        }
        c = renderer.color;
        c.a = .75f;
        renderer.color = c;
    }
}
