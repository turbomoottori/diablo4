using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour {

    GameObject sword;
    public static float swordTime = 0.2f;
    public static float swordSpinTime = 0.5f;
    bool swordActive, attacking;
    public static int attackNum;
    public static int activeWeapon = 1;
    public Shoot shoot;
    int bulletCount = 0;
    float shootcd, specialTimer;

    int damage, bullets;
    float speed, rlSpeed, range;
    string type, special;

    void Start () {
        sword = Resources.Load<GameObject>("sword");
    }
	
	void Update () {

        if (!playerMovement.paused)
        {
            //CHANGE ACTIVE WEAPON
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && activeWeapon != 1 && !attacking)
            {
                activeWeapon = 1;

                if (weapons.weaponType1 != 2)
                {
                    //weapon 1 is sword
                    WeaponChange(1, false);
                    shoot = Shoot.Off;
                }
                else
                {
                    //weapon 1 is gun
                    shoot = Shoot.Cooldown;
                    WeaponChange(1, true);
                } 

                print("weapon 1 selected");
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && activeWeapon != 2 && !attacking)
            {
                activeWeapon = 2;

                if (weapons.weaponType2 != 2)
                {
                    //weapon 2 is sword
                    WeaponChange(2, false);
                    shoot = Shoot.Off;
                }
                else
                {
                    //weapon 2 is gun
                    shoot = Shoot.Cooldown;
                    WeaponChange(2, true);
                }

                print("weapon 2 selected");
            }
            //ATTACKING

            if (Input.GetButtonDown("Fire1"))
                BasicAttack();

            if (Input.GetButtonDown("Fire2"))
                SpecialAttack();

            switch (shoot)
            {
                //gun not available
                case Shoot.Off:
                    if (activeWeapon == 1 && weapons.weaponType1 == 2)
                        shoot = Shoot.Ready;
                    if (activeWeapon == 2 && weapons.weaponType2 == 2)
                        shoot = Shoot.Ready;
                    break;

                //ready to shoot
                case Shoot.Ready:

                    if (activeWeapon == 1)
                        WeaponChange(1, true);
                    else
                        WeaponChange(2, true);

                    if (Input.GetKeyDown(KeyCode.R) && bulletCount != 0)
                        shoot = Shoot.Reload;

                    break;

                //normal shooting
                case Shoot.Shooting:
                    attacking = true;
                    
                    Vector3 bPos = transform.position + transform.forward;
                    GameObject b = Instantiate(Resources.Load<GameObject>("bullet"));
                    b.transform.position = bPos;
                    b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                    b.GetComponent<bullet>().maxRange = range;
                    b.GetComponent<bullet>().dmg = damage;

                    bulletCount += 1;
                    shoot = Shoot.Cooldown;
                    break;

                //rapid fire
                case Shoot.Rapid:

                    if (bulletCount < bullets && !attacking)
                    {
                        attacking = true;
                        InvokeRepeating("RapidFire", 0.01f, 1 / speed);
                    }
                    else if (bulletCount >= bullets)
                    {
                        shoot = Shoot.Cooldown;
                        CancelInvoke("RapidFire");
                    }
                    
                    break;

                //shotgun
                case Shoot.Shotgun:
                    attacking = true;

                    bPos = transform.position + transform.forward;
                    Quaternion bRot = transform.rotation;

                    //position for each bullet
                    Vector3[] posB = new Vector3[] {
                        bPos,
                        bPos + transform.right * 0.5f,
                        bPos + transform.right * -0.5f,
                        bPos + transform.right * 1f,
                        bPos + transform.right * -1f };

                    //rotation for each bullet
                    Quaternion[] rotB= new Quaternion[] {
                        bRot,
                        bRot *= Quaternion.Euler(Vector3.up * 45),
                        bRot *= Quaternion.Euler(Vector3.up * -45),
                        bRot *= Quaternion.Euler(Vector3.up * -60),
                        bRot *= Quaternion.Euler(Vector3.up * 60) };

                    for(int i = 0; i < rotB.Length; i++)
                    {
                        b = Instantiate(Resources.Load<GameObject>("bullet"));
                        b.transform.position = posB[i];
                        b.transform.rotation = rotB[i];
                        b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                        b.GetComponent<bullet>().maxRange = range;
                        b.GetComponent<bullet>().dmg = damage;
                    }
                    
                    bulletCount += 1;
                    shoot = Shoot.Cooldown;
                    break;

                //special gun attacks
                case Shoot.Special:
                    //"big bullet", 4 times damage, long cooldown
                    if (special == "big")
                    {
                        attacking = true;
                        bPos = transform.position + transform.forward;
                        b = Instantiate(Resources.Load<GameObject>("bullet"));
                        b.transform.position = bPos;
                        b.transform.localScale *= 2;
                        b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                        b.GetComponent<bullet>().maxRange = range;
                        b.GetComponent<bullet>().dmg = damage * 4;

                        bulletCount += bullets;
                        shootcd *= 2f;
                        shoot = Shoot.Cooldown;
                    }
                    //unlimited ammo for 3 seconds, long cooldown
                    if (special == "unlimited")
                    {
                        specialTimer += Time.deltaTime;

                        if (Input.GetButton("Fire2"))
                            RapidFire();

                        if (specialTimer >= 3f)
                        {
                            specialTimer = 0;
                            shootcd *= 6;
                            shoot = Shoot.Cooldown;
                        }
                    }
                    //shoots shotgun ammo 3 times
                    if (special == "multi")
                    {
                        attacking = true;
                        bPos = transform.position + transform.forward;
                        bRot = transform.rotation;
                        Vector3 bPos2 = transform.position + transform.forward + transform.up;
                        Vector3 bPos3 = transform.position + transform.forward + (transform.up * 2);

                        //position for each bullet
                        posB = new Vector3[] {
                                bPos,
                                bPos + transform.right * 0.5f,
                                bPos + transform.right * -0.5f,
                                bPos + transform.right * 1f,
                                bPos + transform.right * -1f };

                        Vector3[] posB2 = new Vector3[] {
                                bPos2,
                                bPos2 + transform.right * 0.5f,
                                bPos2 + transform.right * -0.5f,
                                bPos2 + transform.right * 1f,
                                bPos2 + transform.right * -1f };

                        Vector3[] posB3 = new Vector3[] {
                                bPos3,
                                bPos3 + transform.right * 0.5f,
                                bPos3 + transform.right * -0.5f,
                                bPos3 + transform.right * 1f,
                                bPos3 + transform.right * -1f };

                        //rotation for each bullet
                        rotB = new Quaternion[] {
                                bRot,
                                bRot *= Quaternion.Euler(Vector3.up * 45),
                                bRot *= Quaternion.Euler(Vector3.up * -45),
                                bRot *= Quaternion.Euler(Vector3.up * -60),
                                bRot *= Quaternion.Euler(Vector3.up * 60) };

                        for (int i = 0; i < rotB.Length; i++)
                        {
                            b = Instantiate(Resources.Load<GameObject>("bullet"));
                            b.transform.position = posB[i];
                            b.transform.rotation = rotB[i];
                            b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                            b.GetComponent<bullet>().maxRange = range;
                            b.GetComponent<bullet>().dmg = damage;

                            GameObject b2 = Instantiate(Resources.Load<GameObject>("bullet"));
                            b2.transform.position = posB2[i];
                            b2.transform.rotation = rotB[i];
                            b2.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                            b2.GetComponent<bullet>().maxRange = range;
                            b2.GetComponent<bullet>().dmg = damage;

                            GameObject b3 = Instantiate(Resources.Load<GameObject>("bullet"));
                            b3.transform.position = posB3[i];
                            b3.transform.rotation = rotB[i];
                            b3.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                            b3.GetComponent<bullet>().maxRange = range;
                            b3.GetComponent<bullet>().dmg = damage;
                        }

                        bulletCount += bullets;
                        shootcd *= 3f;
                        shoot = Shoot.Cooldown;
                    }

                    break;

                //reloading
                case Shoot.Cooldown:
                    attacking = false;

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
            }
        }
    }

    void WeaponChange(int active, bool isGun)
    {
        //change weapon variables
        if (active == 1)
        {
            damage = weapons.damage1;
            speed = weapons.speed1;
            if (isGun)
            {
                bullets = weapons.bullets1;
                rlSpeed = weapons.rlspeed1;
                range = weapons.range1;
                shootcd = weapons.rlspeed1;
                type = weapons.type1;
                special = weapons.special1;
            }
        }
        else
        {
            damage = weapons.damage2;
            speed = weapons.speed2;
            if (isGun)
            {
                bullets = weapons.bullets2;
                rlSpeed = weapons.rlspeed2;
                range = weapons.range2;
                shootcd = weapons.rlspeed2;
                type = weapons.type2;
                special = weapons.special2;
            }
        }
    }

    void BasicAttack()
    {
        if (activeWeapon == 1)
        {
            if (weapons.weaponType1 == 1)
            {
                //equip one is sword
                StartCoroutine(Attack(Vector3.up, 90f, weapons.speed1));
            }
            else if (weapons.weaponType1 == 2)
            {
                //equip one is gun
                if (shoot == Shoot.Ready)
                {
                    if (weapons.type1 == "normal")
                        shoot = Shoot.Shooting;
                    if (weapons.type1 == "rapid")
                        shoot = Shoot.Rapid;
                    if (weapons.type1 == "shotgun")
                        shoot = Shoot.Shotgun;
                }
            }
            else if (weapons.weaponType1 == 0)
            {
                //no equip
                print("no weapon");
            }
        }
        else if (activeWeapon == 2)
        {
            if (weapons.weaponType2 == 1)
            {
                //equip two is sword
                StartCoroutine(Attack(Vector3.up, 90f, weapons.speed2));
            }
            else if (weapons.weaponType2 == 2)
            {
                //equip two is gun
                if (shoot == Shoot.Ready)
                {
                    if (weapons.type2 == "normal")
                        shoot = Shoot.Shooting;
                    if (weapons.type2 == "rapid")
                        shoot = Shoot.Rapid;
                    if (weapons.type2 == "shotgun")
                        shoot = Shoot.Shotgun;
                }
                //Shoot(weapons.range2);
            }
            else if (weapons.weaponType2 == 0)
            {
                //no equip
                print("no weapon");
            }
        }
    }

    void SpecialAttack()
    {
        if (activeWeapon == 1)
        {
            if (weapons.weaponType1 == 1)
            {
                //equip one is sword
                StartCoroutine(AttackTwo(Vector3.up, swordSpinTime));
            }
            else if (weapons.weaponType1 == 2 && shoot == Shoot.Ready)
            {
                //equip one is gun
                shoot = Shoot.Special;
            }
            else if (weapons.weaponType1 == 0)
            {
                //no equip
                print("no weapon");
            }
        }
        else if (activeWeapon == 2)
        {
            if (weapons.weaponType2 == 1)
            {
                //equip two is sword
                StartCoroutine(AttackTwo(Vector3.up, swordSpinTime));
            }
            else if (weapons.weaponType2 == 2 && shoot == Shoot.Ready)
            {
                //equip two is gun
                shoot = Shoot.Special;
            }
            else if (weapons.weaponType2 == 0)
            {
                //no equip
                print("no weapon");
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
    }

    //basic attack
    IEnumerator Attack(Vector3 axis, float angle, float time)
    {
        attacking = true;
        if (swordActive)
            yield break;
        swordActive = true;
        attackNum = 1;
        GameObject sw;
        sw = Instantiate(sword, transform.position, transform.rotation);
        sw.transform.parent = transform;
        sw.transform.Rotate(0, -45, 0, Space.Self);
        Quaternion from = sw.transform.rotation;
        Quaternion to = sw.transform.rotation;
        to *= Quaternion.Euler(axis * angle);
        float t = 0f;
        while (t < time)
        {
            sw.transform.rotation = Quaternion.Slerp(from, to, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        sw.transform.rotation = to;
        Destroy(sw);
        swordActive = false;
        attacking = false;
        yield return null;
    }

    //spin attack
    IEnumerator AttackTwo(Vector3 axis, float time)
    {
        attacking = true;
        if (swordActive)
            yield break;
        swordActive = true;
        attackNum = 2;
        GameObject sw;
        sw = Instantiate(sword, transform.position, transform.rotation);
        sw.transform.parent = transform;
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
        swordActive = false;
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
        Reload
    }
}
