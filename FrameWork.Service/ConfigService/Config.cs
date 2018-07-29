using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FrameWork.Core.Config;

namespace FrameWork.ConfigService
{
    public class Config : IConfig
    {
        /// <summary>
        /// 获取配置信息文件路径。
        /// </summary>
        /// <returns>配置信息文件路径。</returns>
        private string GetFullPath(Type type)
        {
            var configName = type.Name;
            if (configName.EndsWith("config", StringComparison.OrdinalIgnoreCase))
            {
                configName = configName.Substring(0, configName.Length - 6);
            }
            return string.Concat(GlobalConfig.DirectoryPath, configName, GlobalConfig.Suffix);
        }

        /// <summary>
        /// 获取配置实例
        /// </summary>
        /// <returns>配置实例</returns>
        public TConfig Get<TConfig>() where TConfig : class ,new()
        {
            var fullPath = this.GetFullPath(typeof(TConfig));
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("找不到文件路径", fullPath);
            }

            using (var xmlTextReader = new XmlTextReader(fullPath))
            {
                var xmlSerializer = new XmlSerializer(typeof(TConfig));
                return (TConfig)xmlSerializer.Deserialize(xmlTextReader);
            }
        }

        /// <summary>
        /// 保存配置信息至配置文件。
        /// </summary>
        public void Save<TConfig>(TConfig config) where TConfig : class ,new()
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (!Directory.Exists(GlobalConfig.DirectoryPath))
            {
                Directory.CreateDirectory(GlobalConfig.DirectoryPath);
            }

            var fullPath = this.GetFullPath(typeof(TConfig));
            var xmlSerializer = new XmlSerializer(typeof(TConfig));
            using (var xmlTextWriter = new XmlTextWriter(fullPath, Encoding.UTF8))
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                var xmlNamespace = new XmlSerializerNamespaces();
                xmlNamespace.Add(string.Empty, string.Empty);
                xmlSerializer.Serialize(xmlTextWriter, config, xmlNamespace);
            }
        }
    }
}