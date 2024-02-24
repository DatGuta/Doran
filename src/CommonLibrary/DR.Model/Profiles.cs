using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DR.Model;

internal class Profiles {

    [Key]
    public string ProfileId { get; set; }

    public decimal Phone { get; set; }
}
