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
    float shootcd;

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
                    shoot = Shoot.Off;
                }
                else
                {
                    shoot = Shoot.Cooldown;
                    shootcd = weapons.rlspeed1;
                }
                    

                print("weapon 1 selected");
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f && activeWeapon != 2 && !attacking)
            {
                activeWeapon = 2;

                if (weapons.weaponType2 != 2)
                {
                    shoot = Shoot.Off;
                }
                else
                {
                    shoot = Shoot.Cooldown;
                    shootcd = weapons.rlspeed2;
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
                        shootcd = weapons.rlspeed1;
                    else
                        shootcd = weapons.rlspeed2;

                    if (Input.GetKeyDown(KeyCode.R) && bulletCount != 0)
                        shoot = Shoot.Reload;

                    break;

                //normal shooting
                case Shoot.Shooting:
                    attacking = true;
                    float maxRange;
                    float speed;

                    if (activeWeapon == 1)
                    {
                        maxRange = weapons.range1;
                        speed = weapons.speed1;
                    }
                    else
                    {
                        maxRange = weapons.range2;
                        speed = weapons.speed2;
                    }

                    Vector3 bPos = transform.position + transform.forward;
                    GameObject b = Instantiate(Resources.Load<GameObject>("bullet"));
                    b.transform.position = bPos;
                    b.GetComponent<Rigidbody>().AddForce(transform.forward * 500 * speed);
                    b.GetComponent<bullet>().maxRange = maxRange;

                    if (activeWeapon == 1)
                        b.GetComponent<bullet>().dmg = weapons.damage1;
                    else
                        b.GetComponent<bullet>().dmg = weapons.damage2;

                    bulletCount += 1;
                    shoot = Shoot.Cooldown;
                    break;

                case Shoot.Rapid:
                    attacking = true;
                    //RAPID FIRE HERE
                    break;

                //reloading
                case Shoot.Cooldown:
                    attacking = false;
                    if (activeWeapon == 1 && bulletCount >= weapons.bullets1)
                    {
                        shootcd -= Time.deltaTime;
                        if (shootcd <= 0)
                        {
                            shootcd = weapons.rlspeed1;
                            bulletCount = 0;
                            shoot = Shoot.Ready;
                        }
                    }
                    else if(activeWeapon == 2 && bulletCount >= weapons.bullets2)
                    {
                        shootcd -= Time.deltaTime;
                        if (shootcd <= 0)
                        {
                            shootcd = weapons.rlspeed2;
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
                    if (activeWeapon == 1)
                    {
                        shootcd -= Time.deltaTime;
                        if (shootcd <= 0)
                        {
                            shootcd = weapons.rlspeed1;
                            bulletCount = 0;
                            shoot = Shoot.Ready;
                        }
                    }
                    else
                    {
                        shootcd -= Time.deltaTime;
                        if (shootcd <= 0)
                        {
                            shootcd = weapons.rlspeed2;
                            bulletCount = 0;
                            shoot = Shoot.Ready;
                        }
                    }
                    break;
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
                //Shoot(weapons.range1);
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
            else if (weapons.weaponType1 == 2)
            {
                //equip one is gun
                print("gun special");
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
            else if (weapons.weaponType2 == 2)
            {
                //equip two is gun
                print("gun special");
            }
            else if (weapons.weaponType2 == 0)
            {
                //no equip
                print("no weapon");
            }
        }
    }

    /*void Shoot(float maxRange)
    {
        attacking = true;
        Vector3 bPos = transform.position + transform.forward;
        GameObject b = Instantiate(Resources.Load<GameObject>("bullet"));
        b.transform.position = bPos;
        b.GetComponent<Rigidbody>().AddForce(transform.forward * 1500);
        b.GetComponent<bullet>().maxRange = maxRange;
        if (activeWeapon == 1)
            b.GetComponent<bullet>().dmg = weapons.damage1;
        else
            b.GetComponent<bullet>().dmg = weapons.damage2;
        attacking = false;
    }*/

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
