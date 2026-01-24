---
sidebar_position: 5
---

# Real-World Examples

Complete validation scenarios using SGuard in production code.

## E-Commerce: Checkout Validation

```csharp
public class CheckoutService
{
    private readonly IReadOnlyDictionary<string, int> _stockBySku;
    private readonly ILogger<CheckoutService> _logger;
    
    public void ValidateCart(Cart cart)
    {
        // Basic null checks
        ThrowIf.NullOrEmpty(cart);
        ThrowIf.NullOrEmpty(cart.Items);
        
        // Every item must have positive quantity
        ThrowIf.Any(cart.Items, i => i.Quantity <= 0,
            new ArgumentException("All items must have a positive quantity"));
        
        // Check stock levels
        foreach (var item in cart.Items)
        {
            var stock = _stockBySku.TryGetValue(item.Sku, out var s) ? s : 0;
            ThrowIf.GreaterThan(item.Quantity, stock,
                new InvalidOperationException($"Insufficient stock for SKU '{item.Sku}'"),
                SGuardCallbacks.OnFailure(() => 
                    _logger.LogWarning("Stock check failed for {Sku}", item.Sku)));
        }
        
        // Validate totals
        ThrowIf.LessThanOrEqual(cart.TotalAmount, 0m,
            new ArgumentOutOfRangeException(nameof(cart.TotalAmount), 
                "Total must be greater than zero"));
    }
}
```

## User Management: Registration

```csharp
public class UserService
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserService> _logger;
    
    public async Task<User> CreateUserAsync(CreateUserRequest req)
    {
        // Validate request object
        ThrowIf.NullOrEmpty(req);
        ThrowIf.NullOrEmpty(req.Username, 
            SGuardCallbacks.OnFailure(() => 
                _logger.LogWarning("Registration attempt with empty username")));
        ThrowIf.NullOrEmpty(req.Email);
        
        // Age validation
        ThrowIf.LessThan(req.Age, 13,
            new ArgumentException("User must be 13 or older", nameof(req.Age)));
        ThrowIf.GreaterThan(req.Age, 130,
            new ArgumentException("Age seems unrealistic", nameof(req.Age)));
        
        // Check for existing user
        var existing = await _repository.FindByUsernameAsync(req.Username);
        ThrowIf.NullOrEmpty(existing,
            new InvalidOperationException("Username already taken"));
        
        return new User(req.Username, req.Age, req.Email);
    }
}
```

## Order Processing: Validation Pipeline

```csharp
public class OrderProcessor
{
    private readonly IInventoryService _inventory;
    private readonly IPaymentService _payment;
    private readonly ILogger<OrderProcessor> _logger;
    
    public async Task<OrderResult> ProcessOrderAsync(Order order)
    {
        // Phase 1: Structure validation
        ValidateOrderStructure(order);
        
        // Phase 2: Business rules validation
        await ValidateBusinessRulesAsync(order);
        
        // Phase 3: Process
        return await ExecuteOrderAsync(order);
    }
    
    private void ValidateOrderStructure(Order order)
    {
        ThrowIf.NullOrEmpty(order);
        ThrowIf.NullOrEmpty(order, o => o.Customer);
        ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
        ThrowIf.NullOrEmpty(order, o => o.Items);
        
        // Validate items
        ThrowIf.Any(order.Items, i => i is null,
            new ArgumentException("Order contains null items"));
        ThrowIf.Any(order.Items, i => i.Quantity <= 0,
            new ArgumentException("Order contains items with invalid quantities"));
    }
    
    private async Task ValidateBusinessRulesAsync(Order order)
    {
        // Check inventory
        foreach (var item in order.Items)
        {
            var available = await _inventory.GetAvailableStockAsync(item.ProductId);
            ThrowIf.GreaterThan(item.Quantity, available,
                new InvalidOperationException($"Insufficient stock for product {item.ProductId}"),
                SGuardCallbacks.OnFailure(() => 
                    _logger.LogWarning("Stock unavailable for product {ProductId}", item.ProductId)));
        }
        
        // Validate payment amount
        ThrowIf.LessThanOrEqual(order.TotalAmount, 0,
            new ArgumentException("Order total must be positive"));
    }
    
    private async Task<OrderResult> ExecuteOrderAsync(Order order)
    {
        // Process payment and fulfill order
        // ...
        return new OrderResult { Success = true, OrderId = order.Id };
    }
}
```

## API: Request Validation

```csharp
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            ValidateRequest(request);
            
            var product = await _productService.CreateAsync(request);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    private void ValidateRequest(CreateProductRequest request)
    {
        ThrowIf.NullOrEmpty(request);
        ThrowIf.NullOrEmpty(request.Name);
        ThrowIf.NullOrEmpty(request.Sku);
        
        // Price validation
        ThrowIf.LessThanOrEqual(request.Price, 0,
            new ArgumentException("Price must be positive", nameof(request.Price)));
        
        // SKU format validation (must start with "PRD-")
        if (Is.LessThan(request.Sku, "PRD-", StringComparison.Ordinal) ||
            Is.GreaterThan(request.Sku, "PRD-~", StringComparison.Ordinal))
        {
            throw new ArgumentException("SKU must start with 'PRD-'", nameof(request.Sku));
        }
    }
}
```

## Domain Models: Constructor Validation

```csharp
public class Money
{
    public decimal Amount { get; }
    public string Currency { get; }
    
    public Money(decimal amount, string currency)
    {
        ThrowIf.LessThan(amount, 0,
            new ArgumentException("Amount cannot be negative", nameof(amount)));
        
        ThrowIf.NullOrEmpty(currency);
        ThrowIf.LessThan(currency.Length, 3,
            new ArgumentException("Currency must be 3 characters", nameof(currency)));
        ThrowIf.GreaterThan(currency.Length, 3,
            new ArgumentException("Currency must be 3 characters", nameof(currency)));
        
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }
}
```

## Data Access: Query Validation

```csharp
public class ProductRepository
{
    private readonly DbContext _context;
    private readonly ILogger<ProductRepository> _logger;
    
    public async Task<List<Product>> SearchAsync(ProductSearchCriteria criteria)
    {
        ThrowIf.NullOrEmpty(criteria);
        
        // Validate price range
        if (criteria.MinPrice.HasValue && criteria.MaxPrice.HasValue)
        {
            ThrowIf.GreaterThan(criteria.MinPrice.Value, criteria.MaxPrice.Value,
                new ArgumentException("MinPrice cannot exceed MaxPrice"));
        }
        
        // Validate pagination
        ThrowIf.LessThanOrEqual(criteria.PageSize, 0,
            new ArgumentException("PageSize must be positive", nameof(criteria.PageSize)));
        ThrowIf.GreaterThan(criteria.PageSize, 100,
            new ArgumentException("PageSize cannot exceed 100", nameof(criteria.PageSize)));
        
        return await ExecuteSearchAsync(criteria);
    }
    
    private async Task<List<Product>> ExecuteSearchAsync(ProductSearchCriteria criteria)
    {
        var query = _context.Products.AsQueryable();
        
        if (criteria.MinPrice.HasValue)
            query = query.Where(p => p.Price >= criteria.MinPrice.Value);
        
        if (criteria.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= criteria.MaxPrice.Value);
        
        return await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();
    }
}
```

## Background Services: Job Validation

```csharp
public class EmailNotificationJob
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailNotificationJob> _logger;
    
    public async Task ExecuteAsync(EmailBatch batch)
    {
        ThrowIf.NullOrEmpty(batch);
        ThrowIf.NullOrEmpty(batch.Recipients);
        
        // Validate all recipients
        ThrowIf.Any(batch.Recipients, r => r is null,
            new ArgumentException("Batch contains null recipients"));
        ThrowIf.Any(batch.Recipients, r => string.IsNullOrEmpty(r.Email),
            new ArgumentException("Batch contains recipients with empty emails"),
            SGuardCallbacks.OnFailure(() => 
                _logger.LogError("Email batch validation failed")));
        
        // Batch size limits
        ThrowIf.GreaterThan(batch.Recipients.Count, 1000,
            new ArgumentException("Batch size exceeds limit of 1000"));
        
        await SendEmailsAsync(batch);
    }
    
    private async Task SendEmailsAsync(EmailBatch batch)
    {
        foreach (var recipient in batch.Recipients)
        {
            await _emailService.SendAsync(recipient.Email, batch.Subject, batch.Body);
        }
        
        _logger.LogInformation("Sent {Count} emails", batch.Recipients.Count);
    }
}
```

## Testing: Test Data Validation

```csharp
public class TestDataBuilder
{
    public User BuildTestUser(string username = "testuser", int age = 25, string email = "test@example.com")
    {
        // Even test data should be valid
        ThrowIf.NullOrEmpty(username);
        ThrowIf.NullOrEmpty(email);
        ThrowIf.LessThan(age, 0);
        
        return new User(username, age, email);
    }
    
    public Order BuildTestOrder(int itemCount = 1)
    {
        ThrowIf.LessThanOrEqual(itemCount, 0,
            new ArgumentException("Order must have at least one item"));
        
        var order = new Order
        {
            Customer = BuildTestUser(),
            Items = Enumerable.Range(1, itemCount)
                .Select(i => new OrderItem { ProductId = i, Quantity = 1 })
                .ToList()
        };
        
        return order;
    }
}
```

## Next Steps

- [Performance](../advanced/performance) - Optimize your validations
- [Best Practices](../advanced/best-practices) - Guidelines for effective use
- [API Reference](../api/throwif) - Complete API documentation
