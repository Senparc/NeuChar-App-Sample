using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NeuCharAppSample.Configs;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.AppStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeuCharAppSample.Controllers
{
    /// <summary>
    /// OAuth 授权
    /// </summary>
    public class OAuthController : Controller
    {
        /// <summary>
        /// 域名
        /// </summary>
        private const string OAUTHDOMAIN = "http://localhost:8773/";

        private readonly NeucharSetting _neucharSetting;
        public OAuthController(IOptions<NeucharSetting> neucharSetting)
        {
            _neucharSetting = neucharSetting.Value;
        }

        public ActionResult Index(string appCode)
        {
            var redirectUrl = $"{OAUTHDOMAIN}/OAuth/Callback";
            var url = $"https://neuchar.senparc.com/app/weixinOAuth/Authorize?appCode={appCode}&redirectUrl={redirectUrl}";
            return Redirect(url);
        }
        //获取Code

        public ActionResult Callback(string code)
        {
            //获取P2pCode
            var p2pCodeUrl = $"https://neuchar.senparc.com/api/GetPassport?appKey={_neucharSetting.AppKey}&secret={_neucharSetting.AppSecret}";
            var messageResult = Senparc.CO2NET.HttpUtility.Post.PostGetJson<PassportResult>(p2pCodeUrl, formData: new Dictionary<string, string>(), encoding: Encoding.UTF8);

            var userinfoUrl =
                $"https://neuchar.senparc.com/api/GetMember?code={code}&accessToken={messageResult.Data.Token}";
            var result = Senparc.CO2NET.HttpUtility.Post.PostGetJson<UserInfoJson>(userinfoUrl, null, new Dictionary<string, string>(), Encoding.UTF8);
            return View(result);
        }

        /// <summary>
        /// Neuchar一键登录
        /// 开发接入 OAuth认证配置 配置页面链接
        /// </summary>
        /// <param name="code"></param>
        /// <param name="appCode"></param>
        /// <returns></returns>
        public ActionResult Login(string code, string appCode)
        {
            try
            {
                //获取AccessToken
                //拉取登录用户信息
                var passportResult = Senparc.CO2NET.HttpUtility.Post.PostGetJson<PassportResult>($"https://neuchar.senparc.com/api/GetOAuthPassport?clientId={_neucharSetting.ClientId}&secret={_neucharSetting.ClientSecret}", formData: new Dictionary<string, string>(), encoding: Encoding.UTF8);

                //获取登陆用户信息
                var userInfo = Senparc.CO2NET.HttpUtility.Post.PostGetJson<UserInfoJson>($"https://neuchar.senparc.com/api/GetAccount?code={code}&accessToken={passportResult.Data.Token}", null, new Dictionary<string, string>(), Encoding.UTF8);
                if (userInfo.errcode != ReturnCode.请求成功)
                {
                    return Content(userInfo.errmsg);
                }
                //APPCODE 会在Weixin OAuth中使用，需要开发者持久化保存
                TempData["AppCode"] = appCode;
                return View(userInfo);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }



}
