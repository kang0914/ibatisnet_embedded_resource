using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace iBatisNet.Biz.Utils
{
    public class IBatisNetMapperHelper
    {
        public const string DEFAULT_EMBEDDED_RESOURCE_SQLMAP_CONFIG = "configs.SqlMap.config";
        public const string DEFAULT_EMBEDDED_RESOURCE_PROVIDERS_CONFIG = "configs.providers.config";
        public const string DEFAULT_EMBEDDED_RESOURCE_QUERY_XML = "Query";

        public static ISqlMapper CreateMapperFromEmbeddedResource()
        {
            var defaultNamespace =  Assembly.GetExecutingAssembly().GetName().Name;

            return CreateMapperFromEmbeddedResource($"{DEFAULT_EMBEDDED_RESOURCE_SQLMAP_CONFIG}, {defaultNamespace}",
                                                    $"{DEFAULT_EMBEDDED_RESOURCE_PROVIDERS_CONFIG}, {defaultNamespace}", 
                                                    DEFAULT_EMBEDDED_RESOURCE_QUERY_XML,
                                                    null,
                                                    null);
        }

        public static ISqlMapper CreateMapperFromEmbeddedResource(string connectionString)
        {
            var defaultNamespace = Assembly.GetExecutingAssembly().GetName().Name;

            return CreateMapperFromEmbeddedResource($"{DEFAULT_EMBEDDED_RESOURCE_SQLMAP_CONFIG}, {defaultNamespace}",
                                                    $"{DEFAULT_EMBEDDED_RESOURCE_PROVIDERS_CONFIG}, {defaultNamespace}",
                                                    DEFAULT_EMBEDDED_RESOURCE_QUERY_XML,
                                                    connectionString,
                                                    null);
        }

        //https://stackoverflow.com/questions/21925935/dyanamically-change-the-database-name-in-sqlmapconfig-xml-file
        public static ISqlMapper CreateMapperFromEmbeddedResource(string resourceSqlMap, string resourceProviders, string resourceQueryRootPath,
                                                                  string connectionString, string providerName)
        {
            // Load the config file (embedded resource in assembly).
            XmlDocument xmlDoc = IBatisNet.Common.Utilities.Resources.GetEmbeddedResourceAsXmlDocument(resourceSqlMap);

            // providers
            if(string.IsNullOrEmpty(resourceProviders) == false)
            { 
                xmlDoc["sqlMapConfig"]["providers"].Attributes["embedded"].InnerText = resourceProviders;
            }

            // database(provider)
            if (string.IsNullOrEmpty(providerName) == false)
            {
                xmlDoc["sqlMapConfig"]["database"]["provider"].Attributes["name"].InnerText = providerName;
            }

            // database(connectionString)
            if (string.IsNullOrEmpty(connectionString) == false)
            {
                xmlDoc["sqlMapConfig"]["database"]["dataSource"].Attributes["connectionString"].InnerText = connectionString;
            }

            // sqlMaps
            if(string.IsNullOrEmpty(resourceQueryRootPath) == false)
            { 
                var sqlMapsRoot = xmlDoc["sqlMapConfig"]["sqlMaps"];
                var queryFileNames = IBatisNetMapperHelper.GetAllQueryXML(resourceQueryRootPath);
                var defaultNamespace = Assembly.GetExecutingAssembly().GetName().Name;
                foreach (var queryFile in queryFileNames)
                {
                    System.Xml.XmlElement elem = xmlDoc.CreateElement("sqlMap", sqlMapsRoot.NamespaceURI);
                    elem.SetAttribute("embedded", $"{queryFile}, {defaultNamespace}");
                    sqlMapsRoot.AppendChild(elem);
                }
            }

            // Instantiate Ibatis mapper using the XmlDocument via a Builder,
            // instead of Ibatis using the config file.
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            ISqlMapper ibatisInstance = builder.Configure(xmlDoc);

            return ibatisInstance;
        }

        public static ISqlMapper CreateMapper()
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

            // for <?xml version="1.0" encoding="utf-8" ?>
            XmlNode docNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(docNode);

            // root(sqlMapConfig)
            XmlElement sqlMapConfig = xmlDoc.CreateElement("sqlMapConfig");
            sqlMapConfig.SetAttribute("xmlns", "http://ibatis.apache.org/dataMapper");
            sqlMapConfig.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlDoc.AppendChild(sqlMapConfig);

            // settings
            XmlElement settings = xmlDoc.CreateElement("settings", sqlMapConfig.NamespaceURI);
            sqlMapConfig.AppendChild(settings);

            XmlElement settings_useStatementNamespaces = xmlDoc.CreateElement("setting", sqlMapConfig.NamespaceURI);
            settings_useStatementNamespaces.SetAttribute("useStatementNamespaces", "false");

            XmlElement settings_cacheModelsEnabled = xmlDoc.CreateElement("setting", sqlMapConfig.NamespaceURI);
            settings_cacheModelsEnabled.SetAttribute("cacheModelsEnabled", "true");

            XmlElement settings_validateSqlMap = xmlDoc.CreateElement("setting", sqlMapConfig.NamespaceURI);
            settings_validateSqlMap.SetAttribute("validateSqlMap", "true");

            settings.AppendChild(settings_useStatementNamespaces);
            settings.AppendChild(settings_cacheModelsEnabled);
            settings.AppendChild(settings_validateSqlMap);

            // providers
            XmlElement providers = xmlDoc.CreateElement("providers", sqlMapConfig.NamespaceURI);
            sqlMapConfig.AppendChild(providers);

            providers.SetAttribute("embedded", "configs.providers.config, iBatisNet.Biz");
            
            // database
            XmlElement database = xmlDoc.CreateElement("database", sqlMapConfig.NamespaceURI);
            sqlMapConfig.AppendChild(database);

            XmlElement database_provider = xmlDoc.CreateElement("provider", sqlMapConfig.NamespaceURI);
            database_provider.SetAttribute("name", "sqlServer2.0");

            XmlElement database_dataSource = xmlDoc.CreateElement("dataSource", sqlMapConfig.NamespaceURI);
            database_dataSource.SetAttribute("name", "MtBatisSQL");
            database_dataSource.SetAttribute("connectionString", @"data source=(localDB)\MSSQLLocalDB;initial catalog=TEMP.BIZ.DB;integrated security=True;MultipleActiveResultSets=True;");

            database.AppendChild(database_provider);
            database.AppendChild(database_dataSource);

            // sqlMaps
            XmlElement sqlMaps = xmlDoc.CreateElement("sqlMaps", sqlMapConfig.NamespaceURI);
            sqlMapConfig.AppendChild(sqlMaps);

            var queryFileNames = IBatisNetMapperHelper.GetAllQueryXML("Query");
            var defaultNamespace = Assembly.GetExecutingAssembly().GetName().Name;
            foreach (var queryFile in queryFileNames)
            {
                XmlElement elem = xmlDoc.CreateElement("sqlMap", sqlMapConfig.NamespaceURI);
                elem.SetAttribute("embedded", $"{queryFile}, {defaultNamespace}");
                sqlMaps.AppendChild(elem);
            }

            // test
            xmlDoc.Save("@test.xml");

            // Instantiate Ibatis mapper using the XmlDocument via a Builder,
            // instead of Ibatis using the config file.
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            ISqlMapper ibatisInstance = builder.Configure("@test.xml");

            return ibatisInstance;
        }

        private static string[] GetAllQueryXML(string directory)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var namespaceName = executingAssembly.GetName().Name;
            string folderName = string.Format("{0}.{1}", namespaceName, directory);
            return executingAssembly
                .GetManifestResourceNames()
                .Where(r => r.StartsWith(folderName) && r.EndsWith(".xml"))
                .Select(r => r.Substring(namespaceName.Length + 1))
                .ToArray();
        }
    }
}
