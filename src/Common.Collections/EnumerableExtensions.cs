namespace Common.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Concatenates the members of a constructed System.Collections.Generic.IEnumerable
        /// collection of type System.String, using the specified separator between each
        /// member.
        /// </summary>
        /// <param name="values">A collection that contains the strings to concatenate.</param>
        /// <param name="separator">The string to use as a separator.</param>
        /// <returns>
        /// A string that consists of the members of values delimited by the separator
        /// string. If values has no members, the method returns System.String.Empty.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">values is null.</exception>
        public static string JoinToString<T>(this IEnumerable<T> values, string separator) =>
            string.Join(separator, values);

        public static string JoinToString<T>(this IEnumerable<T> values, char separator) =>
            values.JoinToString(separator.ToString());

        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection) =>
            collection == null || collection.Any() == false;

        public static bool IsNotEmpty<T>(this IEnumerable<T>? collection) => collection?.Any() == true;

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable) => new(enumerable);

        public static ConcurrentHashSet<T> ToConcurrentHashSet<T>(this IEnumerable<T> enumerable) => new(enumerable);

        public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable) => new(enumerable);

        public static NameValueCollection ToNameValueCollection<T>(
            this IEnumerable<T> enumerable,
            Func<T, string> keySelector,
            Func<T, string> valueSelector)
        {
            var collection = new NameValueCollection();
            foreach (var item in enumerable)
            {
                collection.Add(keySelector(item), valueSelector(item));
            }

            return collection;
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var nextBatch = new List<T>(batchSize);

            foreach (var item in collection)
            {
                nextBatch.Add(item);

                if (nextBatch.Count == batchSize)
                {
                    yield return nextBatch;

                    nextBatch = new List<T>(batchSize);
                }
            }

            if (nextBatch.Count > 0)
            {
                yield return nextBatch;
            }
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> first, params T[] second) =>
            // StackOverflow while using .Except(second);
            first.Where(f => !second.Contains(f));

        public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
            {
                throw new NotImplementedException();
            }

            if (action == null)
            {
                throw new NotImplementedException();
            }

            if (enumerable is List<T>)
            {
                ((List<T>)enumerable).ForEach(action);
                return;
            }

            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        #region Empty

        public static IEnumerable<TSource> ThrowIfEmpty<TSource>(this IEnumerable<TSource> enumerable, string message)
        {
            return enumerable.ThrowIfEmpty(() => new Exception(message));
        }

        public static IEnumerable<TSource> ThrowIfEmpty<TSource>(
            this IEnumerable<TSource> enumerable,
            Func<Exception> createExceptionToBeThrown)
        {
            return enumerable.DoIfEmpty(() => { throw createExceptionToBeThrown(); });
        }

        public static IEnumerable<TSource> ThrowIfEmpty<TException, TSource>(
            this IEnumerable<TSource> enumerable,
            string message)
            where TException : Exception, new()
        {
            return enumerable.DoIfEmpty(() => { throw CreateTypedException<TException>(message); });
        }

        public static IEnumerable<TSource> DoIfEmpty<TSource>(
            this IEnumerable<TSource> enumerable,
            Func<TSource> action)
        {
            if (!enumerable.Any())
            {
                action();
            }

            return enumerable;
        }

        #endregion

        #region MoreThanOne

        public static IEnumerable<TSource> ThrowIfMoreThanOne<TSource>(
            this IEnumerable<TSource> enumerable,
            string message,
            bool dumpItems = false)
        {
            var itemsDescription = string.Empty;

            if (dumpItems)
            {
                itemsDescription = GetItemsDescription(enumerable, itemsDescription);
            }

            return enumerable.ThrowIfMoreThanOne(() => new Exception(message + itemsDescription));
        }

        public static IEnumerable<TSource> ThrowIfMoreThanOne<TSource>(
            this IEnumerable<TSource> enumerable,
            string message,
            Func<TSource, string> itemsDescriptionSelector)
        {
            string itemsDescription = string.Empty;

            if (itemsDescriptionSelector != null)
            {
                itemsDescription = ". Items Dump: " + enumerable.Select(itemsDescriptionSelector).JoinToString("; ");
            }

            return enumerable.ThrowIfMoreThanOne(() => new Exception(message + itemsDescription));
        }

        public static IEnumerable<TSource> ThrowIfMoreThanOne<TSource>(
            this IEnumerable<TSource> enumerable,
            Func<Exception> createExceptionToBeThrown)
        {
            return enumerable.DoIfMoreThanOne(() => { throw createExceptionToBeThrown(); });
        }

        public static IEnumerable<TSource> ThrowIfMoreThanOne<TException, TSource>(
            this IEnumerable<TSource> enumerable,
            string message,
            bool dumpItems = false)
            where TException : Exception, new()
        {
            return enumerable.DoIfMoreThanOne(
                () =>
                    {
                        if (dumpItems)
                        {
                            message += GetItemsDescription(enumerable, string.Empty);
                        }

                        throw CreateTypedException<TException>(message);
                    });
        }

        public static IEnumerable<TSource> DoIfMoreThanOne<TSource>(
            this IEnumerable<TSource> enumerable,
            Func<TSource> action)
        {
            if (enumerable.Count() > 1)
            {
                action();
            }

            return enumerable;
        }

        #endregion

        #region Any

        //TODO wrap all these copy-pasted methods with similar logic to more elegant solution
        public static IEnumerable<TSource> ThrowIfAny<TSource>(
            this IEnumerable<TSource> enumerable,
            string message,
            bool dumpItems = false)
        {
            var itemsDescription = string.Empty;

            if (dumpItems)
            {
                itemsDescription = GetItemsDescription(enumerable, itemsDescription);
            }

            return enumerable.ThrowIfAny(() => new Exception(message + itemsDescription));
        }

        public static IEnumerable<TSource> ThrowIfAny<TSource>(
            this IEnumerable<TSource> enumerable,
            string message,
            Func<TSource, string> itemsDescriptionSelector)
        {
            var itemsDescription = string.Empty;

            if (itemsDescriptionSelector != null)
            {
                itemsDescription = ". Items Dump: " + enumerable.Select(itemsDescriptionSelector).JoinToString("; ");
            }

            return enumerable.ThrowIfAny(() => new Exception(message + itemsDescription));
        }

        public static IEnumerable<TSource> ThrowIfAny<TSource>(
            this IEnumerable<TSource> enumerable,
            Func<Exception> createExceptionToBeThrown)
        {
            return enumerable.DoIfAny(() => { throw createExceptionToBeThrown(); });
        }

        public static IEnumerable<TSource> DoIfAny<TSource>(this IEnumerable<TSource> enumerable, Func<TSource> action)
        {
            if (enumerable.Any())
            {
                action();
            }

            return enumerable;
        }

        #endregion

        private static string GetItemsDescription<TSource>(IEnumerable<TSource> enumerable, string itemsDescription)
        {
            itemsDescription = ". Items Dump: " + enumerable.JoinToString("; ");
            return itemsDescription;
        }

        private static TException CreateTypedException<TException>(string message)
            where TException : Exception, new()
        {
            try
            {
                var typedException = Activator.CreateInstance(typeof(TException), message) as TException;
                return typedException;
            }
            catch (MissingMethodException)
            {
                var e = new TException();
                throw new Exception(message, e);
            }
        }
    }
}