using Entities.Models;

namespace first_Application.Data
{
    public static class ApplicationContext
    {
        public static List<Product> Products { get; set; }
        static ApplicationContext()
        {
            Products = new List<Product>()
            {
                new Product{Id=1, ProductName="Computer",Price=1000},
                new Product{Id=2, ProductName="Keyboard",Price=600},
                new Product{Id=3, ProductName="Mouse",Price=200},
                new Product{Id=4, ProductName="Monitor",Price=700}
            };

        }

    }
}
