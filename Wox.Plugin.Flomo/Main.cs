using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Wox.Plugin.Flomo
{
    public class Main : IPlugin
    {
        private PluginInitContext _context;
        private string _apiUrl;
        public void Init(PluginInitContext context)
        {
            _context = context;
            Configuration config = Utils.GetConfig();
            AppSettingsSection appSection = (AppSettingsSection)config.GetSection("appSettings");
            _apiUrl = appSection.Settings["ApiUrl"].Value;
        }
        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();
            const string ico = @"Images\\icon.png";


            if (query.Search.Length == 0)
            {
                results.Add(new Result
                {
                    Title = "请输入Memo",                    
                    IcoPath = ico
                });
                return results;
            }
            else
            {
                var q = query.Search;
                results.Add(new Result
                {
                    Title = "回车发送",
                    SubTitle = $"Memo：{q}",
                    IcoPath = ico,
                    Action = this.SendToFlomoFunc(q)
                });
                return results;
            }
        }

        private Func<ActionContext, bool> SendToFlomoFunc(string text)
        {
            return c =>
            {
                _context.API.ShowMsg(this.SendToFlomo(text) ? "发送成功" : "发送失败");
                return false;
            };
        }

        private bool SendToFlomo(string text)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(new { content = text });
                var result = Utils.PostWebRequest(_apiUrl, jsonData);
                var resultObj = JsonConvert.DeserializeObject<ResultEntity>(result);
                return resultObj.Code == 0;
            }
            catch (Exception)
            {
                return false;
            }
            
        }        

    }

    class ResultEntity
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
