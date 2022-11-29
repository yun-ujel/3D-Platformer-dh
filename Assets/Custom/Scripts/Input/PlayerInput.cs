using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool JumpPressed()
    {
        if (InputManager.instance.GetInputDown(KeybindActions.Jump))
        {
            return true;
        }
        return false;
    }

    public bool JumpHeld()
    {
        if (InputManager.instance.GetInput(KeybindActions.Jump))
        {
            return true;
        }
        return false;
    }

    public bool JumpReleased()
    {
        if (InputManager.instance.GetInputUp(KeybindActions.Jump))
        {
            return true;
        }
        return false;
    }

    public bool DivePressed()
    {
        if (InputManager.instance.GetInputDown(KeybindActions.Dive))
        {
            return true;
        }
        return false;
    }
    public bool DiveHeld()
    {
        if (InputManager.instance.GetInput(KeybindActions.Dive))
        {
            return true;
        }
        return false;
    }

    public bool DiveReleased()
    {
        if (InputManager.instance.GetInputUp(KeybindActions.Dive))
        {
            return true;
        }
        return false;
    }

    public bool CrouchPressed()
    {
        if (InputManager.instance.GetInputDown(KeybindActions.Crouch))
        {
            return true;
        }
        return false;
    }

    public bool CrouchHeld()
    {
        if (InputManager.instance.GetInput(KeybindActions.Crouch))
        {
            return true;
        }
        return false;
    }

    public bool CrouchReleased()
    {
        if (InputManager.instance.GetInputUp(KeybindActions.Crouch))
        {
            return true;
        }
        return false;
    }
}
