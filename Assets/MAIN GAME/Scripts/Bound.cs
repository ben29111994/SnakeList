using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    bool isBound = false;
    Rigidbody rigid;

    private void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rigid.velocity.x > 40)
        {
            rigid.velocity = new Vector3(40, rigid.velocity.y, rigid.velocity.z);
        }
        if (rigid.velocity.z > 40)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, 40);
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -11.5f, 11.5f), Mathf.Clamp(transform.position.y, 0, 0.6f), Mathf.Clamp(transform.position.z, -14, 33.5f));
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if ((other.gameObject.CompareTag("WallPush") || other.gameObject.CompareTag("Player")) && !isBound)
    //    {
    //        isBound = true;
    //        GetComponent<Rigidbody>().velocity = Vector3.zero;
    //        Vector3 dirPush = transform.position - other.contacts[0].point;
    //        dirPush *= 2000;
    //        dirPush = new Vector3(Mathf.Clamp(dirPush.x, -1000, 1000), 0, Mathf.Clamp(dirPush.z, -1000, 1000));
    //        GetComponent<Rigidbody>().AddForce(dirPush);
    //        //float torque;
    //        //if (dirPush.x > dirPush.z)
    //        //{
    //        //    torque = dirPush.x;
    //        //}
    //        //else
    //        //{
    //        //    torque = dirPush.z;
    //        //}
    //        GetComponent<Rigidbody>().AddTorque(dirPush);
    //        StartCoroutine(delayCollision());
    //    }
    //}

    //IEnumerator delayCollision()
    //{
    //    yield return new WaitForSeconds(0.01f);
    //    isBound = false;
    //}
}
