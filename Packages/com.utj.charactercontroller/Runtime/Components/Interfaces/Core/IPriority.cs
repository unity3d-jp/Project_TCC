using System.Collections.Generic;

namespace Unity.TinyCharacterController.Interfaces.Core
{
    /// <summary>
    /// Reorders objects based on priority.
    /// </summary>
    /// <typeparam name="T">A class that inherits from this interface.</typeparam>
    public interface IPriority<T> where T : class, IPriority<T>
    {
        /// <summary>
        /// The priority of the object.
        /// </summary>
        int Priority { get; }
    }

    /// <summary>
    /// Class for priority calculation extension methods.
    /// </summary>
    public static class PriorityExtensions
    {
        /// <summary>
        /// Extracts the class with the highest priority.
        /// Classes with a priority of 0 or lower are treated as non-existent.
        /// </summary>
        /// <param name="values">List</param>
        /// <param name="result">The class with the highest priority.</param>
        /// <typeparam name="T">A class that inherits from IPriority.</typeparam>
        /// <returns>True if the class with the highest priority is found.</returns>
        public static bool GetHighestPriority<T>(this List<T> values, out T result) where T : class, IPriority<T>
        {
            result = null;
            var highestPriority = 0;

            foreach (var value in values)
            {
                if (highestPriority >= value.Priority)
                    continue;

                result = value;
                highestPriority = value.Priority;
            }

            return highestPriority != 0; 
        }
    }
}