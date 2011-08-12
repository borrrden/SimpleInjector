﻿#region Copyright (c) 2010 S. van Deursen
/* The Simple Injector is an easy-to-use Inversion of Control library for .NET
 * 
 * Copyright (C) 2010 S. van Deursen
 * 
 * To contact me, please visit my blog at http://www.cuttingedge.it/blogs/steven/ or mail to steven at 
 * cuttingedge.it.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
 * associated documentation files (the "Software"), to deal in the Software without restriction, including 
 * without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO 
 * EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace SimpleInjector.Extensions
{
    /// <summary>
    /// Internal helper class for precondition validation.
    /// </summary>
    internal static class Requires
    {
        internal static void IsNotNull(object instance, string paramName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        internal static void DoesNotContainNullValues<T>(IEnumerable<T> collection, string paramName)
            where T : class
        {
            if (collection != null && collection.Contains(null))
            {
                throw new ArgumentException("The collection contains null elements.", paramName);
            }
        }

        internal static void IsValidValue(AccessibilityOption accessibility, string paramName)
        {
            if (accessibility != AccessibilityOption.AllTypes &&
                accessibility != AccessibilityOption.PublicTypesOnly)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The value of argument {0} ({1}) is invalid for Enum-type {2}.",
                    paramName, (int)accessibility, typeof(AccessibilityOption).Name), paramName);
            }
        }

        internal static void TypeIsOpenGeneric(Type type, string paramName)
        {
            if (!type.IsGenericTypeDefinition)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The supplied type '{0}' is not an open generic type.", type),
                    paramName);
            }
        }

        internal static void TypeIsNotOpenGeneric(Type type, string paramName)
        {
            if (type.IsGenericTypeDefinition)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The supplied type '{0}' is an open generic type. Use the RegisterOpenGeneric or " +
                    "RegisterManyForOpenGeneric extension method for registering open generic types.", type),
                    paramName);
            }
        }

        internal static void DoesNotContainOpenGenericTypes(IEnumerable<Type> serviceTypes, string paramName)
        {
            foreach (var type in serviceTypes)
            {
                TypeIsNotOpenGeneric(type, paramName);
            }
        }

        internal static void ServiceIsAssignableFromImplementation(Type service, Type implementation,
            string paramName)
        {
            if (service != implementation && 
                !Helpers.ServiceIsAssignableFromImplementation(service, implementation))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The supplied type '{0}' does not inherit from or implement '{1}'.",
                    implementation, service),
                    paramName);
            }
        }

        internal static void ServiceIsAssignableFromImplementations(Type serviceType, 
            IEnumerable<Type> typesToRegister, string paramName)
        {
            var invalidType = (
                from type in typesToRegister
                where !Helpers.ServiceIsAssignableFromImplementation(serviceType, type)
                select type).FirstOrDefault();

            if (invalidType != null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "The supplied type '{0}' does not implement '{1}'.", invalidType, serviceType),
                    paramName);
            }
        }

        internal static void ServiceTypeDiffersFromImplementationType(Type serviceType, Type implementation, 
            string paramName, string implementationParamName)
        {
            if (serviceType == implementation)
            {
                throw new ArgumentException(paramName + " and " + implementationParamName + 
                    " must be different types.", paramName);
            }
        }
    }
}