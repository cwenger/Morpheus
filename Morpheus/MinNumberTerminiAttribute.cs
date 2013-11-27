using System;

namespace Morpheus
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MinNumberTerminiAttribute : Attribute
    {
        public int MinNumberTermini { get; private set; }

        public MinNumberTerminiAttribute(int minNumberTermini)
        {
            MinNumberTermini = minNumberTermini;
        }
    }
}
