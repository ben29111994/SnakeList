using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine.EventSystems;
using GPUInstancer;
using System.Linq;
using VisCircle;
using UnityStandardAssets.ImageEffects;

public class GameController : MonoBehaviour
{
    [Header("Variable")]
    public static GameController instance;
    public int maxLevel;
    public bool isStartGame = false;
    public List<Color> listColors = new List<Color>();
    public List<Color> listGroundColors = new List<Color>();
    public List<Color> listFogColors = new List<Color>();
    public List<Color> listBoundColors = new List<Color>();
    int groundColor;
    int currentColor;
    int colorChangeCount;
    public Color themeColor;
    public bool isDestroy = true;
    public bool isFirstHit = false;
    int maxPlusEffect = 0;
    public float standardDistance;
    bool isJump;
    bool isJumpShort;
    public bool isInvincible;
    bool isMagnet;
    int magnetLimit = 0;
    bool isGun;
    Vector3 desJump;
    GameObject dupMap;
    float followSpeed;
    bool isVibrate = false;
    int isFlyMap = 0;
    bool isHold = false;
    Rigidbody rigid;
    public bool isMyBot = false;
    public bool isPush = false;
    public List<int> listRotate = new List<int>();
    bool isPower = true;
    bool waitForTapWin = false;
    bool isBox = false;

    [Header("UI")]
    public GameObject winPanel;
    public GameObject losePanel;
    public Slider progressBar;
    public Text currentLevelText;
    public Text nextLevelText;
    int currentLevel;
    public Text scoreText;
    public Text moneyText;
    public static int score;
    public static int money;
    public Canvas canvas;
    public GameObject startGameMenu;
    public InputField levelInput;
    public GameObject nextButton;
    public Image title;
    public GameObject loading;
    public Text winMenu_title;
    public Text winMenu_score;
    public Text winMenu_money;
    public Text shopMenu_money;
    public List<GameObject> listPowerUp = new List<GameObject>();
    public List<GameObject> listShop = new List<GameObject>();
    public List<GameObject> listChecked = new List<GameObject>();
    public GameObject shopMenu;
    public GameObject powerUpMenu;
    public Sprite powerUpIcon;
    public Sprite shopIcon;
    public GameObject newItem;
    public Image shopImage;
    public GameObject unlockItem;
    public List<Sprite> listImage = new List<Sprite>();

    [Header("Objects")]
    public GameObject plusVarPrefab;
    public GameObject conffeti;
    GameObject conffetiSpawn;
    public GameObject pixelExplode;
    public GameObject bombExplode;
    public GameObject mapReader;
    public GameObject env;
    public GameObject map;
    public GameObject wall;
    public GameObject light1;
    public GameObject light2;
    public GameObject ruleObject;
    public GameObject powerUpObject;
    public GameObject clearEffect;
    public ParticleSystem shineEffect;
    public GameObject ground;
    public GameObject mapFlowEffect;
    public GameObject mouth;
    public GameObject followPlayer;
    public GameObject rope;

    [Header("Snake Manager")]
    public int startCount;
    public Transform prefab;
    private Vector3 lastPosition;
    public List<Transform> balls = new List<Transform>();
    private List<Vector3> points = new List<Vector3>();
    public Transform center;
    Vector3 dirCast;
    int addCount = 0;
    public float size;
    public float height;

    [Header("Head Manager")]
    public GameObject target;
    private Vector3 velocity = Vector3.zero;
    float h;
    float v;
    Vector3 dir;
    public float speed;
    public GameObject bulletPrefab;
    float zBoundUp = 27.5f;
    float zBound = -7.5f;

    [Header("Rules")]
    public List<GameObject> listRules = new List<GameObject>();
    public List<GameObject> listLevel = new List<GameObject>();
    public int currentRule;

    private void OnEnable()
    {
        //PlayerPrefs.DeleteAll();
        Application.targetFrameRate = 60;
        DOTween.SetTweensCapacity(3000, 3000);
        instance = this;
        ChangeThemeColor();
        InitSnake();
        StartCoroutine(delayRefreshInstancer());
        StartCoroutine(delayStart());
        rigid = GetComponent<Rigidbody>();
    }

    void ChangeThemeColor()
    {
        colorChangeCount = 0;
        listColors.Clear();
        groundColor = PlayerPrefs.GetInt("themeColor");
        if(groundColor > listGroundColors.Count - 1)
        {
            groundColor = 0;
        }
        PlayerPrefs.SetInt("themeColor", groundColor);
        themeColor = listGroundColors[groundColor];
        env.transform.GetChild(0).GetComponent<Renderer>().material.color = themeColor;
        ground = env.transform.GetChild(0).transform.GetChild(2).transform.gameObject;
        ground.GetComponent<Renderer>().material.color = listFogColors[groundColor];
        env.transform.GetChild(0).transform.GetChild(1).GetComponent<Renderer>().material.color = listBoundColors[groundColor];
        int randomSnakeColor = 4;
        switch (randomSnakeColor)
        {
            case 0:
                listColors.Add(new Color32(255, 60, 0, 255));
                listColors.Add(new Color32(26, 219, 1, 255));
                break;
            case 1:
                listColors.Add(new Color32(230, 230, 230, 255));
                listColors.Add(new Color32(221, 226, 3, 255));
                break;
            case 2:
                listColors.Add(new Color32(255, 0, 0, 255));
                listColors.Add(new Color32(4, 189, 239, 255));
                break;
            case 3:
                listColors.Add(new Color32(241, 152, 47, 255));
                listColors.Add(new Color32(230, 230, 230, 255));
                break;
            case 4:
                listColors.Add(new Color32(255, 60, 0, 255));
                listColors.Add(new Color32(255, 255, 255, 255));
                break;

            default:
                listColors.Add(new Color32(255, 60, 0, 255));
                listColors.Add(new Color32(26, 219, 1, 255));
                break;
        }
        currentColor = 0;
        GetComponent<Renderer>().material.color = listColors[currentColor];
        GetComponent<Renderer>().material.SetColor("_EmissionColor", listColors[currentColor] * 0.1f);
        colorChangeCount++;
    }

    IEnumerator RuleInit()
    {
        yield return new WaitForSeconds(0);
        currentRule = PlayerPrefs.GetInt("currentRule");
        Debug.Log("Rule: " + currentRule);
        if (currentRule != 9 && currentRule != 10 && currentRule != 4)
        {
            wall = env.transform.GetChild(0).transform.GetChild(0).gameObject;
            wall.SetActive(false);
        }
        switch (currentRule)
        {
            //bong bóng
            case 0:
                ruleObject = Instantiate(listRules[0]);
                map.transform.position = new Vector3(map.transform.position.x, 5, map.transform.position.z);
                map.GetComponent<PowerUpAnimation>().enabled = true;
                break;
            //box
            case 1:
                ruleObject = Instantiate(listRules[1]);
                var currentBox = PlayerPrefs.GetInt("currentBox");
                var currentBoxObject = ruleObject.transform.GetChild(currentBox);
                var boxNum = ruleObject.transform.childCount;
                foreach (Transform child in ruleObject.transform)
                {
                    if (child.name != currentBoxObject.name)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                currentBox++;
                if (currentBox > boxNum - 1)
                {
                    currentBox = 0;
                }
                PlayerPrefs.SetInt("currentBox", currentBox);
                unlockItem.SetActive(true);
                unlockItem.transform.GetChild(0).GetComponent<Image>().sprite = listImage[0];
                break;
            //tường trước mặt
            case 2:
                ruleObject = Instantiate(listRules[2]);
                //zBoundUp = ruleObject.transform.position.z;
                break;
            //1 bom trước mặt
            case 3:
                ruleObject = Instantiate(listRules[3]);
                break;
            //4 con bot 4 góc
            case 4:
                ruleObject = Instantiate(listRules[4]);
                unlockItem.SetActive(true);
                unlockItem.transform.GetChild(0).GetComponent<Image>().sprite = listImage[1];
                break;
            //sấm sét đi nhanh
            case 5:
                ruleObject = Instantiate(listRules[5]);
                unlockItem.SetActive(true);
                unlockItem.transform.GetChild(0).GetComponent<Image>().sprite = listImage[2];
                break;
            //lỗ chết
            case 6:
                ruleObject = Instantiate(listRules[6]);
                break;
            //nam châm
            case 7:
                ruleObject = ruleObject = Instantiate(listRules[7]);
                unlockItem.SetActive(true);
                unlockItem.transform.GetChild(0).GetComponent<Image>().sprite = listImage[3];
                break;
            //x2 size
            case 8:
                ruleObject = ruleObject = Instantiate(listRules[8]);
                int randomItem = Random.Range(0, 2);
                foreach (Transform item in ruleObject.transform)
                {
                    var id = int.Parse(item.name);
                    Debug.Log(id);
                    if (id != randomItem)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
                unlockItem.SetActive(true);
                unlockItem.transform.GetChild(0).GetComponent<Image>().sprite = listImage[4];
                break;
            //beach ball
            case 9:
                ruleObject = Instantiate(listRules[9]);
                unlockItem.SetActive(true);
                unlockItem.transform.GetChild(0).GetComponent<Image>().sprite = listImage[5];
                break;
            //lỗ hút đẩy được
            case 10:
                //ruleObject = Instantiate(listRules[10]);
                ruleObject = Instantiate(listRules[2]);
                break;
            //tối, rọi đèn
            case 11:
                ruleObject = Instantiate(listRules[11]);
                var lightObject = ruleObject.transform.GetChild(1);
                lightObject.transform.parent = null;
                lightObject.transform.position = new Vector3(Random.Range(-10f, 10f), lightObject.transform.position.y, Random.Range(16, 24));
                Destroy(lightObject.gameObject, 30);
                ruleObject.transform.parent = transform;
                ruleObject.transform.localPosition = Vector3.zero;
                light1.SetActive(false);
                light2.SetActive(false);
                RenderSettings.ambientSkyColor = new Color32(30, 30, 30, 0);
                //Camera.main.GetComponent<GlobalFog>().enabled = false;
                break;
            //súng bắn
            case 12:
                ruleObject = Instantiate(listRules[12]);
                break;
            //pumber đẩy đc
            case 13:
                //ruleObject = Instantiate(listRules[13]);
                ruleObject = ruleObject = Instantiate(listRules[8]);
                int randomItem2 = Random.Range(0, 2);
                foreach (Transform item in ruleObject.transform)
                {
                    var id = int.Parse(item.name);
                    Debug.Log(id);
                    if (id != randomItem2)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
                break;
            //pumber nhảy
            case 14:
                ruleObject = Instantiate(listRules[14]);
                map.transform.position = new Vector3(map.transform.position.x, 3, map.transform.position.z);
                map.GetComponent<PowerUpAnimation>().enabled = true;
                map.GetComponent<PowerUpAnimation>().SetAnimateYOffset(true);
                unlockItem.SetActive(true);
                unlockItem.transform.GetChild(0).GetComponent<Image>().sprite = listImage[0];
                break;
            //chèn khối
            case 15:
                ruleObject = Instantiate(listRules[15]);
                map.transform.position = new Vector3(map.transform.position.x, 2.75f, map.transform.position.z);
                break;
        }
    }

    public void LoadLevel()
    {
        wall = env.transform.GetChild(0).transform.GetChild(0).gameObject;
        wall.SetActive(false);
        if (listLevel[currentLevel] != null)
        {
            ruleObject = Instantiate(listLevel[currentLevel]);
        }
        else
        {
            if (currentLevel != 0 && currentLevel != 22 && currentLevel != 42 && currentLevel != 43 && currentLevel != 46 && currentLevel != 54 && currentLevel != 74)
            {
                ruleObject = Instantiate(listRules[15]);
                map.transform.position = new Vector3(map.transform.position.x, 2.75f, map.transform.position.z);
                //currentRule = 11;
                //ruleObject = Instantiate(listRules[11]);
                //var lightObject = ruleObject.transform.GetChild(1);
                //lightObject.transform.parent = null;
                //lightObject.transform.position = new Vector3(Random.Range(-10f, 10f), lightObject.transform.position.y, Random.Range(16, 24));
                //Destroy(lightObject.gameObject, 30);
                //ruleObject.transform.parent = transform;
                //ruleObject.transform.localPosition = Vector3.zero;
                //light1.SetActive(false);
                //light2.SetActive(false);
                //RenderSettings.ambientSkyColor = new Color32(30, 30, 30, 0);
            }
        }
    }

    IEnumerator delayStart()
    {
        yield return new WaitForSeconds(0.01f);
        //AnalyticsManager.instance.CallEvent(AnalyticsManager.EventType.StartEvent);
        rigid.isKinematic = true;
        isJump = false;
        isJumpShort = false;
        isInvincible = false;
        isMagnet = false;
        isGun = false;
        standardDistance = 0.5f;
        followSpeed = 0.05f;
        size = 1.5f;
        height = 0.25f;
        magnetLimit = 0;
        float zBound = -7.5f;
        int isFlyMap = 0;

        center.position = transform.position;
        var getColor = pixelExplode.GetComponent<ParticleSystem>().main;
        getColor.startColor = themeColor;
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        if ((currentLevel % 5 == 0 && currentLevel != 0) || currentLevel > 65)
        {
            StartCoroutine(RuleInit());
        }
        else
        {
            LoadLevel();
        }
        currentLevelText.text = currentLevel.ToString();
        nextLevelText.text = (currentLevel + 1).ToString();
        progressBar.value = 0;
        progressBar.maxValue = AddRemoveInstances.instance.instancesList.Count();
        money = PlayerPrefs.GetInt("money");
        moneyText.text = money.ToString();
        score = 0;
        scoreText.text = score.ToString();
        shopMenu_money.text = money.ToString();
        for (int i = 0; i < listShop.Count; i++)
        {
            string convertId = i.ToString();
            var isPurchased = PlayerPrefs.GetInt(convertId);
            if (isPurchased == 0)
            {
                listChecked[i].gameObject.SetActive(false);
                listPowerUp[i].SetActive(false);
            }
        }
        shopImage.sprite = shopIcon;
        shineEffect.Play();
        //if (listRotate.Contains(currentLevel) && currentRule != 15)
        //{
        //    map.GetComponent<PowerUpAnimation>().enabled = true;
        //    map.GetComponent<PowerUpAnimation>()._animateRotation = true;
        //}
        title.DOFade(1, 0);
        title.DOFade(0, 3);
        startGameMenu.SetActive(true);
        //levelInput.gameObject.SetActive(true);
        //nextButton.SetActive(true);
    }

    void InitSnake()
    {
        prefab.transform.localScale = Vector3.one * 1.5f;
        for (int i = 0; i < startCount; i++)
        {
            Transform _transform = Instantiate(prefab).transform;
            balls.Add(_transform);
            points.Add(balls[i].position);
            _transform.name = i.ToString();
            _transform.GetComponent<Renderer>().material.color = listColors[currentColor];
            _transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", listColors[currentColor] * 0.1f);
            colorChangeCount++;
            if (colorChangeCount >= 5)
            {
                currentColor++;
                if (currentColor > listColors.Count - 1)
                {
                    currentColor = 0;
                }
                colorChangeCount = 0;
            }
        }
        lastPosition = balls[0].position;
    }

    private void FixedUpdate()
    {
        if(waitForTapWin && Input.GetMouseButtonDown(0))
        {
            waitForTapWin = false;
            winPanel.SetActive(true);
            StartCoroutine(delayMoney());
        }

        if (isStartGame)
        {
            HeadControl();
        }

        if (balls.Count > 0)
        {
            Vector3 vector4 = target.transform.position - balls[0].transform.position;
            float magnitude = vector4.magnitude;
            if (magnitude > standardDistance)
            {
                balls[0].transform.position = Vector3.SmoothDamp(balls[0].transform.position, target.transform.position, ref velocity, followSpeed);
            }
            balls[0].transform.rotation = target.transform.rotation;
        }

        if (isJump)
        {
            float desiredY = Mathf.Abs(80 - transform.position.z);
            Vector3 desiredPosition = new Vector3(0, height + desiredY, 80);
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 100 * Time.deltaTime);
        }
        if (isJumpShort)
        {
            float desiredY = Mathf.Abs(desJump.z - transform.position.z);
            desiredY += Mathf.Abs(desJump.x - transform.position.x);
            Vector3 desiredPosition = new Vector3(desJump.x, height + desiredY, desJump.z);
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 25 * Time.deltaTime);
        }
    }

    private void Update()
    {
        if (balls.Count > 0)
        {
            SnakeMove();
        }
    }
    private void SnakeMove()
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

    private void HeadControl()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isHold = true;
        }

        if (Input.GetMouseButton(0) && isHold)
        {
#if UNITY_EDITOR
            h = Input.GetAxis("Mouse X");
            v = Input.GetAxis("Mouse Y");
#endif
#if UNITY_IOS
            if (Input.touchCount > 0)
            {
                h = Input.touches[0].deltaPosition.x / 8;
                v = Input.touches[0].deltaPosition.y / 8;
            }
#endif
            dir = new Vector3(h, 0, v);
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir),5*Time.deltaTime);
            }
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
            followPlayer.transform.position =  transform.position;
            followPlayer.transform.localScale = transform.localScale;
            rigid.AddForce(Vector3.down * 100);
            //rigid.AddRelativeForce(Vector3.forward * speed * 2000);
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -12.5f + size, 12.5f - size), transform.position.y, Mathf.Clamp(transform.position.z, -15.5f + size, 35.5f - size));

        if(Input.GetMouseButtonUp(0))
        {
            isHold = false;
        }
    }

    public void AddSnake()
    {
        if (isStartGame)
        {
            progressBar.value++;
            score++;
            scoreText.text = score.ToString();
            //SoundManager.instance.PlaySoundPitch(SoundManager.instance.hit);
            if (maxPlusEffect < 10)
            {
                Vector3 posSpawn = scoreText.transform.position;
                StartCoroutine(PlusEffect(posSpawn));
            }
            if (AddRemoveInstances.instance.instancesList.Count <= 0)
            {
                //foreach (var item in balls)
                //{
                //    var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
                //    if (prefab != null)
                //    {
                //        var getColor = prefab.GetComponent<ParticleSystem>().main;
                //        getColor.startColor = item.GetComponent<Renderer>().material.color;
                //        prefab.transform.position = item.transform.position;
                //        prefab.SetActive(true);
                //        prefab.GetComponent<ParticleSystem>().Play();
                //    }
                //    Destroy(item.gameObject);
                //}
                //GetComponent<MeshRenderer>().enabled = false;
                //balls.Clear();
                //points.Clear();
                mapFlowEffect.SetActive(true);
                MapFlowEffect.instance.isStop = false;
                StartCoroutine(Win());
            }
            addCount++;
            if (addCount >= 20)
            {
                try
                {
                    addCount = 0;
                    Transform _transform = Instantiate(prefab, balls[balls.Count - 1].position, Quaternion.identity).transform;
                    balls.Add(_transform);
                    _transform.name = (balls.Count - 1).ToString();
                    points.Add(_transform.position);
                    _transform.GetComponent<Renderer>().material.color = listColors[currentColor];
                    _transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", listColors[currentColor] * 0.1f);
                    colorChangeCount++;
                    if (colorChangeCount >= 5)
                    {
                        currentColor++;
                        if (currentColor > listColors.Count - 1)
                        {
                            currentColor = 0;
                        }
                        colorChangeCount = 0;
                    }
                }
                catch { }
            }
        }
    }

    private void RemoveSnake()
    {
        Vector3 zero = Vector3.zero;

        //remove the main ball
        if (this.balls.Count > 0)
        {
            balls[0].gameObject.SetActive(false);
            zero = balls[0].position;
            this.balls.RemoveAt(0);
        }

        //gameover
        if (this.balls.Count <= 0)
        {
            // game over
        }
        else // set next ball as main ball
        {
            Vector3 vector2 = balls[0].position;
            Vector3 vector4 = zero - vector2;
            Vector3 normalized = vector4.normalized;
            int count = 0;
            for (int i = 0; i < this.points.Count; i++)
            {
                if (Vector3.Dot(vector2 - this.points[i], normalized) >= 0f)
                {
                    break;
                }
                count = i;
            }

            points.RemoveRange(0, count);
            SnakeMove();
        }
    }

    IEnumerator BoundCalculator(int hitPos)
    {
        float x = 0;
        float y = 0;
        float z = 0;

        int count = 0;
        try
        {
            for (int i = 0; i < hitPos; i++)
            {
                count++;
                x += balls[i].transform.position.x;
                y += balls[i].transform.position.y;
                z += balls[i].transform.position.z;
            }
        }
        catch { }

        if (count != 0)
        {
            center.position = new Vector3(x / count, y / count, z / count);
        }
        else
            center.position = transform.position;

        List<GameObject> listRend = new List<GameObject>();
        List<GameObject> listDistint = new List<GameObject>();
        try
        {
            for (int i = 0; i <= hitPos; i++)
            {
                RaycastHit[] hits;
                dirCast = center.localPosition - balls[i].localPosition;
                float range = Vector3.Distance(balls[i].localPosition, center.position);

                hits = Physics.BoxCastAll(new Vector3(balls[i].localPosition.x, balls[i].localPosition.y, balls[i].localPosition.z), new Vector3(0.5f, 2, 0.5f), new Vector3(dirCast.x, dirCast.y, dirCast.z), Quaternion.identity, range);

                for (int j = 0; j < hits.Length; j++)
                {
                    RaycastHit hit = hits[j];
                    var rend = hit.transform.GetComponent<Tile>();
                    if (rend)
                    {
                        if (rend.tag == "Pixel")
                        {
                            listRend.Add(rend.gameObject);
                        }
                    }
                }
            }
        }
        catch { }
        listDistint = listRend.Distinct().ToList();
        foreach (var item in listDistint)
        {
            if (isStartGame)
            {
                var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
                if (prefab != null)
                {
                    var getColor = prefab.GetComponent<ParticleSystem>().main;
                    getColor.startColor = item.GetComponent<Tile>().tileColor;
                    prefab.transform.position = item.transform.position;
                    prefab.SetActive(true);
                    prefab.GetComponent<ParticleSystem>().Play();
                }
                AddRemoveInstances.instance.RemoveInstances(item.GetComponent<GPUInstancerPrefab>());
                AddSnake();
            }
        }
        if (!isBox)
        {
            size *= 1.005f;
            if (size > 3)
            {
                size = 3;
            }
            else
            {
                height *= 1.005f;
                standardDistance *= 1.005f;
            }
            transform.localScale = Vector3.one * size;
            transform.position = new Vector3(transform.position.x, height + transform.position.y, transform.position.z);
            prefab.transform.localScale = Vector3.one * size;
            foreach (var item in balls)
            {
                item.transform.localScale = Vector3.one * size;
                item.transform.position = new Vector3(item.transform.position.x, height + transform.position.y, item.transform.position.z);
            }
        }
        yield return new WaitForSeconds(0.01f);
        isDestroy = true;
    }

    public void PlusEffectMethod()
    {
        if (maxPlusEffect < 10)
        {
            Vector3 posSpawn = scoreText.transform.position;
            StartCoroutine(PlusEffect(posSpawn));
        }
    }

    IEnumerator PlusEffect(Vector3 pos)
    {
        maxPlusEffect++;
        if (!UnityEngine.iOS.Device.generation.ToString().Contains("5") && !isVibrate)
        {
            isVibrate = true;
            StartCoroutine(delayVibrate());
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }
        var plusVar = Instantiate(plusVarPrefab);
        plusVar.transform.SetParent(canvas.transform);
        plusVar.transform.localScale = new Vector3(1, 1, 1);
        //plusVar.transform.position = worldToUISpace(canvas, pos);
        plusVar.transform.position = new Vector3(pos.x + Random.Range(-50,50), pos.y + Random.Range(-100, -75), pos.z);
        plusVar.GetComponent<Text>().DOColor(new Color32(255, 255, 255, 0), 1f);
        plusVar.SetActive(true);
        plusVar.transform.DOMoveY(plusVar.transform.position.y + Random.Range(50, 90), 0.5f);
        plusVar.transform.DOMoveX(scoreText.transform.position.x, 0.5f);
        Destroy(plusVar, 0.5f);
        yield return new WaitForSeconds(0.01f);
        maxPlusEffect--;
    }

    IEnumerator delayVibrate()
    {
        yield return new WaitForSeconds(0.2f);
        isVibrate = false;
    }

    public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public void ButtonStartGame()
    {
        unlockItem.SetActive(false);
        startGameMenu.SetActive(false);
        rigid.isKinematic = false;
        //levelInput.gameObject.SetActive(false);
        //nextButton.SetActive(false);
        isStartGame = true;
        isHold = true;
        map.GetComponent<PowerUpAnimation>()._animateRotation = false;
        map.GetComponent<PowerUpAnimation>().enabled = false;
        map.transform.localRotation = Quaternion.identity;
        shopImage.sprite = powerUpIcon;
    }

    IEnumerator delayMoney()
    {
        int temp = money;
        money += score;
        PlayerPrefs.SetInt("money", money);
        while (temp < money - 50)
        {
            temp += 50;
            winMenu_money.text = temp.ToString();
            yield return new WaitForSeconds(0.00001f);
        }
        temp = money;
        winMenu_money.text = temp.ToString();
    }

    IEnumerator Win()
    {
        loading.SetActive(true);
        if (ruleObject != null)
        {
            Destroy(ruleObject);
        }
        if (powerUpObject != null)
        {
            Destroy(powerUpObject);
        }
        yield return new WaitForSeconds(0.01f);
        if (isStartGame)
        {
            //AnalyticsManager.instance.CallEvent(AnalyticsManager.EventType.EndEvent);
            isStartGame = false;
            foreach (Transform child in transform)
            {
                if (child.name == "mouth")
                {
                    //child.gameObject.SetActive(false);
                }
                else
                    Destroy(child.gameObject);
            }
            losePanel.SetActive(false);
            conffetiSpawn =  Instantiate(conffeti);
            currentRule = PlayerPrefs.GetInt("currentRule");
            currentRule++;
            if(currentRule > listRules.Count - 1)
            {
                currentRule = 0;
            }
            PlayerPrefs.SetInt("currentRule", currentRule);
            winMenu_title.text = "LEVEL " + currentLevel.ToString() + "\n" + "COMPLETED";
            currentLevel++;
            if (currentLevel > maxLevel)
            {
                currentLevel = 0;
            }
            PlayerPrefs.SetInt("currentLevel", currentLevel);
            groundColor++;
            if (groundColor > listGroundColors.Count - 1)
            {
                groundColor = 0;
            }
            PlayerPrefs.SetInt("themeColor", groundColor);
            winMenu_score.text = score.ToString();
            winMenu_money.text = "0";
            loading.SetActive(false);
            foreach (var item in map.GetComponentsInChildren<Tile>())
            {
                var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
                if (prefab != null)
                {
                    Debug.Log("Bum");
                    var getColor = prefab.GetComponent<ParticleSystem>().main;
                    getColor.startColor = item.GetComponent<Tile>().tileColor;
                    prefab.transform.position = item.transform.position;
                    prefab.SetActive(true);
                    prefab.GetComponent<ParticleSystem>().Play();
                }
                Destroy(item.gameObject);
            }
            ChangeThemeColor();

            isJump = false;
            isJumpShort = false;
            isInvincible = false;
            isMagnet = false;
            isGun = false;
            standardDistance = 0.5f;
            followSpeed = 0.05f;
            size = 1.5f;
            height = 0.25f;
            magnetLimit = 0;
            transform.localScale = Vector3.one * size;
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
            prefab.transform.localScale = Vector3.one * size;
            light1.SetActive(true);
            light2.SetActive(true);
            RenderSettings.ambientSkyColor = new Color32(149, 149, 149, 0);
            GetComponent<SphereCollider>().radius = 0.5f;
            yield return new WaitForSeconds(0.01f);
            wall.SetActive(true);
            dupMap = Instantiate(env, new Vector3(0, 0, 100), Quaternion.identity);
            map.transform.position = new Vector3(map.transform.position.x, map.transform.position.y, 100);
            yield return new WaitForSeconds(0.01f);
            mapReader.SetActive(false);
            mapReader.SetActive(true);
            wall = dupMap.transform.GetChild(0).transform.GetChild(0).gameObject;
            StartCoroutine(delayRefreshInstancer());
            waitForTapWin = true;

            rigid.isKinematic = true;
            transform.DOMove(new Vector3(0, transform.position.y, 7.5f), 1);
            transform.LookAt(new Vector3(0, transform.position.y, 7.5f));
            rope.transform.position = new Vector3(0, 55, 10);
            rope.SetActive(true);
            rope.transform.DOMoveY(20, 1);
            rope.transform.rotation = Quaternion.Euler(0, 0, 0);
            yield return new WaitForSeconds(1);
            transform.DORotateQuaternion(Quaternion.Euler(0, 90, 0), 0);
            transform.parent = rope.transform;
            rope.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 180, 0), 0.2f).SetLoops(-1, LoopType.Incremental);
            rope.transform.DOMoveY(100, 3);
            yield return new WaitForSeconds(3);
            transform.parent = null;
            rope.SetActive(false);
            foreach (var item in balls)
            {
                var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
                if (prefab != null)
                {
                    var getColor = prefab.GetComponent<ParticleSystem>().main;
                    getColor.startColor = item.GetComponent<Renderer>().material.color;
                    prefab.transform.position = item.transform.position;
                    prefab.SetActive(true);
                    prefab.GetComponent<ParticleSystem>().Play();
                }
                Destroy(item.gameObject);
            }
            balls.Clear();
            points.Clear();
            MapFlowEffect.instance.targetColor = themeColor;
            MapFlowEffect.instance.currentColor = themeColor * 1.4f;
            MapFlowEffect.instance.RunMapFlowEffect();
        }
    }

    IEnumerator delayRefreshInstancer()
    {
        yield return new WaitForSeconds(0.01f);
        AddRemoveInstances.instance.Setup();
        loading.SetActive(false);
    }

    public void Lose()
    {
        if (isStartGame)
        {
            //AnalyticsManager.instance.CallEvent(AnalyticsManager.EventType.EndEvent);
            isStartGame = false;
            losePanel.SetActive(true);
        }
    }

    public void LoadScene()
    {
        isPower = true;
        MapFlowEffect.instance.isStop = true;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        isFirstHit = false;
        var temp = conffetiSpawn;
        Destroy(temp);
        StartCoroutine(delayChangeMapCutscene());
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator delayChangeMapCutscene()
    {
        GetComponent<MeshRenderer>().enabled = true;
        InitSnake();
        followSpeed = 0;
        standardDistance = 1f;
        transform.rotation = Quaternion.identity;
        mouth.SetActive(true);
        //isJump = true;
        mapFlowEffect.SetActive(false);
        transform.position = new Vector3(0, 0.25f, 80);
        //while (transform.position.z != 80)
        //{
        //    yield return null;
        //}
        var temp = env;
        env = dupMap;
        transform.rotation = Quaternion.identity;
        foreach (var item in balls)
        {
            item.transform.DOMove(transform.position, 0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        followSpeed = 0.05f;
        standardDistance = 0;
        foreach (var item in balls)
        {
            item.transform.parent = env.transform;
        }
        transform.parent = env.transform;
        map.transform.parent = env.transform;
        isJump = false;
        temp.transform.DOMoveZ(-80, 1);
        Destroy(temp, 1);
        env.transform.DOMoveZ(10, 1);
        yield return new WaitForSeconds(1);
        transform.parent = null;
        map.transform.parent = null;
        foreach (var item in balls)
        {
            item.transform.parent = null;
        }
        StartCoroutine(delayStart());
    }

    public void OnChangeMap()
    {
        if (levelInput != null)
        {
            int level = int.Parse(levelInput.text.ToString());
            Debug.Log(level);
            if (level < maxLevel)
            {
                PlayerPrefs.SetInt("currentLevel", level);
                SceneManager.LoadScene(0);
            }
        }
    }

    public void ButtonNextLevel()
    {
        isStartGame = true;
        currentRule = PlayerPrefs.GetInt("currentRule");
        currentRule++;
        if (currentRule > listRules.Count - 1)
        {
            currentRule = 0;
        }
        PlayerPrefs.SetInt("currentRule", currentRule);
        currentLevel++;
        if (currentLevel > maxLevel)
        {
            currentLevel = 0;
        }
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        groundColor++;
        if(groundColor > listGroundColors.Count - 1)
        {
            groundColor = 0;
        }
        PlayerPrefs.SetInt("themeColor", groundColor);
        SceneManager.LoadScene(0);
    }

    public void ButtonPreviousLevel()
    {
        currentLevel--;
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        SceneManager.LoadScene(0);
    }

    IEnumerator delayMagnet(GameObject other)
    {
        other.transform.DOMove(transform.position, 0.5f);
        yield return new WaitForSeconds(0.01f);
        magnetLimit--;
        yield return new WaitForSeconds(0.2f);
        if (other.gameObject != null)
        {
            other.transform.DOKill();
            other.GetComponent<Tile>().isCheck = true;
            if (!isFirstHit)
            {
                isFirstHit = true;
                foreach (var item in AddRemoveInstances.instance.instancesList)
                {
                    item.GetComponent<Rigidbody>().isKinematic = false;
                    item.GetComponent<Rigidbody>().useGravity = true;
                    if (item.transform.localScale.x < 1)
                    {
                        item.transform.DOScale(Vector3.one, 5);
                    }
                }
            }
            var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
            if (prefab != null)
            {
                prefab.SetActive(true);
                var getColor = prefab.GetComponent<ParticleSystem>().main;
                getColor.startColor = other.GetComponent<Tile>().tileColor;
                prefab.transform.position = other.transform.position;
                prefab.GetComponent<ParticleSystem>().Play();
            }
            AddRemoveInstances.instance.RemoveInstances(other.GetComponent<GPUInstancerPrefab>());
            AddSnake();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Pixel") && isMagnet && !other.GetComponent<Tile>().isCheck)
        {
            if (magnetLimit < 50)
            {
                magnetLimit++;
                StartCoroutine(delayMagnet(other.gameObject));
            }
        }
    }

    public void delayPhysicMethod()
    {
        StartCoroutine(delayPhysic());
    }

    IEnumerator delayPhysic()
    {
        foreach (var item in AddRemoveInstances.instance.instancesList)
        {
            if (item != null)
            {
                item.GetComponent<Rigidbody>().isKinematic = false;
                item.GetComponent<Rigidbody>().useGravity = true;
                if (item.transform.localScale.x < 1)
                {
                    item.transform.DOScale(Vector3.one, 5);
                }
                yield return new WaitForSeconds(0.0001f);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Pixel") && !isMagnet)
        {
            if (!isFirstHit)
            {
                isFirstHit = true;
                foreach (var item in AddRemoveInstances.instance.instancesList)
                {
                    item.GetComponent<Rigidbody>().isKinematic = false;
                    item.GetComponent<Rigidbody>().useGravity = true;
                    if (item.transform.localScale.x < 1)
                    {
                        item.transform.DOScale(Vector3.one, 1);
                        //item.transform.localScale = Vector3.one;
                    }
                }
            }
            if (isBox)
            {
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
                AddSnake();
            }
            //isPush = true;
            other.gameObject.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * 0.00016f);
        }

        if (other.gameObject.CompareTag("Box") && !isBox)
        {
            if(other.gameObject.GetComponent<PowerUpAnimation>() != null)
            {
                other.gameObject.GetComponent<PowerUpAnimation>().enabled = false;
            }
            isBox = true;
            followPlayer.SetActive(false);
            //other.gameObject.tag = "Untagged";
            other.transform.parent.transform.localScale = Vector3.one;
            standardDistance = 0;
            size /= 10;
            height /= 10;
            standardDistance /= 10;
            transform.localScale = Vector3.one * size;
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
            prefab.transform.localScale = Vector3.one * size;
            foreach (var item in balls)
            {
                item.transform.localScale = Vector3.one * size;
                item.transform.position = new Vector3(item.transform.position.x, height, item.transform.position.z);
            }
            other.transform.parent = transform;
            other.transform.localPosition = new Vector3(0, other.transform.localPosition.y, 0);
            other.transform.localRotation = Quaternion.identity;
            GetComponent<SphereCollider>().radius = 2;
            StartCoroutine(delayDestroy(other.gameObject, 10));
        }

        if(other.gameObject.CompareTag("Hole Magnet"))
        {
            rigid.isKinematic = true;
            GetComponent<SphereCollider>().isTrigger = true;
        }     

        if (other.gameObject.CompareTag("Ball") || other.gameObject.CompareTag("Hole Push") || other.gameObject.CompareTag("Push"))
        {
            isPush = true;
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 5000);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pixel") && !isMagnet)
        {
            if (!isFirstHit)
            {
                isFirstHit = true;
                foreach (var item in AddRemoveInstances.instance.instancesList)
                {
                    item.GetComponent<Rigidbody>().isKinematic = false;
                    item.GetComponent<Rigidbody>().useGravity = true;                   
                    if (item.transform.localScale.x < 1)
                    {
                        item.transform.DOScale(Vector3.one, 1);
                        //item.transform.localScale = Vector3.one;
                    }
                }
            }
            if (isBox)
            {
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
                AddSnake();
            }
            //isPush = true;
            other.gameObject.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * 0.00015f);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (isStartGame && isDestroy && int.Parse(other.name) > 5)
            {
                isDestroy = false;
                var hitPos = int.Parse(other.gameObject.name.ToString());
                StartCoroutine(BoundCalculator(hitPos));
            }
        }

        if (other.gameObject.CompareTag("Bomb") && !isInvincible)
        {
            var explode = Instantiate(pixelExplode);
            explode.GetComponent<ParticleSystem>().maxParticles = 9;
            explode.GetComponent<ParticleSystem>().startColor = Color.yellow;
            explode.transform.position = other.transform.position;
            Destroy(other.transform.parent.gameObject);
            foreach (var item in balls)
            {
                var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
                if (prefab != null)
                {
                    var getColor = prefab.GetComponent<ParticleSystem>().main;
                    getColor.startColor = item.GetComponent<Renderer>().material.color;
                    prefab.transform.position = item.transform.position;
                    prefab.SetActive(true);
                    prefab.GetComponent<ParticleSystem>().Play();
                }
                item.GetComponent<MeshRenderer>().enabled = false;
            }
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(transform.GetChild(0).gameObject);
            Lose();
        }

        if (other.gameObject.CompareTag("Coin"))
        {
            other.transform.DOMoveY(other.transform.position.y + 7, 0.3f);
            //SoundManager.instance.PlaySound(SoundManager.instance.cash);
            score += 10;
            scoreText.text = score.ToString();
            Destroy(other.gameObject, 0.3f);
        }

        if (other.gameObject.CompareTag("Hole") && !isInvincible)
        {
            GameObject target = other.gameObject;
            StartCoroutine(delayHoleDie(target));
        }

        if (other.gameObject.CompareTag("Jump"))
        {
            standardDistance = 0.5f;
            StartCoroutine(delayJumpShort(transform.position + transform.forward * 10));
        }

        if (other.gameObject.CompareTag("Magnet"))
        {
            isMagnet = true;
            isInvincible = true;
            other.transform.GetChild(0).GetComponent<PowerUpAnimation>().enabled = false;
            other.transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, 0);
            other.transform.parent = transform;
            other.transform.localPosition = new Vector3(0, other.transform.localPosition.y, 0);
            other.transform.localRotation = Quaternion.Euler(90, 0, 0);
            other.GetComponent<SphereCollider>().radius = 1.5f;
            delayDestroy(other.gameObject, 10);
        }

        if (other.gameObject.CompareTag("Invincible"))
        {
            Destroy(other.gameObject);
            StartCoroutine(delayInvincible());
        }

        if (other.gameObject.CompareTag("DoubleSize"))
        {
            Destroy(other.gameObject);
            size *= 2;
            if (size > 3)
            {
                size = 3;
            }
            else
            {
                height *= 2;
                standardDistance *= 2;
            }
            transform.localScale = Vector3.one * size;
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
            prefab.transform.localScale = Vector3.one * size;
            foreach (var item in balls)
            {
                item.transform.localScale = Vector3.one * size;
                item.transform.position = new Vector3(item.transform.position.x, height, item.transform.position.z);
            }
        }

        if (other.gameObject.CompareTag("Gun"))
        {
            isGun = true;
            StartCoroutine(Shooting());
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Balloon"))
        {
            StartCoroutine(delayBalloon(other.gameObject));
        }

        if (other.gameObject.CompareTag("Light"))
        {
            Destroy(other.gameObject);
            light1.SetActive(true);
            light2.SetActive(true);
            RenderSettings.ambientSkyColor = new Color32(149, 149, 149, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hole Magnet"))
        {
            GetComponent<SphereCollider>().isTrigger = false;
            rigid.isKinematic = false;
        }
    }

    IEnumerator Shooting()
    {
        while (isGun)
        {
            yield return new WaitForSeconds(0.1f);
            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            if (bullet != null)
            {
                bullet.transform.position = (transform.position + transform.forward);
                bullet.transform.rotation = Quaternion.identity;
                bullet.SetActive(true);
                bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 2000);
                Destroy(bullet, 2);
            }
        }
    }

    IEnumerator delayHoleDie(GameObject hole)
    {
        transform.DOMove(new Vector3(hole.transform.position.x, -2, hole.transform.position.z), 0.05f);
        yield return new WaitForSeconds(0.05f);
        float height = balls.Count;
        if(height < 15)
        {
            height = 15;
        }
        transform.DOMoveY(-height, 1.5f);
        yield return new WaitForSeconds(1.5f);
        Lose();
    }

    IEnumerator delayDisable(GameObject target, float time)
    {
        yield return new WaitForSeconds(time);
        if (target != null)
        {
            target.gameObject.SetActive(false);
            target.transform.position = transform.position;
            target.transform.rotation = Quaternion.identity;
        }
    }

    IEnumerator delayDestroy(GameObject target, float time)
    {
        while (isStartGame)
        {
            yield return null;
        }
        if (target != null)
            Destroy(target);
        isBox = false;
        standardDistance = 0;
        size = 1.5f;
        height = 0.25f;
        standardDistance = 0.5f;
        transform.localScale = Vector3.one * size;
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
        prefab.transform.localScale = Vector3.one * size;
        foreach (var item in balls)
        {
            item.transform.localScale = Vector3.one * size;
            item.transform.position = new Vector3(item.transform.position.x, height, item.transform.position.z);
        }
        isInvincible = false;
        GetComponent<SphereCollider>().radius = 0.5f;
        followPlayer.SetActive(true);
    }

    IEnumerator delayBalloon(GameObject target)
    {
        target.transform.parent = transform;
        target.transform.localPosition = new Vector3(0, 1.4f, 0);
        rigid.useGravity = false;
        rigid.isKinematic = true;
        transform.DOLocalMoveY(10, 5);

        if (isFlyMap <= 50)
        {
            yield return new WaitForSeconds(5);
        }
        else if(isFlyMap > 50)
        {
            while (!isFirstHit)
            {
                yield return null;
            }
        }
        transform.DOKill();

        var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
        if (prefab != null)
        {
            prefab.SetActive(true);
            var getColor = prefab.GetComponent<ParticleSystem>().main;
            getColor.startColor = Color.magenta;
            prefab.transform.position = target.transform.position;
            prefab.GetComponent<ParticleSystem>().Play();
        }
        Destroy(target);
        transform.DOMoveY(height, 0.25f);
        standardDistance = 0;
        yield return new WaitForSeconds(0.25f);
        rigid.useGravity = true;
        rigid.isKinematic = false;
        standardDistance = 0.5f;
        if(!isFirstHit)
        {
            ruleObject = Instantiate(listRules[0]);
            map.transform.position = new Vector3(map.transform.position.x, 5, map.transform.position.z);
            map.GetComponent<PowerUpAnimation>().enabled = true;
        }
    }

    IEnumerator delayJumpShort(Vector3 des)
    {
        desJump = des;
        //isStartGame = false;
        rigid.AddForce(transform.up * 3000);
        yield return new WaitForSeconds(0.25f);
        rigid.AddForce(-transform.up * 1000);
        //isJumpShort = true;
        //while (transform.position != des)
        //{
        //    yield return null;
        //}
        isJumpShort = false;
        //isStartGame = true;
        standardDistance = 0.5f;
    }

    IEnumerator delayInvincible()
    {
        isInvincible = true;
        speed += 10;
        yield return new WaitForSeconds(10);
        speed -= 10;
        isInvincible = false;
    }

    //public void OnPowerUpButton()
    //{
    //    powerUpMenu.SetActive(false);
    //    int rule = int.Parse(EventSystem.current.currentSelectedGameObject.name);
    //    switch (rule)
    //    {
    //        case 0:
    //            powerUpObject = Instantiate(listRules[7]);
    //            break;
    //        case 1:
    //            powerUpObject = Instantiate(listRules[5]);
    //            break;
    //        case 2:
    //            powerUpObject = Instantiate(listRules[8]);
    //            int randomItem = Random.Range(0, 3);
    //            foreach (Transform item in powerUpObject.transform)
    //            {
    //                var id = int.Parse(item.name);
    //                if (id != randomItem)
    //                {
    //                    item.gameObject.SetActive(false);
    //                }
    //            }
    //            break;
    //        case 3:
    //            powerUpObject = Instantiate(listRules[9]);
    //            wall.SetActive(true);
    //            break;
    //        case 4:
    //            isMyBot = true;
    //            powerUpObject = Instantiate(listRules[4]);
    //            wall.SetActive(true);
    //            break;
    //        case 5:
    //            powerUpObject = Instantiate(listRules[1]);
    //            var currentBox = PlayerPrefs.GetInt("currentBox");
    //            var currentBoxObject = powerUpObject.transform.GetChild(currentBox);
    //            var boxNum = powerUpObject.transform.childCount;
    //            foreach (Transform child in powerUpObject.transform)
    //            {
    //                if (child.name != currentBoxObject.name)
    //                {
    //                    child.gameObject.SetActive(false);
    //                }
    //            }
    //            currentBox++;
    //            if (currentBox > boxNum - 1)
    //            {
    //                currentBox = 0;
    //            }
    //            PlayerPrefs.SetInt("currentBox", currentBox);
    //            break;
    //        case 6:
    //            powerUpObject = Instantiate(listRules[13]);
    //            break;
    //        case 7:
    //            powerUpObject = Instantiate(listRules[10]);
    //            wall.SetActive(true);
    //            break;          
    //    }
    //    listPowerUp[rule].SetActive(false);
    //    string convertId = rule.ToString();
    //    PlayerPrefs.SetInt(convertId, 0);
    //    isPower = false;
    //}

    //public void OnPurchaseButton()
    //{
    //    int rule = int.Parse(EventSystem.current.currentSelectedGameObject.name);
    //    int price = int.Parse(listShop[rule].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text);
    //    if(money > price)
    //    {
    //        money -= price;
    //        PlayerPrefs.SetInt("money", money);
    //        shopMenu_money.text = money.ToString();
    //        moneyText.text = money.ToString();
    //        PlayerPrefs.SetInt(rule.ToString(), 1);
    //        newItem.SetActive(true);
    //        listChecked[rule].gameObject.SetActive(true);
    //        listPowerUp[rule].SetActive(true);
    //    }
    //}

    //public void OnPowerUpShopButton()
    //{
    //    //SoundManager.instance.PlaySound(SoundManager.instance.button);
    //    if (isStartGame)
    //    {
    //        if (isPower)
    //        {
    //            if (!powerUpMenu.activeSelf)
    //            {
    //                newItem.SetActive(false);
    //                powerUpMenu.SetActive(true);
    //            }
    //            else
    //            {
    //                powerUpMenu.SetActive(false);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        shopMenu.SetActive(true);
    //    }
    //}

    //public void OnBackButton()
    //{
    //    shopMenu.SetActive(false);
    //}
}
