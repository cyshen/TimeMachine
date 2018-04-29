using HoloToolkit.Unity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.WSA.Persistence;
using HoloToolkit.Unity.SpatialMapping;

public class RoomLoader : MonoBehaviour
{

    public GameObject surfaceObject;            // prefab for surface mesh objects
    public string fileName;                     // name of file used to store mesh
    public string anchorStoreName;              // name of world anchor for room

    List<Mesh> roomMeshes;                      // list of meshes saved from spatial mapping
    WorldAnchorStore anchorStore;               // store of world anchors

    List<List<GameObject>> fileList = new List<List<GameObject>>();
    List<string> fileNames = new List<string>();
    List<bool> fileOnDisplay = new List<bool>();
    List<Color> colors = new List<Color> { Color.blue, Color.green, Color.red, Color.yellow, Color.cyan };
    int color_count = 0;
    bool miniature = false;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        /* Sharon Shen, cyshen 04 2018
         * Update roomLoader based on three cases: 1. New file is selected to display. 
         * 2. Displaying file is slected to hide. 3. No file is selected to show, resume green mesh
         */
        //Sharon Shen, cyshen 04 2018, check the state of model scale (mini/normal)
        if (GameObject.Find("miniature").GetComponent<Renderer>().material.color == new Color(1.0f, 0.0f, 0.0f, 0.0f)) miniature = true;
        else if (GameObject.Find("normalscale").GetComponent<Renderer>().material.color == new Color(1.0f, 0.0f, 0.0f, 0.0f)) miniature = false;
        //Sharon Shen, cyshen 03 2018, if any file is selected to display
        if (GameObject.Find("file_clicked") != null)
        {
            fileName = GameObject.Find("file_clicked").transform.GetChild(0).GetComponent<TextMesh>().text.Replace("/", "-").Replace("\n", "").Replace(":","-");
            Debug.Log("Load FileName: " + fileName);
            GameObject.Find("file_clicked").name = "file_show";

            // if already loaded but hidden, i.e. it's not the first time for this file to be selected
            if (fileNames.IndexOf(fileName) != -1)
            {
                int clickedFileIndex = fileNames.IndexOf(fileName);
                List<GameObject> objList = fileList[clickedFileIndex];
                fileOnDisplay[clickedFileIndex] = true;
                foreach (GameObject obj in objList)
                {
                    //obj.GetComponent<Renderer>().material.color.a = 1.0f;
                    obj.GetComponent<Renderer>().enabled = true;
                    if (miniature) obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    else obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
            // else if the file is called the first time 
            else
            { 
                // get instance of WorldAnchorStore
                WorldAnchorStore.GetAsync(AnchorStoreReady);
                fileOnDisplay.Add(true);
            }
            // Stop showing spatial understanding mesh
            SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh = false;
        }
            
        else if (GameObject.Find("file_hide") != null)
        {
            Debug.Log("Hide file mesh");
            fileName = GameObject.Find("file_hide").transform.GetChild(0).GetComponent<TextMesh>().text.Replace("/", "-").Replace("\n", "").Replace(":", "-");
            int clickedFileIndex = fileNames.IndexOf(fileName);
            List<GameObject> objList = fileList[clickedFileIndex];
            fileOnDisplay[clickedFileIndex] = false;
            foreach (GameObject obj in objList)
            {
                //obj.GetComponent<Renderer>().material.color.a = 1.0f;
                obj.GetComponent<Renderer>().enabled = false;
            }
            GameObject.Find("file_hide").name = "file";
            // Resume spatial understanding mesh if no file is currently displaying
            if (GameObject.Find("file_show")==null)
                SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh = true;
        }
        // set the scale small if miniature is selected
        for (int i = 0; i< fileOnDisplay.Count; i++)
        {
            if (fileOnDisplay[i])
            {
                List<GameObject> objList = fileList[i];
                foreach (GameObject obj in objList)
                {
                    if (miniature) obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    else obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            }
 
        }

    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        // save instance
        anchorStore = store;

        // load room meshes
        roomMeshes = MeshSaver.Load(fileName) as List<Mesh>;

        List<GameObject> objList = new List<GameObject>();

        foreach (Mesh surface in roomMeshes)
        {
            //GameObject obj = Instantiate(surfaceObject) as GameObject;
            // Sharon Shen 04 2018, create a sphere as a basis for it to build a mesh model
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = new Vector3(0, 0, 0);
            if (miniature) obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            obj.GetComponent<Renderer>().material.color = colors[color_count];
            obj.GetComponent<MeshFilter>().mesh = surface;
            //obj.GetComponent<MeshCollider>().sharedMesh = surface;
            if (!anchorStore.Load(surface.name, obj))
                Debug.Log("WorldAnchor load failed...");

            Debug.Log("Mesh " + surface.name + " Position: " + obj.transform.position + "\n--- Rotation: " + obj.transform.localRotation + "\n--- Scale: " + obj.transform.localScale);
            objList.Add(obj);
        }
        fileList.Add(objList);
        fileNames.Add(fileName);
        //GameObject.Find("GameManager").GetComponent<GameManager>().RoomLoaded();
        color_count = (color_count+1) % 5;
    }
}