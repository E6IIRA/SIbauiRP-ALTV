using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Extensions
{
    internal static class IEnumerableExtensions
    {
        internal static Task ForEach<T>(this IEnumerable<T> elements, Action<T> action)
        {
            foreach (var element in elements)
            {
                action(element);
            }

            return Task.CompletedTask;
        }
    }
}
