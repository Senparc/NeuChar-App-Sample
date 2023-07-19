using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NeuCharAppSample.Configs;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar.App.AppStore;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs.User;
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
        private readonly NeucharSetting _neucharSetting;
        private readonly IServiceProvider _serviceProvider;

        public OAuthController(IOptions<NeucharSetting> neucharSetting, IServiceProvider serviceProvider)
        {
            _neucharSetting = neucharSetting.Value;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 微信端用户入口页面
        /// </summary>
        /// <param name="appCode"></param>
        /// <returns></returns>
        public ActionResult Index(string appCode)
        {
            var redirectUrl = $"{_neucharSetting.OAuthDomain}/OAuth/Callback";
            var url = $"{Senparc.NeuChar.App.AppStore.Config.DefaultDomainName}/app/weixinOAuth/Authorize?appCode={appCode.UrlDecode()}&redirectUrl={redirectUrl.UrlEncode()}";
            return Redirect(url);
        }

        /// <summary>
        /// 微信端用户验证并登录页面
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Callback(string code)
        {
            //获取P2pCode
            var p2pCodeUrl = $"{Senparc.NeuChar.App.AppStore.Config.DefaultDomainName}/api/GetPassport?appKey={_neucharSetting.AppKey}&secret={_neucharSetting.AppSecret}";
            var messageResult = Senparc.CO2NET.HttpUtility.Post.PostGetJson<PassportResult>(_serviceProvider,p2pCodeUrl, formData: new Dictionary<string, string>(), encoding: Encoding.UTF8);

            var userinfoUrl =
                $"{Senparc.NeuChar.App.AppStore.Config.DefaultDomainName}/api/GetMember?code={code}&accessToken={messageResult.Data.Token}";
            var result = Senparc.CO2NET.HttpUtility.Post.PostGetJson<UserInfoJson>(_serviceProvider,userinfoUrl, null, new Dictionary<string, string>(), Encoding.UTF8);
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
                var passportResult = Senparc.CO2NET.HttpUtility.Post.PostGetJson<PassportResult>(_serviceProvider,$"{Senparc.NeuChar.App.AppStore.Config.DefaultDomainName}/api/GetOAuthPassport?clientId={_neucharSetting.ClientId}&secret={_neucharSetting.ClientSecret}", formData: new Dictionary<string, string>(), encoding: Encoding.UTF8);

                //获取登陆用户信息
                var userInfo = Senparc.CO2NET.HttpUtility.Post.PostGetJson<UserInfoJson>(_serviceProvider,$"{Senparc.NeuChar.App.AppStore.Config.DefaultDomainName}/api/GetAccount?code={code}&accessToken={passportResult.Data.Token}", null, new Dictionary<string, string>(), Encoding.UTF8);
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
