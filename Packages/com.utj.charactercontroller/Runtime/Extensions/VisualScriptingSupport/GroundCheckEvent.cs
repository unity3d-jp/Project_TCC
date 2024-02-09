using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.VisualScripting
{
    [UnitTitle("OnChangeGround")]
    [UnitSubtitle("GroundCheck")]
    [UnitCategory("Events\\TCC")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.VisualScripting.GroundCheckEvent")] 
    public class GroundCheckEvent : EventUnit<GroundObject>
    {
        protected override bool register => true;
        
        [DoNotSerialize] public ValueOutput GroundObject { get; private set; }

        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            stack.gameObject.AddComponent<GroundCheckEventReceiver>();
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.GroundCheckOnChangeGround, reference.self);
        }

        protected override void Definition()
        {
            base.Definition();
            GroundObject = ValueOutput<GameObject>(nameof(GroundObject));
        }

        protected override void AssignArguments(Flow flow, GroundObject obj)
        {
            flow.SetValue(GroundObject, obj.Obj);
        }
    }

    public struct GroundObject
    {
        public GameObject Obj;
    }
}