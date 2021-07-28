using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Persons
{
    public class PersonModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SampleFaceId { get; set; }
        public string SampleFaceSourceType { get; set; }
        public string SampleFaceState { get; set; }
        public string PersonModelId { get; set; }
        public string SampleFaceUrl { get; set; }
    }
}
