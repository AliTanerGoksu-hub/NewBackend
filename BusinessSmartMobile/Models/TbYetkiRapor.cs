using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessSmartMobile.Models
{
    public class TbYetkiRapor
    {
        public int ID { get; set; }
        public string PERSONELKODU { get; set; }
        public string PERSONELADI { get; set; }
        public int RaporID { get; set; }
        public string RaporAciklama { get; set; }
        public bool YetkisiVar { get; set; }
    }
}
