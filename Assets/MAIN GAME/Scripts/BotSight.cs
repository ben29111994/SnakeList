using System.Collections.Generic;
using UnityEngine;

public class BotSight : MonoBehaviour
{
    public GameObject bot;

    //private void OnTriggerEnter(Collider other)
    //{
    //    //if (other.tag == "Player")
    //    //{
    //    //    try
    //    //    {
    //    //        var runAway = transform.InverseTransformDirection(other.transform.position * 100);
    //    //        runAway.y = bot.transform.position.y;
    //    //        bot.GetComponent<Bot>().StopInvoke();
    //    //        bot.GetComponent<Bot>().ChangeDir(runAway);
    //    //    }
    //    //    catch { }
    //    //}

    //    //if (other.CompareTag("WallPush") || other.CompareTag("Wall"))
    //    //{
    //    //    //Debug.Log("Wall!");
    //    //    bot.GetComponent<Bot>().StopInvoke();
    //    //    bot.GetComponent<Bot>().CheckWall(true);
    //    //    bot.GetComponent<Bot>().ChangeDir(new Vector3(Random.Range(-1, 1), transform.position.y, Random.Range(10, 15)));
    //    //}

    //    //if (other.CompareTag("Pixel"))
    //    //{
    //    //    bot.GetComponent<Bot>().StopInvoke();
    //    //    bot.GetComponent<Bot>().ChangeDir(new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z));
    //    //}
    //}

    //public void Update()
    //{
    //    RaycastHit objectHit;
    //    if (Physics.Raycast(transform.position, transform.forward, out objectHit, 5))
    //    {
    //        //do something if hit object ie
    //        if (objectHit.collider.CompareTag("Player"))
    //        {
    //            var runAway = transform.InverseTransformDirection(objectHit.transform.position * 10);
    //            runAway.y = bot.transform.position.y;
    //            bot.GetComponent<Bot>().StopInvoke();
    //            bot.GetComponent<Bot>().ChangeDir(runAway);
    //        }
    //        if(objectHit.collider.CompareTag("Pixel"))
    //        {
    //            bot.GetComponent<Bot>().StopInvoke();
    //            bot.GetComponent<Bot>().ChangeDir(new Vector3(objectHit.transform.position.x, transform.position.y, objectHit.transform.position.z));
    //        }
    //    }            
    //    //if (objectHit.collider.CompareTag("Wall"))
    //        //{
    //        //    bot.GetComponent<Bot>().StopInvoke();
    //        //    bot.GetComponent<Bot>().CheckWall(true);
    //        //    bot.GetComponent<Bot>().ChangeDir(new Vector3(0, transform.position.y, 10));
    //        //}
    //}

    //private void OnTriggerExit(Collider other)
    //{
        //if (other.tag == "Player")
        //{
        //    try
        //    {
        //        var runAway = transform.InverseTransformDirection(other.transform.position * 100);
        //        runAway.y = bot.transform.position.y;
        //        bot.GetComponent<Bot>().StopInvoke();
        //        bot.GetComponent<Bot>().ChangeDir(runAway);
        //    }
        //    catch { }
        //}     

        //if (other.tag == "Wall")
        //{
        //    bot.GetComponent<Bot>().CheckWall(false);
        //    bot.GetComponent<Bot>().StartInvoke(1f);
        //}
    //}
}
