namespace SkincareProductEcommerce.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        public double totalPrice { get; set; }

    }
}
