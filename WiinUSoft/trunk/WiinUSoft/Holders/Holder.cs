using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiinUSoft.Holders
{
    public abstract class Holder
    {
        public System.Collections.Concurrent.ConcurrentDictionary<string, float> Values { get; protected set; }
        //public Dictionary<string, float> Values { get; protected set; }
        public Dictionary<string, string> Mappings { get; protected set; }
        public Dictionary<string, bool> Flags { get; protected set; }

        public void SetValue(string name, bool value)
        {
            SetValue(name, value ? 1.0f : 0.0f);
        }

        public void SetValue(string name, float value)
        {
            if (Mappings.ContainsKey(name))
            {
                if (Values.ContainsKey(name))
                {
                    Values[name] = Math.Abs(value);
                }
                else if (!Values.ContainsKey(name))
                {
                    //Values.Add(name, Math.Abs(value));
                    Values.TryAdd(name, Math.Abs(value));
                }
            }
        }

        public void SetMapping(string name, string mapping)
        {
            if (Mappings.ContainsKey(name))
            {
                Mappings[name] = mapping;
            }
            else
            {
                Mappings.Add(name, mapping);
            }
        }

        public void ClearMapping(string name)
        {
            if (Mappings.ContainsKey(name))
            {
                Mappings.Remove(name);
            }
        }

        public void ClearAllMappings()
        {
            Mappings.Clear();
        }

        public bool GetFlag(string name)
        {
            if (Flags.ContainsKey(name))
            {
                return Flags[name];
            }
            else
            {
                return false;
            }
        }

        public abstract void Update();
        public abstract void Close();
    }
}
