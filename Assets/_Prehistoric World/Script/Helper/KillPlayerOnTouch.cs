using UnityEngine;
using System.Collections;

public class KillPlayerOnTouch : MonoBehaviour {
	public bool killEnemies = false;
	public bool killAnything = false;
	public bool ignoreShield = false;
	public bool thisIsKillZone = false;

    //private void Update()
    //{
    //    if (GameManager.Instance.State != GameManager.GameState.Playing)
    //        if (GameManager.Instance.Player.transform.position.y < transform.position.x)
    //            LevelManager.Instance.KillPlayer();
    //}
    //IEnumerator OnTriggerStay2D(Collider2D other){
    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            yield break;
        var player = other.GetComponent<Player> ();

		yield return new WaitForEndOfFrame ();

		if (GameManager.Instance.State != GameManager.GameState.Playing)
			yield break;



		if (killAnything)
			other.gameObject.SetActive (false);
		
		else if (player != null) {
            if (thisIsKillZone)
            {
                LevelManager.Instance.KillPlayer();
                
                yield break;
            }
			
			if (player.gameObject.layer == LayerMask.NameToLayer ("HidingZone"))
				yield break;

			if (ignoreShield) {
				LevelManager.Instance.KillPlayer ();
               
                yield break;
			}
			
			if (player.isPlaying)
//				LevelManager.Instance.KillPlayer ();
				GameManager.Instance.Player.TakeDamage(int.MaxValue,Vector2.zero,gameObject, other.transform.position);
		} else if (killEnemies && other!=null &&  other.gameObject.GetComponent (typeof(ICanTakeDamage))){
//			other.gameObject.SetActive (false);
			var dam =(ICanTakeDamage) other.gameObject.GetComponent (typeof(ICanTakeDamage));
			dam.TakeDamage (int.MaxValue, Vector2.zero, gameObject, other.transform.position);
		}
		
	}


}
