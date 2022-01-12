using UnityEngine;

public class ChunkController : MonoBehaviour
{
    public GameObject voxelPrefab;
    public int chunkSize = 16;

    private void Start()
    {
        Debug.Log("Thing");
        BuildChunk();
        Debug.Log("Other Thing");
    }

    void BuildChunk()
    {
        if (!voxelPrefab)
            return;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                Vector3 pos = new Vector3(x, 0, z);
        
                Instantiate(voxelPrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}