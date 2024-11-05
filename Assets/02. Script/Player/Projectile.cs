using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Projectiles
{
    arrow, //기본 스킨
}

public class Projectile : MonoBehaviour
{
    public static Projectile projectile = null;

    public string shooter_name;
    public float distance;
    public float speed;
    public Vector2 direction;
    public List<string> hitted_unit;
    public Sprite icon;

    private void Awake()
    {
        if (null == projectile)
        {
            projectile = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
