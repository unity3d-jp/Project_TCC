//Register a string name for your Custom Scripting Event to hook it to an Event. You can save this class in a separate file and add multiple Events to it as public static strings.

using Unity.TinyCharacterController;
using Unity.TinyCharacterController.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.VisualScripting
{
    [UnitTitle("OnLanding")]
    [UnitCategory("Events\\TCC")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.VisualScripting.OnLandingEvent")]   
    public class OnLandingEvent : EventUnit<float>
    {
        [DoNotSerialize] public ValueOutput FallSpeed { get; private set; }// The Event output data to return when the Event is triggered.
    
        protected override bool register => true;

        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            stack.gameObject.AddComponent<GravityEventReceiver>();
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.GravityOnLanding, reference.self);
        }

        protected override void Definition()
        {
            base.Definition();
            FallSpeed = ValueOutput<float>(nameof(FallSpeed));
        }

        protected override void AssignArguments(Flow flow, float data)
        {
            flow.SetValue(FallSpeed, data);
        }
    }
    
    [UnitTitle("OnLeave")]
    [UnitCategory("Events\\TCC")]
    public class OnLeavingEvent : EventUnit<float>
    {
        protected override bool register => true;

        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            stack.gameObject.AddComponent<GravityEventReceiver>();
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.GravityOnLanding, reference.self);
        }
    }
}