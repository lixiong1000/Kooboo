//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.Scripting.Global.SiteItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kooboo.Sites.Scripting
{
    public class k
    {
        private object _locker = new object();

        public RenderContext RenderContext { get; set; }

        public k(RenderContext context)
        {
            this.RenderContext = context;
        }

        private kDataContext _data;
        public kDataContext DataContext
        {
            get
            {
                if (_data == null)
                {
                    lock (_locker)
                    {
                        if (_data == null)
                        {
                            _data = new kDataContext(this.RenderContext);
                        }
                    }
                }
                return _data;
            }
        }

        private Response _response;
        public Response Response
        {
            get
            {
                if (_response == null)
                {
                    lock (_locker)
                    {
                        if (_response == null)
                        {
                            _response = new Response(this.RenderContext);
                        }
                    }
                }
                return _response;
            }
        }

        private Request _request;

        public Request Request
        {
            get
            {
                if (_request == null)
                {
                    lock (_locker)
                    {
                        if (_request == null)
                        {
                            _request = new Request(this.RenderContext);
                        }
                    }
                }
                return _request;
            }
        }

        private Session _session;
        public Session Session
        {
            get
            {
                if (_session == null)
                {
                    lock (_locker)
                    {
                        if (_session == null)
                        {
                            _session = Manager.GetOrSetSession(this.RenderContext);
                        }
                    }
                }
                return _session;
            }
        }

        private InfoModel _siteinfo;

        public InfoModel Info
        {
            get
            {
                if (_siteinfo == null)
                {
                    lock (_locker)
                    {
                        if (_siteinfo == null)
                        {
                            _siteinfo = new InfoModel();
                            if (this.RenderContext.WebSite != null)
                            {
                                _siteinfo.Culture = this.RenderContext.Culture;
                                _siteinfo.Name = this.RenderContext.WebSite.Name;
                                _siteinfo.Setting = this.RenderContext.WebSite.CustomSettings;
                                _siteinfo.User = new UserModel(this.RenderContext.User); 
                            }
                        }
                    }

                }
                return _siteinfo;
            }
        }

        public class InfoModel
        {
            public string Culture { get; set; }

            public string Name { get; set; }

            private Dictionary<string, string> _setting;
            public Dictionary<string, string> Setting
            {
                get
                {
                    if (_setting == null)
                    {
                        _setting = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }
                    return _setting;
                }
                set { _setting = value; }
            }

         
            public UserModel User
            {     get;set;    
            }    
        }

  

        private kSiteDb _sitedb;
        public kSiteDb SiteDb
        {
            get
            {
                if (_sitedb == null)
                {
                    lock (_locker)
                    {
                        if (_sitedb == null)
                        {
                            if (this.RenderContext.WebSite !=null)
                            {
                                _sitedb = new kSiteDb(this.RenderContext);
                            }       
                        }
                    }
                }
                return _sitedb;
            }
        }

        public kSiteDb Site
        {
            get
            {
                return this.SiteDb; 
            }
        }

        private Curl _url;
        public Curl Url
        {
            get
            {
                if (_url == null)
                {
                    lock (_locker)
                    {
                        if (_url == null)
                        {
                            _url = new Curl();
                        }
                    }
                }
                return _url;
            }
        }

        private FileIO _file;

        public FileIO File
        {
            get
            {
                if (_file == null)
                {
                    lock (_locker)
                    {
                        if (_file == null)
                        {
                            _file = new FileIO(this.RenderContext);
                        }
                    }
                }
                return _file;
            }
        }

        private Cookie _cookie;

        public Cookie Cookie
        {
            get
            {
                if (_cookie == null)
                {
                    lock (_locker)
                    {
                        if (_cookie == null)
                        {
                            _cookie = new Cookie(this.RenderContext);
                        }
                    }
                }
                return _cookie;
            }
        }


        public Dictionary<string, string> config { get; set; }

        [Attributes.SummaryIgnore]
        public Kooboo.Sites.FrontEvent.IFrontEvent @event { get; set; }


        public Kooboo.Sites.Diagnosis.KDiagnosis diagnosis { get; set; }

        private kKeyValue _sitestore;

        public kKeyValue KeyValue
        {
            get
            {
                if (_sitestore == null)
                {
                    lock (_locker)
                    {
                        if (_sitestore == null)
                        {
                            _sitestore = new kKeyValue(this.RenderContext);
                        }
                    }
                }
                return _sitestore;
            }
        }

        private kDatabase _database;

        public kDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    lock (_locker)
                    {
                        if (_database == null)
                        {
                            _database = new kDatabase(this.RenderContext);
                        }
                    }
                }
                return _database;
            }
        }

        public kDatabase DB
        {

            get { return this.Database;  }
        }

        private KScriptExtension _ex;

        public KScriptExtension Extension
        {
            get
            {
                if (_ex == null)
                {
                    lock(_locker)
                    {
                        if (_ex == null)
                        {
                            _ex = new KScriptExtension(this.RenderContext);
                        }
                    }    
                }
                return _ex;
            }
        }

        public KScriptExtension ex
        {
            get
            {
                return this.Extension; 
            }
        }

        private Kooboo.Sites.Scripting.Global.Mail _mail;
        public Kooboo.Sites.Scripting.Global.Mail mail
        {
            get
            {
                if (_mail == null)
                {
                    lock (_locker)
                    {
                        if (_mail == null)
                        {
                            _mail = new Global.Mail(this.RenderContext);
                        }  
                    }     
                }
                return _mail;
            }      
        }

        Kooboo.Sites.Scripting.Global.Security _security; 

        public Kooboo.Sites.Scripting.Global.Security Security
        {
            get
            {
                if (_security == null)
                {
                    lock (_locker)
                    {
                        if (_security == null)
                        {
                            _security = new Global.Security();
                        }
                    }
                }
                return _security;
            }
        }

        #region APIHelper

        public void Help()
        {
            //var html = new HelperRender().Render(RenderContext);
            var html = new Kooboo.Sites.Scripting.Helper.ScriptHelper.ScriptHelperRender().Render(RenderContext);
            Response.write(html);
            //Help("k");
        }

        public void ViewHelp()
        {
            var html = new ViewHelpRender().Render(RenderContext);
            Response.write(html);
        }


        public List<object> ReturnValues = new List<object>();



        public void output(object obj)
        {
            this.ReturnValues.Add(obj);
        }


        public void export(object obj)
        {
            output(obj);
        }

        private Type GetPropertyType(string fullname)
        {
            string spe = ".";
            fullname = fullname.ToLower().Replace("k.", "");
            List<string> psname = fullname.Split(spe.ToCharArray()).ToList();

            //walkaround:cz
            var currenttype = this.GetType();
            foreach (var item in psname)
            {
                var allpreps = _GetProperties(currenttype);

                foreach (var p in allpreps)
                {
                    if (p.Key.ToLower() == item)
                    {
                        currenttype = p.Value;
                    }
                }
            }
            return currenttype;
        }

        private List<PropertyViewModel> GetProperty(Type type, string methodbase)
        {
            List<PropertyViewModel> result = new List<PropertyViewModel>();

            var allparas = _GetProperties(type);

            foreach (var item in allparas)
            {
                PropertyViewModel model = new PropertyViewModel() { Name = methodbase + "." + item.Key, ReturnType = item.Value };

                result.Add(model);
            }
            return result;
        }

        private string print(List<PropertyViewModel> input, string baseurl)
        {
            if (input == null || input.Count() == 0)
            {
                return string.Empty;
            }

            string result = "<br/><br/><h3>----- Properties -----</h3>";

            foreach (var item in input)
            {
                if (item.ReturnType == null || item.ReturnType == typeof(string) || !item.ReturnType.IsClass)
                {
                    result += "<br/><br/><b>" + item.Name + "</b>";
                    bool iscol = Lib.Reflection.TypeHelper.IsCollection(item.ReturnType);

                    if (iscol)
                    {
                        var coltype = Lib.Reflection.TypeHelper.GetEnumberableType(item.ReturnType);
                        result += "<br/>Return type:  Array";
                        result += "<br/>Data type: " + coltype.Name;
                    }
                    else
                    {
                        result += "<br/>Return type: " + item.ReturnType.Name;
                    }
                }
                else
                {
                    string name = item.Name;
                    Dictionary<string, string> para = new Dictionary<string, string>();
                    para.Add("name", name);
                    string url = Lib.Helper.UrlHelper.AppendQueryString(baseurl, para);
                    result += "<br/><br/> <a href='" + url + "'><b>" + name + "</b></a>";
                }
            }

            return result;
        }

        private string print(List<MethodViewModel> input)
        {
            if (input == null || input.Count() == 0)
            {
                return string.Empty;
            }
            string result = "<br/><br/><h3>----- Methods -----</h3>";

            foreach (var item in input)
            {
                string name = item.Name;
                result += "<br/><br/><b> " + name;

                string para = string.Empty;

                if (item.Parameters != null && item.Parameters.Count > 0)
                {
                    foreach (var p in item.Parameters)
                    {
                        para += p.Value + " " + p.Key + ", ";
                    }
                    para = para.TrimEnd(' ');
                    para = para.TrimEnd(',');
                }
                result += "(" + para + ")</b>";

                if (item.ReturnType != typeof(void))
                {
                    bool iscol = Lib.Reflection.TypeHelper.IsCollection(item.ReturnType);

                    if (iscol)
                    {
                        var coltype = Lib.Reflection.TypeHelper.GetEnumberableType(item.ReturnType);
                        result += "<br/>Return type: Array";
                        result += "<br/>Data type: " + coltype.Name;
                    }
                    else
                    {
                        result += "<br/>Return type: " + item.ReturnType.Name;
                    }
                }

                if (!string.IsNullOrEmpty(item.Sample))
                {
                    result += "<br/><br/>Sample:<br/> " + item.Sample;
                }
            }

            return result;
        }

        private List<MethodViewModel> GetMethod(Type type, string MethodPathBase)
        {
            if (type == typeof(KScriptExtension))
            {
                return new List<MethodViewModel>();
            }

            List<MethodViewModel> methods = new List<MethodViewModel>();

            var allmethods = Kooboo.Lib.Reflection.TypeHelper.GetPublicMethods(type);

            foreach (var item in allmethods)
            {
                var requirepara = item.GetCustomAttribute(typeof(Kooboo.Attributes.SummaryIgnore));
                if (requirepara == null)
                {
                    MethodViewModel model = new MethodViewModel();
                    model.Name = MethodPathBase + "." + item.Name;
                    var paras = item.GetParameters();
                    foreach (var p in paras)
                    {
                        model.Parameters.Add(p.Name, p.ParameterType.Name);
                    }

                    model.ReturnType = item.ReturnType;

                    if (item.ReturnType != typeof(void))
                    {
                        if (item.ReturnType.IsClass && item.ReturnType != typeof(string))
                        {
                            var SampleResponseData = Kooboo.Lib.Development.FakeData.GetFakeValue(item.ReturnType);
                            model.Sample = Lib.Helper.JsonHelper.Serialize(SampleResponseData);
                        }
                    }
                    methods.Add(model);
                }
            }

            return methods;
        }

        private Dictionary<string, Type> _GetProperties(Type objectType)
        {
            if (objectType == typeof(KScriptExtension))
            {
                return ExtensionContainer.List;
            }

            Dictionary<string, Type> fieldlist = new Dictionary<string, Type>();

            FieldInfo[] fieldInfo = objectType.GetFields();

            foreach (var item in objectType.GetProperties())
            {
                if (item.CanRead && item.PropertyType.IsPublic)
                {
                    var requirepara = item.GetCustomAttribute(typeof(Kooboo.Attributes.SummaryIgnore));
                    if (requirepara == null)
                    {
                        fieldlist.Add(item.Name, item.PropertyType);
                    }
                }
            }
            return fieldlist;
        }

        private List<MethodViewModel> GetRepositoryMethod(Type type, string MethodPathBase)
        {
            List<MethodViewModel> methods = new List<MethodViewModel>();

            var repo = Activator.CreateInstance(type) as IRepository;

            bool isRoutable = Kooboo.Attributes.AttributeHelper.IsRoutable(repo.ModelType);

            var allmethods = Kooboo.Lib.Reflection.TypeHelper.GetPublicMethods(type);

            List<string> takeMethods = new List<string>();
            if (isRoutable)
            {
                takeMethods.Add("GetByUrl");
            }
            takeMethods.Add("Get");
            takeMethods.Add("All");

            foreach (var methodname in takeMethods)
            {
                var finds = allmethods.FindAll(o => o.Name == methodname);
                if (finds == null || finds.Count() == 0)
                {
                    continue;
                }
                var item = finds[0];

                int paracount = item.GetParameters().Count();

                foreach (var f in finds)
                {
                    var count = f.GetParameters().Count();
                    if (count < paracount)
                    {
                        item = f;
                        paracount = count;
                    }
                }

                MethodViewModel model = new MethodViewModel();
                model.Name = MethodPathBase + "." + item.Name;
                var paras = item.GetParameters();
                foreach (var p in paras)
                {
                    model.Parameters.Add(p.Name, p.ParameterType.Name);
                }

                model.ReturnType = item.ReturnType;

                if (item.ReturnType != typeof(void))
                {
                    if (item.ReturnType.IsClass && item.ReturnType != typeof(string))
                    {
                        var SampleResponseData = Kooboo.Lib.Development.FakeData.GetFakeValue(item.ReturnType);
                        model.Sample = Lib.Helper.JsonHelper.Serialize(SampleResponseData);
                    }
                }
                methods.Add(model);
            }

            return methods;
        }

        #endregion

    }

    public class MethodViewModel
    {
        public MethodViewModel()
        {
            this.Name = string.Empty;
            this.Sample = string.Empty;
            this.Parameters = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public string Sample { get; set; }
        public Type ReturnType { get; set; }
    }


    public class PropertyViewModel
    {
        public PropertyViewModel()
        {
            this.Name = string.Empty;
        }
        public string Name { get; set; }

        public Type ReturnType { get; set; }
    }

}
