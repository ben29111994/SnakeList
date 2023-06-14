using UnityEngine;
using GPUInstancer;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public Color tileColor;
    private Renderer meshRenderer;
    public bool isCheck = false;
    GameController gameController;

    private void OnEnable()
    {
        Init();
        gameController = GameObject.FindGameObjectWithTag("Player").GetComponent<GameController>();
    }

    public void Init()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if ((transform.position.y > 100 || transform.position.y < -10) && !isCheck)
        {
            isCheck = true;
            var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
            if (prefab != null)
            {
                prefab.SetActive(true);
                var getColor = prefab.GetComponent<ParticleSystem>().main;
                getColor.startColor = tileColor;
                prefab.transform.position = transform.position;
                prefab.GetComponent<ParticleSystem>().Play();
            }
            AddRemoveInstances.instance.RemoveInstances(GetComponent<GPUInstancerPrefab>());
            gameController.PlusEffectMethod();
            gameController.AddSnake();
        }
    }

    public void SetTransfrom(Vector3 pos,Vector3 scale)
    {
        transform.localPosition = pos;
        transform.localScale = new Vector3(scale.x,scale.y,scale.z);
    }

    public void SetColor(Color inputColor)
    {
        tileColor = inputColor;       
        meshRenderer.material.color = tileColor;
        tag = "Pixel";
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Destroy"))
        {
            var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
            if (prefab != null)
            {
                prefab.SetActive(true);
                var getColor = prefab.GetComponent<ParticleSystem>().main;
                getColor.startColor = tileColor;
                prefab.transform.position = transform.position;
                prefab.GetComponent<ParticleSystem>().Play();
            }
            try { AddRemoveInstances.instance.RemoveInstances(GetComponent<GPUInstancerPrefab>()); }
            catch { }
            gameController.PlusEffectMethod();
            gameController.AddSnake();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Destroy") && !isCheck)
        {
            isCheck = true;
            var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
            if (prefab != null)
            {
                prefab.SetActive(true);
                var getColor = prefab.GetComponent<ParticleSystem>().main;
                getColor.startColor = tileColor;
                prefab.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                prefab.GetComponent<ParticleSystem>().Play();
            }
            AddRemoveInstances.instance.RemoveInstances(GetComponent<GPUInstancerPrefab>());
            gameController.PlusEffectMethod();
            gameController.AddSnake();
        }

        if (tag != "Pixel" && other.gameObject.CompareTag("Bomb") && !isCheck)
        {
            isCheck = true;
            transform.GetChild(0).GetComponent<SphereCollider>().radius = 6 / transform.localScale.x;
            other.GetComponent<SphereCollider>().radius = 6 / transform.localScale.x;
            gameController.isInvincible = true;
            var explode = Instantiate(gameController.pixelExplode);
            explode.GetComponent<ParticleSystem>().maxParticles = 9;
            explode.GetComponent<ParticleSystem>().startColor = Color.yellow;
            explode.transform.position = other.transform.position;
            Destroy(gameObject, 0.05f);
        }

        if (tag == "Pixel" && other.gameObject.CompareTag("Bomb") && !isCheck)
        {
            isCheck = true;
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
                getColor.startColor = tileColor;
                prefab.transform.position = transform.position;
                prefab.GetComponent<ParticleSystem>().Play();
            }
            AddRemoveInstances.instance.RemoveInstances(GetComponent<GPUInstancerPrefab>());
            gameController.AddSnake();
        }
    }

    public void Check()
    {
        isCheck = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        //if (other.gameObject.CompareTag("Destroy") && !isCheck)
        //{
        //    isCheck = true;
        //    var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
        //    if (prefab != null)
        //    {
        //        prefab.SetActive(true);
        //        var getColor = prefab.GetComponent<ParticleSystem>().main;
        //        getColor.startColor = tileColor;
        //        prefab.transform.position = transform.position;
        //        prefab.GetComponent<ParticleSystem>().Play();
        //    }
        //    AddRemoveInstances.instance.RemoveInstances(GetComponent<GPUInstancerPrefab>());
        //    gameController.PlusEffectMethod();
        //    gameController.AddSnake();
        //}

        if ((other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Ball")) && !isCheck && tag == "Pixel")
        {
            isCheck = true;
            if (!other.gameObject.CompareTag("Ball"))
            {
                //other.gameObject.SetActive(false);
                Destroy(other.gameObject);
            }
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

            var prefab = PoolManager.instance.GetObject(PoolManager.NameObject.pixelExplode);
            if (prefab != null)
            {
                prefab.SetActive(true);
                var getColor = prefab.GetComponent<ParticleSystem>().main;
                getColor.startColor = tileColor;
                prefab.transform.position = transform.position;
                prefab.GetComponent<ParticleSystem>().Play();
            }
            AddRemoveInstances.instance.RemoveInstances(GetComponent<GPUInstancerPrefab>());
            gameController.AddSnake();
        }

        if(other.gameObject.CompareTag("Push") && !isCheck && tag == "Pixel" && gameController.isPush)
        {
            isCheck = true;
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
            }
        }

        if (tag != "Pixel" && (other.gameObject.CompareTag("Ball") || other.gameObject.CompareTag("Bullet")) && !isCheck)
        {
            isCheck = true;
            transform.GetChild(0).GetComponent<SphereCollider>().radius = 6/transform.localScale.x;
            gameController.isInvincible = true;
            var explode = Instantiate(gameController.pixelExplode);
            explode.GetComponent<ParticleSystem>().maxParticles = 9;
            explode.GetComponent<ParticleSystem>().startColor = Color.yellow;
            explode.transform.position = other.transform.position;
            Destroy(other.gameObject, 0.05f);
            Destroy(gameObject, 0.05f);
        }

        //if (tag != "Pixel" && other.gameObject.CompareTag("Pixel"))
        //{
        //    other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward);
        //}
        if (other.gameObject.CompareTag("WallPush"))
        {
            //GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().AddForce((Vector3.up + transform.position - new Vector3(0, 0, 12)) * 0.01f);
        }
    }

    //private void OnCollisionStay(Collision other)
    //{
    //    if (other.gameObject.CompareTag("WallPush"))
    //    {
    //        //GetComponent<BoxCollider>().isTrigger = true;
    //        GetComponent<Rigidbody>().AddForce((Vector3.up + transform.position - new Vector3(0, 0, 12)) * 0.1f);
    //    }
    //}
}
