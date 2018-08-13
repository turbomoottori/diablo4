using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    Vector2 direction;

    public Transform trans;
    public Rigidbody2D rb2d;
    public float maxSpeed = 100.0f;

    private float currentAngle = 0.0f;
    private float targetAngle = 0.0f;

    // Use this for initialization
    void Start ()
    {
        direction = new Vector2();

	}

    void setTargetAngle(float a)
    {
        targetAngle = a;
    }


    /*
        Käännetään sprite annettuun kulmaan (huom. ei radiaaneja)
        Tämä sen vuoksi, jotta voidaan määritellä kääntymiselle nopeus, jossa aloitusnopeus on isompi ja lähestyessä kohdekulmaa tämä nopeus hidastuu.
        Funktio ottaa huomioon myös tilanteen, jossa kohdekulmaan on yli 180 asteen matka, jolloinka se kääntyy toiseen suuntaan (lyhyempi matka)
    */
    void rotateSprite(float rotatingSpeed)
    {
        float counterRotate = 0; //aina oletuksena 0, jottei muodostu inversiota.

        //tarkistetaan onko pidempi vai lyhyempi matka. Jos pidempi, lisätään inversio (kääntyy toiseen suuntaan)
        if (currentAngle - targetAngle >= 180)
            counterRotate = 360;
        
        /*
            Päivitetään nykyistä kulmaa, joka on:
            nykyinen kulma = nykyinen kulma - (nykyinen kulma - kohdekulma - mahdollinen inversio) * delta-aika * kääntönopeus
        */
        currentAngle -= (currentAngle - targetAngle - counterRotate) * Time.deltaTime * rotatingSpeed;

        /*
            Varmistellaan että kulmaluku pysyy ympyrän sisällä, helpottaa currentAnglen toimintaa 
        */
        if (currentAngle > 360)
        {
            currentAngle -= 360;
        }
        if (currentAngle < 0)
        {
            currentAngle += 360;
        }
        
        //lopulta päivitetään uusi kulma spritelle myös
        trans.eulerAngles = new Vector3(trans.eulerAngles.x, trans.eulerAngles.y, currentAngle);
        
     
    }

	// Update is called once per frame
	void Update ()
    {
        //otetaan syöte talteen
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");

        Vector2 dir = new Vector2(0, 0);

        //muutetaan se helpommin käsiteltäväksi
        if (direction.x < 0)    dir.x = -1;
        if (direction.x > 0)    dir.x =  1;
        if (direction.y < 0)    dir.y = -1;
        if (direction.y > 0)    dir.y =  1;


        //matkan pituus mikä liikutaan
        //ensin luvun itseisarvo
        float dx = Mathf.Abs(dir.x);
        float dy = Mathf.Abs(dir.y);
        //ja x:n sekä y:n neliöjuuri, jotta saadaan oikea pituus
        float len = Mathf.Sqrt((dx * dx) + (dy * dy));
        
        //templen tarvitaan tilanteissa joissa len on 0, koska nollalla ei voida jakaa.
        float tempLen = len;
        if (len == 0)
            tempLen = 1;

        //velocity = suunta * maksiminopeus / suunnan pituus * delta-aika
        rb2d.velocity = dir * maxSpeed / tempLen * Time.deltaTime;

        //lasketaan sitten astekulma johon hahmo katsoo
        if (direction.x != 0.0 || direction.y != 0.0)
        {
            //palauttaa radinaanina, käännetään se asteiksi (x / 3.1415 * 180.0)
            float angle = Mathf.Atan2(direction.y, direction.x);
            setTargetAngle(angle / 3.1415f * 180.0f);
        }

        //nopeus jolla spriteä pyöritetään
        rotateSprite(15.0f);

    }

    // FixedUpdate is called once per physic step
    void FixedUpdate()
    {
       


    }
}
