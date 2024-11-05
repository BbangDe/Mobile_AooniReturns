using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Fusion;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TempCharacter : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float DefaultSpeed = 4.5f;
    [SerializeField] float RunSpeed = 6f;
    [SerializeField] float RotateSpeed = 0.05f;

    [Header("Joystick Settings")]
    public JoystickPanel joystick;
    public float joystickSensitivity = 1f;

    // networked values
    float animSpeed { get; set; }
    float speed { get; set; }
    bool IsCaught { get; set; }
    public bool Targetable { get; private set; } = true;

    private Rigidbody2D rb2d;

    private TweenerCore<float, float, FloatOptions> speedTween;
    private float speedTarget;

    public float Stamina { get; private set; } = 100;

    public float Speed
    {
        get => speed;
        set
        {
            if (value == 0f)
            {
                speedTween?.Kill();
                speed = 0f;
                speedTarget = 0f;
                return;
            }

            if (speedTarget == value)
            {
                return;
            }

            speedTween?.Kill();
            speedTarget = value;
            speedTween = DOTween.To(() => speed, v =>
            {
                speed = v;
            },
            value, 0.5f).SetEase(Ease.OutCubic);
        }
    }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        CalculatePosition();
    }

    #region Calculate
    private void CalculatePosition()
    {
        var xDir = joystick.Horizontal * joystickSensitivity;
        var yDir = joystick.Vertical * joystickSensitivity;

        var dir = new Vector2(xDir, yDir);

        Debug.Log(dir);
        if (dir == Vector2.zero)
        {
            Speed = 0f;
            animSpeed = 0f;
            return;
        }

        Speed = DefaultSpeed;
        animSpeed = 1;

        rb2d.velocity = Speed * dir.normalized;

        if (dir != Vector2.zero)
        {
            transform.right = dir;
        }
    }
    #endregion

    private void SaveCurrentInfo()
    {
        // 네트워크 동기화 로직
    }
}