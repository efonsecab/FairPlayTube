using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Video
{
    /// <summary>
    /// Represents the Global Keywords
    /// </summary>
    public class GlobalKeywordModel
    {
        /// <summary>
        /// Keywords
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// Appeareances
        /// </summary>
        public int Appeareances { get; set; }
    }
}
