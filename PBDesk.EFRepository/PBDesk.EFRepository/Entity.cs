using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBDesk.EFRepository
{
    public class Entity : IEntity
    {
        public int Id { get; set; } // Id (Primary key)
        public bool IsActive { get; set; }
        public string LastUpdBy { get; set; }
        public DateTime LastUpdDate { get; set; }
    }
}
