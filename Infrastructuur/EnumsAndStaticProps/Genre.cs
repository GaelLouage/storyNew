using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.EnumsAndStaticProps
{
    public static class Genre
    {
        public static string Horror { get; set; } = nameof(Horror);
        public static string Adventure { get; set; } = nameof(Adventure);
        public static string Action { get; set; } = nameof(Action);
    }
}
