using System;
using System.Collections.Generic;

#nullable disable

namespace APIMangas.Models
{
    public partial class Manga
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public DateTime? DateReleased { get; set; }
    }
}
