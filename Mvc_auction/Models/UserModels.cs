using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Mvc_auction.Models
{
    public class UsersModel
    {
        [DisplayName("User Id")]
        public int user_id { get; set; }

        [Display(Name = "User name")]
        public string userName { get; set; }

        [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "Last name")]
        public string lastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string mail { get; set; }

    }
    public class UserDetailsModel
    {
        [DisplayName("User Id")]
        public int user_id { get; set; }

        [Display(Name = "User name")]
        public string userName { get; set; }

        [Display(Name = "Name")]
        public string name { get; set; }
        
        [Display(Name = "Last name")]
        public string lastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string mail { get; set; }

        [Display(Name = "Created date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString="{0:d}",ApplyFormatInEditMode=true)]
        public string createdDate { get; set; }

        [Display(Name = "Modified date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString="{0:d}",ApplyFormatInEditMode=true)]
        public string modifyedDate { get; set; }
     
        [Display(Name = "Is activated")]
        public string isActivated { get; set; }
        [Display(Name = "Is locked out")]
        public string isLockedOut { get; set; }
        [Display(Name = "Date of last lock out")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public string lastLockedOutDate { get; set; }
     

        [Display(Name = "Comments")]
        [DataType(DataType.Text)]
        public string comments { get; set; }

    }


}