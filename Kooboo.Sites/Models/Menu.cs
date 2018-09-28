﻿using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.CustomAttributes;
using Kooboo.Sites.Helper;
using Kooboo.Extensions;
using System.Runtime.Serialization; 

namespace Kooboo.Sites.Models
{
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    [Kooboo.Attributes.NameAsID]
    public class Menu : CoreObject
    {
        public Menu()
        {
            ConstType = ConstObjectType.Menu;
            Mappings = new Dictionary<string, string>();
            this.tempdata = new TempData();
        }

        public Guid RootId { get; set; }

        private Guid _id;
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (this.ParentId == default(Guid))
                    {
                        _id = Data.IDGenerator.Generate(this.Name, this.ConstType);
                    }
                    else
                    {
                        _id = System.Guid.NewGuid();
                    }
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        private string _template;
        /// <summary>
        /// The template in the format with merge field of 
        /// {href}, {anchortext}, {item1}, {ActiveString} 
        ///  If menu contains the UL partonly, the href and anchortext will be empty. 
        /// </summary> 
        public string Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
                if (!string.IsNullOrEmpty(_template))
                {
                    int activeindex = _template.IndexOf("{activeclass:");
                    if (activeindex >= 0)
                    {
                        var endindex = _template.IndexOf("}", activeindex);
                        if (endindex > 0)
                        {
                            var activeclass = _template.Substring(activeindex, endindex - activeindex);
                            activeclass = activeclass.Replace("{activeclass:", "");
                            this.HasActiveClass = true;
                            this.ActiveClass = activeclass;
                        }

                        if (_template.IndexOf(" class", StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            // find the right place to insert 
                            int index = _template.IndexOf(" class", StringComparison.OrdinalIgnoreCase);
                            int nextequalmark = _template.IndexOf("=", index);
                            if (index > -1 && nextequalmark > -1)
                            {
                                string newtemplate = _template.Substring(0, activeindex) + _template.Substring(endindex + 1);
                                var insertposition = MenuHelper.FindClassInsertPosition(newtemplate);

                                if (insertposition > -1)
                                {
                                    _template = newtemplate.Substring(0, insertposition) + MenuHelper.MarkActiveClassReplacer + " " + newtemplate.Substring(insertposition);
                                }
                                else
                                {
                                    _template = _template.Substring(0, activeindex) + "class='" + MenuHelper.MarkActiveClassReplacer + "'" + _template.Substring(endindex + 1);
                                }
                            }
                            else
                            {
                                _template = _template.Substring(0, activeindex) + "class='" + MenuHelper.MarkActiveClassReplacer + "'" + _template.Substring(endindex + 1);
                            }

                        }
                        else
                        {
                            _template = _template.Substring(0, activeindex) + "class='" + MenuHelper.MarkActiveClassReplacer + "'" + _template.Substring(endindex + 1);
                        }
                    }

                    if (_template.IndexOf(MenuHelper.MarkParentId) > -1 || _template.IndexOf(MenuHelper.MarkCurrentId) > -1)
                    {
                        this.RenderId = true;
                    }
                }
            }
        }

        public string Url { get; set; }

        /// The container of sub item, eg. "<ul>{items}</ul>";  
        public string SubItemContainer { get; set; } = $"<ul class=\"menu\">{MenuHelper.MarkSubItems}</ul>";

        /// The common template of Items... It can be that each items also has its own template. If each item has its own template, use that, otherwise use this common template. 
        //[JsonProperty("subitemtemplate")]
        public string SubItemTemplate { get; set; } = $"<li><a href=\"{MenuHelper.MarkHref}\">{MenuHelper.MarkAnchorText}</a>{MenuHelper.MarkSubItems}</li>";

        [KoobooIgnore]
        [Kooboo.Attributes.SummaryIgnore]
        [IgnoreDataMember]
        public TempData tempdata { get; set; }

        public List<Menu> children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<Menu>();
                }
                return _children;
            }
            set
            {
                _children = value;
            }
        }

        private List<Menu> _children;

        [KoobooIgnore]
        [Kooboo.Attributes.SummaryIgnore]
        [IgnoreDataMember]
        public Menu Parent
        {
            get;
            set;
        }

        public Guid ParentId { get; set; }

        [Obsolete]
        public void AddSubMenu(Menu submenu)
        {
            submenu.Parent = this;
            children.Add(submenu);

            if (submenu.tempdata != null)
            {
                if (submenu.tempdata.EndIndex > this.tempdata.EndIndex)
                {
                    this.tempdata.EndIndex = submenu.tempdata.EndIndex;
                }
            }
            submenu.tempdata = null;
        }

        internal bool HasActiveClass { get; set; }
        internal string ActiveClass { get; set; }

        internal bool RenderId { get; set; }

        public Guid DataSourceId { get; set; } = default(Guid);

        /// <summary>
        /// Mapping of datasource fields to href and text... 
        /// </summary>
        public Dictionary<string, string> Mappings { get; set; }

        private Dictionary<string, string> _values;

        public Dictionary<string, string> Values
        {
            get
            {
                if (_values == null)
                {
                    _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _values;
            }
            set
            {
                _values = value;
            }
        }

        public override int GetHashCode()
        {
            string unique = this.Name + this.Url + this._template + this.SubItemTemplate + this.SubItemContainer;

            foreach (var item in Values)
            {
                unique += item.Key + item.Value;
            }


            if (this._children != null && this._children.Count > 0)
            {
                foreach (var item in _children)
                {
                    var code = item.GetHashCode();
                    unique += code.ToString();
                }
            }

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }

    /// <summary>
    /// This is only used to temparaily record some information.
    /// </summary>
    [Serializable]
    public class TempData
    {
        public int EndIndex { get; set; }

        public int StartIndex { get; set; }
    }

}