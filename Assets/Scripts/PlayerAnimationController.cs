using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Animating;
using FishNet.Component.Transforming;
using FishNet.Object;
using FishNet.Example.ColliderRollbacks;

namespace Gunslinger.Controller
{

    public class PlayerAnimationController : NetworkBehaviour
    {
        [SerializeField]
        Animator _animator;
        NetworkAnimator _networkAnimator;

        bool _hasRifle = false;
        bool _isPlaying = false;
        // Start is called before the first frame update
        void Start()
        {
            _networkAnimator = GetComponent<NetworkAnimator>();
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner)
            {
                gameObject.GetComponent<PlayerAnimationController>().enabled = false;
            }
        }
        // Update is called once per frame
        void Update()
        {
            Debug.Log("here");
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    // injure
                    _networkAnimator.SetTrigger("Is Injured");
                }
                else if (Input.GetKeyDown(KeyCode.P))
                {
                    // pistol or rifle
                    _hasRifle = !_hasRifle;
                    _animator.SetBool("Has Rifle", _hasRifle);
                    _animator.SetBool("Is Playing", false);
                    _isPlaying = false;
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    // reset
                    _animator.SetBool("Has Rifle", false);
                    _hasRifle = false;
                    _animator.SetBool("Is Playing", false);
                    _isPlaying = false;
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    // death
                    _networkAnimator.SetTrigger("Is Death");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    // dodge
                    _networkAnimator.SetTrigger("Is Dodging");
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    // current (playing)
                    _isPlaying = !_isPlaying;
                    _animator.SetBool("Is Playing", _isPlaying);
                }
            }

            // Listen for left mouse button press
            if (Input.GetMouseButtonDown(0))
            {
                _networkAnimator.SetTrigger("Is Firing");
            }
        }

    }

}