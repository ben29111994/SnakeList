using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPUInstancer;
using DG.Tweening;

public class HolePush : MonoBehaviour
{
    GameController gameController;
    bool isBound = false;
    int limitCount = 0;
    Rigidbody rigid;

    private void OnEnable()
    {
        gameController = GameObject.FindGameObjectWithTag("Player").GetComponent<GameController>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(rigid.velocity.x > 40)
        {
            rigid.velocity = new Vector3(40, rigid.velocity.y, rigid.velocity.z);
        }
        if (rigid.velocity.z > 40)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, 40);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Pixel") && !other.gameObject.GetComponent<Tile>().isCheck)
        {
            other.gameObject.GetComponent<Tile>().Check();
            other.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            StartCoroutine(delayMagnet(other.gameObject));
        }

        if(other.gameObject.CompareTag("Hole Magnet") && tag == "Hole Push")
        {
            try
            {
                other.gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
                other.gameObject.GetComponent<BoxCollider>().isTrigger = true;
            }
            catch { }
            StartCoroutine(delayMagnet(other.gameObject));
        }

        //if (other.gameObject.CompareTag("WallPush"))
        //{
        //    GetComponent<Rigidbody>().AddForce(other.transform.forward * 2000);
        //}

        //if (/*(*/other.gameObject.CompareTag("WallPush")/* || other.gameObject.CompareTag("Player")) && !isBound*/)
        //{
        //    //isBound = true;
        //    GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    Vector3 dirPush = transform.position - other.contacts[0].point;
        //    dirPush *= 2000;
        //    dirPush = new Vector3(Mathf.Clamp(dirPush.x, -1500, 1500), 0, Mathf.Clamp(dirPush.z, -1500, 1500));
        //    GetComponent<Rigidbody>().AddForce(dirPush);
        //    //StartCoroutine(delayCollision());
        //}
    }

    //Vector3 GetRandomUnitPerpendicular(Vector3 v)
    //{
    //    float angle = Random.Range(0, Mathf.PI * 2f);

    //    // Generate a uniformly-distributed unit vector in the XY plane.
    //    Vector3 inPlane = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

    //    // Rotate the vector into the plane perpendicular to v and return it.
    //    return Quaternion.LookRotation(v) * inPlane;
    //}

    IEnumerator delayMagnet(GameObject other)
    {
        other.transform.DOMove(transform.position, 0.2f);
        yield return new WaitForSeconds(0.1f);
        if(other != null && !other.CompareTag("Pixel"))
        {
            other.transform.DOKill();
            Destroy(other.gameObject);
        }
        try
        {
            if (other.CompareTag("Pixel") && other != null)
            {
                if (!gameController.isFirstHit)
                {
                    gameController.isFirstHit = true;
                    foreach (var item in AddRemoveInstances.instance.instancesList)
                    {
                        item.GetComponent<Rigidbody>().isKinematic = false;
                        item.GetComponent<Rigidbody>().useGravity = true;
                        if (item.transform.localScale.x < 1)
                        {
                            item.transform.DOScale(Vector3.one, 5);
                        }
                    }
                    //foreach (var item in GameObject.FindGameObjectsWithTag("Coin"))
                    //{
                    //    item.GetComponent<Rigidbody>().useGravity = true;
                    //    item.GetComponent<SphereCollider>().isTrigger = false;
                    //}
                    //foreach (var item in GameObject.FindGameObjectsWithTag("Bomb"))
                    //{
                    //    item.transform.parent.GetComponent<Rigidbody>().useGravity = true;
                    //}
                }
                try
                {
                    AddRemoveInstances.instance.RemoveInstances(other.GetComponent<GPUInstancerPrefab>());
                }
                catch { }
                gameController.AddSnake();
            }
        }
        catch { }
    }
}
