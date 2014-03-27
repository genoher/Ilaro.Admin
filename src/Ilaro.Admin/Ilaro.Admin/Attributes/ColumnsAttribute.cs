﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ColumnsAttribute : Attribute
    {
        public IList<string> Columns { get; set; }

        public ColumnsAttribute(params string[] columns)
        {
            Columns = columns;
        }
    }
}