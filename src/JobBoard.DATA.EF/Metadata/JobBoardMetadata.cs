using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace JobBoard.DATA.EF
{

    [MetadataType(typeof(LocationMetadata))]
    public partial class Location
    {
        public string FullLocation
        {
            get { return string.Format($"{StoreNumber} - {City}, {State}"); }
        }
    }

    public class LocationMetadata
    {
        [Required]
        [StringLength(10, ErrorMessage = "* 10 Character Max")]
        [Display(Name = "Store #")]
        public string StoreNumber { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "* 50 Character Max")]
        public string City { get; set; }

        [Required]
        [StringLength(2, ErrorMessage = "* Must be the State abbreviation ")]
        public string State { get; set; }

        public string ManagerId { get; set; }
    }


    [MetadataType(typeof(OpenPositionMetadata))]
    public partial class OpenPosition { }

    public class OpenPositionMetadata
    {

    }


    [MetadataType(typeof(PositionMetadata))]
    public partial class Position { }

    public class PositionMetadata
    {
        [Required]
        [Display(Name = "Job Title")]
        [StringLength(50, ErrorMessage = "* 50 Character Max")]
        public string Title { get; set; }

        [Display(Name = "Job Description")]
        [StringLength(4000, ErrorMessage = "* 4000 Character Max")]
        public string JobDescription { get; set; }

        [Required]
        [Display(Name = "Pay")]
        public string PayRange { get; set; }
    }



    [MetadataType(typeof(ApplicationMetadata))]
    public partial class Application { }

    public class ApplicationMetadata
    {
        [Required]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Application Date")]
        public System.DateTime ApplicationDate { get; set; }

        [Display(Name = "Manager Notes")]
        [StringLength(2000, ErrorMessage = "* 2000 Character Max")]
        [UIHint("MultilineText")]
        [DisplayFormat(NullDisplayText = "None yet")]
        public string ManagerNotes { get; set; }

        [Display(Name = "Application Status")]
        public string ApplicationStatusId { get; set; }

        [Display(Name = "Resume")]
        [StringLength(75, ErrorMessage = "* 75 Character Max")]
        public string ResumeFilename { get; set; }
    }

}
