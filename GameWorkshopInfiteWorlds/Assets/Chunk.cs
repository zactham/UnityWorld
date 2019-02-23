using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Big data structure that is going to hold all blocks like grass dirt 
public class Chunk : MonoBehaviour
{

    public BlockType[,,] chunkData;
    public Block[,,] blockData;

    public void BuildMap()
    {
        chunkData = new BlockType[World.Instance.chunkSize, World.Instance.chunkHeight, World.Instance.chunkSize];
        blockData = new Block[World.Instance.chunkSize, World.Instance.chunkHeight, World.Instance.chunkSize];


        //PerlinNoise Algoritm- generates a texture of float values that follow a specific pattern to render cubes
        for (int x = 0; x < World.Instance.chunkSize; x++)
        {
            for (int z = 0; z < World.Instance.chunkSize; z++)
            {
                bool prevair = true;
                for (int y = World.Instance.chunkHeight - 1; y > 0; y--)
                {
                    if (y < World.Instance.n.GenerateHeight(x + transform.position.x, z + transform.position.z))
                    {
                        if (prevair)
                        {
                            prevair = false;
                            chunkData[x, y, z] = BlockType.GRASS;
                        }
                        else
                        {
                            chunkData[x, y, z] = BlockType.DIRT;
                        }

                    }
                    else if (y < World.Instance.WaterHeight)
                    {
                        chunkData[x, y, z] = BlockType.STONE;
                    }
                    else
                    {
                        chunkData[x, y, z] = BlockType.AIR;
                    }
                }
            }
        }

    }


    //Try to access the chunk but if it throws an exception then it is out of the array so return true 
    //Checks if it is outside of the array and the block type is AIR (empty)
    public bool isEmpty(int x, int y, int z)
    {
        try
        {
            return chunkData[x, y, z] == BlockType.AIR;
        }
        catch (System.IndexOutOfRangeException) { }

        return true;
    }

    public void BuildMesh()
    {
        for (int x = 0; x < World.Instance.chunkSize; x++)
        {
            for(int z = 0; z < World.Instance.chunkSize; z++)
            {
                for (int y = 0; y < World.Instance.chunkHeight; y++)
                {
                    blockData[x, y, z] = new Block(chunkData[x, y, z], new Vector3(x, y, z), gameObject, this);
                    //Check every side of the cube
                    blockData[x, y, z].Draw(isEmpty(x, y, z + 1), isEmpty(x, y, z - 1), isEmpty(x - 1, y, z), isEmpty(x + 1, y, z), isEmpty(x, y + 1, z), isEmpty(x, y - 1, z));
                }
            }
        }
    }

    public void CombineQuads()
    {
        //1. Combine all children meshes
        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while(i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
        MeshRenderer renderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshCollider Mc = gameObject.AddComponent<MeshCollider>();
        Mc.sharedMesh = mf.mesh;

        //5. Delete all uncombined
        foreach (Transform quad in transform)
        {
            GameObject.Destroy(quad.gameObject);
        }


    }

    

    public void BuildChunk()
    {
        BuildMap();
        BuildMesh();
        CombineQuads();
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
