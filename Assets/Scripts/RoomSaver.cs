using HoloToolkit.Unity;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using HoloToolkit.Unity.SpatialMapping;

public class RoomSaver : MonoBehaviour
{
    private int saved_file_count = 0;
    public string fileName ;             // name of file to store meshes
    public string anchorStoreName;      // name of world anchor to store for room

    List<MeshFilter> roomMeshFilters;
    WorldAnchorStore anchorStore;
    int meshCount = 0;

    // Use this for initialization
    void Start()
    {
        Debug.Log("RoomSaver Starts");
        WorldAnchorStore.GetAsync(AnchorStoreReady);
    }

    private void Update()
    {
        
        if (GameObject.Find("Cube_save_clicked") != null)
        {
            
            Debug.Log("Update saveroom");
            GameObject.Find("Cube_save_clicked").name = "Cube_save";
            saveRoom();
        }
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
    }

    public void saveRoom()
    {
        // if the anchor store is not ready then we cannot save the room mesh
        if (anchorStore == null)
        {
            Debug.Log("anchorStore == null");
            return;
        }
            

        // delete old relevant anchors
        string[] anchorIds = anchorStore.GetAllIds();
        for (int i = 0; i < anchorIds.Length; i++)
        {
            if (anchorIds[i].Contains(anchorStoreName))
            {
                anchorStore.Delete(anchorIds[i]);
            }
        }

        Debug.Log("Old anchors deleted...");

        // get all mesh filters used for spatial mapping meshes
        roomMeshFilters = (SpatialUnderstanding.Instance.UnderstandingCustomMesh.GetMeshFilters()) as List<MeshFilter>;

        //cyshen 04 2018 save as .obj file
        string saveFileName = "(" + (++saved_file_count).ToString() + ")" + System.DateTime.Now.ToString().Replace(" ", "\n");
        MeshSaver.saveWaveFront(saveFileName.Replace("/", "-").Replace("\n", "").Replace(":", "-"), roomMeshFilters);

        Debug.Log("Mesh filters fetched...");

        // create new list of room meshes for serialization
        List<Mesh> roomMeshes = new List<Mesh>();

        // cycle through all room mesh filters
        foreach (MeshFilter filter in roomMeshFilters)
        {
            // increase count of meshes in room
            meshCount++;

            // make mesh name = anchor name + mesh count
            string meshName = anchorStoreName + meshCount.ToString();
            filter.mesh.name = meshName;

            Debug.Log("Mesh " + filter.mesh.name + ": " + filter.transform.position + "\n--- rotation " + filter.transform.localRotation + "\n--- scale: " + filter.transform.localScale);
            // add mesh to room meshes for serialization
            roomMeshes.Add(filter.mesh);

            // save world anchor
            WorldAnchor attachingAnchor = filter.gameObject.GetComponent<WorldAnchor>();
            if (attachingAnchor == null)
            {
                attachingAnchor = filter.gameObject.AddComponent<WorldAnchor>();
                Debug.Log("" + filter.mesh.name + ": Using new anchor...");
            }
            else
            {
                Debug.Log("" + filter.mesh.name + ": Deleting existing anchor...");
                DestroyImmediate(attachingAnchor);
                Debug.Log("" + filter.mesh.name + ": Creating new anchor...");
                attachingAnchor = filter.gameObject.AddComponent<WorldAnchor>();
            }
            if (attachingAnchor.isLocated)
            {
                if (!anchorStore.Save(meshName, attachingAnchor))
                    Debug.Log("" + meshName + ": Anchor save failed...");
                else
                    Debug.Log("" + meshName + ": Anchor SAVED...");
            }
            else
            {
                attachingAnchor.OnTrackingChanged += AttachingAnchor_OnTrackingChanged;
            }
            // cyshen 032018, delete saved mesh, or it would stay in the view
            DestroyImmediate(filter);
        }
        
        // cyshen 032018, attach a new cube to menu, with time stamp on it
        Transform brick = GameObject.Find("Cube_save").transform;
        Vector3 menu_pos = GameObject.Find("menu").transform.position;
        Vector3 diff = new Vector3(brick.position.x - menu_pos.x, brick.position.y - menu_pos.y, brick.position.z - menu_pos.z)* saved_file_count;
        Transform brick2 = Instantiate(brick, new Vector3(brick.position.x + diff.x, brick.position.y + diff.y, brick.position.z + diff.z), brick.rotation, GameObject.Find("Panel").transform);
        brick2.name = "file";
        brick2.gameObject.AddComponent<ClickCommands>();
        brick2.GetChild(0).GetComponent<TextMesh>().text = saveFileName;
        brick2.GetChild(0).GetComponent<TextMesh>().fontSize = 10;
        brick2.gameObject.GetComponent<Renderer>().material.color = new Color(0.5f, 0.0f, 0.5f, 0.0f);
        Debug.Log("New Brick Appear");
        // serialize and save meshes
        MeshSaver.Save(saveFileName.Replace("/","-").Replace("\n","").Replace(":","-"), roomMeshes);
        

    }
 
    private void AttachingAnchor_OnTrackingChanged(WorldAnchor self, bool located)
    {
        if (located)
        {
            string meshName = self.gameObject.GetComponent<MeshFilter>().mesh.name;
            if (!anchorStore.Save(meshName, self))
                Debug.Log("" + meshName + ": Anchor save failed...");
            else
                Debug.Log("" + meshName + ": Anchor SAVED...");

            self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
        }
    }
}