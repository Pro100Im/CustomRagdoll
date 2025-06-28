
namespace Ragdoll
{
    public class GettingUpRagdollState : BaseRagdollState
    {
        public GettingUpRagdollState(BaseRagdoll context) : base(context)
        {
        }

        public override void Enter()
        {
            GettingUp();

            _context.DisableRagdoll();
        }

        public override void Execute()
        {
            if(_context.Animator.GetCurrentAnimatorStateInfo(0).IsName(_context.GettingUpAnim) == false)
            {
                _context.TransitionTo(typeof(InactiveRagdollState));
            }
        }

        private void GettingUp()
        {
            var animator = _context.Animator;

            animator.Rebind();
            animator.Play(_context.GettingUpAnim);
            animator.Update(0f);
            animator.enabled = true;
        }
    }
}
