using ServePos.Domain.Entities;
using MenuItemType = ServePos.Shared.MenuItemType;

namespace ServePos.Domain.Tests;

public class CashPaymentTests
{
    [Fact]
    public void CreatesPaymentAndCalculatesChange()
    {
        var payment = new CashPayment(receivedAmount: 20m, totalAmount: 13.50m, receivedById: 1);

        Assert.Equal(6.50m, payment.ChangeAmount);
        Assert.Equal(20m, payment.ReceivedAmount);
        Assert.Equal(13.50m, payment.TotalAmount);
    }

    [Fact]
    public void RejectsInsufficientCash()
    {
        Assert.Throws<InvalidOperationException>(
            () => new CashPayment(receivedAmount: 9.99m, totalAmount: 10m, receivedById: 1));
    }

    [Fact]
    public void ReducesAndRestoresLimitedStock()
    {
        var item = new MenuItem("Limo", null, MenuItemType.Drink, 3m, 1, 0);

        item.DecreaseAvailableQuantity();
        Assert.Equal(0, item.AvailableQuantity);
        Assert.Throws<InvalidOperationException>(item.DecreaseAvailableQuantity);

        item.IncreaseAvailableQuantity();
        Assert.Equal(1, item.AvailableQuantity);
    }
}
