using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _transform;

    public void Run()
    {
        _animator.SetBool("IsRun", true);
    }

    public void Idle()
    {
        _animator.SetBool("IsRun", false);
    }

    public void Jump(bool c)
    {
        _animator.SetBool("IsJump", c);
    }

    public void FlipLeft()
    {
        _transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    public void FlipRight()
    {
        _transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
