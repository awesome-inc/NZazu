using System;
using System.Linq;
using NZazu.Contracts;
using NZazu.Fields;

namespace NZazu.FieldBehavior
{
    public class NZazuFieldBehaviorFactory : INZazuWpfFieldBehaviorFactory
    {
        public INZazuWpfFieldBehavior CreateFieldBehavior(BehaviorDefinition behaviorDefinition)
        {
            if (behaviorDefinition == null) throw new ArgumentNullException(nameof(behaviorDefinition));
            if (string.IsNullOrWhiteSpace(behaviorDefinition.Name))
                throw new ArgumentException("BehaviorDefinition.Name should be set");

            var behaviorTypes = BehaviorExtender.Instance.Behaviors.ToArray();
            var behaviorType =
                behaviorTypes.FirstOrDefault(
                    kvp => string.Compare(kvp.Key, behaviorDefinition.Name, StringComparison.Ordinal) == 0).Value;

            if (behaviorType == null) return null;

            var behavior = (INZazuWpfFieldBehavior) Activator.CreateInstance(behaviorType);
            return Decorate(behavior, behaviorDefinition);
        }


        private static INZazuWpfFieldBehavior Decorate(INZazuWpfFieldBehavior behavior,
            BehaviorDefinition behaviorDefinition)
        {
            if (behavior == null) throw new ArgumentNullException(nameof(behavior));
            if (behaviorDefinition == null) throw new ArgumentNullException(nameof(behaviorDefinition));

            var settings = behaviorDefinition.Settings;
            if (settings == null) return behavior;

            var behaviorSettings = settings.Where(setting => behavior.CanSetProperty(setting.Key));
            foreach (var setting in behaviorSettings)
                behavior.SetProperty(setting.Key, setting.Value);

            return behavior;
        }
    }
}