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
            //var ibatisInstance = IBatisNetMapperHelper.CreateMapperFromEmbeddedResource("configs.SqlMap.config, iBatisNet.Biz",
            //                                                                            "configs.providers.config, iBatisNet.Biz",
            //                                                                            "Query");
            //var ibatisInstance = IBatisNetMapperHelper.CreateMapperFromEmbeddedResource();
            //var ibatisInstance = IBatisNetMapperHelper.CreateMapper();
            var ibatisInstance = IBatisNetMapperHelper.CreateMapperFromEmbeddedResource(@"data source=(localDB)\MSSQLLocalDB;initial catalog=TEMP.BIZ.DB;integrated security=True;MultipleActiveResultSets=True;");

            var result = ibatisInstance.QueryForList<MM_CODE>("MM_CODE.SELECT", null);
        }
    }
}
