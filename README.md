# Pipelinie

![](https://img.shields.io/badge/net-8.0-brightgreen.svg) ![](https://github.com/canro91/Pipelinie/workflows/.NET/badge.svg) ![](https://img.shields.io/github/license/canro91/Pipelinie)

Pipelinie offers abstractions to build and run pipelines. You could use pipelines to perform complex tasks through a set of separate steps. For example, make a reservation, generate an invoice, or create an order.

To learn more about the Pipeline pattern, read [Pipeline pattern: Perform tasks with an assembly line of steps](https://canro91.github.io/2020/02/14/PipelinePattern/).

## Usage

Pipelinie uses four entities:

1. `Command`: It holds all input parameters for a pipeline
2. `PipelineBuilder`: It creates a pipeline and its steps
3. `Pipeline`: It runs all its steps
4. `Step`: It is an operation in the pipeline

### How-to

1. Create a `Command` for your pipeline

```csharp
public class BuyItemCommand : ICommand
{
    // Number of items, Customer name, etc
}
```

2. Create a `PipelineBuilder`.

You could load the definition of your steps from a db, config files, or anywhere else. Pipelinie has a base implementation to load your steps from ASP.NET Core default dependency container. This base implementation uses a default pipeline.

```csharp
public class BuyItemPipelineBuilder : PipelineBuilderBase<BuyItemCommand>
{
    public BuyItemSingleStepPipelineBuilder(IServiceProvider provider)
        : base(provider)
    {
    }

    protected override Type[] StepsTypes
        => new[]
        {
            typeof(UpdateStockStep),
            typeof(ChargeCreditCardStep)
        };
}
```

You can create a pipeline to suit your own needs. In that case, you need to create a builder to return the custom pipeline. Take a look at the [Tests project](https://github.com/canro91/Pipelinie/tree/master/Pipelinie.Tests) to see more examples to create custom pipelines and builders.

3. Create individual steps per each operation in your pipeline

```csharp
public class UpdateStockStep : IStep<BuyItemCommand>
{
    public Task ExecuteAsync(BuyItemCommand command)
    {
        // Put your own logic here...
        return Task.CompletedTask;
    }
}

public class ChargeCreditCardStep : IStep<BuyItemCommand>
{
    public Task ExecuteAsync(BuyItemCommand command)
    {
        // Put your own logic here...
        return Task.CompletedTask;
    }
}
```

5. Enjoy!

```csharp
var command = new BuyItemCommand();
var builder = new BuyItemPipelineBuilder(serviceProvider);
var pipeline = builder.CreatePipeline(command);

await pipeline.ExecuteAsync();
```

## Installation

Grab your own copy

## Contributing

Feel free to report any bug, ask for a new feature or just send a pull-request. All contributions are welcome.
	
## License

MIT
