using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Unity;

namespace DingTalk.Utility.Filters
{
    /// <summary>
    /// 需要在nuget引用之后，单独引用Unity.Configuration
    /// 如果有AOP扩展，还需要引用Unity.Interception.Configuration
    /// 因为我们是用配置文件来做的配置
    /// </summary>
    public class ContainerFactory
    {
        public static IUnityContainer BuildContainer()
        {
            //get
            //{
            return _Container;
            //}
        }

        private static IUnityContainer _Container = null;
        static ContainerFactory()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "CfgFiles\\Unity.Config");//找配置文件的路径
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            UnityConfigurationSection section = (UnityConfigurationSection)configuration.GetSection(UnityConfigurationSection.SectionName);
            _Container = new UnityContainer();
            section.Configure(_Container, "WebApiContainer");
        }
    }
}