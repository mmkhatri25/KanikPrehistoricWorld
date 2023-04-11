using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingZone : MonoBehaviour {
    //	int playerLayer;
    ////	public int hidingLayerNo = 17;
    //
    //	void Start(){
    //		playerLayer = 
    //	}
    public bool canJumpInZone = true;
    public bool canRunInZone = true;
    public Animator anim;

	[Header("Check Enemy in Range")]
	public LayerMask layerEnemy;
	public float checkRadius = 3;

    [Header("3D Material")]
    public Material hiding3DMaterial;
    Material ori3DMaterial;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        //meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer)
            ori3DMaterial = meshRenderer.material;
    }

    void OnTriggerStay2D(Collider2D other){
		if (other.gameObject.layer == LayerMask.NameToLayer ("Player") || (other.gameObject.layer == LayerMask.NameToLayer ("IgnoreAll"))) {

			RaycastHit2D hitEnemy = Physics2D.CircleCast (GameManager.Instance.Player.transform.position, checkRadius, Vector2.zero, 0, layerEnemy);
			if (!hitEnemy) {
                if (other.gameObject.GetComponent(typeof(ICanTakeDamage)))
                {
                    other.gameObject.layer = LayerMask.NameToLayer("HidingZone");
                    if(anim)
                    anim.SetBool("active", true);

                    if (meshRenderer && hiding3DMaterial != null)
                    {
                        meshRenderer.material = hiding3DMaterial;
                        GameManager.Instance.Player.transform.position = new Vector3(GameManager.Instance.Player.transform.position.x, GameManager.Instance.Player.transform.position.y, transform.position.z);
                    }

                    
                }
			}

            GameManager.Instance.Player.canJumpWhenHidingZone = canJumpInZone;
            GameManager.Instance.Player.canRunWhenHidingZone = canRunInZone;
        }
	}


	IEnumerator OnTriggerExit2D(Collider2D other){
		yield return new WaitForSeconds (0.1f);
		if(other && other.gameObject.layer == LayerMask.NameToLayer("HidingZone")){
			if (other.gameObject.GetComponent (typeof(ICanTakeDamage))) {
				if (other.GetComponent<Player> ()) {
						GameManager.Instance.Player.gameObject.layer = LayerMask.NameToLayer ("Player");
                        if(anim)
						anim.SetBool ("active", false);
                        if (meshRenderer)
                        {
                            meshRenderer.material = ori3DMaterial;
                            GameManager.Instance.Player.transform.position = new Vector3(GameManager.Instance.Player.transform.position.x, GameManager.Instance.Player.transform.position.y, -1);
                        }
				}
			}
		}
	}

//	void OnTriggerStay2D(Collider2D other){
//		if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Player")){
//
//			if(other.gameObject.GetComponent (typeof(ICanTakeDamage))){
//				other.gameObject.layer = LayerMask.NameToLayer("HidingZone");
//			}
//		}
//	}
//
//	IEnumerator OnTriggerExit2D(Collider2D other){
//		yield return new WaitForSeconds (0.1f);
//		if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("HidingZone")){
//		if (other.gameObject.GetComponent (typeof(ICanTakeDamage))) {
//			if (other.GetComponent<Player> ()) {
//				GameManager.Instance.Player.gameObject.layer = LayerMask.NameToLayer("Player");;
//			} else {
//				other.gameObject.layer = LayerMask.NameToLayer("Enemy");
//			}
//		}
//		}
//	}
}
