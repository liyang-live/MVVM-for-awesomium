﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Awesomium.Core;
using MVVMAwesomium.Infra;
using MVVMAwesomium.Exceptions;

namespace MVVMAwesomium.AwesomiumBinding
{
    public class LocalBuilder : IJSOLocalBuilder
    {
       private static int _MapCount = 0;
       private IWebView _IWebView;
     
       public LocalBuilder(IWebView iIWebView)
       {
           _IWebView = iIWebView;
       }

        private JSObject UnsafeCreateJSO()
        {
            JSObject res =new JSObject();
            res["_MappedId"] = new JSValue(_MapCount++);
            return res;
        }

     

        public JSObject CreateJSO()
        {
            return _IWebView.EvaluateSafe(() => UnsafeCreateJSO());
        }

        public uint GetID(JSObject iJSObject)
        {
            return  _IWebView.EvaluateSafe(() =>(uint)iJSObject["_MappedId"]);
        }


        public bool HasRelevantId(JSObject iJSObject)
        {
            return ((iJSObject!=null) &&( iJSObject.HasProperty("_MappedId")));
        }

        public JSValue CreateDate(DateTime dt)
        {
            return _IWebView.EvaluateSafe(() =>
                        _IWebView.ExecuteJavascriptWithResult(string.Format("new Date({0})",
                        string.Join(",", dt.Year, dt.Month - 1, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond))));
        }

        private JSValue UpdateObject(JSObject ires)
        {
            ires["_MappedId"] = new JSValue(_MapCount++);
            return ires;
        }


        private JSValue Check(JSObject ires)
        {
            if (ires == null)
                throw ExceptionHelper.NoKoExtension();

            return ires;
        }

        private JSValue CheckUpdate(JSObject ires)
        {
            return UpdateObject(Check(ires));
        }



        public JSValue CreateEnum(Enum ienum)
        {
            return _IWebView.EvaluateSafe(() =>
                CheckUpdate(_IWebView.ExecuteJavascriptWithResult(string.Format("new Enum('{0}',{1},'{2}','{3}')",
                                ienum.GetType().Name, Convert.ToInt32(ienum), ienum.ToString(), ienum.GetDescription()))));
        }

        private JSValue? _JSNull = null;

        public JSValue CreateNull()
        {
            if (!_JSNull.HasValue)
                _JSNull = Check(_IWebView.EvaluateSafe(() => _IWebView.ExecuteJavascriptWithResult("new Null_reference()")));

            return _JSNull.Value;
        }
    }
}
