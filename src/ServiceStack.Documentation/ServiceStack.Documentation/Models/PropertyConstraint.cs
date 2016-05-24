// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Models
{
    using System;

    public class PropertyConstraint
    {
        public string Name { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string[] Values { get; set; }
        public ConstraintType Type { get; set; }

        public static PropertyConstraint CreateRangeConstraint(string name, int? min, int? max)
        {
            if (!min.HasValue && !max.HasValue)
                throw new InvalidOperationException("You must supply either a Min or Max value");

            if ((min ?? int.MinValue) > (max ?? int.MaxValue))
                throw new ArgumentOutOfRangeException(nameof(max), "Min cannot be creater than Max");

            return new PropertyConstraint { Name = name, Min = min, Max = max, Type = ConstraintType.Range };
        }

        public static PropertyConstraint CreateListConstraint(string name, string[] values)
        {
            if ((values == null) || (values.Length == 0))
                throw new ArgumentException("You must supply a list of values", nameof(values));

            return new PropertyConstraint { Name = name, Values = values, Type = ConstraintType.List };
        }
    }

    public enum ConstraintType
    {
        Range,
        List
    }
}