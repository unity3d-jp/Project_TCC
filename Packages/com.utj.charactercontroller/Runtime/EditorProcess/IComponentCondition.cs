using System.Collections.Generic;

namespace Unity.TinyCharacterController.Interfaces.Utility
{
    public interface IComponentCondition
    {
        void OnConditionCheck(List<string> messageList);
    }
}