using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SkincareProductEcommerce.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        [ValidateNever]
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        public OrderHeader OrderHeader { get; set; }

    }
}
