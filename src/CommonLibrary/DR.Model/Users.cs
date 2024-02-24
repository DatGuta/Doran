using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DR.Model;

[Table("User")]
internal class Users {

    [Key]
    public required string UserId { get; set; }

    public required string UserName { get; set; }

}
