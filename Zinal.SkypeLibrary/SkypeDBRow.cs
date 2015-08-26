using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zinal.SkypeLibrary
{
    internal class SkypeDBRow
    {
        public Dictionary<String, Object> Data = new Dictionary<String, Object>();

        public Object this[String key]
        {
            get
            {
                if (Data.ContainsKey(key))
                    return Data[key];

                return null;
            }
        }

    }
}
