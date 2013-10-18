using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBDesk.EFRepository
{
    public interface IEntity
    {
        int Id { get; set; } // Id (Primary key)
        bool IsActive { get; set; }
        string LastUpdBy { get; set; }
        DateTime LastUpdDate { get; set; }

    }
}
