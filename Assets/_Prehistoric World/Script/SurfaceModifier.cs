using UnityEngine;
using System.Collections;
public class SurfaceModifier : MonoBehaviour
{
	[Header("Friction")]
	[Header("* 0 -> 0.99: ICE EFFECT")]
	[Header("* 1.01-> 5: MUD EFFECT")]
	[Range(0, 5)]
	public float Friction;

	[Header("Force")]
	public float addForce = 0;
	public enum ForceDirection { Right, Left }
	public ForceDirection forceDirection;

    private void Start()
    {
        if(addForce != 0)
        {
			if (addForce < 0)
				GetComponent<Animator>().SetBool("isMoveBackward", true);
        }
    }

    public virtual void OnTriggerStay2D(Collider2D collider)
	{
		Player controller = collider.GetComponent<Player>();
		if (controller == null)
			return;

		controller.AddHorizontalForce(Mathf.Abs(addForce) * (forceDirection == ForceDirection.Right ? 1 : -1));
	}
}