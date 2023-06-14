using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

    public static LevelGenerator instance;
    public GameObject map;
    public Tile tilePrefab;
    public GameObject parentObject;
    public Tile coin;
    public Tile bomb;
    float bombCount = 1;
    float ratio;
    public List<TextAsset> listMaps = new List<TextAsset>();

    public TextAsset textAsset;

    void OnEnable()
    {
        instance = this;
        var currentLevel = PlayerPrefs.GetInt("currentLevel");
        //map = Instantiate(list3DMaps[currentLevel]);
        //map.transform.SetParent(parentObject.transform);
        //ratio = map.transform.localScale.x;
        //foreach (Transform child in map.transform)
        //{
        //    GenerateTile(child.gameObject);
        //}
        //Destroy(map);

        //var level = "lv" + currentLevel.ToString();
        ////string path = System.IO.Path.Combine(Application.persistentDataPath, level);
        //textAsset = (TextAsset)Resources.Load(level, typeof(TextAsset));
        //ReadLevelText(textAsset);
        //bombCount = 1;

        textAsset = listMaps[currentLevel];
        ReadLevelText(textAsset);
    }

    private void GenerateTile(Vector3 pos, Color pixelColor, float ratio)
    {
        Tile instance;

        Vector3 scale = Vector3.one * ratio;

        //if (y == texture.height - 2 && x % 2 == 0)
        //{
        //    Vector3 coinScale = coin.transform.localScale * ratio;
        //    var coinPos = new Vector3(x - texture.width / 2 * ratio, (y + 3) * ratio, 0);
        //    instance = Instantiate(coin);
        //    instance.transform.SetParent(currentParent);
        //    instance.SetTransfrom(coinPos, coinScale);
        //    instance.tag = "Coin";
        //    instance.tileColor = Color.yellow;
        //}
        //var randomBomb = Random.Range(0, 100);
        //if (randomBomb > 70 && bombCount > 0)
        //{
        //    bombCount--;
        //    Vector3 bombScale = bomb.transform.localScale * ratio;
        //    //var bombPos = new Vector3(x - texture.width / 2 * ratio, 1.5f * ratio, -8);
        //    instance = Instantiate(bomb);
        //    //instance.transform.SetParent(currentParent);
        //    //instance.SetTransfrom(bombPos, bombScale);
        //    instance.transform.GetChild(0).tag = "Bomb";
        //    instance.tileColor = Color.black;
        //}

        instance = Instantiate(tilePrefab);
        instance.transform.SetParent(parentObject.transform);

        instance.Init();
        instance.SetTransfrom(pos, scale);
        instance.SetColor(pixelColor);
    }

    public void ReadLevelText(TextAsset textAsset)
    {
        List<string> levelTextArray = new List<string>(textAsset.text.Split('\n'));

        for (int i = 0; i < levelTextArray.Count - 1; i++)
        {
            List<string> textFixed = new List<string>(levelTextArray[i].Split(null));

            if (i >= 1)
                InitPosition(textFixed);
        }
    }

    private void InitPosition(List<string> textFixed)
    {
        Vector3 pos = new Vector3(float.Parse((textFixed[0])), float.Parse((textFixed[1])), float.Parse((textFixed[2])));
        Color colorConvert;
        ColorUtility.TryParseHtmlString( "#" + textFixed[3], out colorConvert);
        float scale = float.Parse(textFixed[4]);
        GenerateTile(pos, colorConvert, scale);
    }
}
