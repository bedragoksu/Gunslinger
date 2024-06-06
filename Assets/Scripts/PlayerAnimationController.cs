using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Animating;
using FishNet.Component.Transforming;
using FishNet.Object;
using FishNet.Example.ColliderRollbacks;
using DG.Tweening;

namespace Gunslinger.Controller
{

    public class PlayerAnimationController : NetworkBehaviour
    {
        [SerializeField]
        Animator _animator;
        NetworkAnimator _networkAnimator;

        [SerializeField]
        SkinnedMeshRenderer _skinnedMesh;
        [SerializeField]
        Material _whitishMaterial;

        bool _hasRifle = false;
        bool _isPlaying = false;
        // Start is called before the first frame update

        public ParticleSystem flare;
        public ParticleSystem sparks;
        void Start()
        {
            _networkAnimator = GetComponent<NetworkAnimator>();
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner)
            {
                //gameObject.GetComponent<PlayerAnimationController>().enabled = false;
            }
        }
        // Update is called once per frame
        //void Update()
        //{
        //    if (Input.anyKeyDown)
        //    {
        //        if (Input.GetKeyDown(KeyCode.I))
        //        {
        //            // injure
        //            playInjure();
        //        }
        //        else if (Input.GetKeyDown(KeyCode.P))
        //        {
        //            // pistol or rifle
        //            playSwitchGun();
        //        }
        //        else if (Input.GetKeyDown(KeyCode.R))
        //        {
        //            // reset
        //            resetAnimations();
        //        }
        //        else if (Input.GetKeyDown(KeyCode.X))
        //        {
        //            // death
        //            playDeath();
        //        }
        //        else if (Input.GetKeyDown(KeyCode.D))
        //        {
        //            // dodge
        //            playDodge();
        //        }
        //        else if (Input.GetKeyDown(KeyCode.C))
        //        {
        //            // current (playing)
        //            playRifleAim();
        //        }
        //        else if (Input.GetKeyDown(KeyCode.S))
        //        {
        //            // walking back
        //            playWalkingBack();
        //        }
        //        else if (Input.GetKeyDown(KeyCode.W))
        //        {
        //            playWalkingFront();
        //        }
        //    }

        //    // Listen for left mouse button press
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        playFire();
        //    }
        //}
        [ServerRpc(RequireOwnership = false)]
        public void playAnimFireServer(Vector3 targetPosition)
        {
            playAnimFire(targetPosition);
        }
        [ObserversRpc]
        public void playAnimFire(Vector3 targetPosition)
        {
            playFire(targetPosition);
        }
        [ServerRpc(RequireOwnership = false)]
        public void playAnimInjureServer()
        {
            playAnimInjure();
        }
        [ObserversRpc]
        public void playAnimInjure()
        {
            playInjure();
        }
        [ServerRpc(RequireOwnership = false)]
        public void playAnimDeathServer()
        {
            playAnimDeath();
        }
        [ObserversRpc]
        public void playAnimDeath()
        {
            playDeath();
        }
        [ServerRpc(RequireOwnership = false)]
        public void playAnimDodgeServer()
        {
            playAnimDodge();
        }
        [ObserversRpc]
        public void playAnimDodge()
        {
            playDodge();
        }
        public void playInjure()
        {
            _networkAnimator.SetTrigger("Is Injured");
        }
        public void playSwitchGun()
        {
            _hasRifle = !_hasRifle;
            _animator.SetBool("Has Rifle", _hasRifle);
            _animator.SetBool("Is Playing", false);
            _isPlaying = false;
        }
        public void resetAnimations()
        {
            _animator.SetBool("Has Rifle", false);
            _hasRifle = false;
            _animator.SetBool("Is Playing", false);
            _isPlaying = false;
        }
        public void playDeath()
        {
            gameObject.GetComponent<PlayerController>().enabled = false;

            _networkAnimator.SetTrigger("Is Death");
            //_skinnedMesh.material = _whitishMaterial;
        }
        public void playDodge()
        {
            _networkAnimator.SetTrigger("Is Dodging");
        }
        public void playRifleAim()
        {
            _isPlaying = !_isPlaying;
            _animator.SetBool("Is Playing", _isPlaying);
        }
        public void playFire(Vector3 targetPosition)
        {
            var initialRotation = transform.rotation;
            transform.DOLookAt(targetPosition, 1f).OnComplete(() =>
            {
                // Trigger the firing animation
                _networkAnimator.SetTrigger("Is Firing");

                // Start coroutine to wait for the animation to finish and then return to initial position
                StartCoroutine(WaitForAnimationAndReturn(initialRotation));
            });
        }
        public void playWalkingBack()
        {
            _networkAnimator.SetTrigger("Is Walking Back");
        }
        public void playWalkingFront()
        {
            _networkAnimator.SetTrigger("Is Walking Front");
        }

        private IEnumerator WaitForAnimationAndReturn(Quaternion initialRotation)
        {
            // Wait for the animation to start
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName("Firing Pistol"));

            // Wait until the animation has finished
            yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);

            // Animation has finished, return to the initial position and rotation
            transform.DORotateQuaternion(initialRotation, 1f);
        }
    }

}