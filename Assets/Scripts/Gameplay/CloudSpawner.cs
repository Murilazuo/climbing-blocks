using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] GameObject cloud;
    [SerializeField] float maxTimeToMove, minTimeToMove;
    [SerializeField] float maxY, minY;
    [SerializeField] float maxTimeSpawn, minTimeSpawn;
    [SerializeField] float bounds;
    [SerializeField] float scaleMult;
    [SerializeField] Sprite[] sprites;

    private void Start()
    {
        SpawnCloud();
    }

    void SpawnCloud()
    {
        print("TEsts");

        GameObject obj = Instantiate(cloud, new(transform.position.x, Random.Range(minY, maxY)), Quaternion.identity);

        float timeToMove = Random.Range(minTimeSpawn, maxTimeToMove);

        obj.transform.localScale = Vector2.one * ((scaleMult * timeToMove));
        obj.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        LeanTween.moveX(obj, bounds, timeToMove).setOnComplete(()=>Destroy(obj));

        Invoke(nameof(SpawnCloud),Random.Range(minTimeSpawn,maxTimeSpawn));
    }
}
