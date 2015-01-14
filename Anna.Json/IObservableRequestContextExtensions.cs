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
        public static IDisposable SubscribeAs<T>(this IObservable<RequestContext> queryable, Action<T> handler)
        {
            return queryable.Subscribe(ctx =>
                                       {
                                           var actionParameter = ctx.Get<T>();

                                           handler(actionParameter);

                                           ctx.Respond();
                                       });
        }

        public static IDisposable SubscribeAs<T,TReturn>(this IObservable<RequestContext> queryable, Func<T,TReturn> handler)
        {
            return queryable.Subscribe(ctx =>
            {
                var actionParameter = ctx.Get<T>();

                var returnValue = handler(actionParameter);

                ctx.Respond(returnValue);
            });
        }
    }
}
