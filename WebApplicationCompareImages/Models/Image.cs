namespace WebApplicationCompareImages.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TimesShown { get; set; }
        public int TimesSelected { get; set; }
        public string ImageData { get; set; }

        public Image() 
        {
            
        }
    }
}
