using System;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FrameWork.Core.Mvc.Result
{
    public class JsonResult : ActionResult
    {
        /// <returns>
        /// 内容编码.
        /// </returns>
        public Encoding ContentEncoding { get; set; }

        /// <returns>
        /// 内容类型.
        /// </returns>
        public string ContentType { get; set; }

        /// <returns>
        /// 数据.
        /// </returns>
        public ResponseMessage Message { get; set; }

        public JsonResult(ResponseMessage message)
        {
            this.Message = message;
        }

        private static readonly IsoDateTimeConverter TimeConverter = new IsoDateTimeConverter
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
        };

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.Message == null) return;

            var strJson = JsonConvert.SerializeObject(this.Message, TimeConverter);
            response.Write(strJson);
        }
    }
}