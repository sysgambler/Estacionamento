using System.Collections.Generic;
using Parking.Models;
using Parking.Dto;
using Parking.Database;
using System;

namespace Parking.Business
{
    public class Regras
    {
        private DatabaseContext context;

        public Regras()
        {
            context = new DatabaseContext();
        }

        public IList<ListaMovto> ListarMovto(int Opcao, DateTime? DataInicio = null, DateTime? DataFim = null)
        {
            return context.ListarSql<ListaMovto>(MontaQueryListaMovto(Opcao, DataInicio, DataFim));
        }

        public ListaMovto LerMovto(long idMovto)
        {
            var mov = context.ListarSql<ListaMovto>(MontaQueryLerMovto(idMovto));

            ListaMovto movRet = new ListaMovto();

            if (mov.Count > 0)
            {
                movRet.DataHoraEntrada = mov[0].DataHoraEntrada;
                movRet.DataHoraSaida = mov[0].DataHoraSaida;
                movRet.DscSituacao = mov[0].DscSituacao;
                movRet.IdMovto = mov[0].IdMovto;
                movRet.Placa = mov[0].Placa;
                movRet.TipoSituacao = mov[0].TipoSituacao;
                movRet.Valor = mov[0].Valor;
            }
            else
            {
                movRet.IdMovto = 0;
            }

            return movRet;
        }

        public int DelMovto(long idMovto)
        {
            return context.ExecuteSql(MontaQueryDelMovto(idMovto));
        }

        public VerMovto RegistraMovto(string Placa)
        {
            var _placa = Placa.ToUpper();

            var mov = context.ListarSql<VerMovto>(MontaQueryLerMovto(_placa));

            VerMovto movRet = new VerMovto();

            movRet.ExistePlaca = mov[0].ExistePlaca;
            movRet.ExistePreco = mov[0].ExistePreco;

            if (movRet.ExistePlaca == 0 && movRet.ExistePreco == 1)
            {
                context.ExecuteSql(MontaQueryRegistraMovto(_placa));
            }

            return movRet;
        }

        public CalcMovto CalcMovto(long idMovto)
        {
            var mov = context.ListarSql<CalcMovto>(MontaQueryCalcMovto(idMovto));

            CalcMovto movRet = new CalcMovto();

            movRet.ValorCalc = 0;

            if (mov.Count > 0)
            {
                movRet.DataHoraEntrada = mov[0].DataHoraEntrada;
                movRet.DtHrEntrada = mov[0].DtHrEntrada;
                movRet.DtHrSaida = mov[0].DtHrSaida;
                movRet.Placa = mov[0].Placa;

                var precos = context.ListarSql<Precos>(MontaQueryLerPrecos(movRet.DtHrEntrada));

                foreach (var preco in precos)
                {
                    var vlFracao = preco.ValorHora / 60;

                    var minutos = 0;

                    if (Convert.ToInt32(movRet.DtHrSaida.ToString("HHmm")) < Convert.ToInt32(preco.HoraFim))
                    {
                        minutos = ((movRet.DtHrSaida.Hour * 60) + movRet.DtHrSaida.Minute) -
                                  ((Convert.ToInt32(preco.HoraInicio.Substring(0, 2)) * 60) + Convert.ToInt32(preco.HoraInicio.Substring(2, 2)));
                    }
                    else
                    {
                        minutos = ((Convert.ToInt32(preco.HoraFim.Substring(0, 2)) * 60) + Convert.ToInt32(preco.HoraFim.Substring(2, 2))) -
                                  ((Convert.ToInt32(preco.HoraInicio.Substring(0, 2)) * 60) + Convert.ToInt32(preco.HoraInicio.Substring(2, 2)));
                    }

                    movRet.ValorCalc += vlFracao * minutos;
                }

            }

            return movRet;
        }

        public Boolean SaidaMovto(string Placa, decimal ValorCalc)
        {
            var retorno = false;

            var mov = context.ExecuteSql(MontaQueryRegistraSaidaMovto(Placa.ToUpper(), ValorCalc));

            if (mov > 0)
            {
                retorno = true;
            }

            return retorno;
        }

        // Opcao ==> 1-Em aberto, 2-Pago, 3-Todos
        public string MontaQueryListaMovto(int Opcao, DateTime? DataInicio = null, DateTime? DataFim = null)
        {
            string vlQuery;

            vlQuery = "Select mov.MOV_ID as IdMovto," +
                      "       mov.MOV_CO_PLACA as Placa," +
                      "       Convert(Varchar(10), mov.MOV_DH_ENTRADA, 103) + ' às ' + Convert(Varchar(8), mov.MOV_DH_ENTRADA, 108) as DataHoraEntrada," +
                      "       Convert(Varchar(10), mov.MOV_DH_SAIDA, 103) + ' às ' + Convert(Varchar(8), mov.MOV_DH_SAIDA, 108) as DataHoraSaida," +
                      "       Convert(varchar, Format(mov.MOV_VL_TOTAL, 'N', 'de-de')) as Valor," +
                      "       mov.MOV_TP_SITUACAO as TipoSituacao," +
                      "       Case isNull(mov.MOV_TP_SITUACAO, 0)" +
                      "          When 1 Then 'Em Aberto'" +
                      "          Else 'Pago'" +
                      "       End as DscSituacao," +
                      "       0.0 as ValorCalc" +
                      "   From dbo.PARKING_MOVTO mov";

            if (Opcao != 3)
            {
                vlQuery += " Where mov.MOV_TP_SITUACAO = " + Opcao;
            }

            return vlQuery;
        }

        public string MontaQueryLerMovto(long idMovto)
        {
            return "Select mov.MOV_ID as IdMovto," +
                   "       mov.MOV_CO_PLACA as Placa," +
                   "       Convert(Varchar(10), mov.MOV_DH_ENTRADA, 103) + ' às ' + Convert(Varchar(8), mov.MOV_DH_ENTRADA, 108) as DataHoraEntrada," +
                   "       Convert(Varchar(10), mov.MOV_DH_SAIDA, 103) + ' às ' + Convert(Varchar(8), mov.MOV_DH_SAIDA, 108) as DataHoraSaida," +
                   "       Convert(varchar, Format(mov.MOV_VL_TOTAL, 'N', 'de-de')) as Valor," +
                   "       mov.MOV_TP_SITUACAO as TipoSituacao," +
                   "       Case isNull(mov.MOV_TP_SITUACAO, 0)" +
                   "          When 1 Then 'Em Aberto'" +
                   "          Else 'Pago'" +
                   "       End as DscSituacao," +
                   "       0.0 as ValorCalc" +
                   "   From dbo.PARKING_MOVTO mov" +
                   "   Where mov.MOV_ID = " + idMovto;
        }

        public string MontaQueryLerMovto(string placa)
        {
            return "Select Case IsNull(b.MOV_ID, 0)" +
                   "          When 0 Then 0" +
                   "          Else 1" +
                   "       End as ExistePlaca," +
                   "       Case IsNull(d.PRE_ID, 0)" +
                   "          When 0 Then 0" +
                   "          Else 1" +
                   "       End as ExistePreco" +
                   "   From(Select '" + placa + "' as CodPlaca) as a" +
                   "           Outer Apply(Select Top 1 mov.MOV_ID" +
                   "                          From dbo.PARKING_MOVTO mov" +
                   "                          Where mov.MOV_TP_SITUACAO = 1 And" +
                   "                                mov.MOV_CO_PLACA = a.CodPlaca) b," +
                   "       (Select '" + DateTime.Now.ToString("HHmm") + "' as HoraEntrada," +
                                      + GetTipoDia() + " as TipoDia) as c" +
                   "           Outer Apply(Select Top 1 pre.PRE_ID" +
                   "                          From dbo.PARKING_TABELA_PRECO pre" +
                   "                          Where pre.PRE_TP_DIA = c.TipoDia And" +
                   "                                c.HoraEntrada Between pre.PRE_HR_INICIO And pre.PRE_HR_FIM) d";
        }

        public string MontaQueryDelMovto(long idMovto)
        {
            return "Delete dbo.PARKING_MOVTO" +
                   "   Where MOV_ID = " + idMovto;
        }

        public string MontaQueryRegistraMovto(string Placa)
        {
            return "Insert Into dbo.PARKING_MOVTO Values ('" + Placa + "'," +
                                                              "Getdate()," +
                                                              "Null," +
                                                              "Null," +
                                                              "1)";
        }

        public string MontaQueryCalcMovto(long idMovto)
        {
            return "Select mov.MOV_CO_PLACA as Placa," +
                   "       mov.MOV_DH_ENTRADA as DtHrEntrada," +
                   "       Getdate() as DtHrSaida," +
                   "       Convert(Varchar(10), mov.MOV_DH_ENTRADA, 103) + ' às ' + Convert(Varchar(8), mov.MOV_DH_ENTRADA, 108) as DataHoraEntrada," +
                   "       0.0 as ValorCalc" +
                   "   From dbo.PARKING_MOVTO mov" +
                   "   Where mov.MOV_TP_SITUACAO = 1 And" +
                   "         mov.MOV_ID = " + idMovto;
        }

        public string MontaQueryRegistraSaidaMovto(string Placa, decimal ValorCalc)
        {
            return "Update dbo.PARKING_MOVTO" +
                   "   Set MOV_VL_TOTAL = " + ValorCalc.ToString("0.00").Replace(',', '.') + "," +
                   "       MOV_DH_SAIDA = Getdate()," + 
                   "       MOV_TP_SITUACAO = 2" +
                   "   Where MOV_TP_SITUACAO = 1 And" +
                   "         MOV_CO_PLACA = '" + Placa + "'";
        }

        public string MontaQueryLerPrecos(DateTime DtHrEntrada)
        {
            return "Select pre.PRE_HR_INICIO as HoraInicio," +
                   "       pre.PRE_HR_FIM as HoraFim," +
                   "       pre.PRE_VL_HORA as ValorHora" +
                   "   From dbo.PARKING_TABELA_PRECO pre" +
                   "   Where pre.PRE_TP_DIA = " + GetTipoDia() + " And" +
                   "         (('" + DtHrEntrada.ToString("HHmm") + "' Between pre.PRE_HR_INICIO And pre.PRE_HR_FIM) or" +
                   "          ((Substring(convert(varchar, getdate(), 8), 1, 2) +" +
                   "            Substring(convert(varchar, getdate(), 8), 4, 2)) Between pre.PRE_HR_INICIO And pre.PRE_HR_FIM))" +
                   "   Order by 1";
        }

        private int GetTipoDia()
        {
            return (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday) ? 2 : 1;
        }
    }
}