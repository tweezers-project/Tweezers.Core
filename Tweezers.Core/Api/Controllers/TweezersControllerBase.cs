﻿using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.DataHolders;
using Tweezers.Discoveries.Attributes;
using Tweezers.Schema.Interfaces;

namespace Tweezers.Api.Controllers
{
    public abstract class TweezersControllerBase : ControllerBase
    {
        protected IDatabaseProxy DatabaseProxy { get; set; }

        public PropertyInfo DetermineIdAttr<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            return properties.Single(pi => pi.GetCustomAttributes<TweezersIdAttribute>().Any());
        }

        protected T DeleteTweezersIgnores<T>(T obj)
        {
            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                bool isIgnored = prop.GetCustomAttributes<TweezersIgnoreAttribute>().Any();
                if (isIgnored)
                    prop.SetValue(obj, null);
            }

            return obj;
        }

        protected ActionResult ForbiddenResult(string method, string message)
        {
            return StatusCode(403, new TweezersErrorBody() { Message = message, Method = method});
        }

        protected ActionResult NotFoundResult(string message)
        {
            return StatusCode(404, new TweezersErrorBody() {Message = message});
        }
    }
}
