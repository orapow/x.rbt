using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.com
{
    public class upload : xapi
    {
        /// <summary>
        /// 上传类型
        /// 对应tp.code
        /// </summary>
        public string type { get; set; }
        public string url { get; set; }


        protected string pathbase = "upload/";

        protected HttpPostedFile uploadFile;

        List<tp> tps = new List<tp>() {
            new tp() {code="img", name="图片", exts="[jpg][png][gif]", size=0},
            new tp() {code="file", name="文件", exts="[rar][zip][xls][csv][txt][doc][pdf]", size=0},
            new tp() {code="doc", name="文档", exts="[xls][xls][csv][txt][doc][docx][pdf][ppt][pptx]", size=0},
            new tp() {code="media", name="媒体", exts="[flv][mp4][wav][mp3][swf]", size=0},
            new tp() {code="paper", name="flash文档", exts="[swf]", size=0}
        };

        /// <summary>
        /// 返回数据
        /// </summary>
        /// <returns></returns>
        public override byte[] GetResponse()
        {
            InitApi();
            uploadFile = Context.Request.Files[0];
            var rsp = upFile();
            return Encoding.UTF8.GetBytes(Serialize.ToJson(rsp));
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns>
        /// 文件Url
        /// </returns>
        protected file upFile()
        {
            pathbase = pathbase + DateTime.Now.ToString("yyyy-MM-dd") + "/";
            var uploadpath = Context.Server.MapPath(pathbase);
            var url = string.Empty;

            var tp = tps.FirstOrDefault(o => o.code == type);
            if (tp == null) throw new XExcep("T类型不允许");

            var ext = getFileExt();
            if (!tp.exts.Contains("[" + ext + "]")) throw new XExcep("T不允许的文类型，允许类型：" + tp.exts);

            if (tp.size > 0 && uploadFile.ContentLength >= (tp.size * 1024 * 1024)) throw new XExcep("T文件大小超出限制，限制为：" + tp.size + "MB");

            if (!Directory.Exists(uploadpath)) Directory.CreateDirectory(uploadpath);

            var filename = reName();
            uploadFile.SaveAs(uploadpath + filename);
            url = "http://" + cfg.domain + "/" + pathbase + filename;

            return new file()
            {
                ext = ext,
                size = uploadFile.ContentLength,
                url = url
            };

        }

        private string reName()
        {
            return System.Guid.NewGuid() + "." + getFileExt();
        }

        private bool checkSize(int size)
        {
            return uploadFile.ContentLength >= (size * 1024 * 1024);
        }

        private string getFileExt()
        {
            var temp = uploadFile.FileName.Split('.');
            return temp[temp.Length - 1].ToLower();
        }

        private void createFolder(string uploadpath)
        {
            if (!Directory.Exists(uploadpath))
            {
                Directory.CreateDirectory(uploadpath);
            }
        }

        protected class file : XResp
        {
            public string url { get; set; }
            public decimal size { get; set; }
            public string ext { get; set; }
        }

        class tp
        {
            public string code { get; set; }
            public string name { get; set; }
            public string exts { get; set; }
            public int size { get; set; }
        }
    }
}
