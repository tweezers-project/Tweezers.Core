﻿using System.Linq;
using System.Reflection;
using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Enums;

namespace Tweezers.Discoveries.Containers
{
    public sealed class PropertyMetadata
    {
        public PropertyMetadata(PropertyInfo p)
        {
            PropertyName = p.Name;
            DisplayName = p.GetCustomAttributes(typeof(TweezersFieldAttribute))
                              .Cast<TweezersFieldAttribute>()
                              .SingleOrDefault()?.DisplayName ?? p.Name;

            PropertyType = p.PropertyType.ToPropertyType();
        }

        public string PropertyName { get; set; }

        public string DisplayName { get; set; }
        
        public PropertyType PropertyType { get; set; }
    }
}
