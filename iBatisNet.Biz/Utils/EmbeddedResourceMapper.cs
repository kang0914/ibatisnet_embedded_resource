using IBatisNet.DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iBatisNet.Biz.Utils
{
    /// <summary>
	/// A singleton class to access the default SqlMapper defined by the SqlMap.Config
	/// </summary>
	public sealed class EmbeddedResourceMapper
    {
        #region Fields
        private static volatile ISqlMapper _mapper = null;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public static void Configure(object obj)
        {
            _mapper = null;
        }

        /// <summary>
        /// Init the 'default' SqlMapper defined by the SqlMap.Config file.
        /// </summary>
        public static void InitMapper(string connectionString)
        {
            var tempMapper = Utils.IBatisNetMapperHelper.CreateMapperFromEmbeddedResource(connectionString);
            _mapper = tempMapper;
        }

        /// <summary>
        /// Get the instance of the SqlMapper defined by the SqlMap.Config file.
        /// </summary>
        /// <returns>A SqlMapper initalized via the SqlMap.Config file.</returns>
        public static ISqlMapper Instance()
        {
            if (_mapper == null)
            {
                lock (typeof(SqlMapper))
                {
                    if (_mapper == null) // double-check
                    {
                        InitMapper("");
                    }
                }
            }
            return _mapper;
        }

        /// <summary>
        /// Get the instance of the SqlMapper defined by the SqlMap.Config file. (Convenience form of Instance method.)
        /// </summary>
        /// <returns>A SqlMapper initalized via the SqlMap.Config file.</returns>
        public static ISqlMapper Get()
        {
            return Instance();
        }
    }
}
