using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [SerializeField] private Keybindings keybindings;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public string GetKeyForAction(KeybindActions key)
    {
        foreach(Keybindings.KeybindingCheck keybindCheck in keybindings.keybindingChecks)
        {
            if(keybindCheck.Action == key)
            {
                if(keybindCheck.mouseButton != MouseActions.None)
                {
                    return keybindCheck.mouseButton.ToString();
                }
                else if(keybindCheck.keyCode != KeyCode.None)
                {
                    return keybindCheck.keyCode.ToString();
                }
            }
        }

        return "None";
    }

    public bool GetInputDown(KeybindActions key)
    {
        foreach (Keybindings.KeybindingCheck keybindCheck in keybindings.keybindingChecks)
        {
            if (keybindCheck.Action == key)
            {
                if (Input.GetKeyDown(keybindCheck.keyCode)) 
                {
                    return Input.GetKeyDown(keybindCheck.keyCode);
                }
                if (keybindCheck.mouseButton != MouseActions.None)
                {
                    if (keybindCheck.mouseButton == MouseActions.left)
                    {
                        return Input.GetMouseButtonDown(0);
                    }
                    else if (keybindCheck.mouseButton == MouseActions.right)
                    {
                        return Input.GetMouseButtonDown(1);
                    }
                    else if (keybindCheck.mouseButton == MouseActions.middle)
                    {
                        return Input.GetMouseButtonDown(2);
                    }
                }
            }
        }

        return false;
    }

    public bool GetInput(KeybindActions key)
    {
        foreach (Keybindings.KeybindingCheck keybindCheck in keybindings.keybindingChecks)
        {
            if (keybindCheck.Action == key)
            {
                if (Input.GetKey(keybindCheck.keyCode))
                {
                    return Input.GetKey(keybindCheck.keyCode);
                }
                if (keybindCheck.mouseButton != MouseActions.None)
                {
                    if (keybindCheck.mouseButton == MouseActions.left)
                    {
                        return Input.GetMouseButton(0);
                    }
                    else if (keybindCheck.mouseButton == MouseActions.right)
                    {
                        return Input.GetMouseButton(1);
                    }
                    else if (keybindCheck.mouseButton == MouseActions.middle)
                    {
                        return Input.GetMouseButton(2);
                    }
                }
            }
        }
        return false;
    }

    public bool GetInputUp(KeybindActions key)
    {
        foreach (Keybindings.KeybindingCheck keybindCheck in keybindings.keybindingChecks)
        {
            if (keybindCheck.Action == key)
            {
                if (Input.GetKeyUp(keybindCheck.keyCode))
                {
                    return Input.GetKeyUp(keybindCheck.keyCode);
                }
                if (keybindCheck.mouseButton != MouseActions.None)
                {
                    if (keybindCheck.mouseButton == MouseActions.left)
                    {
                        return Input.GetMouseButtonUp(0);
                    }
                    else if (keybindCheck.mouseButton == MouseActions.right)
                    {
                        return Input.GetMouseButtonUp(1);
                    }
                    else if (keybindCheck.mouseButton == MouseActions.middle)
                    {
                        return Input.GetMouseButtonUp(2);
                    }
                }
            }
        }
        return false;
    }
}
