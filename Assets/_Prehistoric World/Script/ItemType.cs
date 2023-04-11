using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemType : MonoBehaviour, ITriggerPlayer
{
    public enum Type { coin, bullet, magnet, health}
    public Type itemType;
    public int amount = 1;
    public int points = 10;
    [Range(0, 1)]
    public float soundVol = 0.8f;
    public AudioClip sound;

    [Header("OPTION")]
    public bool gravity = false;
    public float timeLiveAfterSpawned = 6;
    public Vector2 forceSpawn = new Vector2(-5, 5);
    public GameObject effect;

    [Header("MAGNET")]
    public float magnetTime = 10;

    Rigidbody2D rig;
    bool isCollected = false;
    bool allowCollect = false;

    public void Init(bool useGravity, Vector2 pushForce)
    {
        gravity = useGravity;
        if (pushForce != Vector2.zero)
            forceSpawn = pushForce;

        if (itemType == Type.coin || itemType == Type.bullet)
        {
            Invoke("Collect", 0.5f);
        }
    }

    IEnumerator Start()
    {
        if (gravity)
        {
            var rig = gameObject.AddComponent<Rigidbody2D>();
            rig.velocity = new Vector2(Random.Range(-forceSpawn.x, forceSpawn.x), forceSpawn.y);
            rig.fixedAngle = true;
            rig.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            GetComponent<Collider2D>().isTrigger = false;
            yield return new WaitForSeconds(0.1f);

            while(rig.velocity.y > 0) { yield return null; }
            allowCollect = true;
            yield return new WaitForSeconds(timeLiveAfterSpawned);
            Destroy(gameObject);
        }
        else
        {
            GetComponent<Collider2D>().isTrigger = true;
            allowCollect = true;
        }
    }

    public void Collect()
    {
        if (!allowCollect || isCollected)
            return;

        isCollected = true;

        switch (itemType)
        {
            case Type.coin:
                GameManager.Instance.AddCoin(amount, transform);
                break;
            case Type.bullet:
                GameManager.Instance.AddNormalBullet(amount, transform);
                break;
            case Type.magnet:
                if (Magnet.Instance)
                    Magnet.Instance.ActiveMagnet(magnetTime);
                break;
            case Type.health:
               GameManager.Instance.Player.GiveHealth(amount, gameObject);
                break;
        }

        SoundManager.PlaySfx(sound, soundVol);

        if (effect != null)
            SpawnSystemHelper.GetNextObject(effect, true, transform.position);
        Destroy(gameObject);
    }

    public void OnTrigger()
    {
        Collect();
    }
}
