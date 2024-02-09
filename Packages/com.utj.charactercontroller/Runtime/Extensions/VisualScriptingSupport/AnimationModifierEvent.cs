using Unity.VisualScripting;
using Unity.TinyCharacterController.Check;

namespace Unity.TinyCharacterController.VisualScripting
{
    /// <summary>
    /// 
    /// </summary>
    [UnitTitle("AnimatorModifierAdded")]
    [UnitCategory("Events\\TCC")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.VisualScripting.AnimationModifierCheckEventAdded")]   
    public class AnimationModifierCheckEventAdded : EventUnit<AnimatorModifierCheck>
    {
        protected override bool register => true;
        
        [DoNotSerialize] public ValueInput Key { get; private set; }

        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            if( !stack.gameObject.TryGetComponent(out AnimationModifierEventReceiver _))
                stack.gameObject.AddComponent<AnimationModifierEventReceiver>();
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.AnimationModifierCheckOnChanged, reference.self);
        }

        protected override bool ShouldTrigger(Flow flow, AnimatorModifierCheck args)
        {
            var key = flow.GetValue<string>(Key);
            return args.IsAdded(key);
        }

        protected override void Definition()
        {
            base.Definition();
            Key = ValueInput<string>(nameof(Key), "");
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    [UnitTitle("AnimatorModifierRemoved")]
    [UnitCategory("Events/TCC")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.VisualScripting.AnimationModifierCheckEventRemoved")]   

    public class AnimationModifierCheckEventRemoved : EventUnit<AnimatorModifierCheck>
    {
        protected override bool register => true;
        
        [DoNotSerialize] public ValueInput Key { get; private set; }

        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            if( stack.gameObject.TryGetComponent(out AnimationModifierEventReceiver _) == false)
                stack.gameObject.AddComponent<AnimationModifierEventReceiver>();
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.AnimationModifierCheckOnChanged, reference.self);
        }

        protected override bool ShouldTrigger(Flow flow, AnimatorModifierCheck args)
        {
            var key = flow.GetValue<string>(Key);
            return args.IsRemoved(key);
        }

        protected override void Definition()
        {
            base.Definition();
            Key = ValueInput<string>(nameof(Key), "");
        }
    }
}