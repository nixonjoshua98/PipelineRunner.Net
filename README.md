# Pipeline Runner

A minimal pipeline runner for .Net

## Overview

Pipeline Runner provides a simple, lightweight framework for building and executing sequential processing pipelines.

## Features

- **Composable** - Chain multiple steps together
- **Lightweight** - Minimal dependencies and overhead
- **Type-safe** - Strongly-typed pipeline definitions
- **Multi-Context Support** - Use the same pipeline with different context types

## Installation

```bash
dotnet add package PipelineRunner.Net
```

## Quick Start

### 1. Define a Context

Contexts hold data that flows through your pipeline:

```csharp
public class OrderContext : IPipelineContext
{
    public decimal Amount { get; set; }
    public bool IsProcessed { get; set; }
}
```

### 2. Create Filters

Filters process the context. Each filter receives a `next` delegate to continue the pipeline:

```csharp
public class ValidateOrderFilter : IFilter<OrderContext>
{
    public async Task ExecuteAsync(OrderContext context, PipelineDelegate<OrderContext> next)
    {
        if (context.Amount <= 0)
            throw new InvalidOperationException("Invalid amount");

        await next(context);
    }
}

public class ProcessOrderFilter : IFilter<OrderContext>
{
    public async Task ExecuteAsync(OrderContext context, PipelineDelegate<OrderContext> next)
    {
        context.IsProcessed = true;

        Console.WriteLine($"Processing order: ${context.Amount}");

        await next(context);
    }
}
```

### 3. Build and Execute the Pipeline

```csharp
var pipeline = new PipelineBuilder()
    .AddFilter<ValidateOrderFilter>()
    .AddFilter<ProcessOrderFilter>()
    .Build();

var order = new OrderContext { Amount = 99.99m };
await pipeline.ExecuteAsync(order);

Console.WriteLine($"Order processed: {order.IsProcessed}"); // True
```

### 4. Reuse with Multiple Context Types

```csharp
public class PaymentContext : IPipelineContext
{
    public string CardNumber { get; set; }
}

public class ValidatePaymentFilter : IFilter<PaymentContext>
{
    public async Task ExecuteAsync(PaymentContext context, PipelineDelegate<PaymentContext> next)
    {
        await next(context);
    }
}

var multiPipeline = new PipelineBuilder()
    .AddFilter<ValidateOrderFilter>()      // For OrderContext
    .AddFilter<ProcessOrderFilter>()       // For OrderContext
    .AddFilter<ValidatePaymentFilter>()    // For PaymentContext
    .Build();

await multiPipeline.ExecuteAsync(new OrderContext { Amount = 50 });
await multiPipeline.ExecuteAsync(new PaymentContext { CardNumber = "1234" });
```