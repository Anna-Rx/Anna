using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Anna.Request;

namespace Anna.Json
{
    // ReSharper disable once InconsistentNaming
    public static class IObservableRequestContextExtensions
    {
        /// <summary>
        /// Subscribe extension that converts the request data to a particular type
        /// </summary>
        /// <typeparam name="T">type to convert to</typeparam>
        /// <param name="queryable">queryable interface</param>
        /// <param name="handler">handler method</param>
        /// <returns>disposable</returns>
        public static IDisposable SubscribeAs<T>(this IObservable<RequestContext> queryable, Action<T> handler)
        {
            return queryable.Subscribe(ctx =>
                                       {
                                           var actionParameter = ctx.Get<T>();

                                           handler(actionParameter);

                                           ctx.Respond();
                                       });
        }

        /// <summary>
        /// Subscribe extension that converts the request data to a particular type
        /// </summary>
        /// <typeparam name="T">type to bind data to</typeparam>
        /// <typeparam name="TReturn">type of data to return back</typeparam>
        /// <param name="queryable">queryable interface</param>
        /// <param name="handler">handler method</param>
        /// <returns>dispoable</returns>
        public static IDisposable SubscribeAs<T,TReturn>(this IObservable<RequestContext> queryable, Func<T,TReturn> handler)
        {
            return queryable.Subscribe(ctx =>
            {
                var actionParameter = ctx.Get<T>();

                var returnValue = handler(actionParameter);

                string stringValue = returnValue as string;

                if (stringValue != null)
                {
                    ctx.Respond(stringValue);
                }
                else
                {
                    ctx.Respond(returnValue);
                }
            });
        }
    }
}
