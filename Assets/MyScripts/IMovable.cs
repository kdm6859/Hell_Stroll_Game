using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    public float MoveSpeed { get; set; }
    public float RunSpeed { get; set; }
    public float RotSpeed { get; set; }
    public float JumpForce { get; set; }

    public void SetVelocity(Vector3 velocity);

}