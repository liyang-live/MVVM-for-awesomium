﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Awesomium.Core;
using System.Reflection;
using System.Windows.Input;

using MVVMAwesomium.Infra;

namespace MVVMAwesomium.AwesomiumBinding
{
    public class JSGenericObject : GlueBase, IJSObservableBridge
    {
        public JSGenericObject(JSValue value, object icValue)
        {
            JSValue = value;
            CValue = icValue;
        }


        protected override void ComputeString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append("{");

            bool f = true;
            foreach (var it in _Attributes.Where(kvp => kvp.Value.Type != JSCSGlueType.Command))
            {
                if (!f)
                    sb.Append(",");

                sb.Append(string.Format(@"""{0}"":", it.Key));

                f = false;
                it.Value.BuilString(sb, alreadyComputed);
            }

            sb.Append("}");
        }

        private Dictionary<string, IJSCSGlue> _Attributes = new Dictionary<string, IJSCSGlue>();

        public IDictionary<string, IJSCSGlue> Attributes { get { return _Attributes; } }

        public JSValue JSValue { get; private set; }

        private JSValue _MappedJSValue;

        public JSValue MappedJSValue { get { return _MappedJSValue; } }

        public void SetMappedJSValue(JSValue ijsobject, IJSCBridgeCache mapper)
        {
            _MappedJSValue = ijsobject;
        }

        private IDictionary<string, JSObject> _Silenters=new Dictionary<string, JSObject>(); 

        public object CValue { get; private set; }

        public JSCSGlueType Type { get { return JSCSGlueType.Object; } }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return _Attributes.Values; 
        }

        public void UpdateCSharpProperty(string PropertyName, IJSCBridgeCache converter, JSValue newValue )
        {
            PropertyInfo propertyInfo = CValue.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance);
            if (!propertyInfo.CanWrite)
                return;

            var type = propertyInfo.PropertyType.GetUnderlyingNullableType() ?? propertyInfo.PropertyType;
            IJSCSGlue glue = converter.GetCachedOrCreateBasic(newValue, type);
            _Attributes[PropertyName] = glue;
            propertyInfo.SetValue(CValue, glue.CValue, null);
        }

        

        public void Reroot(string PropertyName, IJSCSGlue newValue)
        { 
            _Attributes[PropertyName]=newValue;

            JSObject silenter = null;
            if ( _Silenters.TryGetValue(PropertyName,out silenter))
            {
                silenter.InvokeAsync("silent", newValue.GetJSSessionValue());      
            }
            else
            {
                WebCore.QueueWork(() =>
                    {
                        var jso = (JSObject)_MappedJSValue;
                        if (!_Silenters.TryGetValue(PropertyName, out silenter))
                        {
                            silenter = (JSObject)jso[PropertyName];
                            _Silenters.Add(PropertyName, silenter);
                        }
              
                        silenter.Invoke("silent", newValue.GetJSSessionValue());
                    });
            }
        }     
    }
}
