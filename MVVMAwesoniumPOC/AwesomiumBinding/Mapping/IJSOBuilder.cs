﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Awesomium.Core;

namespace MVVMAwesomium.AwesomiumBinding
{
    public interface IJSOBuilder
    {
        JSObject CreateJSO();

        JSValue CreateDate(DateTime dt);
    }  
}
