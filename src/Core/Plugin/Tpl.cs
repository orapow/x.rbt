using System;
using System.IO;
using System.Collections.Generic;

using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using NVelocity.Exception;
using Commons.Collections;

namespace X.Core.Plugin
{
    public class Tpl
    {
        #region Fields

        private static bool CACHE = false;

        private static string ENCODING_DEFAULT = "utf-8";

        private static long INTERVAL = 1 * 60;

        private static string RESOURCE_PATH;

        #endregion

        #region Constructors

        private Tpl()
        {
            InitVelocityEngine();
        }

        #endregion

        #region Properties

        public static Tpl Instance
        {
            get
            {
                return Nested.Engine;
            }
        }

        #endregion

        #region Public Methods

        public static void Configuration(string path)
        {
            Configuration(path, "utf-8", false, 0);
        }

        public static void Configuration(string path, long interval)
        {
            Configuration(path, "utf-8", false, interval);
        }

        public static void Configuration(string path, string encoding, bool cache, long interval)
        {
            Tpl.RESOURCE_PATH = path;
            Tpl.ENCODING_DEFAULT = encoding;
            Tpl.CACHE = cache;
            Tpl.INTERVAL = interval;
        }

        public string Merge(string templatePath, IDictionary<string, object> dict)
        {
            Template tpl = null;

            try
            {
                tpl = Velocity.GetTemplate(templatePath);
            }
            catch (ResourceNotFoundException) { throw; }
            catch (ParseErrorException) { throw; }
            catch (MethodInvocationException) { throw; }
            catch (Exception) { throw; }

            if (tpl == null)
            {
                return string.Empty;
            }

            VelocityContext context = new VelocityContext();

            if (dict != null)
            {
                foreach (KeyValuePair<string, object> item in dict)
                {
                    context.Put(item.Key, item.Value);
                }
            }

            StringWriter writer = new StringWriter();

            tpl.Merge(context, writer);

            return writer.GetStringBuilder().ToString();
        }

        public string Evaluate(string tplString, IDictionary<string, object> dict)
        {
            VelocityContext context = new VelocityContext();

            if (dict != null)
            {
                foreach (KeyValuePair<string, object> item in dict)
                {
                    context.Put(item.Key, item.Value);
                }
            }

            StringWriter writer = new StringWriter();

            Velocity.Evaluate(context, writer, String.Empty, tplString);

            return writer.GetStringBuilder().ToString();
        }

        #endregion

        #region Private Methods

        private void InitVelocityEngine()
        {
            if (string.IsNullOrEmpty(Tpl.RESOURCE_PATH))
            {
                DefaultInit();
            }

            ExtendedProperties props = new ExtendedProperties();
            //props.AddProperty(RuntimeConstAnis.RESOURCE_LOADER, "file");
            props.AddProperty(RuntimeConstants.ENCODING_DEFAULT, ENCODING_DEFAULT);

            if (!string.IsNullOrEmpty(Tpl.RESOURCE_PATH))
            {
                props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, Tpl.RESOURCE_PATH);
            }

            if (Tpl.CACHE)
            {
                props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_CACHE, Tpl.CACHE);
                props.AddProperty("file.resource.loader.modificationCheckInterval", Tpl.INTERVAL);
            }

            Velocity.Init(props);
        }

        /// <summary>
        /// 临时初始化方法，今后应该在Application_Start里面初始化Configuration方法
        /// </summary>
        private void DefaultInit()
        {
            Configuration(System.Web.HttpContext.Current.Server.MapPath("~/tpl"), "utf-8", false, 0 * 60);
        }

        #endregion

        private class Nested
        {
            static Nested() { }
            internal static readonly Tpl Engine = new Tpl();
        }
    }

    public class TplDict : Dictionary<string, object>
    {
        public void Update(string key, object value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }
        public int GetInt(string key)
        {
            int o = 0;
            if (!ContainsKey(key)) return o;
            int.TryParse(this[key].ToString(), out o);
            return o;
        }

        public decimal GetDecimal(string key)
        {
            decimal o = 0;
            if (!ContainsKey(key)) return o;
            decimal.TryParse(this[key].ToString(), out o);
            return o;
        }

        public float GetFloat(string key)
        {
            float o = 0;
            if (!ContainsKey(key)) return o;
            float.TryParse(this[key].ToString(), out o);
            return o;
        }

        public string GetString(string key)
        {
            return GetString(key, "");
        }
        public string GetString(string key, string def)
        {
            if (!ContainsKey(key)) return def;
            return this[key].ToString();
        }
    }
}