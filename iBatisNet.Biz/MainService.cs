using iBatisNet.Biz.Utils;
using IBatisNet.Common.Transaction;
using IBatisNet.DataMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iBatisNet.Biz
{
    public class MainService
    {
        private ISqlMapper _mapper
        {
            get
            {
                try
                {
                    ISqlMapper mapper = Mapper.Instance();
                    return mapper;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public IList<MM_CODE> MM_CODE_SELECT()
        {
            return _mapper.QueryForList<MM_CODE>("MM_CODE.SELECT", null);
        }

        public int MM_CODE_INSERT(MM_CODE model)
        {
            return _mapper.Update("MM_CODE.INSERT", model);
        }

        public void Test()
        {
            string serverName = "test_serverName";
            string databaseName = "test_databaseName";

            // Load the config file (embedded resource in assembly).
            System.Xml.XmlDocument xmlDoc = IBatisNet.Common.Utilities.Resources.GetEmbeddedResourceAsXmlDocument("configs.SqlMap.config, iBatisNet.Biz");

            // Overwrite the connectionString in the XmlDocument, hence changing database.
            // NB if your connection string needs extra parameters,
            // such as `Integrated Security=SSPI;` for user authentication,
            // then append that to InnerText too.
            //xmlDoc["sqlMapConfig"]["database"]["dataSource"]
            //    .Attributes["connectionString"]
            //    .InnerText = "Server=" + serverName + ";Database=" + databaseName;

            // query xml
            var sqlMapsRoot = xmlDoc["sqlMapConfig"]["sqlMaps"];
            var queryFileNames = EmbeddedResourceHelper.GetAllQueryXML("Query");
            foreach (var queryFile in queryFileNames)
            {
                System.Xml.XmlElement elem = xmlDoc.CreateElement("sqlMap", sqlMapsRoot.NamespaceURI);
                elem.SetAttribute("embedded", $"{queryFile}, {Assembly.GetExecutingAssembly().GetName().Name}");
                sqlMapsRoot.AppendChild(elem);
            }

            // Instantiate Ibatis mapper using the XmlDocument via a Builder,
            // instead of Ibatis using the config file.
            IBatisNet.DataMapper.Configuration.DomSqlMapBuilder builder = new IBatisNet.DataMapper.Configuration.DomSqlMapBuilder();
            IBatisNet.DataMapper.ISqlMapper ibatisInstance = builder.Configure(xmlDoc);

            var result = ibatisInstance.QueryForList<MM_CODE>("MM_CODE.SELECT", null);
        }
    }
}
