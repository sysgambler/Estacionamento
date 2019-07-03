using System;
using System.Web.Mvc;
using Parking.Models;
using Parking.Business;

namespace Parking.Controllers
{
    public class MovimentoController : Controller
    {
        //private readonly IRegras _regras;

        private Regras regras;

        public MovimentoController() //(IRegras regras)
        {
            regras = new Regras();
        }

        // GET: Movimento
        public ActionResult Movimento()
        {
            return View(regras.ListarMovto(1));
        }

        [HttpPost]
        public ActionResult GetMovto(int idMovto)
        {
            var mov = (regras.LerMovto(idMovto));
            return Json(mov);
        }

        [HttpPost]
        public ActionResult DelMovto(int idMovto)
        {
            var mov = (regras.DelMovto(idMovto));
            return Json(mov);
        }

        [HttpPost]
        public ActionResult RegistraMovto(string Placa)
        {
            var mov = (regras.RegistraMovto(Placa));
            
            return Json(mov);
        }

        [HttpPost]
        public ActionResult CalcMovto(int idMovto)
        {
            var mov = (regras.CalcMovto(idMovto));

            return Json(mov);
        }

        [HttpPost]
        public ActionResult SaidaMovto(string Placa, string ValorCalc)
        {
            var mov = regras.SaidaMovto(Placa, Convert.ToDecimal(ValorCalc.Replace('.', ',')));

            return Json(mov);
        }
    }
}