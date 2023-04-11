using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour, ICanTakeDamage {
	public enum BlockTyle{Destroyable, Rocky, Hidden}
	public BlockTyle blockTyle;
	public LayerMask enemiesLayer;

	public int maxHit = 1;
	public float pushEnemyUp = 7f;
	public float sizeDetectEnemies = 0.25f;
	public int pointToAdd = 100;

    public float offsetCheckEnemyY = 0.1f;

	[Header("Destroyable")]
	public GameObject DestroyEffect;

	public Sprite imageBlockStatic;

	[Header("Sound")]
	public AudioClip soundDestroy;
	[Range(0,1)]
	public float soundDestroyVolume = 0.5f;

	Animator anim;
	SpriteRenderer spriteRenderer;
	Sprite oldSprite;
	int currentHitLeft;
    bool allowSpawnRewardedItem = true;

    [ReadOnly] public bool isShowed = false;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		oldSprite = spriteRenderer.sprite;
		currentHitLeft = Mathf.Clamp (maxHit, 1, int.MaxValue);

        spriteRenderer.enabled = blockTyle != BlockTyle.Hidden;

    }
	
   public void BoxHit(GameObject instigator = null)
    {
        if (isWaitNextHit)
            return;

        if (currentHitLeft <= 0)
            return;

        StartCoroutine(BoxHitCo(instigator));
    }

    bool isWaitNextHit = false;

    IEnumerator BoxHitCo(GameObject instigator)
    {
        isWaitNextHit = true;

        CheckEnemiesOnTop();

        anim.SetTrigger("hit");

        if (allowSpawnRewardedItem)
        {
            var spawnItem = GetComponent<EnemySpawnItem>();
            if (spawnItem != null)
            {
                spawnItem.SpawnItem();
            }
        }

       currentHitLeft--;

        if (currentHitLeft > 0)
        {
            yield return null;
            isWaitNextHit = false;
            yield break;
        }

        if (blockTyle == BlockTyle.Destroyable)
        {
                if (DestroyEffect != null)
                    SpawnSystemHelper.GetNextObject(DestroyEffect, true, transform.position);

                isWaitNextHit = false;
                SoundManager.PlaySfx(soundDestroy);
                Destroy(gameObject);
        }
        else if (blockTyle == BlockTyle.Rocky || blockTyle == BlockTyle.Hidden)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = imageBlockStatic;
            isShowed = true;
        }

        yield return null;
        isWaitNextHit = false;
    }

	void CheckEnemiesOnTop(){
		//check if any enemies on top? kill them
		var hits = Physics2D.CircleCastAll (transform.position + Vector3.up * offsetCheckEnemyY, sizeDetectEnemies, Vector2.zero, 0, enemiesLayer);
		if (hits.Length > 0) {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<Block>() == null)
                {

                    var damage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                    if (damage != null)
                        damage.TakeDamage(10000, Vector2.up * pushEnemyUp, gameObject, Vector2.zero); //kill it right away
                }
            }
        }
        else
        {
            var hitItems = Physics2D.CircleCastAll(transform.position + Vector3.up * offsetCheckEnemyY, sizeDetectEnemies, Vector2.zero, 0, 1 << LayerMask.NameToLayer("SpawnItems"));
            if (hitItems.Length > 0)
            {
                foreach (var hit in hitItems)
                {
                    if (hit.collider.gameObject.GetComponent<Block>() == null)
                    {

                        var damage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                        if (damage != null)
                            damage.TakeDamage(10000, Vector2.up * pushEnemyUp, gameObject, Vector2.zero); //kill it right away
                    }
                }
            }
        }
	}

    public Texture myTexture;

    void OnDrawGizmos(){
        if (Application.isPlaying)
            return;

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position + Vector3.up * offsetCheckEnemyY, sizeDetectEnemies);

       
        GetComponent<SpriteRenderer>().enabled = blockTyle != BlockTyle.Hidden;
        if (blockTyle == BlockTyle.Hidden)
        {
            Gizmos.color = new Color(1,1,1,0.3f);
            Gizmos.DrawCube(transform.position, GetComponent<BoxCollider2D>().size);
        }

        if(myTexture)
        Gizmos.DrawGUITexture(new Rect(100, 100, 50, 50), myTexture);
    }

    public void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        BoxHit(instigator);
    }
}
