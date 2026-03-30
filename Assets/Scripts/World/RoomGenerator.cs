using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] rooms;
    public int roomCount = 5;
    public float roomSpacing = 20f;

    void Start()
    {
        for(int i = 0; i < roomCount; i++)
        {
            GameObject room = rooms[Random.Range(0, rooms.Length)];

            Vector3 pos = new Vector3(i * roomSpacing, 0, 0);

            Instantiate(room, pos, Quaternion.identity);
        }
    }
}