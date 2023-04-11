using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadBullet : MonoBehaviour
{
    public int numberBullet = 10;
    public int damagePerBullet = 50;
    public Projectile projectile;
    public float bulletSpeed = 5;

    // Start is called before the first frame update
    void OnEnable()
    {

        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        float angleStep = 360f / numberBullet;
        float angle = 0;
        for (int i = 0; i < numberBullet; i++)
        {
            angle = angleStep * i;
            var _projectile = SpawnSystemHelper.GetNextObject(projectile.gameObject, false);
            //var _projectile = Instantiate(projectile.gameObject);
            _projectile.transform.position = transform.position;
            _projectile.GetComponent<Projectile>().Initialize(gameObject, UltiHelper.AngleToVector2(angle), Vector2.zero, false, false, damagePerBullet, bulletSpeed);
            _projectile.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
