using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour {

    private Vector3 savedOffset;
    public Transform player;
    public float camSpeed;
    Vector3 offset, target;
    float min, max, angle;
    float minFov, maxFov;
    public LayerMask mask;

	void Start () {
        savedOffset = new Vector3(0, 9.5f, -7.9f);
        transform.position = player.position + savedOffset;

        offset = transform.position - player.position;

        min = 7f;
        max = 15f;

        minFov = 50;
        maxFov = 100;
    }
	
	void Update () {
        //camera position
        Vector3 targetPos = player.position;
        Vector3 targ = targetPos + offset;
        Vector3 curr = transform.position;
        curr += (targ - curr) * Time.deltaTime * camSpeed;
        transform.position = curr;

        //camera rotation
        target = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);
        var targetRot = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, (camSpeed / 2) * Time.deltaTime);

        if (Input.GetKey(KeyCode.Minus) && offset.y < max)
            offset.y += 0.1f;
        if (Input.GetKey(KeyCode.Period) && offset.y > min)
            offset.y -= 0.1f;

        //change FOV
        if(Input.GetKeyDown(KeyCode.L) && Camera.main.fieldOfView < maxFov)
            Camera.main.fieldOfView += 5;
        if(Input.GetKeyDown(KeyCode.K) && Camera.main.fieldOfView > minFov)
            Camera.main.fieldOfView -= 5;

        //hides objects that are too close to camera
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.02f);
        foreach(Collider c in hitColliders)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("CanHide"))
                StartCoroutine(FadeOut(FindHideableGameObject(c.gameObject).gameObject, c.gameObject));
        }
        
        //hides objects between camera and player
        RaycastHit hit, h;
        if (Physics.Linecast(transform.position, targetPos - transform.position, out hit, mask) && Physics.Linecast(targetPos, transform.position, out h, mask))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("CanHide"))
                StartCoroutine(FadeOut(FindHideableGameObject(hit.collider.gameObject).gameObject, hit.collider.gameObject));
            if (h.collider.gameObject.layer == LayerMask.NameToLayer("CanHide"))
                StartCoroutine(FadeOut(FindHideableGameObject(h.collider.gameObject).gameObject, h.collider.gameObject));

            print(hit.collider.name + h.collider.name);
        }
    
    }

    //fades gameobject out
    IEnumerator FadeOut(GameObject g, GameObject objectWithCollider)
    {
        print("fadeout");
        if (g.GetComponent<Animator>().GetBool("fade") == true)
            yield return null;
        else
        {
            g.GetComponent<Animator>().SetBool("fade", true);
            yield return new WaitUntil(() => IsSeen(objectWithCollider) == false);
            g.GetComponent<Animator>().SetBool("fade", false);
        }
    }

    //checks if object is seen by raycast from camera to player
    bool SeenByRaycast1(GameObject g)
    {
        RaycastHit[] cameraToPlayer = Physics.RaycastAll(transform.position, player.transform.position - transform.position, Vector3.Distance(player.transform.position, transform.position));
        for (int i = 0; i < cameraToPlayer.Length; i++)
        {
            if (cameraToPlayer[i].collider.gameObject == g)
                return true;
        }
        return false;
    }

    //checks if object is seen by raycast from player to camera
    bool SeenByRaycast2(GameObject g)
    {
        RaycastHit[] playerToCamera = Physics.RaycastAll(player.transform.position, transform.position - player.transform.position, Vector3.Distance(player.transform.position,transform.position));
        for(int i = 0; i < playerToCamera.Length; i++)
        {
            if (playerToCamera[i].collider.gameObject == g)
                return true;
        }
        return false;
    }

    //checks if object is seen by either raycast or if object is too close to camera
    bool IsSeen(GameObject g)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.02f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (FindHideableGameObject(hitColliders[i].gameObject))
                return true;
        }

        if (!SeenByRaycast1(g) && !SeenByRaycast2(g))
            return false;

        return true;
    }

    //finds gameobject with house tag
    Transform FindHideableGameObject(GameObject g)
    {
        if (g.transform.parent != null)
        {
            foreach (Transform child in g.transform.parent)
                if (child.tag == "House")
                    return child;
        }
        else if (g.transform.parent == null)
        {
            if (g.tag == "House")
                return g.transform;

            foreach (Transform child in g.transform)
                if (child.tag == "House")
                    return child;
        }

        return null;
    }
}
