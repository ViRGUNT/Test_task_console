using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace NewsReader.Models
{
    [Table(Name = "News")]
    public class News
    {

        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "ID")]
        public int ID { get; set; }

        [Column(Name = "Publication_Date")]
        public DateTime Publication_Date { get; set; }

        [Column(Name = "Guid")]
        public string Guid { get; set; }

        [Column(Name = "SourseUrl")]
        public string SourseUrl { get; set; }

        [Column(Name = "SourceName")]
        public string SourceName { get; set; }

        [Column(Name = "News_title")]
        public string News_title { get; set; }

        [Column(Name = "News_description")]
        public string News_description { get; set; }
    }
}
