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

            var fieldTypes = BehaviorExtender.Instance.Behaviors.ToArray();
            var behaviorType =
                fieldTypes.FirstOrDefault(
                    kvp => string.Compare(kvp.Key, behaviorDefinition.Name, StringComparison.Ordinal) == 0).Value;

            if (behaviorType == null) return null;

            var behavior = (NZazuFieldBehavior)Activator.CreateInstance(behaviorType);
            return Decorate(behavior, behaviorDefinition);
        }


        private NZazuFieldBehavior Decorate(NZazuFieldBehavior field, BehaviorDefinition behaviorDefinition)
        {
            return field;
        }
    }
}