using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDB : MonoBehaviour
{

    public GameObject[] ModelSet;
    public int resWidth = 64;
    public int resHeight = 64;
    public Camera captureCamera;
    public Light LightSource;
    public int Counter = 0;
    public List<ObjectParameters> ObjectParametersList;
    public Camera checkCam;
    private Plane[] planes;
    public int remainingImages = 100;

    public ObjectParameters BuildScene()
    {
        var pos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1.1f), Random.Range(-1f, 2.5f));
        var sc = new Vector3(Random.Range(1.1f, 1.3f), Random.Range(1.1f, 1.3f), Random.Range(1.1f, 1.3f));
        var clr = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        var rot = Random.rotation;
        var index = Random.Range(0, ModelSet.Length);

        var g = Instantiate(ModelSet[index], pos, rot);
        g.transform.localScale = sc;

        Renderer rend = g.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Standard");
        rend.material.SetColor("_Color", clr);

        g.transform.parent = transform;
        LightSource.transform.rotation = Random.rotation;

        var objCol = g.GetComponent<Collider>();

        if (GeometryUtility.TestPlanesAABB(planes, objCol.bounds))
        {
            return new ObjectParameters()
            {
                position = pos,
                scale = sc,
                color = clr,
                rotation = rot,
                model = ModelSet[index].name
            };
        }
        Debug.Log("Bad position!");
        return null;
    }

    public void ClearScene()
    {
        var killList = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            killList[i] = transform.GetChild(i).gameObject;
        }

        foreach (var killTarget in killList)
        {
            Destroy(killTarget);
        }
    }

    public void Start()
    {
        ObjectParametersList = new List<ObjectParameters>();

        ClearScene();
        planes = GeometryUtility.CalculateFrustumPlanes(checkCam);
        //InvokeRepeating("Run", 1, 0.9f);
        StartCoroutine(CaptureImages());
    }



    public IEnumerator CaptureImages()
    {
        while (remainingImages >=0)
        {
            var modelParams = BuildScene();
            if (modelParams != null)
            {
                Counter++;

                RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
                captureCamera.targetTexture = rt;
                Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
                captureCamera.Render();
                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                captureCamera.targetTexture = null;
                RenderTexture.active = null; // JC: added to avoid errors
                Destroy(rt);
                byte[] bytes = screenShot.EncodeToPNG();

                modelParams.imageName = string.Format("prim-{0}.png", Counter);

                System.IO.File.WriteAllBytes(string.Format("{0}/demo/{1}",
                                 Application.dataPath, modelParams.imageName), bytes);

                

                ObjectParametersList.Add(modelParams);

                yield return new WaitForEndOfFrame();
                remainingImages--;
            }
            yield return new WaitForEndOfFrame();
            ClearScene();
            yield return new WaitForEndOfFrame();
        }

        var db = "";
        foreach(var item in ObjectParametersList)
        {
            db += item.ToString();
            db += "\n";
        }
        System.IO.File.WriteAllText(string.Format("{0}/demo/db.csv",
                         Application.dataPath), db);


        Debug.Log(string.Format("DONE!!"));
    }


}
