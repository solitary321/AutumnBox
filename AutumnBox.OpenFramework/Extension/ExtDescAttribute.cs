﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/1 4:08:35 (UTC +8:00)
** desc： ...
*************************************************/

namespace AutumnBox.OpenFramework.Extension
{
    /// <summary>
    /// 拓展模块说明特性
    /// </summary>
    public class ExtDescAttribute : ExtInfoI18NAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="desc"></param>
        public ExtDescAttribute(string desc):base(desc) {
       
        }
    }
}
