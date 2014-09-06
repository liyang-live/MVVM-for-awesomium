﻿using Awesomium.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MVVMAwesonium.Infra;

namespace MVVMAwesonium.Test
{
    public class TestWebViewBase
    {
        protected IWebView _WebView = null;
        protected SynchronizationContext _SynchronizationContext;

        public TestWebViewBase()
        {
            _SynchronizationContext = Init().Result;
           DoSafe(()=> Console.WriteLine("Init Awesonium Thread"));
        }
      
        private Task<SynchronizationContext> Init()
        {
            TaskCompletionSource<SynchronizationContext> tcs = new TaskCompletionSource<SynchronizationContext>();
            Task.Factory.StartNew(() =>
            {
                WebCore.Initialize(new WebConfig());
                WebSession session = WebCore.CreateWebSession(WebPreferences.Default);

                _WebView = WebCore.CreateWebView(500, 500);
                _WebView.Source = new Uri(string.Format("{0}\\src\\index.html", Assembly.GetExecutingAssembly().GetPath()));

                WebCore.Started += (o, e) => { tcs.SetResult(SynchronizationContext.Current); };

                while (_WebView.IsLoading)
                {
                    WebCore.Run();
                }
            }
            );

            return tcs.Task;
        }

      

        protected T GetSafe<T>(Func<T> UnsafeGet)
        {
            T res = default(T);
            _SynchronizationContext.Send(_ => res = UnsafeGet(), null);
            return res;
        }

        protected void DoSafe(Action Doact)
        {
            _SynchronizationContext.Send(_ => Doact(), null);
        }
    }
}
