using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Manager.Localization;
using Piranha.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Extensions
{
    public static class AppFieldListExtensions
    {
        public static void AutoRegister(this AppFieldList fields, Type fieldType)
        {
            if (!typeof(IField).IsAssignableFrom(fieldType))
            {
                return;
            }

            MethodInfo generic = null;

            if (typeof(SelectFieldBase).IsAssignableFrom(fieldType))
            {
                var method = typeof(AppFieldList).GetMethod("RegisterSelect");
                generic = method.MakeGenericMethod(fieldType.GenericTypeArguments.First());
            }
            else
            {
                var method = typeof(AppFieldList).GetMethod("Register");
                generic = method.MakeGenericMethod(fieldType);
            }

            generic.Invoke(App.Fields, null);
        }
    }
}
