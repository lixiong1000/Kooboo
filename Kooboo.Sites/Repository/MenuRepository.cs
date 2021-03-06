//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.IndexedDB;
using System;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class MenuRepository : SiteRepositoryBase<Menu>
    {
        internal override ObjectStoreParameters StoreParameters
        {
            get
            {
                ObjectStoreParameters para = new ObjectStoreParameters();
                para.AddColumn<Menu>(o => o.ParentId);
                para.SetPrimaryKeyField<Menu>(o => o.Id);
                para.SetPrimaryKeyField<Menu>(o => o.Id);
                return para;
            }
        }
        
        public override Menu GetByNameOrId(string NameOrGuid)
        {
            Guid key;
            bool parseok = Guid.TryParse(NameOrGuid, out key);
            if (!parseok)
            {
                byte consttype = Service.ConstTypeService.GetConstType(typeof(Menu));
                key = Data.IDGenerator.GetOrGenerate(NameOrGuid, consttype);
            }
            return Get(key);
        }
        
        private bool HasActive(Menu menu, ref string searchstring)
        {
            if (!string.IsNullOrEmpty(menu.Template) && menu.Template.Contains(searchstring))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(menu.SubItemTemplate) && menu.SubItemTemplate.Contains(searchstring))
            {
                return true;
            }

            foreach (var item in menu.children)
            {
                var result = HasActive(item, ref searchstring);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsNameAvailable(string MenuName)
        {
            var current = this.GetByNameOrId(MenuName);
            return current == null;
        }

        public string GetNewMenuName()
        {
            string name = "";

            for (int i = 0; i < 1000; i++)
            {
                name = "untitled" + i.ToString();
                if (IsNameAvailable(name))
                {
                    return name;
                }
            }

            return null;
        }

       public void Swap(Guid RootId, Guid IdA, Guid IdB, Guid UserId = default(Guid))
        {
            var menu = this.Get(RootId); 
            if (menu !=null)
            {
               if (_swap(menu, IdA, IdB))
                {
                    this.AddOrUpdate(menu, UserId); 
                }
            }
        }
        
        private bool _swap(Menu ParentMenu, Guid IdA, Guid IdB)
        {
            if (ParentMenu.children == null || ParentMenu.children.Count() ==0)
            {
                return false;
            }

            var founda = ParentMenu.children.Find(o => o.Id == IdA);
            var foundb = ParentMenu.children.Find(o => o.Id == IdB); 

            if (founda !=null && foundb !=null)
            {
                int x=0;
                int y=0;

                for (int i = 0; i < ParentMenu.children.Count; i++)
                {
                    var item = ParentMenu.children[i]; 
                    if (item.Id == IdA)
                    {
                        x = i; 
                    }
                   if (item.Id == IdB)
                    {
                        y = i; 
                    } 
                }

                if (x !=y)
                {
                    var old = ParentMenu.children[x];
                    ParentMenu.children[x] = ParentMenu.children[y];
                    ParentMenu.children[y] = old;
                    return true; 
                } 
            }
            else if (founda == null && foundb == null)
            {
                foreach (var item in ParentMenu.children)
                {
                    var ok = _swap(item, IdA, IdB); 
                    if (ok)
                    {
                        return true; 
                    }
                }
            }

            return false; 
        }
    }
}
