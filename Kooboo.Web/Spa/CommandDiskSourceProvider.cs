//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Render.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Web.Spa
{
    public class CommandDiskSourceProvider : ICommandSourceProvider
    {
        public CommandDiskSourceProvider()
        { }

        public CommandDiskSourceProvider(SpaRenderOption option)
        {
            this.option = option;
        }

        public SpaRenderOption option { get; set; }

        private List<string> _startPageNames;
        private List<string> StartPageNames
        {
            get
            {
                if (_startPageNames == null)
                {
                    _startPageNames = new List<string>();
                    _startPageNames.Add("index");
                    _startPageNames.Add("default");
                }
                return _startPageNames;
            }
        }


        private string GetRoot(RenderContext context)
        {
            if (option != null)
            {
                string root = this.option.GetDiskRoot(context);
                if (!string.IsNullOrEmpty(option.StartPath))
                {
                    root = RenderHelper.CombinePath(root, option.StartPath);
                }
                return root;
            }

            return null;
        }


        private string FindFile(string FullPath)
        {
            if (System.IO.File.Exists(FullPath))
            {
                return FullPath;
            }

            var fileinfo = new System.IO.FileInfo(FullPath);

            if (!fileinfo.Directory.Exists)
            {
                return null;
            }

            var dir = fileinfo.Directory;
            if (!string.IsNullOrEmpty(fileinfo.Name))
            {
                var files = dir.GetFiles(fileinfo.Name + ".*", SearchOption.TopDirectoryOnly);
                if (files != null && files.Count() > 0)
                {
                    return files[0].FullName;
                }
            }
            return null;
        }

        private string ExtendViewSearch(string root, string relative, List<string> searchfolders)
        {
            foreach (var item in searchfolders)
            {
                string viewrelative = "/" + item + relative;
                viewrelative = RenderHelper.CombinePath(root, viewrelative);
                var result = FindFile(viewrelative);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            return null;
        }

        public byte[] GetBinary(RenderContext context, string RelativeUrl)
        {
            RelativeUrl = CleanQuestionMark(RelativeUrl);
            string root = GetRoot(context);
            string route = null;
            if (RelativeUrl.StartsWith("/") || RelativeUrl.StartsWith("\\"))
            {
                RelativeUrl = RelativeUrl.Substring(1);
            }

            route = "/" + RelativeUrl;
            string fullpath = RenderHelper.CombinePath(root, route);
            string FileName = FindFile(fullpath);

            if (!string.IsNullOrEmpty(FileName))
            {
                return GetBinary(context, option, RelativeUrl, FileName);
                //return System.IO.File.ReadAllBytes(FileName);
            }
            return null;
        }

        private string SearchRoute(RenderContext context, string RelativeUrl)
        {

            string root = option.GetDiskRoot(context);
            if (!string.IsNullOrEmpty(option.StartPath))
            {
                root = RenderHelper.CombinePath(root, option.StartPath);
            }

            string result = null;

            bool isDirectory = false;
            if (RelativeUrl.EndsWith("/"))
            {
                isDirectory = true;
            }
            else
            {
                string diskpath = RenderHelper.CombinePath(root, RelativeUrl);
                isDirectory = System.IO.Directory.Exists(diskpath);
            }

            if (isDirectory)
            {
                foreach (var item in this.StartPageNames)
                {
                    string fullpath = RenderHelper.CombinePath(root, RelativeUrl);
                    fullpath = RenderHelper.CombinePath(fullpath, item);

                    result = FindFile(fullpath);

                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = ExtendViewSearch(root, RelativeUrl, option.ViewFolders);
                    }
                    if (!string.IsNullOrEmpty(result))
                    { return result; }
                }
            }
            else
            {
                string fullpath = RenderHelper.CombinePath(root, RelativeUrl);

                result = FindFile(fullpath);

                if (string.IsNullOrWhiteSpace(result))
                {
                    result = ExtendViewSearch(root, RelativeUrl, option.ViewFolders);
                }
            }

            return result;
        }

        public string GetString(RenderContext context, string RelativeUrl)
        {
            RelativeUrl = CleanQuestionMark(RelativeUrl);

            string fullfilename = SearchRoute(context, RelativeUrl);

            if (!string.IsNullOrEmpty(fullfilename))
            {
                return GetText(context, option, RelativeUrl, fullfilename);  
            }
            return null; 
        }

        public string GetLayout(RenderContext context, string RelativeUrl)
        {
            RelativeUrl = CleanQuestionMark(RelativeUrl);
            string root = GetRoot(context);

            string LayoutRoute = null;
            if (RelativeUrl.StartsWith("/") || RelativeUrl.StartsWith("\\"))
            {
                RelativeUrl = RelativeUrl.Substring(1);
            }

            LayoutRoute = "/" + RelativeUrl;
            string fullpath = RenderHelper.CombinePath(root, LayoutRoute);
            string FileName = FindFile(fullpath);

            if (string.IsNullOrEmpty(FileName))
            {
                foreach (var item in this.option.LayoutFolders)
                {
                    LayoutRoute = "/" + item + "/" + RelativeUrl;
                    fullpath = RenderHelper.CombinePath(root, LayoutRoute);
                    FileName = FindFile(fullpath);
                    if (!string.IsNullOrEmpty(FileName))
                    {
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(FileName))
            {
               return GetText(context, option, RelativeUrl, FileName); 
               // return System.IO.File.ReadAllText(FileName);
            }
            return null;
        }

        private string CleanQuestionMark(string RelativeUrl)
        {
            if (!string.IsNullOrEmpty(RelativeUrl))
            {
                int markindex = RelativeUrl.IndexOf("?");

                if (markindex > -1)
                {
                    return RelativeUrl.Substring(0, markindex);
                }
            }
            return RelativeUrl;
        }

        private static byte[] GetBinary(RenderContext context, SpaRenderOption option, string RelativeUrl, string FullFileName)
        {
                byte[] result = null;
#if DEBUG
                {
                    result = System.IO.File.ReadAllBytes(FullFileName);
                }
#endif
                if (result == null)
                {
                    Guid key = Kooboo.Lib.Security.Hash.ComputeGuidIgnoreCase(RelativeUrl);
                    result = Kooboo.Data.Cache.RenderCache.GetBinary(key);
                    if (result == null)
                    {
                        result = System.IO.File.ReadAllBytes(FullFileName);
                        Kooboo.Data.Cache.RenderCache.SetBinary(key, result);
                    }
                }
                return result;
           
        }

        private static string GetText(RenderContext context, SpaRenderOption option, string RelativeUrl, string FullFileName)
        {
            Guid key = Lib.Security.Hash.ComputeGuidIgnoreCase(FullFileName);
            string text = null;
#if DEBUG
            {
                text = System.IO.File.ReadAllText(FullFileName);
                key = Lib.Security.Hash.ComputeGuidIgnoreCase(text);
            }
#endif

            if (option.EnableMultilingual)
            {
                string htmlbody = Kooboo.Data.Cache.MultiLingualRender.GetHtml(context, key);
                if (htmlbody == null)
                {
                    if (text == null)
                    {
                        text = File.ReadAllText(FullFileName);
                    }
                    htmlbody = Kooboo.Data.Cache.MultiLingualRender.SetGetHtml(context, key, text);
                }
                return htmlbody; 
            }
            else
            {
                if (text == null)
                {
                    text = Kooboo.Data.Cache.RenderCache.GetHtml(key);
                    if (text == null)
                    {
                        text = File.ReadAllText(FullFileName);
                    }
                    Kooboo.Data.Cache.RenderCache.SetHtml(key, text); 
                } 
                return text;  
            } 
        }

        public Stream GetStream(RenderContext context, string RelativeUrl)
        {
            var binary = GetBinary(context, RelativeUrl); 
            if (binary !=null)
            {
                return new System.IO.MemoryStream(binary); 
            }
            return null; 
        }
    }



}
