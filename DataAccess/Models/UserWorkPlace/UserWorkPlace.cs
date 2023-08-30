using DataAccess.Models.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models.Users;

namespace DataAccess.Models.WorkPlace
{
    [Table("UserWorkPlace", Schema = "sirpsi")]
    public partial class UserWorkPlace
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; } = null!;
        public string WorkPlaceId { get; set; }
    }
}
