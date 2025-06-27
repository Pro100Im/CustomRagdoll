using System.Linq;
using UnityEngine;

namespace Ragdoll
{
    [RequireComponent(typeof(Animator))]
    public class BaseRagdoll : MonoBehaviour
    {
        [field: SerializeField] public Animator Animator { get; private set; }
        [Space]
        [SerializeField] private float _magnitudeThreshold = 5f;
        [Space]
        [SerializeField] private Character _character;

        [field: SerializeField] public string GettingUpAnim { get; private set; } = "Getting Up";

        public Transform HipsBone { get; private set; }

        public Rigidbody[] RagdollRigidbodies { get; private set; }

        public BoneTransform[] _standUpBoneTransforms { get; private set; }
        public BoneTransform[] _ragdollBoneTransforms { get; private set; }

        private Transform[] _bones;

        private BaseRagdollState _state;

        private void Awake()
        {
            RagdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            HipsBone = Animator.GetBoneTransform(HumanBodyBones.Hips);

            _bones = HipsBone.GetComponentsInChildren<Transform>();
            _standUpBoneTransforms = new BoneTransform[_bones.Length];
            _ragdollBoneTransforms = new BoneTransform[_bones.Length];

            for(int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
            {
                _standUpBoneTransforms[boneIndex] = new BoneTransform();
                _ragdollBoneTransforms[boneIndex] = new BoneTransform();
            }

            TransitionTo(new InactiveRagdollState());
        }

        public void TransitionTo(BaseRagdollState state)
        {
            _state = state;
            _state.SetContext(this);
            _state.Enter();
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
                TransitionTo(new FallingRagdollState());

                Vector3 contactPoint = collision.contacts[0].point;

                var nearestBone = FindNearestBone(contactPoint);

                if(nearestBone != null)
                {
                    var otherRb = collision.rigidbody;

                    if(otherRb == null)
                        return;

                    var massDifference = otherRb.mass - nearestBone.mass;

                    if(massDifference > 0f)
                    {
                        var direction = (nearestBone.transform.position - collision.contacts[0].point).normalized;
                        var forceMagnitude = massDifference;

                        nearestBone.AddForceAtPosition(impulse + direction * forceMagnitude * 2, contactPoint, ForceMode.Impulse);
                    }
                    else
                    {
                        nearestBone.AddForceAtPosition(impulse, contactPoint, ForceMode.Impulse);
                    }           
                }
            }
        }

        private Rigidbody FindNearestBone(Vector3 point)
        {
            Rigidbody closest = null;
            float minDist = float.MaxValue;

            foreach(var rb in RagdollRigidbodies.Skip(1))
            {
                float dist = Vector3.Distance(rb.worldCenterOfMass, point);
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