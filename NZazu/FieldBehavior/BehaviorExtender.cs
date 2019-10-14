using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NZazu.FieldBehavior
{
    public class BehaviorExtender
    {
        private readonly Dictionary<string, Type> _fieldTypes = new Dictionary<string, Type>();

        internal BehaviorExtender()
        {
            // over here you can add new default behaviors
            _fieldTypes.Add("Empty", typeof(EmptyNZazuFieldBehavior));
            _fieldTypes.Add("OpenUrlOnStringEnter", typeof(OpenUrlOnStringEnterBehavior));
            _fieldTypes.Add("SetBorder", typeof(SetBorderBehavior));
        }

        internal IEnumerable<KeyValuePair<string, Type>> Behaviors => _fieldTypes.ToArray();

        internal void RegisterType(string name, Type type)
        {
            if (_fieldTypes.ContainsKey(name))
            {
                Trace.TraceInformation("A registration for '{0}' already exists for {1}. Replacing with {2}", name,
                    _fieldTypes[name].Name, type.Name);
                _fieldTypes[name] = type;
            }
            else
            {
                _fieldTypes.Add(name, type);
            }
        }

        internal void UnregisterType(string name)
        {
            if (_fieldTypes.ContainsKey(name))
                _fieldTypes.Remove(name);
            else
                Trace.TraceInformation("A registration for '{0}' does not exist. Nothing removed", name);
        }

        #region lazy static singleton

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<BehaviorExtender> instance =
            new Lazy<BehaviorExtender>(() => new BehaviorExtender());

        public static BehaviorExtender Instance => instance.Value;

        // wrapper for comfort. instance method set to private
        public static void Register(string name, Type type)
        {
            Instance.RegisterType(name, type);
        }

        // even more comfort and type-safety
        public static void Register<TBehavior>(string name) where TBehavior : INZazuWpfFieldBehavior
        {
            Instance.RegisterType(name, typeof(TBehavior));
        }

        public static bool IsRegistered(string name)
        {
            return Instance._fieldTypes.ContainsKey(name);
        }

        // wrapper for comfort. instance method set to private
        public static void Unregister(string name)
        {
            Instance.UnregisterType(name);
        }

        public static IEnumerable<KeyValuePair<string, Type>> GetBehaviors()
        {
            return Instance.Behaviors;
        }

        #endregion
    }
}