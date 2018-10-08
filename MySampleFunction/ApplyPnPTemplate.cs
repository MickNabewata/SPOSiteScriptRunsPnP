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
    /// �֐���`�N���X
    /// </summary>
    public static class ApplyPnPTemplate
    {
        /// <summary>
        /// ApplyPnPTemplate�֐�
        /// </summary>
        /// <param name="req">HTTP�v��</param>
        /// <param name="log">���O���C�^�[</param>
        /// <returns>HTTP����</returns>
        [FunctionName("ApplyPnPTemplate")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                log.Info("ApplyPnPTemplate���J�n���܂��B");

                // AppSettings����SharePoint�̔F�؏����擾
                string account = ConfigurationManager.AppSettings["SPOAccount"];
                string password = ConfigurationManager.AppSettings["SPOPassword"];
                string azureBlobKey = ConfigurationManager.AppSettings["AzureBlobKey"];
                string azureBlobContainer = ConfigurationManager.AppSettings["AzureBlobContainer"];
                string templateFileName = ConfigurationManager.AppSettings["TemplateFileName"];

                // ���N�G�X�g�{������K�p����擾
                dynamic body = await req.Content.ReadAsAsync<object>();
                string webUrl = body?.webUrl;
                if (webUrl == null)
                {
                    return req.CreateErrorResponse(HttpStatusCode.BadRequest, "webUrl�p�����[�^���K�v�ł��B");
                }
                else
                {
                    // �K�p���URI�ɕϊ�
                    Uri applyToUri = new Uri(webUrl);
                    log.Info($"webUrl�F{ applyToUri.AbsoluteUri }");
                    
                    // �e���v���[�g��K�p
                    Apply(applyToUri, account, password, azureBlobKey, azureBlobContainer, templateFileName, log);

                    // �ԋp
                    return req.CreateResponse(HttpStatusCode.OK, "����");
                }
            }
            catch(Exception ex)
            {
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// �e���v���[�g��K�p
        /// </summary>
        /// <param name="webUrl">�K�p��URI</param>
        /// <param name="account">���O�C���A�J�E���g</param>
        /// <param name="password">���O�C���p�X���[�h</param>
        /// <param name="log">���O���C�^�[</param>
        private static void Apply(Uri webUrl, string account, string password, string azureBlobKey, string azureBlobContainer, string templateFileName, TraceWriter log)
        {
            log.Info($"�K�p�J�n");
            using (ClientContext ctx = new ClientContext(webUrl))
            {
                log.Info($"�F�؏��쐬");
                SecureString securePassword = new SecureString();
                foreach (char c in password.ToCharArray()) securePassword.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials(account, securePassword);

                log.Info($"�e���v���[�g�擾");
                var provider = new XMLAzureStorageTemplateProvider(azureBlobKey, azureBlobContainer);
                var template = provider.GetTemplate(templateFileName);
                log.Info($"���X�g���F{ template.Lists?.Count }");
                foreach (var list in template.Lists)
                {
                    log.Info(list.Title);
                }

                log.Info($"�e���v���[�g�K�p���쐬");
                var templateInfo = new ProvisioningTemplateApplyingInformation();
                
                templateInfo.ProgressDelegate = delegate (String message, Int32 progress, Int32 total)
                {
                    log.Info($"{progress}/{total} - {message}");
                };

                log.Info($"���s");
                Web web = ctx.Web;
                web.ApplyProvisioningTemplate(template, templateInfo);
            }
            log.Info($"�K�p�I��");
        }
    }
}
