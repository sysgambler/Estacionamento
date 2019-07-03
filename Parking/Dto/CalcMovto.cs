using System;

namespace Parking.Models
{
    public class CalcMovto
    {
        public string Placa { get; set; }
        public string DataHoraEntrada { get; set; }
        public DateTime DtHrEntrada { get; set; }
        public DateTime DtHrSaida { get; set; }
        public decimal ValorCalc { get; set; }
    }
}