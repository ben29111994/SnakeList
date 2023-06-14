using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GPUInstancer;
using DG.Tweening;

public class Bot : MonoBehaviour
{
    public GameObject player;
    public GameObject gpu;
    GameController gameController;
    AddRemoveInstances addRemoveInstances;
    bool isTurn = true;
    bool isHitWall = false;
    public Color botColor;
    Vector3 look;
    public float standardDistance;
    public GameObject targetObject;
    public bool isDie = false;

    [Header("Snake Manager")]
    public int startCount;
    public Transform prefab;
    private Vector3 lastPosition;
    public List<Transform> balls = new List<Transform>();
    private List<Vector3> points = new List<Vector3>();
    public Transform center;
    Vector3 dirCast;
    int addCount = 0;

    [Header("Head Manager")]
    public GameObject target;
    private Vector3 velocity = Vector3.zero;
    float h;
    float v;
    Vector3 dir;
    public float speed;

    void OnEnable()
    {
        botColor = GetComponent<Renderer>().material.color;
        player = GameObject.FindGameObjectWithTag("Player");
        gameController = player.GetComponent<GameController>();
        gpu = GameObject.FindGameObjectWithTag("GPU");
        addRemoveInstances = gpu.GetComponent<AddRemoveInstances>();
        standardDistance = 0.5f;
        InitSnake();
        StartInvoke(0);
    }

    void InitSnake()
    {
        for (int i = 0; i < startCount; i++)
        {
            Transform _transform = Instantiate(prefab).transform;
            _transform.parent = transform.parent;
            balls.Add(_transform);
            points.Add(balls[i].position);
            _transform.name = i.ToString();
            _transform.GetComponent<Renderer>().material.color = botColor;
            _transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", botColor * 0.1f);
        }
        lastPosition = balls[0].position;
    }

    private void FixedUpdate()
    {
        if (gameController.isStartGame)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -11.5f, 11.5f), transform.position.y, Mathf.Clamp(transform.position.z, -14f, 33.5f));
        }

        if (balls.Count > 0 && !isDie)
        {
            Vector3 vector4 = target.transform.position - balls[0].transform.position;
            float magnitude = vector4.magnitude;
            if (magnitude > standardDistance)
            {
                balls[0].transform.position = Vector3.SmoothDamp(balls[0].transform.position, target.transform.position, ref velocity, 0.05f);
            }
            balls[0].transform.rotation = target.transform.rotation;
        }
    }

    void Update()
    {
        if(targetObject == null)
        {
            CancelInvoke();
            StartInvoke(0);
        }
        look = dir - transform.position;
        //float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(look));
        //angle = Mathf.Abs(angle);
        //if (angle > 180)
        //{
        //    var decision = Random.Range(0, 10);
        //    if (decision > 1 && !isHitWall)
        //    {
        //        isTurn = false;
        //    }
        //    else
        //    {
        //        isTurn = true;
        //    }
        //}
        //else
        //{
        //    isTurn = true;
        //}
        if (isTurn && dir != Vector3.zero && Vector3.Distance(dir, transform.position) > transform.localScale.x + 2 && Vector3.Distance(look, transform.position) > transform.localScale.x + 2)
        {
            var targetRot = Quaternion.LookRotation(look);
            var modTargetRot = targetRot;
            modTargetRot.x = 0;
            targetRot = modTargetRot;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * 720);
        }

        if (balls.Count > 0 && !isDie)
        {
            SnakeMove();
        }
    }

    private void SnakeMove()
    {
        try
        {
            if (lastPosition != balls[0].position)
            {
                lastPosition = balls[0].position;
                points.Insert(0, lastPosition);

                int num = 1;
                float spacing = standardDistance;
                int index = 0;
                while ((index < (this.points.Count - 1)) && (num < this.balls.Count))
                {
                    Vector3 vector2 = this.points[index];
                    Vector3 vector3 = this.points[index + 1];
                    Vector3 vector4 = vector3 - vector2;
                    float magnitude = vector4.magnitude;
                    if (magnitude > 0f)
                    {
                        Vector3 vector6 = vector3 - vector2;
                        Vector3 normalized = vector6.normalized;
                        Vector3 vector7 = vector2;
                        while ((spacing <= magnitude) && (num < this.balls.Count))
                        {
                            vector7 += (Vector3)(normalized * spacing);
                            magnitude -= spacing;
                            this.balls[num].transform.position = vector7;
                            if (num + 1 <= balls.Count - 1)
                                this.balls[num].transform.rotation = this.balls[num + 1].transform.rotation;
                            num++;
                            spacing = standardDistance;
                        }
                        spacing -= magnitude;
                    }
                    index++;
                }
                Vector3 vector8 = this.points[this.points.Count - 1];
                for (int i = num; i < this.balls.Count; i++)
                {
                    balls[num].transform.position = vector8;
                    if (num + 1 <= balls.Count - 1)
                        this.balls[num].transform.rotation = this.balls[num + 1].transform.rotation;
                }
                index++;
                if (index < this.points.Count)
                {
                    this.points.RemoveRange(index, this.points.Count - index);
                }
            }
        }
        catch { }
    }

    public void RandomDir()
    {
        //dir = new Vector3(Random.Range(-11.5f, 11.5f), transform.position.y, Random.Range(-13.5f, 33.5f));
        if (!isDie)
        {
            var randomPick = Random.Range(0, addRemoveInstances.instancesList.Count);
            targetObject = addRemoveInstances.instancesList[randomPick].gameObject;
            dir = new Vector3(addRemoveInstances.instancesList[randomPick].transform.position.x, transform.position.y, addRemoveInstances.instancesList[randomPick].transform.position.z);
            //foreach (var item in balls)
            //{
            //    var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
            //    if (prefab != null)
            //    {
            //        var getColor = prefab.GetComponent<ParticleSystem>().main;
            //        getColor.startColor = botColor;
            //        prefab.transform.position = item.transform.position;
            //        prefab.SetActive(true);
            //        prefab.GetComponent<ParticleSystem>().Play();
            //        Destroy(item.gameObject);
            //    }
            //}
            //Destroy(gameObject, 0.1f);
        }
    }

    public void ChangeDir(Vector3 target)
    {
        dir = target;
    }

    public void CheckWall(bool checkWall)
    {
        isHitWall = checkWall;
    }

    public void StartInvoke(float delayTime)
    {
        InvokeRepeating("RandomDir", delayTime, 10);
    }

    public void StopInvoke()
    {
        CancelInvoke();
        //StartCoroutine(delayStart());
    }

    IEnumerator delayStart()
    {
        yield return new WaitForSeconds(3);
        StopInvoke();
        StartInvoke(0);
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("Pixel"))
    //    {
    //        StopInvoke();
    //        StartInvoke(0);
    //        if (!gameController.isFirstHit)
    //        {
    //            gameController.isFirstHit = true;
    //            foreach (var item in AddRemoveInstances.instance.instancesList)
    //            {
    //                item.GetComponent<Rigidbody>().isKinematic = false;
    //                item.GetComponent<Rigidbody>().useGravity = true;
    //                if (item.transform.localScale.x < 1)
    //                {
    //                    item.transform.DOScale(Vector3.one, 5);
    //                }
    //            }
    //            foreach (var item in GameObject.FindGameObjectsWithTag("Coin"))
    //            {
    //                item.GetComponent<Rigidbody>().useGravity = true;
    //                item.GetComponent<SphereCollider>().isTrigger = false;
    //            }
    //            foreach (var item in GameObject.FindGameObjectsWithTag("Bomb"))
    //            {
    //                item.transform.parent.GetComponent<Rigidbody>().useGravity = true;
    //            }
    //        }
    //        var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
    //        if (prefab != null)
    //        {
    //            prefab.SetActive(true);
    //            var getColor = prefab.GetComponent<ParticleSystem>().main;
    //            getColor.startColor = other.gameObject.GetComponent<Tile>().tileColor;
    //            prefab.transform.position = other.gameObject.transform.position;
    //            prefab.GetComponent<ParticleSystem>().Play();
    //        }
    //        AddRemoveInstances.instance.RemoveInstances(other.gameObject.GetComponent<GPUInstancerPrefab>());
    //    }      
    //}

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Player") && !gameController.isMyBot && !isDie)
        //{
        //    isDie = true;
        //    foreach (var item in balls)
        //    {
        //        var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
        //        if (prefab != null)
        //        {
        //            var getColor = prefab.GetComponent<ParticleSystem>().main;
        //            getColor.startColor = botColor;
        //            prefab.transform.position = item.transform.position;
        //            prefab.SetActive(true);
        //            prefab.GetComponent<ParticleSystem>().Play();
        //            Destroy(item.gameObject);
        //        }
        //    }
        //    Destroy(gameObject, 0.1f);
        //}

        if (other.gameObject.CompareTag("Pixel"))
        {
            StopInvoke();
            StartInvoke(0);
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
                foreach (var item in GameObject.FindGameObjectsWithTag("Coin"))
                {
                    item.GetComponent<Rigidbody>().useGravity = true;
                    item.GetComponent<SphereCollider>().isTrigger = false;
                }
                foreach (var item in GameObject.FindGameObjectsWithTag("Bomb"))
                {
                    item.transform.parent.GetComponent<Rigidbody>().useGravity = true;
                }
            }
            var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
            if (prefab != null)
            {
                prefab.SetActive(true);
                var getColor = prefab.GetComponent<ParticleSystem>().main;
                getColor.startColor = other.gameObject.GetComponent<Tile>().tileColor;
                prefab.transform.position = other.gameObject.transform.position;
                prefab.GetComponent<ParticleSystem>().Play();
            }
            AddRemoveInstances.instance.RemoveInstances(other.gameObject.GetComponent<GPUInstancerPrefab>());
            gameController.PlusEffectMethod();
            gameController.AddSnake();
        }
    }
}
