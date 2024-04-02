namespace Common.Collections
{
    using System;

    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Linq.Expressions;

    public abstract class SequenceAssertion
    {
        public static class Messages
        {
            public const string BeginningFormat = "Sequence of type '{0}' contains ";

            public static class Elements
            {
                public const string MoreThanOne = "more than one matching element";

                public const string NoOne = "no matching element";

                public const string Some = "some matching element but should not";
            }
        }
    }

    public abstract class SequenceAssertion<T, TSequenceAssertion> : SequenceAssertion, IDisposable
        where TSequenceAssertion : SequenceAssertion<T, TSequenceAssertion>
    {
        public delegate Exception ExceptionFunc(string exceptionMessage);

        protected const int MoreThanOne = 2;

        protected const string SearchCriterionDelimiter = "; ";

        protected static readonly ExceptionFunc DefaultExceptionFunc =
            exceptionMessage => new InvalidOperationException(exceptionMessage);

        protected Func<string>? DumpItemFunc;

        private ExceptionFunc? ifAnyExceptionFunc;

        private ExceptionFunc? ifEmptyExceptionFunc;

        private ExceptionFunc? ifMoreThanOneExceptionFunc;

        private Action<ExceptionFunc>? setExceptionFunc;

        private Func<string>? searchCriteriaFunc;

        private string? searchCriteria;

        private string? typeNameOverride;

        private bool isDisposed;

        public virtual void Dispose()
        {
            if (this.isDisposed) return;

            this.isDisposed = true;
#if DEBUG
            // System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} is being disposed");
#endif
            this.ifAnyExceptionFunc = null;
            this.ifEmptyExceptionFunc = null;
            this.ifMoreThanOneExceptionFunc = null;
            this.setExceptionFunc = null;
            this.searchCriteriaFunc = null;
            this.DumpItemFunc = null;

            this.searchCriteria = null;
            this.typeNameOverride = null;
        }

        public TSequenceAssertion IfAny()
        {
            this.setExceptionFunc = func => this.ifAnyExceptionFunc = func;
            return (TSequenceAssertion)this;
        }

        public TSequenceAssertion IfEmpty()
        {
            this.setExceptionFunc = func => this.ifEmptyExceptionFunc = func;
            return (TSequenceAssertion)this;
        }

        public TSequenceAssertion IfMoreThanOne()
        {
            this.setExceptionFunc = func => this.ifMoreThanOneExceptionFunc = func;
            return (TSequenceAssertion)this;
        }

        public TSequenceAssertion Throw(Func<Exception> exceptionFunc)
        {
            this.setExceptionFunc?.Invoke(_ => exceptionFunc());
            this.setExceptionFunc = null;
            return (TSequenceAssertion)this;
        }

        public TSequenceAssertion Throw(ExceptionFunc exceptionFunc)
        {
            this.setExceptionFunc?.Invoke(exceptionFunc);
            this.setExceptionFunc = null;
            return (TSequenceAssertion)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TException">must have ctor with one string arg for message</typeparam>
        /// <returns></returns>
        public TSequenceAssertion Throw<TException>(string? exceptionMessage = default)
            where TException : Exception =>
            this.Throw(
                message => (TException)Activator.CreateInstance(typeof(TException), exceptionMessage ?? message));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TException">must have ctor with one string arg for message</typeparam>
        /// <returns></returns>
        public TSequenceAssertion Throw<TException>(Func<string> exceptionMessage)
            where TException : Exception =>
            this.Throw(_ => (TException)Activator.CreateInstance(typeof(TException), exceptionMessage()));

        public TSequenceAssertion ThrowWithMessage(Func<string> getExceptionMessage) =>
            this.Throw(_ => DefaultExceptionFunc(getExceptionMessage()));

        public TSequenceAssertion ThrowWithMessage(string exceptionMessage) =>
            this.Throw(_ => DefaultExceptionFunc(exceptionMessage));

        public TSequenceAssertion Throw() => this.Throw(DefaultExceptionFunc);

        public TSequenceAssertion OverrideSequenceType(string typeName)
        {
            this.typeNameOverride = typeName;
            return (TSequenceAssertion)this;
        }

        public TSequenceAssertion OverrideSequenceType<TType>()
        {
            this.typeNameOverride = typeof(TType).FullName;
            return (TSequenceAssertion)this;
        }

        public TSequenceAssertion WithSearchCriteria(string searchCriteriaValue)
        {
            this.searchCriteria = searchCriteriaValue;
            return (TSequenceAssertion)this;
        }

        public TSequenceAssertion WithSearchCriteria(Func<string> searchCriteriaValueFunc)
        {
            this.searchCriteriaFunc = searchCriteriaValueFunc;
            return (TSequenceAssertion)this;
        }

        // object types should be representative like Enum or ITypeDef, values will be converted to string
        public TSequenceAssertion WithSearchParams(params object[] args)
        {
            this.searchCriteriaFunc = () => args.Select(
                    arg => arg == null
                        ? "null"
                        : $"{arg.GetType().Name}=\"{arg}\"")
                .JoinToString(SearchCriterionDelimiter);
            return (TSequenceAssertion)this;
        }

        public TSequenceAssertion AddSearchParam(string key, object value)
        {
            this.searchCriteria += $"{key}=\"'{value}'\"{SearchCriterionDelimiter}";
            return (TSequenceAssertion)this;
        }

        protected void Verify(Func<int> getItemCount)
        {
            var itemCount = getItemCount.InitLazy();

            ExceptionFunc? exceptionFunc = null;

            string? message = null;
            if (this.ifEmptyExceptionFunc != null && itemCount.Value == 0)
            {
                message = Messages.Elements.NoOne;
                exceptionFunc = this.ifEmptyExceptionFunc;
            }
            else if (this.ifMoreThanOneExceptionFunc != null && itemCount.Value > 1)
            {
                message = Messages.Elements.MoreThanOne;
                exceptionFunc = this.ifMoreThanOneExceptionFunc;
            }
            else if (this.ifAnyExceptionFunc != null && itemCount.Value > 0)
            {
                message = Messages.Elements.Some;
                exceptionFunc = this.ifAnyExceptionFunc;
            }

            if (exceptionFunc == null)
            {
                return;
            }

            message = string.Format(Messages.BeginningFormat, this.typeNameOverride ?? typeof(T).Name) + message;

            this.searchCriteria = this.searchCriteria ?? this.searchCriteriaFunc?.Invoke();
            if (!string.IsNullOrWhiteSpace(this.searchCriteria))
            {
                message += $" with the search criteria '{this.searchCriteria}'";
            }

            if (this.DumpItemFunc != null)
            {
                message += ". Items: " + this.DumpItemFunc();
            }

            try
            {
                throw exceptionFunc(message);
            }
            finally
            {
                this.Dispose();
            }
        }

        protected T Get(ICollection<T> items, Only only)
        {
            switch (items.Count)
            {
                case 1:
                case MoreThanOne when only.HasFlag(Only.First):
                    var first = items.First();
                    this.Dispose();
                    return first;
                case 0 when only.HasFlag(Only.Default):
                    this.Dispose();
                    return default(T);
            }

            if (this.ifEmptyExceptionFunc == null) this.ifEmptyExceptionFunc = DefaultExceptionFunc;
            if (this.ifMoreThanOneExceptionFunc == null) this.ifMoreThanOneExceptionFunc = DefaultExceptionFunc;

            this.Verify(() => items.Count);
            throw new NotSupportedException("Should not reach this code");
        }

        [Flags]
        protected enum Only
        {
            Undefined = 0,

            Default = 1,

            Single = 1 << 1,

            First = 1 << 2
        }
    }

    public class EnumerableAssertion<T> : SequenceAssertion<T, EnumerableAssertion<T>>
    {
        private IEnumerable<T> sequence;

        public override void Dispose()
        {
            this.sequence = null;
            base.Dispose();
        }

        internal EnumerableAssertion(IEnumerable<T> sequence)
        {
            this.sequence = sequence;
        }

        /// <summary>
        /// Warning! The method will walk over every item in the sequence
        /// </summary>
        /// <param name="getExceptionMessage"></param>
        /// <returns></returns>
        public EnumerableAssertion<T> ThrowWithMessage(Func<IEnumerable<T>, string> getExceptionMessage) =>
            this.Throw(_ => DefaultExceptionFunc(getExceptionMessage(this.sequence)));

        /// <summary>
        /// Warning! The method will walk over every item in the sequence
        /// </summary>
        /// <returns></returns>
        public EnumerableAssertion<T> AllowDump() => this.AllowDump(item => item.ToString());

        /// <summary>
        /// Warning! The method will walk over every item in the sequence
        /// </summary>
        /// <returns></returns>
        public EnumerableAssertion<T> AllowDump(Func<string> dumpItems)
        {
            this.DumpItemFunc = dumpItems;
            return this;
        }

        /// <summary>
        /// Warning! The method will walk over every item in the sequence
        /// </summary>
        /// <param name="dumpItem"></param>
        /// <param name="items"></param>
        /// <param name="itemSeparator"></param>
        /// <returns></returns>
        public EnumerableAssertion<T> AllowDump(
            Func<T, string> dumpItem,
            IEnumerable<T> items = null,
            string itemSeparator = null)
        {
            this.DumpItemFunc = () =>
                (items ?? this.sequence).Select(dumpItem).JoinToString(itemSeparator ?? SearchCriterionDelimiter);
            return this;
        }

        public EnumerableAssertion<T> AllowSequenceDump(Func<IEnumerable<T>, string> dumpItems)
        {
            this.DumpItemFunc = () => dumpItems(this.sequence);
            return this;
        }

        public T Single(Func<T, bool> predicate = null) => this.Get(Only.Single, predicate);

        public T SingleOrDefault(Func<T, bool> predicate = null) => this.Get(Only.Single | Only.Default, predicate);

        public T First(Func<T, bool> predicate = null) => this.Get(Only.First, predicate);

        public T FirstOrDefault(Func<T, bool> predicate = null) => this.Get(Only.First | Only.Default, predicate);

        public IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector) => this.AsEnumerable().Select(selector);

        public IEnumerable<TResult> SelectMany<TResult>(Func<T, IEnumerable<TResult>> selector) =>
            this.AsEnumerable().SelectMany(selector);

        public IEnumerable<T> AsEnumerable() => this.sequence;

        public EnumerableAssertion<T> Verify()
        {
            this.Verify(this.sequence.Take(MoreThanOne).Count);
            return this;
        }

        private T Get(Only only, Func<T, bool> predicate = null)
        {
            if (predicate != null)
            {
                this.sequence = this.sequence.Where(predicate);
            }

            return this.Get(this.sequence.Take(MoreThanOne).ToList(), only);
        }
    }

    public class QueryableAssertion<T> : SequenceAssertion<T, QueryableAssertion<T>>
    {
        private IQueryable<T> queryable;

        public QueryableAssertion(IQueryable<T> queryable)
        {
            this.queryable = queryable;
        }

        public override void Dispose()
        {
            this.queryable = null;
            base.Dispose();
        }

        public T Single(Expression<Func<T, bool>> predicate = null) => this.Get(Only.Single, predicate);

        public T SingleOrDefault(Expression<Func<T, bool>> predicate = null) =>
            this.Get(Only.Single | Only.Default, predicate);

        public T First(Expression<Func<T, bool>> predicate = null) => this.Get(Only.First, predicate);

        public T FirstOrDefault(Expression<Func<T, bool>> predicate = null) =>
            this.Get(Only.First | Only.Default, predicate);

        private T Get(Only only, Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                this.queryable = this.queryable.Where(predicate);
            }

            return this.Get(this.queryable.Take(MoreThanOne).ToList(), only);
        }
    }

    public static class SequenceAssertionExtensions
    {
        public static EnumerableAssertion<T> AllowVerboseException<T>(this IEnumerable<T> enumerable) =>
            new EnumerableAssertion<T>(enumerable);

        public static EnumerableAssertion<T> IfEmpty<T>(this IEnumerable<T> enumerable) =>
            enumerable.AllowVerboseException().IfEmpty();

        public static EnumerableAssertion<T> IfMoreThanOne<T>(this IEnumerable<T> enumerable) =>
            enumerable.AllowVerboseException().IfMoreThanOne();

        public static QueryableAssertion<T> AllowVerboseException<T>(this IQueryable<T> queryable) =>
            new QueryableAssertion<T>(queryable);

        public static QueryableAssertion<T> IfEmpty<T>(this IQueryable<T> queryable) =>
            queryable.AllowVerboseException().IfEmpty();

        public static QueryableAssertion<T> IfMoreThanOne<T>(this IQueryable<T> queryable) =>
            queryable.AllowVerboseException().IfMoreThanOne();
    }
}