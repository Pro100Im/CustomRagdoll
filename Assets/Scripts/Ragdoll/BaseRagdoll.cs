using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ragdoll
{
    [RequireComponent(typeof(Animator))]
    public class BaseRagdoll : MonoBehaviour
    {
        [field: SerializeField] public float TimeToResetBones { get; private set; } = 0.5f;
        [field: SerializeField] public Animator Animator { get; private set; }
        [Space]
        [SerializeField] private float _magnitudeThreshold = 5f;
        [Space]
        [SerializeField] private Character _character;
        [Space]
        [SerializeField] private string _backGetUpAnim = "Stand Up";
        [SerializeField] private string _faceGetUpAnim = "Standing Up";

        public string BackGetUpAnim => _backGetUpAnim;
        public string FaceGetUpAnim => _faceGetUpAnim;

        public bool IsFacingUp { get; set; }

        public Transform HipsBone { get; private set; }

        public Rigidbody[] RagdollRigidbodies { get; private set; }

        public BoneTransform[] BackStandUpBoneTransforms { get; private set; }
        public BoneTransform[] FaceStandUpBoneTransforms { get; private set; }
        public BoneTransform[] RagdollBoneTransforms { get; private set; }

        public Transform[] Bones { get; private set; }

        public float GroundY
        {
            get
            {
                if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
                {
                    return hitInfo.point.y;
                }

                return transform.position.y;
            }
        }

        private Dictionary<Type, BaseRagdollState> _states;

        private BaseRagdollState _state;

        private void Awake()
        {
            RagdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            HipsBone = Animator.GetBoneTransform(HumanBodyBones.Hips);
            Bones = HipsBone.GetComponentsInChildren<Transform>();

            BackStandUpBoneTransforms = new BoneTransform[Bones.Length];
            FaceStandUpBoneTransforms = new BoneTransform[Bones.Length];
            RagdollBoneTransforms = new BoneTransform[Bones.Length];

            for(int boneIndex = 0; boneIndex < Bones.Length; boneIndex++)
            {
                BackStandUpBoneTransforms[boneIndex] = new BoneTransform();
                FaceStandUpBoneTransforms[boneIndex] = new BoneTransform();
                RagdollBoneTransforms[boneIndex] = new BoneTransform();
            }

            _states = new Dictionary<Type, BaseRagdollState>
            {
                { typeof(InactiveRagdollState), new InactiveRagdollState(this)},
                { typeof(FallingRagdollState), new FallingRagdollState(this)},
                { typeof(ResettingRagdollState), new ResettingRagdollState(this)},
                { typeof(GettingUpRagdollState), new GettingUpRagdollState(this)}
            };

            TransitionTo(typeof(InactiveRagdollState));
        }

        public void TransitionTo(Type stateType)
        {
            if(_states.TryGetValue(stateType, out var state))
            {
                _state = state;
                _state.Enter();
            }
        }

        public void DisableRagdoll()
        {
            foreach(var rigidbody in RagdollRigidbodies.Skip(1))
            {
                rigidbody.isKinematic = true;
            }
        }

        public void EnableRagdoll()
        {
            foreach(var rigidbody in RagdollRigidbodies)
            {
                rigidbody.isKinematic = false;
            }
        }

        public void LockCharacter(bool value) => _character.SetCharacterEnable(!value);

        private void FixedUpdate()
        {
            _state.Execute();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var impulse = collision.impulse;

            if(impulse.magnitude > _magnitudeThreshold)
            {
                TransitionTo(typeof(FallingRagdollState));

                var contactPoint = collision.contacts[0].point;
                var nearestBone = FindNearestBone(contactPoint);

                if(nearestBone == null)
                    return;

                var otherRb = collision.rigidbody;

                if(otherRb == null)
                    return;

                var massDifference = otherRb.mass - nearestBone.mass;

                if(massDifference > 0f)
                {
                    var direction = (nearestBone.transform.position - collision.contacts[0].point).normalized;
                    var forceMagnitude = massDifference;

                    nearestBone.AddForceAtPosition(impulse + direction * forceMagnitude * 5, contactPoint, ForceMode.Impulse);
                }
            }
        }

        private Rigidbody FindNearestBone(Vector3 point)
        {
            Rigidbody closest = null;
            var minDist = float.MaxValue;

            foreach(var rb in RagdollRigidbodies.Skip(1))
            {
                var dist = Vector3.Distance(rb.worldCenterOfMass, point);

                if(dist < minDist)
                {
                    minDist = dist;
                    closest = rb;
                }
            }

            return closest;
        }
    }
}