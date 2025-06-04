using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace PizzaOrderingAI
{
    public class PizzaOrdering
    {
        private List<Pizza> Pizzas { get; set; } =
        [
            new Pizza { Id = 1, Name = "Margherita" },
            new Pizza { Id = 2, Name = "Pepperoni" },
            new Pizza { Id = 3, Name = "Hawaiian" },
        ];

        private Order? CurrentOrder { get; set; } = null;

        [KernelFunction("get_pizzas")]
        [Description("Get the list of all available pizzas")]
        public IReadOnlyList<Pizza> GetPizzas()
        {
            return Pizzas.AsReadOnly();
        }

        private Pizza GetPizzaById(int id)
        {
            return Pizzas.FirstOrDefault(p => p.Id == id);
        }

        [KernelFunction("place_order_by_pizza_id")]
        [Description("Add a Pizza to the Order")]
        public Order? PlaceOrder(int pizzaId)
        {
            CurrentOrder ??= new Order();
            CurrentOrder.Pizzas.Add(GetPizzaById(pizzaId));
            return CurrentOrder;
        }

        [KernelFunction("get_order_pizzas")]
        [Description("Get the current Order's Pizzas")]
        public Order? GetOrderedPizzas()
        {
            return CurrentOrder;
        }

        [KernelFunction("clear_current_order")]
        [Description("Clear the current Order Pizzas")]
        public Order? ClearOrders()
        {
            CurrentOrder = null;
            return CurrentOrder;
        }
    }
}
