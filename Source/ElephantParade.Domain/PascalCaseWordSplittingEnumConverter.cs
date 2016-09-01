// -----------------------------------------------------------------------
// <copyright file="PascalCaseWordSplittingEnumConverter.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PascalCaseWordSplittingEnumConverter : EnumConverter
    {
        public PascalCaseWordSplittingEnumConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string stringValue = (string)base.ConvertTo(context, culture, value, destinationType);
                stringValue = SplitString(stringValue);
                return stringValue;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public static string SplitString(string stringValue)
        {
            StringBuilder buf = new StringBuilder(stringValue);
            // assume first letter is upper!
            bool lastWasUpper = true;
            int lastSpaceIndex = -1;
            for (int i = 1; i < buf.Length; i++)
            {
                bool isUpper = char.IsUpper(buf[i]);
                if (isUpper & !lastWasUpper)
                {
                    buf.Insert(i, ' ');
                    lastSpaceIndex = i;
                }

                if (!isUpper && lastWasUpper)
                {
                    if (lastSpaceIndex != i - 2)
                    {
                        buf.Insert(i - 1, ' ');
                        lastSpaceIndex = i - 1;
                    }
                }
                lastWasUpper = isUpper;
            }
            return buf.ToString();
        }
    }
}
