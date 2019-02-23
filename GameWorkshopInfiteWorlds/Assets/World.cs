using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Using singleton here - singleton is being extended so it can easily be referenced for any object being added in the world
public class World : Singleton<World>
{
    public int seed;
    public Material mat;
    public int chunkSize = 16;
    public int worldHeight = 1;
    public int chunkHeight = 150;
    public int initialWorldSize = 2;
    public int WaterHeight = 15;
    public GameObject chunk;
    public Dictionary<Vector3, Chunk> WorldList;
    public Vector3 PlayerStartPosition;
    public Noise n;

    //Acts as a wait function 
    IEnumerator BuildWorld()
    {
        Random.seed = seed; // set  the seed random
        WorldList = new Dictionary<Vector3, Chunk>();
        n = GetComponent<Noise>();
        n.offsetX = Random.Range(0f, 99999f);
        n.offsetZ = Random.Range(0f, 99999f);

        for(int x = 0; x < initialWorldSize; x++)
            for(int z = 0; z < initialWorldSize; z++)
                for(int y = 0; y < worldHeight; y++)
                {
                    GameObject gem = GameObject.Instantiate(chunk, new Vector3(x * chunkSize, y * chunkHeight, z * chunkSize), Quaternion.identity);//Quat is a default rotation
                    WorldList.Add(new Vector3(x, y, z), gem.GetComponent<Chunk>());
                    gem.GetComponent<Chunk>().BuildChunk();
                    yield return new WaitForSeconds(0.5f);
                }

    }



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BuildWorld());
;    }

    // Update is called once per frame
    void Update()
    {
        
    }




}
