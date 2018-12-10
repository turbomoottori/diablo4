using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class attack : MonoBehaviour {

    public Animator anim;
    //GameObject sword;
    public static float swordTime = 0.2f;
    public static float swordSpinTime = 0.5f;
    bool attacking;
    public static int attackNum;
    public static int activeWeapon = 1;
    public Shoot shoot;
    int bulletCount = 0;
    float shootcd, specialTimer;

    int damage, bullets;
    float speed, rlSpeed, range;
    string type;
    GunType activeType;

    Vector3 bulletPosition;
    Quaternion bulletRotation;
    Vector3[] shotgunBulletPositions;
    Quaternion[] shotgunBulletRotations;

    void Start() {
        //sword = Resources.Load<GameObject>("sword");
        SetBulletPositions();
    }

    void SetBulletPositions()
    {
        bulletPosition = transform.position + transform.forward + (transform.up * 2);
        bulletRotation = transform.rotation;

        shotgunBulletPositions = new Vector3[]
        {
            bulletPosition,
            bulletPosition + transform.right * 0.5f,
            bulletPosition + transform.right * -0.5f,
            bulletPosition + transform.right * 1f,
            bulletPosition + transform.right * -1f,
        };

        shotgunBulletRotations = new Quaternion[]
        {
            bulletRotation,
            bulletRotation *= Quaternion.Euler(Vector3.up * 45),
            bulletRotation *= Quaternion.Euler(Vector3.up * -45),
            bulletRotation *= Quaternion.Euler(Vector3.up * -60),
            bulletRotation *= Quaternion.Euler(Vector3.up * 60)
        };
    }

    void Update() {
        if (!playerMovement.paused)
        {
            //CHANGE ACTIVE WEAPON
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && activeWeapon != 1 && !attacking)
            {
                WeaponChange(1);
                print("weapon 1 selected");
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && activeWeapon != 2 && !attacking)
            {
                WeaponChange(2);
                print("weapon 2 selected");
            }
            //ATTACKING

            if (Input.GetKeyDown(keys.savedKeys.attackKey))
                BasicAttack();

            if (Input.GetKeyDown(keys.savedKeys.spAttackKey))
                SpecialAttack();

            switch (shoot)
            {
                //gun not available
                case Shoot.Off:
                    if (activeWeapon == 1 && items.equippedOne is Gun)
                        shoot = Shoot.Ready;
                    if (activeWeapon == 2 && items.equippedTwo is Gun)
                        shoot = Shoot.Ready;
                    break;

                //ready to shoot
                case Shoot.Ready:

                    if (activeWeapon == 1)
                        WeaponChange(1);
                    else
                        WeaponChange(2);

                    if (Input.GetKeyDown(KeyCode.R) && bulletCount != 0)
                        shoot = Shoot.Reload;

                    if ((activeType == GunType.basic && gameControl.basicAmmo <= 0) || (activeType == GunType.shotgun && gameControl.shotgunAmmo <= 0) || (activeType == GunType.rapid && gameControl.rapidAmmo <= 0))
                        shoot = Shoot.OutOfAmmo;

                    break;

                //normal shooting
                case Shoot.Shooting:
                    attacking = true;
                    anim.SetTrigger("shoot");
                    SetBulletPositions();
                    GameObject b = Instantiate(Resources.Load<GameObject>("bullet"));
                    b.transform.position = bulletPosition;
                    b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                    b.GetComponent<bullet>().maxRange = range;
                    b.GetComponent<bullet>().dmg = damage;

                    bulletCount += 1;
                    gameControl.basicAmmo -= 1;

                    shoot = Shoot.Cooldown;
                    break;

                //rapid fire
                case Shoot.Rapid:

                    if (bulletCount < bullets && !attacking)
                    {
                        attacking = true;
                        anim.SetTrigger("shoot");
                        InvokeRepeating("RapidFire", 0.01f, 1 / speed);
                    }
                    else if (bulletCount >= bullets || gameControl.rapidAmmo <= 0)
                    {
                        shoot = Shoot.Cooldown;
                        CancelInvoke("RapidFire");
                    }

                    break;

                //shotgun
                case Shoot.Shotgun:
                    attacking = true;
                    anim.SetTrigger("shoot");
                    SetBulletPositions();

                    for (int i = 0; i < shotgunBulletRotations.Length; i++)
                    {
                        b = Instantiate(Resources.Load<GameObject>("bullet"));
                        b.transform.position = shotgunBulletPositions[i];
                        b.transform.rotation = shotgunBulletRotations[i];
                        b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                        b.GetComponent<bullet>().maxRange = range;
                        b.GetComponent<bullet>().dmg = damage;
                    }

                    bulletCount += 1;
                    gameControl.shotgunAmmo -= 1;

                    shoot = Shoot.Cooldown;
                    break;

                //special gun attacks
                case Shoot.Special:
                    anim.SetTrigger("shoot");
                    switch (activeType)
                    {
                        case GunType.basic:
                            attacking = true;
                            SetBulletPositions();
                            b = Instantiate(Resources.Load<GameObject>("bullet"));
                            b.transform.position = bulletPosition;
                            b.transform.localScale *= 2;
                            b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                            b.GetComponent<bullet>().maxRange = range;
                            b.GetComponent<bullet>().dmg = damage * 4;

                            bulletCount += bullets;
                            gameControl.basicAmmo -= 2;

                            shootcd *= 2f;
                            shoot = Shoot.Cooldown;
                            break;
                        case GunType.rapid:
                            specialTimer += Time.deltaTime;

                            if (Input.GetButton("Fire2"))
                                RapidFireUnlimited();

                            if (specialTimer >= 3f)
                            {
                                specialTimer = 0;
                                shootcd *= 6;
                                shoot = Shoot.Cooldown;
                            }
                            break;
                        case GunType.shotgun:
                            attacking = true;
                            SetBulletPositions();

                            for (int i = 0; i < shotgunBulletRotations.Length; i++)
                            {
                                b = Instantiate(Resources.Load<GameObject>("bullet"));
                                b.transform.position = shotgunBulletPositions[i];
                                b.transform.rotation = shotgunBulletRotations[i];
                                b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                                b.GetComponent<bullet>().maxRange = range;
                                b.GetComponent<bullet>().dmg = damage;

                                GameObject b2 = Instantiate(Resources.Load<GameObject>("bullet"));
                                b2.transform.position = shotgunBulletPositions[i] + Vector3.up;
                                b2.transform.rotation = shotgunBulletRotations[i];
                                b2.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                                b2.GetComponent<bullet>().maxRange = range;
                                b2.GetComponent<bullet>().dmg = damage;

                                GameObject b3 = Instantiate(Resources.Load<GameObject>("bullet"));
                                b3.transform.position = shotgunBulletPositions[i] + (Vector3.up * 2);
                                b3.transform.rotation = shotgunBulletRotations[i];
                                b3.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                                b3.GetComponent<bullet>().maxRange = range;
                                b3.GetComponent<bullet>().dmg = damage;
                            }

                            bulletCount += bullets;
                            gameControl.shotgunAmmo -= 2;

                            shootcd *= 3f;
                            shoot = Shoot.Cooldown;
                            break;
                    }
                    break;

                //reloading
                case Shoot.Cooldown:
                    attacking = false;

                    if ((activeType == GunType.basic && gameControl.basicAmmo <= 0) || (activeType == GunType.shotgun && gameControl.shotgunAmmo <= 0) || (activeType == GunType.rapid && gameControl.rapidAmmo <= 0))
                        shoot = Shoot.OutOfAmmo;

                    if (bulletCount >= bullets)
                    {
                        shootcd -= Time.deltaTime;
                        if (shootcd <= 0)
                        {
                            shootcd = rlSpeed;
                            bulletCount = 0;
                            shoot = Shoot.Ready;
                        }
                    }
                    else
                    {
                        shoot = Shoot.Ready;
                    }
                    break;

                //manual reload
                case Shoot.Reload:
                    shootcd -= Time.deltaTime;
                    if (shootcd <= 0)
                    {
                        shootcd = rlSpeed;
                        bulletCount = 0;
                        shoot = Shoot.Ready;
                    }

                    break;
                case Shoot.OutOfAmmo:
                    if ((activeType == GunType.basic && gameControl.basicAmmo > 0) || (activeType == GunType.shotgun && gameControl.shotgunAmmo > 0) || (activeType == GunType.rapid && gameControl.rapidAmmo > 0))
                        shoot = Shoot.Ready;
                    break;
            }
        }
    }

    //changes currently equipped weapon
    void WeaponChange(int active)
    {
        if (active == 1)
        {
            activeWeapon = 1;
            if (items.equippedOne != null)
            {
                damage = items.equippedOne.damage;
                speed = items.equippedOne.speed;
                if (items.equippedOne is Gun)
                {
                    Gun g = items.equippedOne as Gun;
                    SetGunProperties(g);
                }
            }
        }
        else
        {
            activeWeapon = 2;
            if (items.equippedTwo != null)
            {
                damage = items.equippedTwo.damage;
                speed = items.equippedTwo.speed;
                if (items.equippedTwo is Gun)
                {
                    Gun g = items.equippedTwo as Gun;
                    SetGunProperties(g);
                }
            }
        }
    }

    void SetGunProperties(Gun g)
    {
        range = g.range;
        activeType = g.type;

        if (g.type == GunType.basic)
        {
            rlSpeed = 0.5f;
            shootcd = 0.5f;
            bullets = 5;
        }
        else if (g.type == GunType.rapid)
        {
            rlSpeed = 0.75f;
            shootcd = 0.75f;
            bullets = 10;
        }
        else
        {
            rlSpeed = 1f;
            shootcd = 1f;
            bullets = 2;
        }
    }

    void BasicAttack()
    {

        if (activeWeapon == 1)
        {
            if (items.equippedOne == null)
            {
                //no weapon
                print("no weapon");
            }
            else if(items.equippedOne is Gun)
            {
                if (shoot == Shoot.Ready)
                {
                    Gun g = items.equippedOne as Gun;
                    if (g.type == GunType.basic)
                        shoot = Shoot.Shooting;
                    else if (g.type == GunType.rapid)
                        shoot = Shoot.Rapid;
                    else
                        shoot = Shoot.Shotgun;
                }
            }
            else
            {
                StartCoroutine(Attack(Vector3.up, 90f, speed));
            }
        }
        else if (activeWeapon == 2)
        {
            if (items.equippedTwo == null)
            {
                //no weapon
                print("no weapon");
            }
            else if (items.equippedTwo is Gun)
            {
                if (shoot == Shoot.Ready)
                {
                    Gun g = items.equippedTwo as Gun;
                    if (g.type == GunType.basic)
                        shoot = Shoot.Shooting;
                    else if (g.type == GunType.rapid)
                        shoot = Shoot.Rapid;
                    else
                        shoot = Shoot.Shotgun;
                }
            }
            else
            {
                StartCoroutine(Attack(Vector3.up, 90f, speed));
            }
        }
    }

    void SpecialAttack()
    {
        if (activeWeapon == 1)
        {
            if (items.equippedOne == null)
            {
                print("no weapon");
            }
            else if(items.equippedOne is Gun)
            {
                if (shoot == Shoot.Ready)
                    shoot = Shoot.Special;
            }
            else
            {
                StartCoroutine(AttackTwo(Vector3.up, swordSpinTime));
            }
        }
        else if (activeWeapon == 2)
        {
            if (items.equippedTwo == null)
            {
                print("no weapon");
            }
            else if (items.equippedTwo is Gun)
            {
                if (shoot == Shoot.Ready)
                    shoot = Shoot.Special;
            }
            else
            {
                StartCoroutine(AttackTwo(Vector3.up, swordSpinTime));
            }
        }
    }

    void RapidFire()
    {
        attacking = true;
        Vector3 bPos = transform.position + transform.forward;
        GameObject b = Instantiate(Resources.Load<GameObject>("bullet"));
        b.transform.position = bPos;
        b.GetComponent<Rigidbody>().AddForce(transform.forward * 300 * speed);
        b.GetComponent<bullet>().maxRange = range;
        b.GetComponent<bullet>().dmg = damage;
        bulletCount += 1;
        gameControl.rapidAmmo -= 1;
    }

    void RapidFireUnlimited()
    {
        attacking = true;
        Vector3 bPos = transform.position + transform.forward;
        GameObject b = Instantiate(Resources.Load<GameObject>("bullet"));
        b.transform.position = bPos;
        b.GetComponent<Rigidbody>().AddForce(transform.forward * 300 * speed);
        b.GetComponent<bullet>().maxRange = range;
        b.GetComponent<bullet>().dmg = damage;
    }

    //basic attack
    IEnumerator Attack(Vector3 axis, float angle, float time)
    {
        //FIX FIX FIX FIX FIX 
        if (attacking)
            yield break;
        attacking = true;
        anim.SetTrigger("attack");
        attackNum = 1;
        GameObject sw = Instantiate(Resources.Load("sword") as GameObject, GameObject.Find("Ranne.R.001").transform);
        sw.transform.localPosition = Vector3.zero;
        sw.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        yield return new WaitForSeconds(1);
        Destroy(sw);
        attacking = false;
        yield return null;
    }

    //spin attack
    IEnumerator AttackTwo(Vector3 axis, float time)
    {
        if (attacking)
            yield break;
        attacking = true;
        attackNum = 2;
        GameObject sw = Instantiate(Resources.Load("sword") as GameObject, GameObject.Find("Ranne.R.001").transform);
        float from = transform.eulerAngles.y;
        float to = from + 360f;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float yRot = Mathf.Lerp(from, to, t / time) % 360f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRot, transform.eulerAngles.z);
            yield return null;
        }
        Destroy(sw);
        attacking = false;
        yield return null;
    }

    public enum Shoot
    {
        Off,
        Ready,
        Shooting,
        Rapid,
        Shotgun,
        Special,
        Cooldown,
        Reload,
        OutOfAmmo
    }
}
