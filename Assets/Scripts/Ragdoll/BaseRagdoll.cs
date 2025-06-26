using System;
using System.Linq;
using UnityEngine;

namespace Ragdoll
{
    [RequireComponent(typeof(Animator))]
    public class BaseRagdoll : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private Rigidbody[] _ragdollRigidbodies;

        private BaseRagdollState _state;

        private void Awake()
        {
            _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            DisableRagdoll();
            //TransitionTo();
        }

        public void TransitionTo(BaseRagdollState state)
        {
            _state = state;
            _state.SetContext(this);
        }

        private void DisableRagdoll()
        {
            foreach(var rigidbody in _ragdollRigidbodies.Skip(1))
            {
                rigidbody.isKinematic = true;
            }

            _animator.enabled = true;
            //_characterController.enabled = true;
        }

        private void EnableRagdoll()
        {
            foreach(var rigidbody in _ragdollRigidbodies)
            {
                rigidbody.isKinematic = false;
            }

            _animator.enabled = false;
            //_characterController.enabled = false;
        }
    }
}