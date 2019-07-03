using System;

namespace Parking.Models
{
    public class ListaMovto
    {
        public long? IdMovto { get; set; }
        public string Placa { get; set; }
        public string DataHoraEntrada { get; set; }
        public string DataHoraSaida { get; set; }
        public String Valor { get; set; }
        public int? TipoSituacao { get; set; }
        public String DscSituacao { get; set; }
        public decimal ValorCalc { get; set; }
    }
}