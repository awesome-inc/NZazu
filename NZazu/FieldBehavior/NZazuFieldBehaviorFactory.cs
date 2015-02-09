using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using NZazu.Contracts;
using NZazu.Contracts.Checks;
using NZazu.Fields;

namespace NZazu.FieldBehavior
{
    /// <summary>
    /// class to add registrations to 
    /// </summary>
    public static class RegistrationExensions
    {

    }

    public class NZazuFieldBehaviorFactory : INZazuWpfFieldBehaviorFactory
    {
        private readonly ICheckFactory _checkFactory;
        protected internal readonly Dictionary<string, Type> FieldTypes = new Dictionary<string, Type>();

        public NZazuFieldBehaviorFactory(ICheckFactory checkFactory = null)
        {
            _checkFactory = checkFactory ?? new CheckFactory();

            FieldTypes.Add("label", typeof(NZazuLabelField));
            FieldTypes.Add("string", typeof(NZazuTextField));
            FieldTypes.Add("bool", typeof(NZazuBoolField));
            FieldTypes.Add("int", typeof(NZazuIntegerField));
            FieldTypes.Add("date", typeof(NZazuDateField));
            FieldTypes.Add("double", typeof(NZazuDoubleField));
        }


        public INZazuWpfFieldBehavior CreateFieldBehavior(BehaviorDefinition behaviorDefinition)
        {
            if (behaviorDefinition == null) throw new ArgumentNullException("behaviorDefinition");
            if (string.IsNullOrWhiteSpace(behaviorDefinition.Name)) throw new ArgumentException("BehaviorDefinition.Name should be set");

            NZazuFieldBehavior behavior;
            if (FieldTypes.ContainsKey(behaviorDefinition.Name))
                behavior = (NZazuFieldBehavior)Activator.CreateInstance(FieldTypes[behaviorDefinition.Name]);
            else
                return null;

            return Decorate(behavior, behaviorDefinition);
        }


        private NZazuFieldBehavior Decorate(NZazuFieldBehavior field, BehaviorDefinition behaviorDefinition)
        {
            return field;
        }

        internal void Register(string name, Type type)
        {
            throw new NotImplementedException();
        }
    }

    internal class NZazuFieldBehavior : INZazuWpfFieldBehavior
    {
        public void AttachTo(Control valueControl)
        {
            throw new NotImplementedException();
        }

        public void Detach()
        {
            throw new NotImplementedException();
        }
    }
}