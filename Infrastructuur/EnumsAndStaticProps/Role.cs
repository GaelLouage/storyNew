using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.EnumsAndStaticProps
{
    public static class Role
    {
        public static string SuperAdmin { get; set; } = nameof(SuperAdmin);
        public static string Admin { get; set; } = nameof(Admin);
        public static string User { get; set; } = nameof(User);
    }
}
