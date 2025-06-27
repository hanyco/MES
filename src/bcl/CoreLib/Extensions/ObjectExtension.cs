using System.Reflection;

using Library.Coding;

namespace Library.Extensions;

[DebuggerStepThrough]
[StackTraceHidden]
public static class ObjectExtension
{
    extension(object)
    {
    }

    extension(PropertyInfo)
    {
        /// <summary>
        /// Gets the specified attribute from the given property.
        /// </summary>
        /// <typeparam name="TAttribute"> The type of the attribute. </typeparam>
        /// <param name="property"> The property. </param>
        /// <returns> The attribute. </returns>
        public static TAttribute? GetAttribute<TAttribute>(in PropertyInfo property)
            where TAttribute : Attribute =>
            property is null
                    ? throw new ArgumentNullException(nameof(property))
                    : property.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault().Cast().As<TAttribute>();
    }

    extension(Type)
    {
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <typeparam name="TAttribute"> The type of the attribute. </typeparam>
        /// <param name="type">         The type. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <param name="inherited">    if set to <c> true </c> [inherited]. </param>
        /// <returns> </returns>
        public static TAttribute? GetAttribute<TAttribute>(in Type type,
            in TAttribute? defaultValue = null,
            bool inherited = true)
            where TAttribute : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(TAttribute), inherited);
            return attributes.Length > 0 ? (TAttribute)attributes[0] : defaultValue;
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <typeparam name="TDelegate"> The type of the delegate. </typeparam>
        /// <param name="objType">      Type of the object. </param>
        /// <param name="name">         The name. </param>
        /// <param name="bindingFlags"> The binding flags. </param>
        /// <returns> </returns>
        public static TDelegate? GetMethod<TDelegate>(Type objType,
            in string name,
            in BindingFlags bindingFlags = BindingFlags.Public)
            where TDelegate : class
        {
            var methodInfo = objType.GetMethod(name, bindingFlags);
            return methodInfo is not null
                ? Delegate.CreateDelegate(typeof(TDelegate), null, methodInfo).Cast().As<TDelegate>()
                : null;
        }
    }
}