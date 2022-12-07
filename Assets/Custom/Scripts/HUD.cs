using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI scoreText;
    public PlayerController pc;

    public bool hasCrouched = false;
    public bool hasJumped = false;
    public bool hasDived = false;
    public bool hasSlammed = false;
    public bool hasRolled = false;
    public bool hasSlamJumped = false;

    public float collectibleCounter = 0f;
    public float collectibleMax = 0f;

    public GameObject[] collectibles;
    private void Start()
    {
        collectibles = GameObject.FindGameObjectsWithTag("Collectible");

        foreach (GameObject collectible in collectibles)
        {
            collectibleMax += 1f;
        }
    }

    void Update()
    {
        CheckStats();

        TutorialText();

        scoreText.text = collectibleCounter.ToString("0") + "/" + collectibleMax.ToString("0");

        if (scoreText.color.a > 0f)
        {
            scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, scoreText.color.a - Time.deltaTime);
        }
    }

    void CheckStats()
    {
        if (pc.PI.JumpPressed() && pc.onGround && pc.hardLandingCounter > 0f)
        {
            hasSlamJumped = true;
        }
        if (pc.PI.JumpReleased())
        {
            hasJumped = true;
        }
        if (pc.state == PlayerController.MovementState.crouch)
        {
            hasCrouched = true;
        }
        if (pc.state == PlayerController.MovementState.roll)
        {
            hasRolled = true;
        }
        if (pc.state == PlayerController.MovementState.slam || pc.state == PlayerController.MovementState.slamBuffer)
        {
            hasSlammed = true;
        }
        if (pc.state == PlayerController.MovementState.dive)
        {
            hasDived = true;
        }
    }

    void TutorialText()
    {
        if (!hasJumped)
        {
            if (pc.timeSinceLastAction < -1f)
            {
                tutorialText.text = "Hold SPACE to Jump";
            }
        }
        else if (!hasCrouched)
        {
            if (!pc.PI.CrouchHeld())
            {
                tutorialText.text = "Hold SHIFT to crouch";
            }
        }
        else if (!hasRolled)
        {
            if (pc.timeSinceLastAction < -1f)
            {
                tutorialText.text = "Left Click while crouched to roll";
            }
        }
        else if (!hasDived)
        {
            if (pc.timeSinceLastAction < -1f)
            {
                tutorialText.text = "Left Click while midair to Dive";
            }
        }
        else if (!hasSlammed)
        {
            if (pc.timeSinceLastAction < -1f)
            {
                tutorialText.text = "Crouch while midair to Slam";
            }
        }
        else if (!hasSlamJumped)
        {
            if (pc.timeSinceLastAction < -1f)
            {
                tutorialText.text = "Jump after a Slam to jump higher";
            }
        }
        else
        {
            if (pc.timeSinceLastAction < -1f)
            {
                tutorialText.text = "";
            }
        }
    }

    public void ScoreAdd()
    {
        collectibleCounter += 1f;

        scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 2f);
    }
}
