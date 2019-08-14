using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaneVAssistant.Models
{
    public class UserProfile
    {
        public string Name { get; set; }

        public string Platform { get; set; }

        public string Server { get; set; }

        public string Description { get; set; }

        public string ContactNo { get; set; }

        public string Email { get; set; }

        public bool Edit { get; set; }

        public void ClearAll()
        {
            Name = null;

            Platform = null;

            Server = null;

            Description = null;

            ContactNo = null;

            Email = null;

            Edit = false;
    }

    }
}