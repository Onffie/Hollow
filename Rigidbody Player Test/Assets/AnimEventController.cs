using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventController : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
public void Jump()
    {
        PlayerMovement.Jump();
    }
}
