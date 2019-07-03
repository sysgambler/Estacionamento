using System;
using System.Collections.Generic;
using Parking.Models;

namespace Parking.Business
{
    public interface IRegras
    {
        IList<ListaMovto> ListarMovto(int Opcao, DateTime DataInicio, DateTime DataFim);
    }
}