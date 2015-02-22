using System;
using System.Linq;
using NZazu.Contracts;

namespace NZazu.FieldBehavior
{
    public class NZazuFieldBehaviorFactory : INZazuWpfFieldBehaviorFactory
    {
        public INZazuWpfFieldBehavior CreateFieldBehavior(BehaviorDefinition behaviorDefinition)
        {
            if (behaviorDefinition == null) throw new ArgumentNullException("behaviorDefinition");
            if (string.IsNullOrWhiteSpace(behaviorDefinition.Name)) throw new ArgumentException("BehaviorDefinition.Name should be set");

            var behaviorTypes = BehaviorExtender.Instance.Behaviors.ToArray();
            var behaviorType =
                behaviorTypes.FirstOrDefault(
                    kvp => string.Compare(kvp.Key, behaviorDefinition.Name, StringComparison.Ordinal) == 0).Value;

            if (behaviorType == null) return null;

            var behavior = (INZazuWpfFieldBehavior)Activator.CreateInstance(behaviorType);
            return Decorate(behavior, behaviorDefinition);
        }


        private INZazuWpfFieldBehavior Decorate(INZazuWpfFieldBehavior field, BehaviorDefinition behaviorDefinition)
        {
            return field;
        }
    }
}