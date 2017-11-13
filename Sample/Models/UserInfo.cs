using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sample.Models
{
    [Table("UserInfo")]
    public class UserInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(50)]
        public string LoginName { get; set; }
        [StringLength(50)]
        public string Password { get; set; }
        [StringLength(50)]
        public string NiceName { get; set; }
        [StringLength(50)]
        public string Name { get; set; }

        public int Gender { get; set; }
        [StringLength(50)]
        public string Mobile { get; set; }
        [StringLength(50)]
        public string IdentityCard { get; set; }
        [StringLength(50)]
        public string Birthday { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        public int Status { get; set; }
    }
}