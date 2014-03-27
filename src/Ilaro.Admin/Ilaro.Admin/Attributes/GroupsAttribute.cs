﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ilaro.Admin.Attributes
{
	/// <summary>
	/// Stars on end of name means groups is collapsed by default
	/// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupsAttribute : Attribute
    {
        public IList<string> Groups { get; set; }

        public GroupsAttribute(params string[] groups)
        {
            Groups = groups;
        }
    }
}