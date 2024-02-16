using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationTestManager : MonoBehaviour
{
    // Public list to hold characters
    public GameObject characters;

    
    private AnimationClip[] animations;

    void Start()
    {
        Animation anim = characters.GetComponent<Animation>();
        if (anim != null)
        {
            // Get all animations attached to the Animation component
            animations = new AnimationClip[anim.GetClipCount()];
            int index = 0;
            foreach (AnimationState state in anim)
            {
                animations[index] = state.clip;
                index++;
            }
            foreach (AnimationClip clip in animations)
            {
                Debug.Log(clip.name);
            }
        }
        else
        {
            Debug.LogError("No Animation component found on the characters GameObject.");
        }
    }
    void Update()
    {
        
        // Check if any of the number keys (1 to 7) is pressed
        if (Input.anyKeyDown && Input.inputString.Length == 1 && Input.inputString[0] >= '0' && Input.inputString[0] <= '7')
        {
            // Convert the character input to an integer
            int keyPressed = Input.inputString[0] - '0';
            // Use switch-case to handle different key presses
            if (keyPressed == 2)
            {
                characters.GetComponent<Animation>()[animations[keyPressed].name].speed = -1f;
                characters.GetComponent<Animation>().Play(animations[keyPressed].name);
            }
            characters.GetComponent<Animation>().Play(animations[keyPressed].name);
            Debug.Log("Playing animation: " + animations[keyPressed].name);

        }
    }

}
