using UnityEngine;
using System.Collections;

public class AutoSpawn : MonoBehaviour {
    public bool keepCurrenSpawnItemInResetMode = true;
    public bool spawnOrder = true;
    public GameObject[] SpawnObjects;
	public int maxItemsSpawned = 7;
	public int counter;
	[Tooltip("start spawn item when enable or wait for command message")]
	public bool autoSpawn = false;

	public float TimeMin;
	public float TimeMax;

	public AudioClip spawnSound;
	[Range(0,1)]
	public float spawnSoundVolume = 0.5f;

    string objectID;
    // Use this for initialization
    void Start()
    {
        if (autoSpawn)
            Play();

        objectID = transform.root.gameObject.name;
        counter = 0;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        GlobalValue.AutoSpawnStore(objectID + "", (int)counter);
    }

    public void Play(){
		StartCoroutine (SpawnEnemy (Random.Range (TimeMin, TimeMax)));
	}

	IEnumerator SpawnEnemy(float delay){
        if (counter >= maxItemsSpawned)
            yield break;

        int spawnItem = spawnOrder ? (counter % SpawnObjects.Length) : (Random.Range(0, SpawnObjects.Length));

        Instantiate (SpawnObjects [spawnItem], transform.position, Quaternion.identity);
		counter++;
        SoundManager.PlaySfx(spawnSound, spawnSoundVolume);
        yield return new WaitForSeconds(delay);

        if (maxItemsSpawned > 0 && counter < maxItemsSpawned && GameManager.Instance.State == GameManager.GameState.Playing)
			StartCoroutine (SpawnEnemy (Random.Range (TimeMin, TimeMax)));
	}
}
