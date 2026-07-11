using System.Collections.Generic;
using Linea_Produzione.Models;

namespace Linea_Produzione.Managers.BaseManager
{
    public interface IBaseManager
    {
        #region DistintaBase
        /// <summary>
        /// Cerca di DistintaBase
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<DistintaBaseEntity> CercaDistintaBase(DistintaBaseSearchModel search);

        /// <summary>
        /// Salva DistintaBase
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SalvaDistintaBase(DistintaBaseModel model);

        bool CancellaDistintaBase(long id);
        #endregion
    }
}