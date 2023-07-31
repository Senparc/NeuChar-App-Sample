namespace NeuCharAppSample.Configs
{
    /// <summary>
    /// NeuChar 开发配置参数
    /// </summary>
    public class NeucharSetting
    {
        /// <summary>
        /// OAuth认证 ClientId
        /// <para>获取地址：www.neuchar.com 开发者后台“开发者信息”</para>
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        ///  OAuth认证 Secret
        /// <para>获取地址：www.neuchar.com 开发者后台“开发者信息”</para>
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 开发者Key
        /// <para>获取地址：www.neuchar.com 开发者后台，具体 APP 的 OAuth 配置页面，第一次保存后会显示</para>
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 开发者Secret
        /// <para>获取地址：www.neuchar.com 开发者后台，具体 APP 的 OAuth 配置页面，第一次保存后会显示</para>
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 当前部署站点的 URL，如：http://www.MyDomain.com，不需要以 / 结尾
        /// </summary>
        public string OAuthDomain { get; set;}
    }
}
