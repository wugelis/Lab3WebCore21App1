using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;

namespace Std20.EasyArchitect.ApiHostBase
{
    /// <summary>
    /// ApiHostBase for .NET Standard 2.0
    /// </summary>
    [Route("api/[controller]/{dllName}/{nameSpace}/{className}/{methodName}/{*pathInfo}")]
    [Route("api/[controller]/{dllName}/{nameSpace}/{methodName}/{*pathInfo}")]
    [Route("api/[controller]/{dllName}/{methodName}/{*pathInfo}")]
    [Route("api/[controller]/{methodName}/{*pathInfo}")]
    [Route("api/[controller]/{*pathInfo}")]
    [ApiController]
    public class ApiHostBase: ControllerBase
    {
        public object Get(string dllName, string nameSpace, string className, string methodName)
        {
            if(string.IsNullOrEmpty(dllName) ||
                string.IsNullOrEmpty(nameSpace) ||
                string.IsNullOrEmpty(className) ||
                string.IsNullOrEmpty(methodName))
            {
                return GetMessageJSON($"傳入的 Url 格式不正確！");
            }

            object result = null;
            Assembly assem = null;
            var queryString = getQueryParameters();

            try
            {
                assem = Assembly.Load($"{dllName}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

                if (assem != null)
                {
                    Type targetType = assem.GetType($"{nameSpace}.{className}");
                    object targetObj = Activator.CreateInstance(targetType);
                    var methods = targetType.GetMethods(
                        BindingFlags.Instance |
                        BindingFlags.Default |
                        BindingFlags.Public)
                        .Where(c => c.Name.ToLower() == methodName.ToLower())
                        .FirstOrDefault();

                    if (methods != null)
                    {
                        ParameterInfo[] pInfos = methods.GetParameters();
                        Type pType = pInfos[0].ParameterType;
                        object param01 = getQueryParameters(pType);

                        if(param01 != null)
                        {
                            result = methods.Invoke(targetObj, new object[] { param01 });
                        }
                        else
                        {
                            result = methods.Invoke(targetObj, null);
                        }
                    }
                    else
                    {
                        result = GetMessageJSON($" Method 名稱 {methodName} 不存在！");
                    }
                }
            }
            catch(Exception ex)
            {
                result = GetMessageJSON($"DLL {dllName} 不存在！請確認該 DLL 有存在在 /bin 資料夾裡面！");
            }

            return result;
        }

        private IQueryCollection getQueryParameters()
        {
            var test = HttpContext.Request.Query;

            return test;
        }

        private object getQueryParameters(Type targetType)
        {
            //T target = default(T);
            object target = Activator.CreateInstance(targetType);

            PropertyInfo[] ps =
                target.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default);

            var queryParams = getQueryParameters();

            foreach (var queryParam in queryParams)
            {
                foreach (PropertyInfo p in ps)
                {
                    if (p.Name.ToLower() == queryParam.Key.ToLower())
                    {
                        Type myParameType = p.PropertyType;
                        if(myParameType == typeof(System.Int32))
                        {
                            int resultValue = 0;
                            if(int.TryParse(queryParam.Value.ToString(), out resultValue))
                            {
                                p.SetValue(target, resultValue);
                            }
                        }
                        else if(myParameType == typeof(System.String))
                        {
                            p.SetValue(target, queryParam.Value.ToString());
                        }
                        else
                        {
                            object inputValue = Activator.CreateInstance(myParameType);
                            inputValue = Convert.ChangeType(queryParam.Value, myParameType);


                            p.SetValue(target, inputValue);
                        }
                    }
                }
            }

            return target;
        }
        private static object GetMessageJSON(string message)
        {
            return from msg in new string[] { message }
                   select new
                   {
                       Message = msg
                   };
        }
    }
}
