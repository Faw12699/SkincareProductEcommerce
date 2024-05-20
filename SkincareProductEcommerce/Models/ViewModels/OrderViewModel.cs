namespace SkincareProductEcommerce.Models.ViewModels
{
    public class OrderViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> orderDetailsList { get; set; }
    }
}
