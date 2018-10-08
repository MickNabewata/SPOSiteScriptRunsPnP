using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;

namespace MySampleFunction
{
    /// <summary>
    /// 関数定義クラス
    /// </summary>
    public static class ApplyPnPTemplate
    {
        /// <summary>
        /// ApplyPnPTemplate関数
        /// </summary>
        /// <param name="req">HTTP要求</param>
        /// <param name="log">ログライター</param>
        /// <returns>HTTP応答</returns>
        [FunctionName("ApplyPnPTemplate")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("ApplyPnPTemplateを開始します。");

                // AppSettingsからSharePointの認証情報を取得
                string account = ConfigurationManager.AppSettings["SPOAccount"];
                string password = ConfigurationManager.AppSettings["SPOPassword"];
                string azureBlobKey = ConfigurationManager.AppSettings["AzureBlobKey"];
                string azureBlobContainer = ConfigurationManager.AppSettings["AzureBlobContainer"];
                string templateFileName = ConfigurationManager.AppSettings["TemplateFileName"];

                // リクエスト本文から適用先を取得
                dynamic body = await req.Content.ReadAsAsync<object>();
                string webUrl = body?.webUrl;
                if (webUrl == null)
                {
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, "webUrlパラメータが必要です。");
                }
                else
                {
                    // 適用先をURIに変換
                    Uri applyToUri = new Uri(webUrl);
                    log.Info($"webUrl：{ applyToUri.AbsoluteUri }");
                    
                    // テンプレートを適用
                    Apply(applyToUri, account, password, azureBlobKey, azureBlobContainer, templateFileName, log);

                    // 返却
                    return req.CreateResponse(HttpStatusCode.OK, "成功");
                }
            }
            catch(Exception ex)
            {
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// テンプレートを適用
        /// </summary>
        /// <param name="webUrl">適用先URI</param>
        /// <param name="account">ログインアカウント</param>
        /// <param name="password">ログインパスワード</param>
        /// <param name="log">ログライター</param>
        private static void Apply(Uri webUrl, string account, string password, string azureBlobKey, string azureBlobContainer, string templateFileName, TraceWriter log)
        {
            log.Info($"適用開始");
            using (ClientContext ctx = new ClientContext(webUrl))
            {
                log.Info($"認証情報作成");
                SecureString securePassword = new SecureString();
                foreach (char c in password.ToCharArray()) securePassword.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(account, securePassword);

                log.Info($"テンプレート取得");
                var provider = new XMLAzureStorageTemplateProvider(azureBlobKey, azureBlobContainer);
                var template = provider.GetTemplate(templateFileName);
                log.Info($"リスト数：{ template.Lists?.Count }");
                foreach (var list in template.Lists)
                {
                    log.Info(list.Title);
                }

                log.Info($"テンプレート適用情報作成");
                var templateInfo = new ProvisioningTemplateApplyingInformation();
                
                templateInfo.ProgressDelegate = delegate (String message, Int32 progress, Int32 total)
                {
                    log.Info($"{progress}/{total} - {message}");
                };

                log.Info($"実行");
                Web web = ctx.Web;
                web.ApplyProvisioningTemplate(template, templateInfo);
            }
            log.Info($"適用終了");
        }
    }
}
