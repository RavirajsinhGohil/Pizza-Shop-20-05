namespace PizzaShop.Service.Interfaces;

public interface IRazorViewToStringRenderer
{
    Task<string> RenderViewToStringAsync(string viewName, object model);
}
