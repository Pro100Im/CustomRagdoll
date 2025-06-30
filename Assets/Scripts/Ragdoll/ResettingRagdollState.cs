using UnityEngine;

namespace Ragdoll
{
    public class ResettingRagdollState : BaseRagdollState
    {
        private float _elapsedResetBonesTime;

        public ResettingRagdollState(BaseRagdoll context) : base(context)
        {
            PopulateAnimationStartBoneTransforms(_context.BackStandUpBoneTransforms, _context.BackGetUpAnim);
            PopulateAnimationStartBoneTransforms(_context.FaceStandUpBoneTransforms, _context.FaceGetUpAnim);
        }

        public override void Enter()
        {
            _elapsedResetBonesTime = 0;

            _context.IsFacingUp = _context.HipsBone.forward.y > 0;
            _context.DisableRagdoll();

            AlignRotationToHips();
            AlignPositionToHips();
            PopulateBoneTransforms(_context.RagdollBoneTransforms);
        }

        public override void Execute()
        {
            ResettingBone();
        }

        private void AlignRotationToHips()
        {
            var hipsBone = _context.HipsBone;
            var characterTransform = _context.transform;
            var originalHipsPosition = hipsBone.position;
            var originalHipsRotation = hipsBone.rotation;
            var desiredDirection = hipsBone.up;

            if(_context.IsFacingUp)
                desiredDirection *= -1;

            desiredDirection.y = 0;
            desiredDirection.Normalize();

            var fromToRotation = Quaternion.FromToRotation(characterTransform.forward, desiredDirection);
            characterTransform.rotation *= fromToRotation;

            hipsBone.position = originalHipsPosition;
            hipsBone.rotation = originalHipsRotation;
        }

        private void AlignPositionToHips()
        {
            var groundY = _context.GroundY;
            var hipsBone = _context.HipsBone;
            var characterTransform = _context.transform;
            var standUpBoneTransforms = GetStandUpBoneTransforms();
            var originalHipsPosition = hipsBone.position;
            var positionOffset = standUpBoneTransforms[0].Position;

            positionOffset.y = 0;
            positionOffset = characterTransform.rotation * positionOffset;

            characterTransform.position = hipsBone.position;
            characterTransform.position -= positionOffset;
            characterTransform.position = new Vector3(characterTransform.position.x, groundY, characterTransform.position.z);

            hipsBone.position = originalHipsPosition;
        }

        private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
        {
            var bones = _context.Bones;

            for(int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
            {
                boneTransforms[boneIndex].Position = bones[boneIndex].localPosition;
                boneTransforms[boneIndex].Rotation = bones[boneIndex].localRotation;
            }
        }

        private void PopulateAnimationStartBoneTransforms(BoneTransform[] standUpBoneTransforms, string clipName)
        {
            var animator = _context.Animator;
            var characterTransform = _context.transform;
            var positionBeforeSampling = characterTransform.position;
            var rotationBeforeSampling = characterTransform.rotation;

            foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if(clip.name == clipName)
                {
                    clip.SampleAnimation(characterTransform.gameObject, 0);
                    PopulateBoneTransforms(standUpBoneTransforms);

                    break;
                }
            }

            characterTransform.position = positionBeforeSampling;
            characterTransform.rotation = rotationBeforeSampling;
        }

        private void ResettingBone()
        {
            var bones = _context.Bones;
            var ragdollBoneTransforms = _context.RagdollBoneTransforms;
            var standUpBoneTransforms = GetStandUpBoneTransforms();
            var timeToResetBones = _context.TimeToResetBones;

            _elapsedResetBonesTime += Time.fixedDeltaTime;
            var elapsedPercentage = _elapsedResetBonesTime / timeToResetBones;

            for(int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
            {
                bones[boneIndex].localPosition = Vector3.Lerp(
                    ragdollBoneTransforms[boneIndex].Position,
                    standUpBoneTransforms[boneIndex].Position,
                    elapsedPercentage);

                bones[boneIndex].localRotation = Quaternion.Lerp(
                    ragdollBoneTransforms[boneIndex].Rotation,
                    standUpBoneTransforms[boneIndex].Rotation,
                    elapsedPercentage);
            }

            if(elapsedPercentage <= 1)
                return;

            _context.TransitionTo(typeof(GettingUpRagdollState));
        }

        private BoneTransform[] GetStandUpBoneTransforms()
            => _context.IsFacingUp ? _context.BackStandUpBoneTransforms : _context.FaceStandUpBoneTransforms;
    }
}