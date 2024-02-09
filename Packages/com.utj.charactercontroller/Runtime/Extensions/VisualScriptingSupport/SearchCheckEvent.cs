using Unity.VisualScripting;
using UnityEngine;

namespace Unity.TinyCharacterController.VisualScripting
{
    [UnitTitle("Search.CalculateOrder")]
    [UnitCategory("Events\\TCC")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.VisualScripting.SearchCheckEvent")] 
    public class SearchCheckEvent : EventUnit<SearchCheckEventData>
    {
        [DoNotSerialize] public ValueOutput Index { get; private set; }
        [DoNotSerialize] public ValueOutput InSight { get; private set; }
        [DoNotSerialize] public ValueOutput Visible { get; private set; }
        [DoNotSerialize] public ValueOutput Angle { get; private set; }
        [DoNotSerialize] public ValueOutput Distance { get; private set; }
        [DoNotSerialize] public ValueOutput Point { get; private set; }
        [DoNotSerialize] public ValueOutput Collider { get; private set; }

        protected override bool register => true;

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(EventNames.SearchCheckCalculateOrder, reference.self);
        }
        
        public override void StartListening(GraphStack stack)
        {
            base.StartListening(stack);
            stack.gameObject.AddComponent<SearchCheckReceiver>();
        }

        public override void StopListening(GraphStack stack)
        {
            base.StopListening(stack);
            var component = stack.gameObject.GetComponent<SearchCheckReceiver>();
            if( component != null)
                Object.Destroy(component);
        }

        protected override void Definition()
        {
            base.Definition();
            Index = ValueOutput<int>(nameof(Index));
            Angle = ValueOutput<float>(nameof(Angle));
            Distance = ValueOutput<float>(nameof(Distance));
            Collider = ValueOutput<Collider>(nameof(Collider));
            Point = ValueOutput<Vector3>(nameof(Point));
            InSight = ValueOutput<bool>(nameof(InSight));
            Visible = ValueOutput<bool>(nameof(Visible));
        }

        protected override void AssignArguments(Flow flow, SearchCheckEventData data)
        {
            base.AssignArguments(flow, data);
            flow.SetValue(Index, data.Index);
            flow.SetValue(Angle, data.Angle);
            flow.SetValue(Distance, data.Distance);
            flow.SetValue(Collider, data.Collider);
            flow.SetValue(Point, data.ClosestPoint);
            flow.SetValue(InSight, data.IsInsight);
            flow.SetValue(Visible, data.IsVisible);
        }
    }
}