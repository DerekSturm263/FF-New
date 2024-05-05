
namespace Quantum
{
    /// <summary>
    /// DeLucas:  A class that allows animator state behaviour to run during update.
    /// </summary>

    public abstract unsafe partial class CustomAnimatorBehaviour
    {
        /// <summary>
        /// Performed when a state is entered.
        /// </summary>
        public abstract void OnEnter(Frame f, EntityRef entity, CustomAnimator* animator);

        /// <summary>
        /// Performed during a state's update.
        /// <returns>If true, the state will stop updating behaviours.  Usually done if a transition occurs mid-state</returns>
        public abstract bool OnUpdate(Frame f, EntityRef entity, CustomAnimator* animator);

        /// <summary>
        /// Performed when a state exits
        /// </summary>
        public abstract void OnExit(Frame f, EntityRef entity, CustomAnimator* animator);
    }
}
