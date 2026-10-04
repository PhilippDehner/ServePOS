using ServePos.Domain.Entities;

namespace ServePos.Domain.Tests;

public class OrderItemTests
{
    [Fact]
    public void CancelsEachOrderItemOnlyOnce()
    {
        var item = new OrderItem(menuItemId: 1);

        item.Cancel();

        Assert.True(item.IsCancelled);
        Assert.Throws<InvalidOperationException>(item.Cancel);
    }

    [Fact]
    public void MarksOrderItemAsServed()
    {
        var item = new OrderItem(menuItemId: 1);

        item.MarkServed();

        Assert.True(item.IsServed);
    }
}
