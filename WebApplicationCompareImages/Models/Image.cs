using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationCompareImages.Models
{
    public class Image
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int TimesShown { get; set; }
        public int TimesSelected { get; set; }
        public byte[]? ImageData { get; set; }

        [NotMapped]
        public IFormFile FileData { get; set; }

        public Image() 
        {
            
        }
    }
}
